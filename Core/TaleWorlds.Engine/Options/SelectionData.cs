using System;

namespace TaleWorlds.Engine.Options
{
	public struct SelectionData
	{
		public SelectionData(bool isLocalizationId, string data)
		{
			this.IsLocalizationId = isLocalizationId;
			this.Data = data;
		}

		public bool IsLocalizationId;

		public string Data;
	}
}
