param($installPath, $toolsPath, $package, $project)

$solutionPath = (Get-Item $installPath).parent.parent.fullname

$pluginPS1 = (Get-ChildItem -Path $solutionPath -Filter "DependencyAnalyzer.ps1" -ErrorAction SilentlyContinue -Force | Select-Object -First 1)
$pluginBatch = (Get-ChildItem -Path $solutionPath -Filter "DependencyAnalyzer.bat" -ErrorAction SilentlyContinue -Force | Select-Object -First 1)
if($pluginPS1){
	Remove-Item -Path $pluginPS1.Fullname -Force
}
if($pluginBatch){
	Remove-Item -Path $pluginBatch.Fullname -Force
}
