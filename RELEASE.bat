:: Development Enviorment
::
:: Microsoft.NET v2.0.50727					http://www.microsoft.com/download/en/details.aspx?id=19
:: SharpDevelop v3.2.1.6466					http://sourceforge.net/projects/sharpdevelop/files/SharpDevelop%203.x/3.2/SharpDevelop_3.2.1.6466_Setup.msi/download
:: TortoiseSVN 1.7.4(+command line tools)	https://sourceforge.net/projects/tortoisesvn/files/1.7.4/Application/
:: 7zip										http://www.7-zip.org/download.html


:: GoogleCode://code.google.com/p/sar-tool/downloads/list
:: SourceForge: http://sourceforge.net/projects/sartool/files/

@echo off
pushd "%~dp0"
set SOLUTION=sar.sln
set REPO=https://sar-tool.googlecode.com/svn
set CONFIG=Release
set BASEPATH=%~dp0

:Paths
	set SAR="lib\sar\sar.exe"
	set ZIP="%PROGRAMFILES%\7-Zip\7zG.exe" a -tzip

:Build
	echo "VERSION.MAJOR.MINOR.BUILD".
	set /p VERSION="> "

	svn update
	
	%SAR% -r AssemblyInfo.* ((Version)\(\"\d+\.\d+\.\d+\.\d+\"\)) "Version(\"%VERSION%\")"
	%SAR% -r %SOLUTION% "Format Version 10.00" "Format Version 9.00"
	%SAR% -r %SOLUTION% "Visual Studio 2008" "Visual Studio 2005"

	echo building binaries
	%SAR% -b.net 2.0 %SOLUTION% /p:Configuration=%CONFIG% /p:Platform=\"x86\"
	if errorlevel 1 goto BuildFailed

:BuildComplete
	echo build completed
	
	copy sar\bin\%CONFIG%\*.exe release\*.exe
	copy license.txt release\license.txt
	copy readme.txt release\readme.txt

	copy sar\bin\%CONFIG%\sar.exe sar.exe
	%ZIP% "sar %VERSION%.zip" sar.exe readme.txt license.txt
	del sar.exe

	%SAR% -r %SOLUTION% "Format Version 9.00" "Format Version 10.00"
	%SAR% -r %SOLUTION% "Visual Studio 2005" "Visual Studio 2008"
	
	svn commit -m "version %VERSION%"
	svn copy %REPO%/trunk %REPO%/tags/%VERSION% -m "Tagging the %VERSION% version release of the project"
	svn update
	popd
	exit /b 0


:BuildFailed
	echo build failed
	pause
	%SAR% -r %SOLUTION% "Format Version 9.00" "Format Version 10.00"
	%SAR% -r %SOLUTION% "Visual Studio 2005" "Visual Studio 2008"
	popd
	exit /b 1	
