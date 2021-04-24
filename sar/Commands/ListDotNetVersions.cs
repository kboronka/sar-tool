/* Copyright (C) 2021 Kevin Boronka
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

using sar.Base;
using sar.Tools;
using System;
using System.Collections.Generic;

namespace sar.Commands
{
	public class ListDotNetVersions : Command
	{
		public ListDotNetVersions(Base.CommandHub parent) : base(parent, ".Net - List Versions",
										 new List<string> { "dotNetVersions" },
										 @"-ip.config",
										 new List<string> { "dotNetVersions" })
		{
		}

		public override int Execute(string[] args)
		{
			// sanity check
			if (args.Length != 1)
			{
				throw new ArgumentException("incorrect number of arguments");
			}

			ConsoleHelper.WriteLine("Installed Versions: ", ConsoleColor.White);

			foreach (string dotNetVersion in IO.GetDotNetVersions())
			{
				ConsoleHelper.WriteLine("  " + dotNetVersion, ConsoleColor.Yellow);
			}

			return ConsoleHelper.EXIT_OK;
		}
	}
}
