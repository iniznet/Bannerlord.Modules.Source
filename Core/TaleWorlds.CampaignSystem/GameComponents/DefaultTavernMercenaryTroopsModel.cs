using System;
using TaleWorlds.CampaignSystem.ComponentInterfaces;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	public class DefaultTavernMercenaryTroopsModel : TavernMercenaryTroopsModel
	{
		public override float RegularMercenariesSpawnChance
		{
			get
			{
				return 0.7f;
			}
		}
	}
}
