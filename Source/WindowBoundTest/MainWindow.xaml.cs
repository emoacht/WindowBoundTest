using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shell;

namespace WindowBoundTest
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void Launch_Click(object sender, RoutedEventArgs e)
		{
			var window = new TestWindow();

			if (this.WindowStyleCheckBox.IsChecked is true)
			{
				window.WindowStyle = WindowStyle.None;
			}

			if (this.AllowsTransparencyCheckBox.IsChecked is true)
			{
				window.AllowsTransparency = true;
				window.Opacity = 0.8;
			}

			if (this.WindowChromeCheckBox.IsChecked is true)
			{
				WindowChrome.SetWindowChrome(window, new WindowChrome { GlassFrameThickness = new Thickness(-1) });
			}

			if (this.AdjustsMaximizedBoundCheckBox.IsChecked is true)
			{
				window.AdjustsMaximizedBound = true;
			}

			window.Owner = this;
			window.Show();
		}
	}
}