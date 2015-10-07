/* Copyright (C) 2015 Kevin Boronka
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
 * POSSIBILITY OF SUCH DAMAGE.
 */

using System;

using sar.Base;
using sar.Commands;

namespace sar.Tools
{
	public class CommandHub : Base.CommandHub
	{
		public CommandHub() : base()
		{
			// load all command modules
			base.commandList.AddRange(new Command[] {
			                          	new BuildCHM(this),
			                          	new BuildNSIS(this),
			                          	new BuildSLN(this),
			                          	new CodeReIndent(this),
			                          	new CodeClean(this),
			                          	new AssemblyInfoVersion(this),
			                          	new rdp(this),
			                          	new Kill(this),
			                          	new AppShutdownWait(this),
			                          	new LabviewVersion(this),
			                          	new VboxManage(this),
			                          	new FileBackup(this),
			                          	new FileSearchAndReplace(this),
			                          	new FileTimestamp(this),
			                          	new FileEncode(this),
			                          	new FileFind(this),
			                          	new FileDestory(this),
			                          	new FileDelete(this),
			                          	new FileRemoveDirectory(this),
			                          	new FileBsdHeader(this),
			                          	new FileMove(this),
			                          	new FileLock(this),
			                          	new FanucFixLineNumbers(this),
			                          	new FanucBackup(this),
			                          	new FanucPositionsToCSV(this),
			                          	new FileOpenFolder(this),
			                          	new MSSQL_GenerateScripts(this),
			                          	new DirectoryTimestamp(this),
			                          	new WindowsAutoLogin(this),
			                          	new WindowsLogin(this),
			                          	new WindowsMapDrive(this),
			                          	new WindowsRearm(this),
			                          	new WindowsRestart(this),
			                          	new WindowsShellReplacement(this),
			                          	new NetListAdapters(this),
			                          	new NetSetIP(this),
			                          	new Bower(this)
			                          });
		}
	}
}