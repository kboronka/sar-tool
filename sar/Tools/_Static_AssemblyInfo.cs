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
using System.Reflection;

// http://msdn.microsoft.com/en-us/library/system.reflection.assembly.GetEntryAssembly.aspx
// http://msdn.microsoft.com/en-us/library/system.reflection.assembly.getentryassembly.aspx

namespace sar.Tools
{
	public static class AssemblyInfo
	{
		private static Assembly assembly;
		public static Assembly Assembly
		{
			get
			{
				if (AssemblyInfo.assembly == null)
				{
					try
					{
						AssemblyInfo.assembly = Assembly.GetEntryAssembly();
					}
					catch
					{
						
					}
					
					try
					{
						if (AssemblyInfo.assembly == null)
						{
							AssemblyInfo.assembly = Assembly.GetCallingAssembly();
						}
					}
					catch
					{
						
					}
				}
				
				return AssemblyInfo.assembly;
			}
			set
			{
				AssemblyInfo.assembly = value;
			}
		}
		
		private static string name;
		public static string Name
		{
			get
			{
				if (string.IsNullOrEmpty(AssemblyInfo.name))
				{
					AssemblyInfo.name = AssemblyInfo.Assembly.GetName().Name;
				}
				
				return AssemblyInfo.name;
			}
		}
		
		private static string product;
		public static string Product
		{
			get
			{
				if (string.IsNullOrEmpty(AssemblyInfo.product))
				{
					if (AssemblyInfo.Assembly != null)
					{
						object[] attributes = AssemblyInfo.Assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
						AssemblyInfo.product = attributes.Length == 0 ? "" : ((AssemblyProductAttribute)attributes[0]).Product;
					}
					else
					{
						AssemblyInfo.product = "unknown";
					}
				}
				
				return AssemblyInfo.product;
			}		
		}

		private static string company;
		public static string Company
		{
			get
			{
				if (string.IsNullOrEmpty(AssemblyInfo.company))
				{
					object[] attributes = AssemblyInfo.Assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
					AssemblyInfo.company = attributes.Length == 0 ? "" : ((AssemblyCompanyAttribute)attributes[0]).Company;
				}
				
				return AssemblyInfo.company;
			}
		}

		private static string version;
		public static string Version
		{
			get
			{
				if (string.IsNullOrEmpty(AssemblyInfo.version))
				{
					AssemblyInfo.version = AssemblyInfo.Assembly.GetName().Version.ToString();
				}
				
				return AssemblyInfo.version;
			}
		}
		
		private static string sarVersion;
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
		
		private static string copyright;
		public static string Copyright
		{
			get
			{
				if (string.IsNullOrEmpty(AssemblyInfo.copyright))
				{
					object[] attributes = AssemblyInfo.Assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
					AssemblyInfo.copyright = attributes.Length == 0 ? "" : ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
				}
				
				return AssemblyInfo.copyright;
			}
		}

		private static List<Assembly> assemblies;
		public static List<Assembly> Assemblies
		{
			get
			{
				var locations = new List<string>();
				if (assemblies == null || assemblies.Count == 0)
				{
					assemblies = new List<Assembly>();
					//assemblies.Add(Assembly.GetExecutingAssembly());
					assemblies.Add(AssemblyInfo.Assembly);
					foreach (AssemblyName assemblyName in AssemblyInfo.Assembly.GetReferencedAssemblies())
					{
						string name = assemblyName.Name;
						
						if (!name.StartsWith("System")
						    && !name.StartsWith("mscorlib")
						    && !name.StartsWith("Microsoft.")
						    && !name.StartsWith("CefSharp."))
						{
							try
							{
								assemblies.Add(Assembly.Load(assemblyName));
							}
							catch
							{
								
							}
						}
					}
				}
				
				return assemblies;
			}
		}
	}
}
