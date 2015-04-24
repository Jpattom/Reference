#Usage: This file is to build RACSample in Different Flavours
#Author: Joseph Thomas
#
param(
	[Parameter(Position=0,Mandatory=0)]
	[string]$majorVersion = '2.1',
	[Parameter(Position=1,Mandatory=0)]
	[string]$minorVersion = '0',
	[Parameter(Position=2,Mandatory=0)]
	[string]$patchNumber = '0',
	[Parameter(Position=3,Mandatory=0)]
	[string]$buildNumber = '0',
	[Parameter(Position=4,Mandatory=0)]
	[string]$preRelease = 'build',
	[Parameter(Position=5,Mandatory=0)]
	[string[]]$buildPlatforms = @('Any CPU'),	
	[Parameter(Position=6,Mandatory=0)]
	[string]$gitTag = ''
)

Import-Module .\tools\psake\psake.psm1
git clean -Xdf
foreach ($buildPlatform in $buildPlatforms)  
{ 
	Invoke-psake .\default.ps1  -properties @{ProductVersion="$majorVersion.$minorVersion";PatchVersion=$patchNumber;BuildNumber=$buildNumber;PreRelease=$preRelease;Platform=$buildPlatform;buildConfiguration='Release'} -framework '4.0'
}
git checkout **/AssemblyInfo.cs 
Remove-Module psake