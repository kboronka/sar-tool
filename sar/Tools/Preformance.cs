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

using System.Diagnostics;

namespace sar.Tools
{
	public class Preformance
	{
		private static Preformance singleton;

		PerformanceCounter cpu;
		PerformanceCounter ram;

		private Preformance()
		{
			cpu = new PerformanceCounter("Process", "% Processor Time");
			ram = new PerformanceCounter("Memory", "Working Set");
			/*
						cpu = new PerformanceCounter();
						cpu.CategoryName = "Processor";
						cpu.CounterName = "% Processor Time";
						cpu.InstanceName = "_Total";

						ram = new PerformanceCounter();
						ram.CategoryName = "Memory";
						ram.CounterName = "Working Set";
						ram.InstanceName = "_Total";
			*/
		}

		private Preformance(Process p)
		{
			ram = new PerformanceCounter("Process", "Working Set", p.ProcessName);
			cpu = new PerformanceCounter("Process", "% Processor Time", p.ProcessName);
		}

		private static Preformance Singleton
		{
			get
			{
				if (singleton == null)
				{
					singleton = new Preformance();
				}

				return singleton;
			}
		}

		public static string CPU
		{
			get
			{
				double value = Singleton.cpu.NextValue();
				return value.ToString() + "%";
			}
		}

		public static string RAM
		{
			get
			{
				double value = Singleton.ram.NextValue();
				return value.ToString() + "MB";
			}
		}
	}
}
