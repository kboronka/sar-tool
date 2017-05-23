/* Copyright (C) 2017 Kevin Boronka
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
using Microsoft.Win32;

using sar.Tools;
using sar.Base;

namespace sar.Commands
{
	public class ExcelOpenNewInstances : Command
	{
		public ExcelOpenNewInstances(Base.CommandHub parent)
			: base(parent, "Excel Fix - Open New Instances",
			       new List<string> { "excel.fix" },
			       @"-excel.fix",
			       new List<string>() { @"excel.fix" })
		{
			
		}
		
		public override int Execute(string[] args)
		{
			// sanity check
			if (args.Length != 1)
			{
				throw new ArgumentException("incorrect number of arguments");
			}
			
			throw new NotImplementedException();
			
			//HKEY_CLASSES_ROOT/Excel.Sheet.12/shell/Open/command
			
			RegistryKey excelCommandFolder = Registry.ClassesRoot.OpenSubKey(@"HKEY_CLASSES_ROOT\Excel.Sheet.12\shell\Open\command", true);
			if (excelCommandFolder == null)
			{
				throw new KeyNotFoundException("Excel.Sheet.12 command key was not found");
			}
			
			var defaultKey = (string)excelCommandFolder.GetValue("");
			
			if (defaultKey.Contains("%1"))
			{
				ConsoleHelper.WriteLine("Fix already implemented");
				return ConsoleHelper.EXIT_OK;
			}
			
			// TODO: rename command to command2
			// TODO: rename ddeexec to ddeexed2			
			
			excelCommandFolder.GetValue("", defaultKey + @" ""%1""");
			return ConsoleHelper.EXIT_OK;
		}
	}
}
