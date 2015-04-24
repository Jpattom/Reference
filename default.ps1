properties {

	$ProductVersion = "2.1"
    $PatchVersion = "0"
	$BuildNumber = "0"
	$PreRelease = "alpha"
	$Platform = "Any CPU"
	$buildConfiguration = "Debug"
    $testMessage = 'Executed Test!'
	$compileMessage = 'Executed Compile!'
	$cleanMessage = 'Executed Clean!'
}

$baseDir = Split-Path (Resolve-Path $MyInvocation.MyCommand.Path)
$releaseRoot = "$baseDir\Release"
$packageOutPutDir = "$baseDir\artifacts"
$toolsDir = "$baseDir\tools"
$binariesDir = "$baseDir\binaries"
$coreOnlyDir = "$baseDir\core-only"
$coreOnlyBinariesDir = "$coreOnlyDir\binaries"
$outDir = "$baseDir\build"

$buildBase = "$baseDir\build"

$artifactsDir = "$baseDir\artifacts"
$artifacts32Dir = "$baseDir\artifacts32"

$nunitexec = "$toolsDir\nunit\nunit-console.exe"
$script:nunitTargetFramework = "/framework=4.0";
$script:nunitTestResult = "/xml:TestResult{0}.xml"

include $toolsDir\scripts\buildutils.ps1

task default -depends CreateArtifacts

task CreateArtifacts -depends Compile -description "Creates build Artefacts" {
    $currentArtifatcs = $artifactsDir + ($Platform -replace ' ')
	
	if(Test-Path $currentArtifatcs){
		Remove-Item -Force -Recurse $currentArtifatcs -ErrorAction SilentlyContinue | Out-Null
	}
	mkdir $currentArtifatcs
	mkdir "$currentArtifatcs\COSMOS_Worker"
	Copy-Item $outDir\COSMOS\log4net.* $currentArtifatcs\COSMOS_Worker -Force -Exclude **.Tests.*
	Copy-Item $outDir\COSMOS\MongoDB.* $currentArtifatcs\COSMOS_Worker -Force -Exclude **.Tests.*
	Copy-Item $outDir\COSMOS\NServiceBus.??? $currentArtifatcs\COSMOS_Worker -Force -Exclude **.Testing.*
	Copy-Item $outDir\COSMOS\NServiceBus.*.* $currentArtifatcs\COSMOS_Worker -Force -Exclude **.Testing.*
	Copy-Item $outDir\COSMOS\**.COSMOS.**.dll $currentArtifatcs\COSMOS_Worker -Force -Exclude **.Tests.*,**.Service.MessageHandlers.*
	Copy-Item $outDir\COSMOS\HA.Common.dll $currentArtifatcs\COSMOS_Worker -Force -Exclude **.Tests.*
	Copy-Item $outDir\COSMOS\HA.Contracts.dll $currentArtifatcs\COSMOS_Worker -Force -Exclude **.Tests.*
	Copy-Item $outDir\COSMOS\HA.COSMOS.Worker.dll.config $currentArtifatcs\COSMOS_Worker -Force -Exclude **.Tests.*
	
		
	mkdir "$currentArtifatcs\COSMOS_Service"
	Copy-Item $outDir\COSMOS\log4net.* $currentArtifatcs\COSMOS_Service -Force -Exclude **.Tests.*
	Copy-Item $outDir\COSMOS\NServiceBus.??? $currentArtifatcs\COSMOS_Service -Force -Exclude **.Testing.*
	Copy-Item $outDir\COSMOS\NServiceBus.*.* $currentArtifatcs\COSMOS_Service -Force -Exclude **.Testing.*
	Copy-Item $outDir\COSMOS\**.COSMOS.**.dll $currentArtifatcs\COSMOS_Service -Force -Exclude **.Tests.*,**.COSMOS.DAContracts.*,HA.COSMOS.Entities.*,HA.COSMOS.MessageHandlers.*,**.DAL.*, **.Worker.*,**.Proxies.*
	Copy-Item $outDir\COSMOS\**.WCF.**.dll $currentArtifatcs\COSMOS_Service -Force -Exclude **.Tests.*,**.Proxies.*
	Copy-Item $outDir\COSMOS\HA.Common.dll $currentArtifatcs\COSMOS_Service -Force -Exclude **.Tests.*
	Copy-Item $outDir\COSMOS\HA.Contracts.dll $currentArtifatcs\COSMOS_Service -Force -Exclude **.Tests.*
	Copy-Item $outDir\COSMOS\**.WCF.**.dll.config $currentArtifatcs\COSMOS_Service -Force -Exclude **.Tests.*,**.Proxies.*
	
	mkdir "$currentArtifatcs\COSMOS_Client"
	Copy-Item $outDir\COSMOS\HA.Common.dll $currentArtifatcs\COSMOS_Client -Force -Exclude **.Tests.*
	Copy-Item $outDir\COSMOS\HA.Contracts.dll $currentArtifatcs\COSMOS_Client -Force -Exclude **.Tests.*
	Copy-Item $outDir\COSMOS\**.WCF.Proxies.dll $currentArtifatcs\COSMOS_Client -Force -Exclude **.Tests.*
	Copy-Item $outDir\COSMOS\**.WCF.Messages.dll $currentArtifatcs\COSMOS_Client -Force -Exclude **.Tests.*
	Copy-Item $outDir\COSMOS\**.COSMOS.ValueObjects.dll $currentArtifatcs\COSMOS_Client -Force -Exclude **.Tests.*
	Copy-Item $outDir\COSMOS\COSMOSClientConsole.** $currentArtifatcs\COSMOS_Client -Force -Exclude **.Tests.*
}

task Test -depends Compile  -description "Runs Unit Tests after building all the solutions" {
$testAssemblies = @()
	$testAssemblies +=  dir "$outDir\COSMOS\*Tests.dll"
	exec {&$nunitexec $testAssemblies $script:nunitTargetFramework ($script:nunitTestResult -f ($Platform -replace ' ')) }
	$testMessage
}

task Compile -depends Clean, CompileFramework, CompileCOSMOS  -description "Builds ALL Solutions solution"{ 
}

task CompileFramework -depends Clean -description "Builds FrameWork Solutions solution"{ 
  exec { msbuild $baseDir\src\ObjectBuilder.sln /t:"Clean,Build" /p:Platform=$Platform /p:Configuration=Release /p:OutDir="$outDir\Framework" /m /nodeReuse:false }
  exec { msbuild $baseDir\src\FrameWork.sln /t:"Clean,Build" /p:Platform=$Platform /p:Configuration=Release /p:OutDir="$outDir\Framework" /m /nodeReuse:false }
}

task CompileCOSMOS -description "Builds COSMOS solution"  { 
  exec { msbuild $baseDir\src\COSMOS.sln /t:"Clean,Build" /p:Platform=$Platform /p:Configuration=Release /p:OutDir="$outDir\COSMOS" /m /nodeReuse:false }
}


task Clean -description "Clean The Build Environment here and may be use for Initialization" {
 if(Test-Path $outDir){
		Remove-Item -Force -Recurse $outDir -ErrorAction SilentlyContinue | Out-Null
	}
  $cleanMessage
}

task GenerateAssemblyInfo -description "Generates assembly info for all the projects with version" {

	Write-Output "Build Number: $BuildNumber"
   
	$projectFiles = ls -path $srcDir -include *.csproj -recurse  
	$v1Projects = @()
	
	foreach($projectFile in $projectFiles) {

		$projectDir = [System.IO.Path]::GetDirectoryName($projectFile)
		$projectName = [System.IO.Path]::GetFileName($projectDir)
		$asmInfo = [System.IO.Path]::Combine($projectDir, [System.IO.Path]::Combine("Properties", "AssemblyInfo.cs"))
		$assemblyTitle = gc $asmInfo | select-string -pattern "AssemblyTitle"
		
		if($assemblyTitle -ne $null){
			$assemblyTitle = $assemblyTitle.ToString()
			if($assemblyTitle -ne ""){
				$assemblyTitle = $assemblyTitle.Replace('[assembly: AssemblyTitle("', '') 
				$assemblyTitle = $assemblyTitle.Replace('")]', '') 
				$assemblyTitle = $assemblyTitle.Trim()
				
			}
		}
		else{
			$assemblyTitle = ""	
		}
		
		$projectFileName = [System.IO.Path]::GetFileName($projectFile)
		Write-Output "Project Name: $projectFileName"
		
		if([System.Array]::IndexOf($v1Projects, $projectFileName) -eq -1){
			$asmVersion =  $ProductVersion + ".0.0"

			if($PreRelease -eq "") {
				$fileVersion = $ProductVersion + "." + $PatchVersion + ".0" 
				$infoVersion = $ProductVersion + "." + $PatchVersion
			} else {
				$fileVersion = $ProductVersion + "." + $PatchVersion + "." + $BuildNumber 
				$infoVersion = $ProductVersion + "." + $PatchVersion + "-" + $PreRelease + $BuildNumber 	
			}
		} else {
			$asmVersion =  "1.0.0.0"

			if($PreRelease -eq "") {
				$fileVersion = "1.0.0.0" 
				$infoVersion = "1.0.0"
			} else {
				$fileVersion = "1.0.0." + $BuildNumber 
				$infoVersion = "1.0.0." + "-" + $PreRelease + $BuildNumber 	
			}
		}
				
		$assemblyDescription = gc $asmInfo | select-string -pattern "AssemblyDescription" 
		if($assemblyDescription -ne $null){
			$assemblyDescription = $assemblyDescription.ToString()
			if($assemblyDescription -ne ""){
				$assemblyDescription = $assemblyDescription.Replace('[assembly: AssemblyDescription("', '') 
				$assemblyDescription = $assemblyDescription.Replace('")]', '') 
				$assemblyDescription = $assemblyDescription.Trim()
			}
		}
		else{
			$assemblyDescription = ""
		}
		
		$assemblyProduct =  gc $asmInfo | select-string -pattern "AssemblyProduct" 
		
		if($assemblyProduct -ne $null){
			$assemblyProduct = $assemblyProduct.ToString()
			if($assemblyProduct -ne ""){
				$assemblyProduct = $assemblyProduct.Replace('[assembly: AssemblyProduct("', '') 
				$assemblyProduct = $assemblyProduct.Replace('")]', '') 
				$assemblyProduct = $assemblyProduct.Trim()
			}
		}
		else{
			$assemblyProduct = "Risco Axes Plus"
		}
		
		$notclsCompliant = @("")

		$clsCompliant = (($projectDir.ToString().StartsWith("$srcDir")) -and ([System.Array]::IndexOf($notclsCompliant, $projectName) -eq -1)).ToString().ToLower()
		
		Generate-Assembly-Info $assemblyTitle `
		$assemblyDescription  `
		$clsCompliant `
		"" `
		"release" `
		"Risco Group Ltd." `
		$assemblyProduct `
		"Copyright 2013-2014 Risco. All rights reserved" `
		$asmVersion `
		$fileVersion `
		$infoVersion `
		$asmInfo 
 	}
}

task ? -Description "Helper to display task info" {
	Write-Documentation
}

