@echo off
set SOLUTION=sar.sln
set CONFIG=Release
set MSBUILD="%WinDir%\Microsoft.NET\Framework\v2.0.50727\msbuild.exe"
set MAKENSIS="%PROGRAMFILES(X86)%\NSIS\makensis.exe" /V3
set REPLACE="release\sar.exe"
set ZIP="%PROGRAMFILES%\7-Zip\7zG.exe" a -tzip
pushd "%~dp0"

echo "VERSION.MAJOR.MINOR.BUILD".
set /p VERSION="> "

%REPLACE% AssemblyInfo.cs "0.0.0.0" "%VERSION%"
%REPLACE% %SOLUTION% "Format Version 10.00" "Format Version 9.00"
%REPLACE% %SOLUTION% "Visual Studio 2008" "Visual Studio 2005"

echo building binaries
%MSBUILD% %SOLUTION% /p:Configuration=%CONFIG% /p:Platform="x86"
if errorlevel 1 goto BuildFailed

rem -----------------------------------------------------------------------
rem Build Complete
rem -----------------------------------------------------------------------

copy sar\bin\%CONFIG%\*.exe release\*.exe
copy license.txt release\license.txt

copy sar\bin\%CONFIG%\sar.exe sar.exe
%ZIP% "sar %VERSION%.zip" sar.exe readme.txt license.txt
del sar.exe

%REPLACE% AssemblyInfo.cs "%VERSION%" "0.0.0.0"
%REPLACE% %SOLUTION% "Format Version 9.00" "Format Version 10.00"
%REPLACE% %SOLUTION% "Visual Studio 2005" "Visual Studio 2008"

echo.
echo build completed

popd
exit /b 0

rem -----------------------------------------------------------------------
rem Build Failed
rem -----------------------------------------------------------------------

:BuildFailed

%REPLACE% AssemblyInfo.cs "%VERSION%" "0.0.0.0"
%REPLACE% %SOLUTION% "Format Version 9.00" "Format Version 10.00"
%REPLACE% %SOLUTION% "Visual Studio 2005" "Visual Studio 2008"

echo.
echo build failed
pause

popd
exit /b 1