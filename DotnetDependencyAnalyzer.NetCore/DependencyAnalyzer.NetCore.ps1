$solution = (Get-ChildItem . -Filter "*.sln" -Recurse | Select-Object -First 1)

Get-Content ($solution.FullName) |
  Select-String 'Project\(' |
    ForEach-Object {
      $projectParts = $_ -Split '[,=]' | ForEach-Object { $_.Trim('[ "{}]') }
	  $projectPath = Join-Path -Path ".\" -ChildPath $projectParts[1]
	  
      $plugin = (Get-ChildItem -Path $projectPath -Filter "DotnetDependencyAnalyzer.NetCore.dll" -Recurse -ErrorAction SilentlyContinue -Force | Select-Object -First 1)
	  if ($plugin) {
	    $args = $plugin.FullName + " " + $projectParts[1]
		Start-Process -FilePath 'dotnet' -WorkingDirectory $projectPath -ArgumentList $args -NoNewWindow -Wait
		Write-Output ''
	  }
	}
	Read-Host 'Press ENTER to exit plugin'