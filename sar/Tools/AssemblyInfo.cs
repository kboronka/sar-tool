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
using System.Reflection;

// http://msdn.microsoft.com/en-us/library/system.reflection.assembly.GetEntryAssembly.aspx
// http://msdn.microsoft.com/en-us/library/system.reflection.assembly.getentryassembly.aspx

namespace sar.Tools
{
	public class AssemblyInfo
	{
		private static string name;
		private static string product;
		private static string company;
		private static string version;
		private static string sarVersion;
		private static string copyright;

		public static string Name
		{
			get
			{
				if (string.IsNullOrEmpty(AssemblyInfo.name))
				{
					AssemblyInfo.name = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
				}
				
				return AssemblyInfo.name;
			}
		}
		
		public static string Product
		{
			get
			{
				if (string.IsNullOrEmpty(AssemblyInfo.product))
				{
					object[] attributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
					AssemblyInfo.product = attributes.Length == 0 ? "" : ((AssemblyProductAttribute)attributes[0]).Product;
				}
				
				return AssemblyInfo.product;
			}
		}

		public static string Company
		{
			get
			{
				if (string.IsNullOrEmpty(AssemblyInfo.company))
				{
					object[] attributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
					AssemblyInfo.company = attributes.Length == 0 ? "" : ((AssemblyCompanyAttribute)attributes[0]).Company;
				}
				
				return AssemblyInfo.company;
			}
		}

		public static string Version
		{
			get
			{
				if (string.IsNullOrEmpty(AssemblyInfo.version))
				{
					AssemblyInfo.version = Assembly.GetEntryAssembly().GetName().Version.ToString();
				}
				
				return AssemblyInfo.version;
			}
		}
		
		public static string SarVersion
		{
			get
			{
				if (string.IsNullOrEmpty(AssemblyInfo.sarVersion))
				{
					AssemblyInfo.sarVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
				}
				
				return AssemblyInfo.sarVersion;
			}
		}		
		
		public static string Copyright
		{
			get
			{
				if (string.IsNullOrEmpty(AssemblyInfo.copyright))
				{
					object[] attributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
					AssemblyInfo.copyright = attributes.Length == 0 ? "" : ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
				}
				
				return AssemblyInfo.copyright;
			}
		}
	}
}
