/* Copyright (C) 2013 Kevin Boronka
 * 
 * software is distributed under the BSD license
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
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace skylib.sar
{
	public class Delay : BaseCommand
	{
		public Delay() : base("Delay",
		                      new List<string> { "delay" },
		                      @"-delay <milliseconds>",
		                      new List<string> { "-delay 5000" })
		{
		}
		
		public override int Execute(string[] args)
		{
			// sanity check
			if (args.Length != 2)
			{
				throw new ArgumentException("incorrect number of arguments");
			}
			
			long delay;
			if (!long.TryParse(args[1], out delay) || delay < 0 || delay > long.MaxValue)
			{
				throw new ArgumentException("invalid delay value");
			}
			
			
			Stopwatch timer = new Stopwatch();
			timer.Start();
			
			while (timer.ElapsedMilliseconds < delay)
			{
				Thread.Sleep(50);
				long timeremaining = (delay - timer.ElapsedMilliseconds);
				Progress.Message = timeremaining.ToString() + "ms ";
			}
			
			return Program.EXIT_OK;
		}
	}
}
