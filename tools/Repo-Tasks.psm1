function Import-VSProjectTemplate
{
	Param
	(
		[Parameter(Mandatory = $false)]
		[string]$VSProjectTemplatesPath
	)
	$visualStudioPath = "$env:USERPROFILE\Documents\Visual Studio 2015\Templates\ProjectTemplates"
	if ($PSBoundParameters.ContainsKey("VSProjectTemplatesPath"))
	{
		$visualStudioPath = $VSProjectTemplatesPath
	}

	if (Test-Path $visualStudioPath)
	{
		$templateZipFiles = Get-ChildItem "$PSScriptRoot\ProjectTemplates" -Filter "*.zip"
		foreach ($template in $templateZipFiles)
		{
			$installationPath = "$visualStudioPath\$($template.Name)"
			Copy-Item $template.FullName $installationPath
			Write-Host "Copied $($template.Name) to Visual Studio 2015 ProjectTemplates folder." -ForegroundColor Yellow
		}
	}
	else
	{
		Write-Host "Could not find path to Visual Studio 2015 ProjectTemplates folder." -ForegroundColor Red
	}
}

$taskScriptDir = [System.IO.Path]::GetDirectoryName($PSCommandPath)
$env:repoRoot = [System.IO.Path]::GetDirectoryName($taskScriptDir)
$userPsFileDir = [string]::Empty

[string]$envVariableName="TEST_CSM_ORGID_AUTHENTICATION"

<#
We allow users to include any helper powershell scripts they would like to include in the current session
Currently we support two ways to include helper powershell scripts
1) psuserspreferences environment variable
2) $env:USERPROFILE\psFiles directory
We will include all *.ps1 files from any of the above mentioned locations
#>
if([System.IO.Directory]::Exists($env:psuserpreferences))
{
	$userPsFileDir = $env:psuserpreferences
}
elseif([System.IO.Directory]::Exists("$env:USERPROFILE\psFiles"))
{
	$userPsFileDir = "$env:USERPROFILE\psFiles"
}

if([string]::IsNullOrEmpty($userPsFileDir) -eq $false)
{
	Get-ChildItem $userPsFileDir | WHERE {$_.Name -like "*.ps1"} | ForEach {
	Write-Host "Including $_" -ForegroundColor Green
	. $userPsFileDir\$_
	}
}
else
{
	Write-Host "Loading skipped. 'psuserpreferences' environment variable was not set to load user preferences." -ForegroundColor DarkYellow
}

Import-Module "$PSScriptRoot\Modules\Build-Tasks.psd1"
Import-Module "$PSScriptRoot\Modules\TestFx-Tasks.psd1"

#Execute Init
#Init

Export-ModuleMember -Function Import-VSProjectTemplate