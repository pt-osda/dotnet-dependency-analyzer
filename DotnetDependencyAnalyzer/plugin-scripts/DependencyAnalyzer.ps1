$solution = (Get-ChildItem . -Filter "*.sln" -Recurse | Select-Object -First 1)

Get-Content ($solution.FullName) |
  Select-String 'Project\(' |
    ForEach-Object {
      $projectParts = $_ -Split '[,=]' | ForEach-Object { $_.Trim('[ "{}]') }
	  $pluginPath = Join-Path -Path ".\" -ChildPath "packages/DotnetDependencyAnalyzer.1.2.3/tools"
	  
	  $projectPath = Join-Path -Path ".\" -ChildPath $projectParts[1]
	  $project = (Get-ChildItem -Path $projectPath -Filter "*.csproj" -Recurse -ErrorAction SilentlyContinue -Force | Select-Object -First 1)
	  
      if ($project) {
		$plugin = (Get-ChildItem -Path $pluginPath -Filter "DotnetDependencyAnalyzer.exe" -Recurse -ErrorAction SilentlyContinue -Force | Select-Object -First 1)
		& $plugin.Fullname $projectPath
		Write-Output ''
	  }
	}

