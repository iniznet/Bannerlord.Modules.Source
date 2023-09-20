using System;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade.ComponentInterfaces;

namespace TaleWorlds.MountAndBlade
{
	public class CustomBattleApplyWeatherEffectsModel : ApplyWeatherEffectsModel
	{
		public override void ApplyWeatherEffects()
		{
			Scene scene = Mission.Current.Scene;
			if (scene != null)
			{
				bool flag = scene.GetRainDensity() > 0f;
				bool flag2 = scene.GetSnowDensity() > 0f;
				bool flag3 = flag || flag2;
				bool flag4 = scene.GetFog() > 0f;
				Mission.Current.SetBowMissileSpeedModifier(flag3 ? 0.9f : 1f);
				Mission.Current.SetCrossbowMissileSpeedModifier(flag3 ? 0.9f : 1f);
				Mission.Current.SetMissileRangeModifier(flag4 ? 0.8f : 1f);
			}
		}
	}
}
