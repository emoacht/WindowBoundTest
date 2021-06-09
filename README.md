# Window Bound Test

Test two kinds of window's bounds in WPF.

## Requirements

- .NET 5.0

## Overview

There is a gap (left, right, bottom) between two kinds of window's bounds (location and size) since Windows 8.

- Logical bound: Obtained or specified by most functions related to window's location and size (e.g. GetWindowRect).
- Visual bound: Actually drawn and visualized on the desktop and obtained by [Desktop Window Manager (DWM) functions](https://docs.microsoft.com/en-us/windows/win32/dwm/functions).

In WPF, this gap changes by window state (normal or maximized) and depending on the combination of `WindowStyle`, `AllowsTransparency` and `WindowChrome` properties.

When `WindowStyle=None` and `AllowsTransparency=True`, the two bounds match. But `WindowStyle=None` makes the window to hide the taskbar when maximized. To solve this issue, there is a common hack to catch [WM_GETMINMAXINFO](https://docs.microsoft.com/en-us/windows/win32/winmsg/wm-getminmaxinfo) message and adjust the window's maximized bound to fit the working area of screen.
