$solution = (Get-ChildItem . -Filter "*.sln" -Recurse | Select-Object -First 1)

Get-Content ($solution.FullName) |
  Select-String 'Project\(' |
    ForEach-Object {
      $projectParts = $_ -Split '[,=]' | ForEach-Object { $_.Trim('[ "{}]') }
	  $projectPath = Join-Path -Path ".\" -ChildPath $projectParts[1]
	  $pluginPath = Join-Path -Path $projectPath -ChildPath "bin"
	  
      $plugin = (Get-ChildItem -Path $pluginPath -Filter "DotnetDependencyAnalyzer.exe" -Recurse -ErrorAction SilentlyContinue -Force | Select-Object -First 1)
	  if ($plugin) {
		& $plugin.Fullname $projectParts[1]
		Write-Output ''
	  }
	}
	Read-Host 'Press ENTER to exit plugin'

