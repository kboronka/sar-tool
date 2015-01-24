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
using System.Collections.Generic;
using System.IO;
using Microsoft.Win32;

using sar.Tools;

namespace sar.Controls
{
	public static class WindowsSettings
	{
		public static int ScrollBarSize
		{
			set
			{
				int size = value * -15;
				
				RegistryKey windowMetricsKey = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop\WindowMetrics\", true);

				if (windowMetricsKey == null) throw new KeyNotFoundException("WindowMetrics key was not found");

				windowMetricsKey.SetValue("ScrollHeight", size, RegistryValueKind.String);
				windowMetricsKey.SetValue("ScrollWidth", size, RegistryValueKind.String);
				windowMetricsKey.Close();
			}
		}
	}
}
