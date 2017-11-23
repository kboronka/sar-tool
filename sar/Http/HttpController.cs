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
using System.IO;
using System.Reflection;

using sar.Tools;

namespace sar.Http
{
	public class HttpController
	{
		#region static members and methods
		
		private static Dictionary<string, HttpController> controllers;
		private static HttpController primary;
		
		public static HttpController Primary { get { return primary; } }
		
		public static void LoadControllers()
		{
			HttpController.controllers = new Dictionary<string, HttpController>();
			
			foreach (Assembly assembly in AssemblyInfo.Assemblies)
			{
				foreach (Type type in assembly.GetTypes())
				{
					if (type.Name.EndsWith("Controller"))
					{
						foreach (object attribute in type.GetCustomAttributes(false))
						{
							if (attribute is SarController)
							{
								// add the sar controller
								string controllerName = type.Name.Substring(0, type.Name.Length - "Controller".Length);
								HttpController.controllers.Add(controllerName, new HttpController(type));
							}
						}
					}
				}
			}
		}
		
		public static bool ActionExists(HttpRequest request)
		{
			string[] urlSplit = request.Path.Split('/');
			
			if (urlSplit.Length != 2) return false;
			
			string controllerName = urlSplit[0];
			string actionName = urlSplit[1];
			if (!controllers.ContainsKey(controllerName)) return false;
			if (!controllers[controllerName].actions.ContainsKey(actionName)) return false;
			
			return true;
		}
		
		public static HttpContent RequestPrimary(HttpRequest request)
		{
			object contentObject = HttpController.primary.primaryAction.Invoke(null, new object[] { request });
			return (HttpContent)contentObject;
		}
		
		public static HttpContent RequestAction(HttpRequest request)
		{
			string[] urlSplit = request.Path.Split('/');
			string controllerName = urlSplit[0];
			string actionName = urlSplit[1];
			
			return RequestAction(controllerName, actionName, request);
		}

		public static HttpContent RequestAction(string controllerName, string actionName, HttpRequest request)
		{
			if (!controllers.ContainsKey(controllerName)) throw new FileNotFoundException("controller " + @"""" + controllerName + @"""" + " not found");
			HttpController controller = controllers[controllerName];
			
			if (!controller.actions.ContainsKey(actionName)) throw new FileNotFoundException("action " + @"""" + actionName + @"""" + " not found in controller " + @"""" + controllerName + @"""");
			MethodInfo action = controller.actions[actionName];
			
			object contentObject = action.Invoke(null, new object[] { request });
			return (HttpContent)contentObject;
		}
		
		#endregion
		
		private Type type;
		private MethodInfo primaryAction;
		private Dictionary<string, MethodInfo> actions;
		
		public string FullName
		{
			get { return type.FullName; }
		}
		
		public string Name
		{
			get { return type.Name; }
		}
		
		public MethodInfo PrimaryAction
		{
			get { return primaryAction; }
		}
		
		public HttpController(Type controller)
		{
			this.type = controller;
			this.actions = new Dictionary<string, MethodInfo>();
			
			if (controller.Assembly == AssemblyInfo.Assembly)
			{
				foreach (object obj in controller.GetCustomAttributes(false))
				{
					if (obj is PrimaryController) HttpController.primary = this;
				}
			}
			
			foreach (MethodInfo method in controller.GetMethods())
			{
				if (!method.IsSpecialName && method.IsStatic && method.IsPublic && method.ReturnType == typeof(HttpContent) && method.GetParameters().Length == 1 && method.GetParameters()[0].ParameterType == typeof(HttpRequest))
				{
					this.actions.Add(method.Name, method);
					foreach (object obj in method.GetCustomAttributes(false))
					{
						if (obj is PrimaryView) this.primaryAction = method;
						if (obj is ViewAlias) this.actions.Add(((ViewAlias)obj).Alias, method);
					}
				}
			}
		}
	}

	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public class PrimaryView : Attribute { }

	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class ViewAlias : Attribute
	{
		public string Alias { get; private set; }
		
		public ViewAlias(string alias)
		{
			this.Alias = alias;
		}
	}
	
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class PrimaryController : Attribute { }

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class SarController : Attribute { }
}
