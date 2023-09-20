using System;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.Map
{
	public class WeatherNode
	{
		public bool IsVisuallyDirty { get; private set; }

		public WeatherNode(Vec2 position)
		{
			this.Position = position;
			this.CurrentWeatherEvent = MapWeatherModel.WeatherEvent.Clear;
		}

		public void SetVisualDirty()
		{
			this.IsVisuallyDirty = true;
		}

		public void OnVisualUpdated()
		{
			this.IsVisuallyDirty = false;
		}

		public Vec2 Position;

		public MapWeatherModel.WeatherEvent CurrentWeatherEvent;
	}
}
