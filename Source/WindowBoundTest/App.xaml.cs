using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace WindowBoundTest
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			// https://learn.microsoft.com/en-us/dotnet/framework/migration-guide/mitigation-wpf-window-rendering
			//CoreCompatibilityPreferences.EnableMultiMonitorDisplayClipping = true;

			base.OnStartup(e);
		}
	}
}
