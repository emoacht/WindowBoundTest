# Window Bound Test

Test two kinds of window's bounds in WPF.

## Requirements

- .NET 5.0

## Overview

There is a gap (left, right, bottom) between two kinds of window's bounds (location and size) since Windows 8.

- Logical bound: Obtained and specified by most functions related to window's location and size (e.g. GetWindowRect).
- Visual bound: Actually drawn and visualized on the desktop and obtained by DWM functions.

In WPF, this gap changes by window state (normal or maximized) and depending on `WindowStyle`, `AllowTransparency` and `WindowChrome` properties.

If `WindowStyle=None` and `AllowTransparency=True`, the two bounds match but still the issue where the window hides the taskbar when maximized remains. To solve this issue, there is a common hack to interfere when `WM_GETMINMAXINFO` message comes and adjust the window's maximized size to fit the working area of screen.
