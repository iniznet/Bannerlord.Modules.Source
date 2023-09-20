using System;

namespace TaleWorlds.TwoDimension.Standalone.Native.Windows
{
	public enum WindowMessage : uint
	{
		Quit = 18U,
		Close = 16U,
		Size = 5U,
		KeyDown = 256U,
		KeyUp,
		RightButtonUp = 517U,
		RightButtonDown = 516U,
		LeftButtonUp = 514U,
		LeftButtonDown = 513U,
		MouseMove = 512U,
		MouseWheel = 522U,
		KillFocus = 8U,
		SetFocus = 7U
	}
}
