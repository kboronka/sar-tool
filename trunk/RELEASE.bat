@echo off
pushd "%~dp0"
set SOLUTION=sar.sln
set BASEURL=http://svn.wp-plugins.org/countdown-timer
set CONFIG=Release

:: Paths
	set BITS=x86
	if "%PROCESSOR_ARCHITECTURE%" == "AMD64" set BITS=x64
	if "%PROCESSOR_ARCHITEW6432%" == "AMD64" set BITS=x64

	IF %BITS% == x86 (
		echo OS is 32bit
		set MAKENSIS="%PROGRAMFILES%\NSIS\makensis.exe" /V3
		set MSBUILD="%WinDir%\Microsoft.NET\Framework\v2.0.50727\msbuild.exe"
		set REPLACE="lib\sar\sar.exe"
		set ZIP="%PROGRAMFILES%\7-Zip\7zG.exe" a -tzip
	) ELSE (
		echo OS is 64bit
		set MAKENSIS="%PROGRAMFILES(X86)%\NSIS\makensis.exe" /V3
		set MSBUILD="%WinDir%\Microsoft.NET\Framework\v2.0.50727\msbuild.exe"
		set REPLACE="lib\sar\sar.exe"
		set ZIP="%PROGRAMFILES%\7-Zip\7zG.exe" a -tzip
	)

:: Build Soultion
	echo "VERSION.MAJOR.MINOR.BUILD".
	set /p VERSION="> "

	svn update
	%REPLACE% AssemblyInfo.cs "0.0.0.0" "%VERSION%"
	%REPLACE% %SOLUTION% "Format Version 10.00" "Format Version 9.00"
	%REPLACE% %SOLUTION% "Visual Studio 2008" "Visual Studio 2005"

	echo building binaries
	%MSBUILD% %SOLUTION% /p:Configuration=%CONFIG% /p:Platform="x86"
	if errorlevel 1 goto BuildFailed

:: Build Complete
	copy sar\bin\%CONFIG%\*.exe release\*.exe
	copy license.txt release\license.txt

	copy sar\bin\%CONFIG%\sar.exe sar.exe
	%ZIP% "sar %VERSION%.zip" sar.exe readme.txt license.txt
	del sar.exe

	%REPLACE% AssemblyInfo.cs "%VERSION%" "0.0.0.0"
	%REPLACE% %SOLUTION% "Format Version 9.00" "Format Version 10.00"
	%REPLACE% %SOLUTION% "Visual Studio 2005" "Visual Studio 2008"
	
	svn commit -m "version %VERSION%"
	echo svn copy %BASEURL%/trunk %BASEURL%/tags/%VERSION% -m "Tagging the %VERSION% version release of the project"
	svn copy %BASEURL%/trunk %BASEURL%/tags/%VERSION% -m "Tagging the %VERSION% version release of the project"
	
	echo
	echo
	echo build completed, trunk has been tagged

	popd
	exit /b 0


:: Build Failed
	:BuildFailed
	%REPLACE% AssemblyInfo.cs "%VERSION%" "0.0.0.0"
	%REPLACE% %SOLUTION% "Format Version 9.00" "Format Version 10.00"
	%REPLACE% %SOLUTION% "Visual Studio 2005" "Visual Studio 2008"

	echo
	echo
	echo build failed
	pause

	popd
	exit /b 1