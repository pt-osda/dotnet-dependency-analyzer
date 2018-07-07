param($installPath, $toolsPath, $package, $project)

$solutionPath = (Get-Item $installPath).parent.parent.fullname
$projectPath = Join-Path -Path $solutionPath -ChildPath $project.Name

$pluginPS1 = (Get-ChildItem -Path $projectPath -Filter "DependencyAnalyzer.ps1" -ErrorAction SilentlyContinue -Force | Select-Object -First 1)
$pluginBatch = (Get-ChildItem -Path $projectPath -Filter "DependencyAnalyzer.bat" -ErrorAction SilentlyContinue -Force | Select-Object -First 1)
Move-Item -Path $pluginPS1.Fullname -Destination $solutionPath -Force
Move-Item -Path $pluginBatch.Fullname -Destination $solutionPath -Force