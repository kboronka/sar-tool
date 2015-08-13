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
using System.Collections.Generic;
using System.IO;

using sar.Tools;
using sar.Base;


namespace sar.Commands
{
	public class Bower : Command
	{
		public Bower(Base.CommandHub parent) : base(parent, "Bower Update",
		                                            new List<string> { "bower" },
		                                            "-bower",
		                                            new List<string> { @"-bower" })
		{
			
		}
		
		public override int Execute(string[] args)
		{			
			var nodejs = IO.FindApplication("node.exe", "nodejs");
			var bower = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\npm\node_modules\bower\bin\bower";
		
			
			if (!File.Exists(bower)) throw new ApplicationException("Bower not found");
			
			//ConsoleHelper.Run(nodejs, bower + " install");
			string output;
			ConsoleHelper.Run(nodejs, bower + " update", out output);
			
			ConsoleHelper.WriteLine(output, ConsoleColor.White);
			
			ConsoleHelper.WriteLine("Bower update was successfully completed", ConsoleColor.DarkYellow);
			return ConsoleHelper.EXIT_OK;
		}
	}
}
