﻿using System;

namespace TaleWorlds.TwoDimension.Standalone.Native.Windows
{
	[Flags]
	public enum WindowStyle : uint
	{
		Overlapped = 0U,
		OverlappedWindow = 13565952U,
		WS_POPUP = 2147483648U,
		WS_CHILD = 1073741824U,
		WS_MINIMIZE = 536870912U,
		WS_VISIBLE = 268435456U,
		WS_DISABLED = 134217728U,
		WS_CLIPSIBLINGS = 67108864U,
		WS_CLIPCHILDREN = 33554432U,
		WS_MAXIMIZE = 16777216U,
		WS_CAPTION = 12582912U,
		WS_BORDER = 8388608U,
		WS_DLGFRAME = 4194304U,
		WS_VSCROLL = 2097152U,
		WS_HSCROLL = 1048576U,
		WS_SYSMENU = 524288U,
		WS_THICKFRAME = 262144U,
		WS_GROUP = 131072U,
		WS_TABSTOP = 65536U
	}
}
