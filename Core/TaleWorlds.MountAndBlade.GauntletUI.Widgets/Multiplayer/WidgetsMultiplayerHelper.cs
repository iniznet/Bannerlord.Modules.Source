using System;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer
{
	public static class WidgetsMultiplayerHelper
	{
		public static string GetFactionColorCode(string lowercaseFactionCode, bool useSecondary)
		{
			if (useSecondary)
			{
				if (lowercaseFactionCode == "aserai")
				{
					return "#4E1A13FF";
				}
				if (lowercaseFactionCode == "battania")
				{
					return "#F3F3F3FF";
				}
				if (lowercaseFactionCode == "empire")
				{
					return "#F1C232FF";
				}
				if (lowercaseFactionCode == "khuzait")
				{
					return "#FFE9D4FF";
				}
				if (lowercaseFactionCode == "sturgia")
				{
					return "#D9D9D9FF";
				}
				if (!(lowercaseFactionCode == "vlandia"))
				{
					return "#000000FF";
				}
				return "#F4CA14FF";
			}
			else
			{
				if (lowercaseFactionCode == "aserai")
				{
					return "#CC8324FF";
				}
				if (lowercaseFactionCode == "battania")
				{
					return "#335F22FF";
				}
				if (lowercaseFactionCode == "empire")
				{
					return "#802463FF";
				}
				if (lowercaseFactionCode == "khuzait")
				{
					return "#418174FF";
				}
				if (lowercaseFactionCode == "sturgia")
				{
					return "#2A5599FF";
				}
				if (!(lowercaseFactionCode == "vlandia"))
				{
					return "#FFFFFFFF";
				}
				return "#8D291AFF";
			}
		}
	}
}
