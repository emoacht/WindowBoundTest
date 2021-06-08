using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shell;

namespace WindowBoundTest
{
	public partial class TestWindow : Window
	{
		public TestWindow()
		{
			InitializeComponent();

			this.Loaded += OnLoaded;
		}

		public bool AdjustsMaximizedBound { get; set; }

		private HwndSource _targetSource;

		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);

			if (AdjustsMaximizedBound)
			{
				_targetSource = PresentationSource.FromVisual(this) as HwndSource;
				_targetSource?.AddHook(WndProc);
			}
		}

		private const int WM_GETMINMAXINFO = 0x0024;

		private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			switch (msg)
			{
				case WM_GETMINMAXINFO:
					if (WindowHelper.TryGetMonitorRect(hwnd, out Rect monitorRect, out Rect workRect))
					{
						// Get maximized location and size excluding taskbar.
						var maximizedLocation = new Point(workRect.X - monitorRect.X, workRect.Y - monitorRect.Y);
						var maximizedSize = new Point(workRect.Width, workRect.Height);

						var info = Marshal.PtrToStructure<WindowHelper.MINMAXINFO>(lParam);
						info.ptMaxPosition = maximizedLocation;
						info.ptMaxSize = maximizedSize;
						Marshal.StructureToPtr(info, lParam, true);
					}
					break;
			}
			return IntPtr.Zero;
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			this.Status.Text =
				$"WindowStyle={this.WindowStyle}\r\n" +
				$"AllowsTransparency={this.AllowsTransparency}\r\n" +
				$"WindowChrome={((WindowChrome.GetWindowChrome(this) is not null) ? "Yes" : "No")}\r\n" +
				$"AdjustsMaximizedBound={this.AdjustsMaximizedBound}";

			Check();
		}

		private void Maximize_Click(object sender, RoutedEventArgs e)
		{
			switch (this.WindowState)
			{
				case WindowState.Normal:
					this.WindowState = WindowState.Maximized;
					break;

				case WindowState.Maximized:
					this.WindowState = WindowState.Normal;
					break;
			}

			Check();
		}

		private void Check()
		{
			WindowHelper.TryGetDwmWindowMargin(this, out Rect baseRect, out Rect dwmRect, out Thickness margin);

			this.Result.Text =
				$"Base Rect Location:{baseRect.Left},{baseRect.Top} Size:{baseRect.Width}-{baseRect.Height}\r\n" +
				$"DWM Rect  Location:{dwmRect.Left},{dwmRect.Top} Size:{dwmRect.Width}-{dwmRect.Height}\r\n" +
				$"Margin    {margin}";
		}

		private void Close_Click(object sender, RoutedEventArgs e) => this.Close();
	}
}