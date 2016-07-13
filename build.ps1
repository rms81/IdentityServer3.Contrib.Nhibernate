Param(
	[string]$buildNumber = "0",
	[string]$preRelease = "beta"
)

gci .\src -Recurse "packages.config" |% {
	"Restoring " + $_.FullName
	.\src\.nuget\nuget.exe install $_.FullName -o .\src\packages
}

Import-Module .\src\packages\psake.4.6.0\tools\psake.psm1

if(Test-Path Env:\APPVEYOR_BUILD_NUMBER){
	$buildNumber = [int]$Env:APPVEYOR_BUILD_NUMBER
	Write-Host "Using APPVEYOR_BUILD_NUMBER"

	$task = "appVeyor"
}

if(Test-Path env:BuildRunner) {
        $buildRunner = Get-Content env:BuildRunner

		if($buildRunner -eq "myget") {
			$buildNumber = [int]$Env:BuildCounter
			Write-Host "Using MYGET_BUILD_NUMBER"

			$task = "myGet"
		}
}

"Build number $buildNumber"

Invoke-Psake .\default.ps1 $task -properties @{ buildNumber=$buildNumber; preRelease=$preRelease }

Remove-Module psake