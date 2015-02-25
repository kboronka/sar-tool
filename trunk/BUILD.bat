:Required Software
	:: Microsoft.NET v2.0						http://www.filehippo.com/download_dotnet_framework_2/
	:: Microsoft.NET v3.5						http://www.filehippo.com/download_dotnet_framework_3/
	:: TortoiseSVN (+command line tools)		http://www.filehippo.com/download_tortoisesvn/
	:: 7zip 32bit								http://www.filehippo.com/download_7zip_32/
	:: 7zip 64bit								http://www.filehippo.com/download_7-zip_64/
	:: SharpDevelop v4x							http://www.icsharpcode.net/OpenSource/SD/Download/#SharpDevelop4x

:Optional Software
	:: php										http://windows.php.net/download/

:DownloadLink
	:: GoogleCode: https://code.google.com/p/sar-tool/downloads/list
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
	
	%SAR% -f.bsd \sar\*.cs "Kevin Boronka"
	%SAR% -f.bsd \sarControls\*.cs "Kevin Boronka"
	%SAR% -f.bsd \sarTesting\*.cs "Kevin Boronka"
	%SAR% -f.bsd \quickLog\source\*.cs "Kevin Boronka"

	%SAR% -assy.ver \sar\AssemblyInfo.* %VERSION%
	%SAR% -assy.ver \sarControls\AssemblyInfo.* %VERSION%
	%SAR% -assy.ver \quickLog\source\AssemblyInfo.* %VERSION%

	%SAR% -f.del sar\bin\%CONFIG%\*.* /q /svn
	%SAR% -f.del quickLog\source\bin\%CONFIG%\*.* /q /svn
	
	echo building binaries
	%SAR% -b.net 3.5 %SOLUTION% /p:Configuration=%CONFIG% /p:Platform=\"NET35\"
	%SAR% -b.net 2.0 sarQuckLog.sln /p:Configuration=%CONFIG% /p:Platform=\"NET20\"
	if errorlevel 1 goto BuildFailed

:BuildComplete
:BuildComplete
	copy sar\bin\%CONFIG%\*.exe release\*.exe
	copy sar\bin\%CONFIG%\*.pdb release\*.pdb
	copy sarControls\bin\%CONFIG%\sarControls.dll release\sarControls.dll
	copy sarControls\bin\%CONFIG%\sarControls.pdb release\sarControls.pdb

	copy quickLog\source\bin\%CONFIG%\*.exe quickLog\release\*.exe
	copy quickLog\source\bin\%CONFIG%\*.pdb quickLog\release\*.pdb
	
	echo build completed
	pause
	popd
	exit /b 0

:BuildFailed
	echo build failed
	pause
	popd
	exit /b 1	
