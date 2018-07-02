param($installPath, $toolsPath, $package, $project)

$rootDir = (Get-Item $installPath).parent.parent.fullname
$solution = (Get-ChildItem $rootDir -Filter "*.sln" -Recurse | Select-Object -First 1)

Get-Content ($solution.FullName) |
  Select-String 'Project\(' |
    ForEach-Object {
	  $projectParts = $_ -Split '[,=]' | ForEach-Object { $_.Trim('[ "{}]') }
	  $projectPath = Join-Path -Path $rootDir -ChildPath $projectParts[1]
	  
	  $pluginPS1 = (Get-ChildItem -Path $projectPath -Filter "DependencyAnalyzer.ps1" -ErrorAction SilentlyContinue -Force | Select-Object -First 1)
	  $pluginEXE = (Get-ChildItem -Path $projectPath -Filter "DependencyAnalyzer.exe" -ErrorAction SilentlyContinue -Force | Select-Object -First 1)
	  if ($pluginPS1 -And $pluginEXE) {
		Move-Item -Path $pluginPS1.Fullname -Destination $rootDir -Force
		Move-Item -Path $pluginEXE.Fullname -Destination $rootDir -Force
	  }
	}


