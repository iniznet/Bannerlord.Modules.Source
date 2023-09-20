using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem
{
	public class AtmosphereGrid
	{
		public void Initialize()
		{
			this.states = Campaign.Current.MapSceneWrapper.GetAtmosphereStates().ToList<AtmosphereState>();
		}

		public AtmosphereState GetInterpolatedStateInfo(Vec3 pos)
		{
			AtmosphereGrid.<>c__DisplayClass3_0 CS$<>8__locals1 = new AtmosphereGrid.<>c__DisplayClass3_0();
			CS$<>8__locals1.pos = pos;
			List<AtmosphereGrid.AtmosphereStateSortData> list = new List<AtmosphereGrid.AtmosphereStateSortData>();
			int num = 0;
			foreach (AtmosphereState atmosphereState in this.states)
			{
				list.Add(new AtmosphereGrid.AtmosphereStateSortData
				{
					Position = atmosphereState.Position,
					InitialIndex = num++
				});
			}
			AtmosphereGrid.<>c__DisplayClass3_0 CS$<>8__locals2 = CS$<>8__locals1;
			CS$<>8__locals2.pos.z = CS$<>8__locals2.pos.z * 0.3f;
			list.Sort((AtmosphereGrid.AtmosphereStateSortData x, AtmosphereGrid.AtmosphereStateSortData y) => x.Position.Distance(CS$<>8__locals1.pos).CompareTo(y.Position.Distance(CS$<>8__locals1.pos)));
			AtmosphereState atmosphereState2 = new AtmosphereState();
			float num2 = 0f;
			bool flag = true;
			string text = "color_grade_empire_harsh";
			atmosphereState2.ColorGradeTexture = text;
			foreach (AtmosphereGrid.AtmosphereStateSortData atmosphereStateSortData in list)
			{
				AtmosphereState atmosphereState3 = this.states[atmosphereStateSortData.InitialIndex];
				float num3 = atmosphereState3.Position.Distance(CS$<>8__locals1.pos);
				float num4 = 1f - MBMath.SmoothStep(atmosphereState3.distanceForMaxWeight, atmosphereState3.distanceForMinWeight, num3);
				if ((double)num4 >= 0.001)
				{
					if (flag)
					{
						text = atmosphereState3.ColorGradeTexture;
					}
					atmosphereState2.HumidityAverage += atmosphereState3.HumidityAverage * num4;
					atmosphereState2.HumidityVariance += atmosphereState3.HumidityVariance * num4;
					atmosphereState2.TemperatureAverage += atmosphereState3.TemperatureAverage * num4;
					atmosphereState2.TemperatureVariance += atmosphereState3.TemperatureVariance * num4;
					num2 += num4;
					flag = false;
				}
			}
			if (num2 > 0f)
			{
				atmosphereState2.ColorGradeTexture = text;
				atmosphereState2.HumidityAverage /= num2;
				atmosphereState2.HumidityVariance /= num2;
				atmosphereState2.TemperatureAverage /= num2;
				atmosphereState2.TemperatureVariance /= num2;
			}
			return atmosphereState2;
		}

		private List<AtmosphereState> states = new List<AtmosphereState>();

		private struct AtmosphereStateSortData
		{
			public Vec3 Position;

			public int InitialIndex;
		}
	}
}
