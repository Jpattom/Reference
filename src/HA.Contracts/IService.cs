using System;
using System.ServiceModel;

namespace HA.Contracts
{
    [ServiceContract(CallbackContract = typeof(IService))]
    [ServiceKnownType("GetKnownTypes", typeof(KnownTypeProvider))]
    public interface IService
    {
        [OperationContract(IsOneWay=true)]
        void InvokeService(IWCFMessage wcfMessage);
    }
}
