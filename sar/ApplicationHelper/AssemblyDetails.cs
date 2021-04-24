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

using System;
using System.Collections.Generic;
using System.Reflection;

// http://msdn.microsoft.com/en-us/library/system.reflection.assembly.GetEntryAssembly.aspx
// http://msdn.microsoft.com/en-us/library/system.reflection.assembly.getentryassembly.aspx

namespace sar.ApplicationHelper
{
	public class AssemblyDetails
	{
		public Assembly Assembly { get; private set; }
		public string Name { get; private set; }
		public string Product { get; private set; }
		public string Company { get; private set; }
		public Version Version { get; private set; }
		public string Copyright { get; private set; }
		public List<Assembly> Assemblies { get; private set; }

		public AssemblyDetails()
			: this(GetAssembly())
		{

		}

		public AssemblyDetails(Assembly assembly)
		{
			this.Assembly = assembly;
			this.Name = this.Assembly.GetName().Name;
			this.Version = this.Assembly.GetName().Version;

			object[] attributes = this.Assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
			if (attributes.Length == 0)
			{
				throw new ArgumentNullException("Product name not specified");
			}
			else
			{
				this.Product = ((AssemblyProductAttribute)attributes[0]).Product;
			}

			attributes = this.Assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
			this.Company = attributes.Length == 0 ? "" : ((AssemblyCompanyAttribute)attributes[0]).Company;

			attributes = this.Assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
			this.Copyright = attributes.Length == 0 ? "" : ((AssemblyCopyrightAttribute)attributes[0]).Copyright;

			this.Assemblies = GetReferencedAssemblies(assembly);
		}

		private static List<Assembly> GetReferencedAssemblies(Assembly assembly)
		{
			var assemblies = new List<Assembly>();
			assemblies.Add(assembly);

			foreach (AssemblyName assemblyName in assembly.GetReferencedAssemblies())
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

			return assemblies;
		}

		public static Assembly GetAssembly()
		{
			try
			{
				return Assembly.GetEntryAssembly();
			}
			catch
			{

			}

			try
			{
				return Assembly.GetCallingAssembly();
			}
			catch
			{

			}

			throw new ApplicationException("Assembly Not Found");
		}
	}
}
