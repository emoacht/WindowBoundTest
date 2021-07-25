using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace WindowBoundTest
{
	internal static class WindowHelper
	{
		#region Win32

		[DllImport("User32.dll")]
		private static extern IntPtr MonitorFromWindow(
			IntPtr hwnd,
			MONITOR_DEFAULTTO dwFlags);

		private enum MONITOR_DEFAULTTO : uint
		{
			MONITOR_DEFAULTTONULL = 0x00000000,
			MONITOR_DEFAULTTOPRIMARY = 0x00000001,
			MONITOR_DEFAULTTONEAREST = 0x00000002,
		}

		[DllImport("User32.dll", CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetMonitorInfo(
			IntPtr hMonitor,
			ref MONITORINFO lpmi);

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct MONITORINFO
		{
			public uint cbSize;
			public RECT rcMonitor;
			public RECT rcWork;
			public uint dwFlags;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct POINT
		{
			public int x;
			public int y;

			public static implicit operator Point(POINT point) => new Point(point.x, point.y);
			public static implicit operator POINT(Point point) => new POINT { x = (int)point.X, y = (int)point.Y };
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct RECT
		{
			public int left;
			public int top;
			public int right;
			public int bottom;

			public static implicit operator Rect(RECT rect)
			{
				if ((rect.right < rect.left) || (rect.bottom < rect.top))
					return Rect.Empty;

				return new Rect(
					rect.left,
					rect.top,
					rect.right - rect.left,
					rect.bottom - rect.top);
			}

			public static implicit operator RECT(Rect rect)
			{
				return new RECT
				{
					left = (int)rect.Left,
					top = (int)rect.Top,
					right = (int)rect.Right,
					bottom = (int)rect.Bottom
				};
			}
		}

		[DllImport("User32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool GetWindowRect(
			IntPtr hWnd,
			out RECT lpRect);

		[DllImport("Dwmapi.dll", SetLastError = true)]
		private static extern int DwmGetWindowAttribute(
			IntPtr hwnd,
			uint dwAttribute,
			out RECT pvAttribute, // IntPtr
			uint cbAttribute);

		private const uint DWMWA_EXTENDED_FRAME_BOUNDS = 9;

		public const int S_OK = 0x0;
		public const int S_FALSE = 0x1;

		[StructLayout(LayoutKind.Sequential)]
		public struct MINMAXINFO
		{
			public POINT ptReserved;
			public POINT ptMaxSize;
			public POINT ptMaxPosition;
			public POINT ptMinTrackSize;
			public POINT ptMaxTrackSize;
		}

		#endregion

		#region Monitor

		public static bool TryGetMonitorRect(IntPtr windowHandle, out Rect monitorRect, out Rect workRect)
		{
			var monitorHandle = MonitorFromWindow(
				windowHandle,
				MONITOR_DEFAULTTO.MONITOR_DEFAULTTONULL);
			if (monitorHandle != IntPtr.Zero)
			{
				var monitorInfo = new MONITORINFO { cbSize = (uint)Marshal.SizeOf<MONITORINFO>() };

				if (GetMonitorInfo(
					monitorHandle,
					ref monitorInfo))
				{
					monitorRect = monitorInfo.rcMonitor;
					workRect = monitorInfo.rcWork;
					return true;
				}
			}
			monitorRect = Rect.Empty;
			workRect = Rect.Empty;
			return false;
		}

		#endregion

		#region Window

		public static bool TryGetWindowRect(Window window, out Rect windowRect)
		{
			var windowHandle = new WindowInteropHelper(window).Handle;

			if (GetWindowRect(
				windowHandle,
				out RECT rect))
			{
				windowRect = rect;
				return true;
			}
			windowRect = Rect.Empty;
			return false;
		}

		public static bool TryGetDwmWindowRect(Window window, out Rect windowRect)
		{
			var windowHandle = new WindowInteropHelper(window).Handle;

			if (DwmGetWindowAttribute(
				windowHandle,
				DWMWA_EXTENDED_FRAME_BOUNDS,
				out RECT rect,
				(uint)Marshal.SizeOf<RECT>()) == S_OK)
			{
				windowRect = rect;
				return true;
			}
			windowRect = Rect.Empty;
			return false;
		}

		public static bool TryGetDwmWindowMargin(Window window, out Rect windowBaseRect, out Rect windowDwmRect, out Thickness windowMargin)
		{
			var windowHandle = new WindowInteropHelper(window).Handle;

			if (GetWindowRect(
				windowHandle,
				out RECT baseRect))
			{
				if (DwmGetWindowAttribute(
					windowHandle,
					DWMWA_EXTENDED_FRAME_BOUNDS,
					out RECT dwmRect,
					(uint)Marshal.SizeOf<RECT>()) == S_OK)
				{
					windowBaseRect = baseRect;
					windowDwmRect = dwmRect;
					windowMargin = new Thickness(
						dwmRect.left - baseRect.left,
						dwmRect.top - baseRect.top,
						baseRect.right - dwmRect.right,
						baseRect.bottom - dwmRect.bottom);
					return true;
				}
			}
			windowBaseRect = default;
			windowDwmRect = default;
			windowMargin = default;
			return false;
		}

		#endregion
	}
}