using HA.Common;
using HA.Contracts;
using HA.COSMOS.ValueObjects;
using HA.WCF.Messages;
using HA.WCF.Proxies;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace COSMOSClientConsole
{
    public class COSMOSCallBackProxy : IService
    {
        internal static string securityToken;
        internal static COSMOSUSerContext rmacUserContext;
        public void InvokeService(IWCFMessage wcfMessage)
        {
            Console.WriteLine(string.Format("reply {0} of {1} ProcessId {2} ProcessType {3}", wcfMessage.ProcessContext.MessageNumber,
               wcfMessage.ProcessContext.TotalNumberOfMessages, wcfMessage.ProcessContext.ProcessId, wcfMessage.ProcessContext.ProcessType));
            var errorCode = ((WCFMessage)wcfMessage).ErrorCode;
            switch (wcfMessage.ProcessContext.ProcessType)
            {
                case ProcessTypes.Login:

                    switch (errorCode)
                    {
                        case UserServiceErrorNumbers.UserPasswordExpired:
                            Console.WriteLine("invalid Login: User Password Expired");
                            break;
                        case UserServiceErrorNumbers.UserNameOrPasswordIncorect:
                            Console.WriteLine("invalid Login: User Name Or Password Incorect");
                            break;
                        case UserServiceErrorNumbers.Default:
                            securityToken = wcfMessage.SecurityToken;
                            var userContexts = wcfMessage.GetServiceParams();
                            rmacUserContext = (COSMOSUSerContext)wcfMessage.UserContext;
                            break;
                    }
                    break;
                case ProcessTypes.GetAllAppUsers:
                    switch (errorCode)
                    {
                        case UserServiceErrorNumbers.UnableToAutherizeUser:
                            Console.WriteLine("invalid Login: User Name Or Password Incorect");
                            break;
                        case UserServiceErrorNumbers.Default:
                            var result = wcfMessage.GetServiceParams();
                            foreach (EditAppUserVO user in result)
                            {
                                Console.WriteLine(user.UserName);
                            }
                            break;
                    }

                    break;
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Starting from COSMOS Client");
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("http://localhost:5052/");
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                COSMOSUSerContext userContext = null;
                LoginVO loginVo = new LoginVO();
                Console.Write("User Name:");

                loginVo.UserName = Console.ReadLine();
                Console.Write("Password:");
                loginVo.Password = Console.ReadLine();
                ServiceMessage wcfmessage = new ServiceMessage();
                wcfmessage.ServiceParams = new object[] { loginVo };
                wcfmessage.ProcessContext = ProcessContextUtil.GetContextForNewProcess(ProcessTypes.Login);
                
                var content = new StringContent(JsonConvert.SerializeObject(wcfmessage), Encoding.UTF8, "application/json");
                var commandResponse = client.PostAsync("api/COSMOS", content).Result;

                if (commandResponse.IsSuccessStatusCode)
                {
                    var errorCode = commandResponse.Content.ReadAsAsync<int>();
                    if (errorCode.Result == ReplyCodes.Sucess)
                    {
                        var serviceResult = GetServiceResult(client, wcfmessage.ProcessContext.ProcessId);
                        if (serviceResult != null)
                            userContext = serviceResult.UserContext;
                        Console.WriteLine("Press Enter to Submit Jobs");
                        Console.ReadLine();
                       
                        if (userContext != null)
                        {
                            Console.WriteLine("Going for executing Job");
                            ServiceMessage message = new ServiceMessage();
                            message.ServiceParams = new object[] { "Some Text for Job", "Second text for Job" };
                            message.ProcessContext = ProcessContextUtil.GetContextForNewProcess(ProcessTypes.DoJob);
                            message.UserContext = userContext;
                            message.SecurityToken = userContext.SecurityToken;
                            content = new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json");
                            commandResponse = client.PostAsync("api/COSMOS", content).Result;
                            if (commandResponse.IsSuccessStatusCode)
                            {
                                var commandResponseBusinessResult = commandResponse.Content.ReadAsAsync<int>().Result;
                                if (commandResponseBusinessResult == ReplyCodes.Sucess)
                                {
                                    var nextResult = GetServiceResult(client, message.ProcessContext.ProcessId);
                                    if (nextResult != null)
                                    {
                                        foreach (object obj in nextResult.ServiceParams)
                                        {
                                            Console.WriteLine(obj);
                                        }
                                    }
                                    Console.ReadLine(); 
                                }
                                
                            }
                        }
                    }
                    else
                    {
                        switch (errorCode.Result)
                        {
                            case UserServiceErrorNumbers.UserPasswordExpired:
                                Console.WriteLine("invalid Login: User Password Expired");
                                break;
                            case UserServiceErrorNumbers.UserNameOrPasswordIncorect:
                                Console.WriteLine("invalid Login: User Name Or Password Incorect");
                                break;
                            case UserServiceErrorNumbers.Default:
                                break;
                        }
                        Console.ReadLine();

                    }
                }
                else
                {
                    Console.WriteLine("Unable to Post {0} ({1})", (int)commandResponse.StatusCode, commandResponse.ReasonPhrase);
                    Console.ReadLine();
                }
               

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadLine();
            }
        }

        private static ServiceMessage GetServiceResult(HttpClient client, Guid processId)
        {
            HttpResponseMessage queryResponse = null;
            ServiceMessage result = null;
            bool canProceed = true;
            var requestUri = string.Format("api/COSMOS?processId={0}", processId);
            do
            {
                queryResponse = client.GetAsync(requestUri).Result;  // Blocking call!

                if (queryResponse != null)
                {
                    if (queryResponse.IsSuccessStatusCode)
                    {
                        result = queryResponse.Content.ReadAsAsync<ServiceMessage>().Result;
                        if (result != null)
                        {
                            Console.WriteLine(result.UserContext.SecurityToken);

                        }
                        else
                        {
                            Console.WriteLine("Attempt failed to get the result of {0} \nSleeping for a second releasing the resource", processId);
                            System.Threading.Thread.Sleep(1000);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Unable to Get {0} ({1})", (int)queryResponse.StatusCode, queryResponse.ReasonPhrase);
                        canProceed = false;

                    }
                }
            } while (result == null && canProceed);
            return result;
        }
    }
}
