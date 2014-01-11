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
using System.IO;

namespace sar.Tools
{
	public class ApplicationInfo
	{
		private static string commonDataDirectory;
		public static string CommonDataDirectory
		{
			get
			{	
				if (string.IsNullOrEmpty(ApplicationInfo.commonDataDirectory))
				{
					string root = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
					string product = AssemblyInfo.Product;
					string company = AssemblyInfo.Company;
					
					if (String.IsNullOrEmpty(product))
						throw new ArgumentNullException("Product name not specified in AssemblyInfo");

					if (!String.IsNullOrEmpty(company))
						root += "\\" + company;						
					
					root += "\\" + product + "\\";
					
					if (!Directory.Exists(root))
						Directory.CreateDirectory(root);

					ApplicationInfo.commonDataDirectory = root;
				}
				
				return ApplicationInfo.commonDataDirectory;
			}
		}

		private static string localDataDirectory;
		public static string DataDirectory
		{
			get
			{
				if (string.IsNullOrEmpty(ApplicationInfo.localDataDirectory))
				{
					string root = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
					string product = AssemblyInfo.Product;
					string company = AssemblyInfo.Company;
					
					if (String.IsNullOrEmpty(product))
						throw new ArgumentNullException("Product name not specified in AssemblyInfo");

					if (!String.IsNullOrEmpty(company))
						root += "\\" + company;						
					
					root += "\\" + product + "\\";
					
					if (!Directory.Exists(root))
						Directory.CreateDirectory(root);

					ApplicationInfo.localDataDirectory = root;
				}
				
				return ApplicationInfo.localDataDirectory;
			}
		}


		private static string currentDirectory;
		public static string CurrentDirectory
		{
			get
			{
				if (string.IsNullOrEmpty(ApplicationInfo.currentDirectory))
				{
					ApplicationInfo.currentDirectory = Directory.GetCurrentDirectory();
				}
				return ApplicationInfo.currentDirectory;
			}
		}
		
		private static applicationPath;
		public static string ApplicationPath
		{
			get
			{
				if (string.IsNullOrEmpty(applicationPath))
				{
					applicationPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
				}
				
				return applicationPath;
			}
		}
	}
}