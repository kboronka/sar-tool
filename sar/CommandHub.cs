/* Copyright (C) 2014 Kevin Boronka
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
using System.Collections.Generic;

using sar.Base;
using sar.Commands;
using sar.Tools;

namespace sar.Tools
{
	public class CommandHub : Base.CommandHubBase
	{
		private List<BaseCommand> allCommands;
		public CommandHub() : base()
		{
			// load all command modules
			this.allCommands = new List<BaseCommand>() {
				new Help(this),
				new BuildCHM(this),
				new BuildNSIS(this),
				new BuildSLN(this),
				new CodeReIndent(this),
				new CodeClean(this),
				new AssemblyInfoVersion(this),
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
				new FileRemoveDirectory(this),
				new FileBsdHeader(this),
				new FileMirror(this),
				new FileCopy(this),
				new FileLock(this),
				new DirectoryTimestamp(this),
				new WindowsLogin(this),
				new WindowsMapDrive(this),
				new WindowsRearm(this),
				new WindowsRestart(this),
				new NetListAddaptors(this),
				new SkyUpdaterUpdate(this),
				new SkyUpdaterGenerate(this),
				new SkyUpdaterAdd(this),
				new Delay(this)
			};
		}
	}
}