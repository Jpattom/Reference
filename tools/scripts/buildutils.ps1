function Generate-Assembly-Info{

param(
	[string]$assemblyTitle,
	[string]$assemblyDescription,
	[string]$clsCompliant = "true",
	[string]$internalsVisibleTo = "",
	[string]$configuration, 
	[string]$company, 
	[string]$product, 
	[string]$copyright, 
	[string]$version,
	[string]$fileVersion,
	[string]$infoVersion,	
	[string]$file = $(throw "file is a required parameter.")
)
	if($infoVersion -eq ""){
		$infoVersion = $fileVersion
	}
	
	$asmInfo = "using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Runtime.CompilerServices;

[assembly: AssemblyTitle(""$assemblyTitle"")]
[assembly: AssemblyDescription(""$assemblyDescription"")]
[assembly: AssemblyVersion(""$version"")]
[assembly: AssemblyFileVersion(""$fileVersion"")]
[assembly: AssemblyCopyright(""$copyright"")]
[assembly: AssemblyProduct(""$product"")]
[assembly: AssemblyCompany(""$company"")]
[assembly: AssemblyConfiguration(""$configuration"")]
[assembly: AssemblyInformationalVersion(""$infoVersion"")]
[assembly: ComVisible(false)]		
"
	if($clsCompliant.ToLower() -eq "true"){
		 $asmInfo += "[assembly: CLSCompliantAttribute($clsCompliant)]
"
	} 
	
	if($internalsVisibleTo -ne ""){
		$asmInfo += "[assembly: InternalsVisibleTo(""$internalsVisibleTo"")]
"	
	}

	$dir = [System.IO.Path]::GetDirectoryName($file)
	
	if ([System.IO.Directory]::Exists($dir) -eq $false)
	{
		Write-Host "Creating directory $dir"
		[System.IO.Directory]::CreateDirectory($dir)
	}
	Write-Host "Generating assembly info file: $file"
	$asmInfo | Out-File -Encoding UTF8 $file
}