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
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Security.Principal;

namespace sar.Tools
{
	public static class ActiveDirectory
	{
		public static string CurrentUserName()
		{
			string username = WindowsIdentity.GetCurrent().Name;
			if (username.LastIndexOf(@"\") != -1)
			{
				return username.Substring(username.LastIndexOf(@"\") + 1);
			}
			
			return username;
		}
		
		public static string GetDistinguishedName(string username)
		{
			DirectorySearcher searcher = new DirectorySearcher();
			searcher.SearchScope = SearchScope.Subtree;
			searcher.PropertiesToLoad.Add("distinguishedName");
			searcher.PageSize = 1;
			searcher.ServerPageTimeLimit = TimeSpan.FromSeconds(2);
			searcher.Filter = "(&(objectCategory=user)(sAMAccountName=" + username + "))";

			SearchResult searchResult = searcher.FindOne();

			if (searchResult == null) throw new Exception ("no result found for " + username);
			
			return searchResult.Properties["distinguishedName"][0].ToString();
		}
	}
}