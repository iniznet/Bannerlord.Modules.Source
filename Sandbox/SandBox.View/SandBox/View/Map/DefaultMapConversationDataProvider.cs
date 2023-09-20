﻿using System;
using TaleWorlds.Core;

namespace SandBox.View.Map
{
	public class DefaultMapConversationDataProvider : IMapConversationDataProvider
	{
		string IMapConversationDataProvider.GetAtmosphereNameFromData(MapConversationTableauData data)
		{
			string text;
			if (data.TimeOfDay <= 3f || data.TimeOfDay >= 21f)
			{
				text = "night";
			}
			else if (data.TimeOfDay > 8f && data.TimeOfDay < 16f)
			{
				text = "noon";
			}
			else
			{
				text = "sunset";
			}
			if (data.Settlement == null || data.Settlement.IsHideout)
			{
				if (data.IsCurrentTerrainUnderSnow)
				{
					return "conv_snow_" + text + "_0";
				}
				switch (data.ConversationTerrainType)
				{
				case 3:
					return "conv_steppe_" + text + "_0";
				case 5:
					return "conv_desert_" + text + "_0";
				case 10:
					return "conv_forest_" + text + "_0";
				}
				return "conv_plains_" + text + "_0";
			}
			else
			{
				string text2 = Enum.GetName(typeof(CultureCode), data.Settlement.Culture.GetCultureCode()).ToLower();
				if (data.IsInside)
				{
					return "conv_" + text2 + "_lordshall_0";
				}
				return string.Concat(new string[] { "conv_", text2, "_town_", text, "_0" });
			}
		}
	}
}
