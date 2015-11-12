:BuildEnvironment
	@echo off
	pushd "%~dp0"

:Paths
	set uSAR="release\sar.exe"
	set ZIP="%PROGRAMFILES%\7-Zip\7zG.exe" a -tzip

	%uSAR% -bower
	%uSAR% f.bk ".\sarTesting\Http\Views\assets\sar-controls\release\*.*" ".\lib\sar-controls"	
	%uSAR% file.removedirectory ".\sarTesting\Http\Views\assets\sar-controls" /q
	%uSAR% file.removedirectory ".\sarTesting\Http\Views\assets\sar-tool" /q