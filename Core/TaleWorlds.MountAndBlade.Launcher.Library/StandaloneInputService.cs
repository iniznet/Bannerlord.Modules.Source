using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.TwoDimension.Standalone;

namespace TaleWorlds.MountAndBlade.Launcher.Library
{
	public class StandaloneInputService : IInputService
	{
		public StandaloneInputService(GraphicsForm graphicsForm)
		{
		}

		bool IInputService.MouseEnabled
		{
			get
			{
				return true;
			}
		}

		bool IInputService.KeyboardEnabled
		{
			get
			{
				return true;
			}
		}

		bool IInputService.GamepadEnabled
		{
			get
			{
				return false;
			}
		}
	}
}
