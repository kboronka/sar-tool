:BuildEnvironment
	@echo off
	pushd "%~dp0"

:Paths
	set uSAR="release\sar.exe"

	%uSAR% -bower
	%uSAR% f.bk ".\sarTesting\Http\Views\assets\sar-controls\release\*.*" ".\lib\sar-controls"	
	%uSAR% file.removedirectory ".\sarTesting\Http\Views\assets\sar-controls" /q
	%uSAR% file.removedirectory ".\sarTesting\Http\Views\assets\sar-tool" /q