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
	set BASEPATH=%~dp0

:Paths
	set SAR="lib\sar\sar.exe"

:Build
	svn cleanup
	svn update
	%SAR% -f.bsd \sar\*.cs "Kevin Boronka"
	%SAR% -sky.gen SkyUpdate.info release\sar.exe https://sar-tool.googlecode.com/svn/trunk/release/sar.exe
	popd
	exit /b 1	
