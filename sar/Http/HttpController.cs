/* Copyright (C) 2015 Kevin Boronka
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
using System.Collections.Generic;
using System.Reflection;


namespace sar.Http
{	
	public class HttpController
	{
		#region static members and methods
		
		private static Dictionary<string, HttpController> controllers;
		
		public static void LoadControllers()
		{
			foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
			{
				if (type.Name.EndsWith("Controller")) HttpController.AddController(type);
			}
			
			foreach (Type type in Assembly.GetEntryAssembly().GetTypes())
			{
				if (type.Name.EndsWith("Controller")) HttpController.AddController(type);
			}
		}
		
		public static void AddController(Type controller)
		{
			if (controllers == null)
			{
				controllers = new Dictionary<string, HttpController>();
			}
			
			string controllerName = controller.Name.Substring(0, controller.Name.Length - "Controller".Length);

			controllers.Add(controllerName, new HttpController(controller));
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
		
		public static HttpContent RequestAction(HttpRequest request)
		{
			string[] urlSplit = request.Path.Split('/');
			string controllerName = urlSplit[0];
			string actionName = urlSplit[1];
			
			if (!controllers.ContainsKey(controllerName)) throw new FileNotFoundException("controller " + @"""" + controllerName + @"""" + " not found");
			HttpController controller = controllers[controllerName];
			
			if (!controller.actions.ContainsKey(actionName)) throw new FileNotFoundException("action " + @"""" + actionName + @"""" + " not found in controller " + @"""" + controllerName + @"""");
			MethodInfo action = controller.actions[actionName];
			
			object contentObject = action.Invoke(null, new object[] { request });
			return (HttpContent)contentObject;
		}
		
		#endregion
		
		private Type type;
		private Dictionary<string, MethodInfo> actions;
		
		public HttpController(Type controller)
		{
			this.type = controller;
			this.actions = new Dictionary<string, MethodInfo>();
			
			foreach (MethodInfo method in controller.GetMethods())
			{
				if (!method.IsSpecialName && method.IsStatic && method.IsPublic && method.ReturnType == typeof(HttpContent) && method.GetParameters().Length == 1 && method.GetParameters()[0].ParameterType == typeof(HttpRequest))
				{
					this.actions.Add(method.Name, method);
				}
			}
		}
	}
}
