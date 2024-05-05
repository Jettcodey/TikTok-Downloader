Set objShell = CreateObject("WScript.Shell")

' Command to execute
strCommand = "cmd /c winget install --id Microsoft.Powershell --source winget"

' Show command prompt window
objShell.Run strCommand, 1, True
