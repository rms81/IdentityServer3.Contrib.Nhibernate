Param(
	[string]$buildNumber = "0",
	[string]$preRelease = "beta"
)


Import-Module .\src\packages\psake.4.6.0\tools\psake.psm1

if(Test-Path Env:\APPVEYOR_BUILD_NUMBER){
	$buildNumber = [int]$Env:APPVEYOR_BUILD_NUMBER
	Write-Host "Using APPVEYOR_BUILD_NUMBER"

	$task = "appVeyor"
}

"Build number $buildNumber"

Invoke-Psake .\default.ps1 $task -properties @{ buildNumber=$buildNumber; preRelease=$preRelease }

Remove-Module psake