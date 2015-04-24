param(
	[Parameter(Position=0,Mandatory=0)]
	$nsbPowershellDll = '.\NServiceBus.PowerShell.dll',
	[Parameter(Position=1,Mandatory=0)]
	[string[]]$nsbInfrastructures = @('All')	
)

$resultstring = "";

if($PSVersionTable.PSVersion.Major -lt 3){
	Write-Error "ERROR: The $nsbPowershellDll can be loaded only if powershell 3.0 or above please upgrade powershell";
    exit 1;
}
 
if($PSVersionTable.CLRVersion.Major -lt 4){
	Write-Error "ERROR: The $nsbPowershellDll can be loaded only if clr version is 4.0 or above please upgrade clr"
    exit 1;
}

$error.clear();
Import-Module $nsbPowershellDll;
if($error.count -eq 0)
{
	if(($nsbInfrastructures -contains 'All') -or ($nsbInfrastructures -contains 'DTC'))
	{
		$testDTC = Test-NServiceBusDTCInstallation;
		if($testDTC[0] -eq $false)
		{
			Install-NServiceBusDTC -WhatIf;
			$testDTC = Test-NServiceBusDTCInstallation;
			if($testDTC[0] -eq $false)
			{
				$resultstring = $resultstring + "NServiceBus DTC not installed properly. `r`n";
				Write-Error "NServiceBus DTC not installed properly." -ErrorAction Continue;
			}
			else
			{
				$resultstring = $resultstring + "NServiceBus DTC installed and running smoothly. `r`n";
				Write-Host "NServiceBus DTC installed and running smoothly." -ErrorAction Continue;
			}
		}
		else
		{
			$resultstring = $resultstring + "NServiceBus DTC already installed and running smoothly. `r`n";
			Write-Host "NServiceBus DTC already installed and running smoothly." -ErrorAction Continue;
		}
	}
	
	if(($nsbInfrastructures -contains 'All') -or ($nsbInfrastructures -contains 'PerformanceCounters'))
	{
		$testPerformanceCounters = Test-NServiceBusPerformanceCountersInstallation;
		if($testPerformanceCounters[0] -eq $false)
		{
			Install-NServiceBusPerformanceCounters -WhatIf;
			$testPerformanceCounters = Test-NServiceBusPerformanceCountersInstallation;
			if($testPerformanceCounters[0] -eq $false)
			{
				$resultstring =  $resultstring  + "NServiceBus Performance Counters not installed properly `r`n";
				Write-Error "NServiceBus Performance Counters not installed properly" -ErrorAction Continue;
			}
			else
			{
				$resultstring =  $resultstring  + "NServiceBus Performance Counters installed properly `r`n";
				Write-Host "NServiceBus Performance Counters installed properly" -ErrorAction Continue;
			}
		}
		else
		{
			$resultstring = $resultstring + "NServiceBus Performance Counters already installed properly; `r`n";
			Write-Host "NServiceBus Performance Counters already installed properly" -ErrorAction Continue;
		}
	}
	
	if(($nsbInfrastructures -contains 'All') -or ($nsbInfrastructures -contains 'MSMQ'))
	{
		$request = Test-NServiceBusMSMQInstallation;
		if($request[0] -eq $false)
		{
			Install-NServiceBusMSMQ -WhatIf;
			$request = Test-NServiceBusMSMQInstallation;
			if($request[0] -eq $false)
			{
				$resultstring =  $resultstring  + "NServiceBus MSMQ not installed properly. `r`n";
				Write-Error "NServiceBus MSMQ not installed properly" -ErrorAction Continue;
			}
			else
			{
				$resultstring =  $resultstring  + "NServiceBus MSMQ installed properly. `r`n";
				Write-Host "NServiceBus MSMQ installed properly" -ErrorAction Continue;
			}			
		}
		else
		{
			$resultstring = $resultstring + "NServiceBus MSMQ already installed properly. `r`n";
			Write-Host "NServiceBus MSMQ already installed properly" -ErrorAction Continue;
		}
	}
	
	if(($nsbInfrastructures -contains 'All') -or ($nsbInfrastructures -contains 'RavenDB'))
	{
		$testRavenDB = Test-NServiceBusRavenDBInstallation
		if($testRavenDB[0] -eq $false)
		{
			Install-NServiceBusRavenDB -WhatIf;
			$testRavenDB = Test-NServiceBusRavenDBInstallation;
			if($testRavenDB[0] -eq $false)
			{
				$resultstring =  $resultstring  + " NServiceBus RavenDB not installed properly. `r`n";
				Write-Error "NServiceBus RavenDB not installed properly." -ErrorAction Continue;
			}
			else
			{
				$resultstring = $resultstring + "NServiceBus RavenDB installed properly. `r`n";
				Write-Host "NServiceBus RavenDB installed properly." -ErrorAction Continue;
			}
		}
		else
		{
			$resultstring = $resultstring + "NServiceBus RavenDB already installed properly. `r`n";
			Write-Host "NServiceBus RavenDB already installed properly." -ErrorAction Continue;
		}
	}
	Remove-Module NServiceBus.PowerShell;
	return  $resultstring 
  
}
else
{
	Write-Error "ERROR: Unable to load  NServiceBus.PowerShell.dll from $nsbPowershellDll" -ErrorAction Continue;
    exit 1;
}
 
 