Write-Output 'Started setup'
$solutionDir = (Get-Item -Path ".\").FullName
$success = $false
$i = 0

while ($i -lt 7) {
	$solutionFile = (Get-ChildItem -Path $solutionDir -Filter "*.sln" -ErrorAction SilentlyContinue -Force | Select-Object -First 1)
    if ($solutionFile) {
		$success = $true
		break
	}
	$solutionDir = Split-Path -Path $solutionDir -Parent
	$i += 1
}

if($success){
	Copy-Item -Path .\DependencyAnalyzer.NetCore.exe -Destination $solutionDir
	Copy-Item -Path .\DependencyAnalyzer.NetCore.ps1 -Destination $solutionDir
	Write-Output 'Finished setup'
}
else{
	Write-Output 'Setup failed. Cannot found solution root folder'
}
Read-Host 'Press ENTER to exit'