:SoftwareRequired
	:: Microsoft.NET v2.0						http://www.filehippo.com/download_dotnet_framework_2/
	:: Microsoft.NET v3.5						http://www.filehippo.com/download_dotnet_framework_3/
	:: TortoiseSVN (+command line tools)		http://www.filehippo.com/download_tortoisesvn/
	:: 7zip 32bit								http://www.filehippo.com/download_7zip_32/
	:: 7zip 64bit								http://www.filehippo.com/download_7-zip_64/
	:: SharpDevelop v3.2.1.6466					http://sourceforge.net/projects/sharpdevelop/files/SharpDevelop%203.x/3.2/SharpDevelop_3.2.1.6466_Setup.msi/download

:DownloadLink
	:: GoogleCode://code.google.com/p/sar-tool/downloads/list
	:: SourceForge: http://sourceforge.net/projects/sartool/files/

:BuildEnvironment
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
	%SAR% -f.bsd \sar\*.cs "Kevin Boronka"
	%SAR% -r \sar\AssemblyInfo.* ((Version)\(\"\d+\.\d+\.\d+\.\d+\"\)) "Version(\"%VERSION%\")"

	echo building binaries
	%SAR% -b.net 3.5 %SOLUTION% /p:Configuration=%CONFIG% /p:Platform=\"x86\"
	if errorlevel 1 goto BuildFailed

	
:BuildComplete
	copy sar\bin\%CONFIG%\*.exe release\*.exe
	copy license.txt release\license.txt
	copy readme.txt release\readme.txt

	copy sar\bin\%CONFIG%\sar.exe sar.exe
	%ZIP% "sar %VERSION%.zip" sar.exe readme.txt license.txt
	del sar.exe
	
	svn commit -m "sar version %VERSION%"
	svn copy %REPO%/trunk %REPO%/tags/%VERSION% -m "Tagging the %VERSION% version release of the project"
	svn update
	
	cd lib\skylib-source
	svn commit -m "sar version %VERSION%"
	svn update
	cd BASEPATH

	echo build completed
	popd
	exit /b 0


:BuildFailed
	echo build failed
	pause
	popd
	exit /b 1	
