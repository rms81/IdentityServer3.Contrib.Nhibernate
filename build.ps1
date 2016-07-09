Param(
	[string]$buildNumber = "0",
	[string]$preRelease = "beta"
)

gci .\source -Recurse "packages.config" |% {
	"Restoring " + $_.FullName
	.\source\.nuget\nuget.exe i $_.FullName -o .\source\packages
}

Import-Module .\source\packages\psake.4.4.6\tools\psake.psm1

if(Test-Path Env:\APPVEYOR_BUILD_NUMBER){
	$buildNumber = [int]$Env:APPVEYOR_BUILD_NUMBER
	Write-Host "Using APPVEYOR_BUILD_NUMBER"

	$task = "appVeyor"
}

"Build number $buildNumber"

Invoke-Psake .\default.ps1 $task -properties @{ buildNumber=$buildNumber; preRelease=$preRelease }

Remove-Module psake