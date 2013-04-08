/* Copyright (C) 2013 Kevin Boronka
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
using skylib.Tools;
using System.Collections.Generic;

namespace skylib.sar
{
	public abstract class BaseCommand
	{
		private string usage;
		private List<string> examples;
		private string name;
		
		private List<string> commands;
		private delegate int FunctionPointer (string[] args);
		public Delegate function;
		
		public List<string> Commands
		{
			get
			{
				return this.commands;
			}
		}
		
		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public string Usage
		{
			get
			{
				return this.usage;
			}
		}
		
		public List<string> Examples
		{
			get
			{
				return this.examples;
			}
		}

		protected BaseCommand(string name, List<string> commands, string help, List<string> examples)
		{
			this.function = new FunctionPointer(this.Execute);
			this.usage = help;
			this.name = name;
			this.commands = commands;
			this.examples = examples;
			
			foreach (string command in commands)
			{
				CommandHub.Add(command.ToLower(), this);
			}
		}

		public abstract int Execute(string[] args);
	}

}
