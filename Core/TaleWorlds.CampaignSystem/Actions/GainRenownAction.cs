using System;

namespace TaleWorlds.CampaignSystem.Actions
{
	public static class GainRenownAction
	{
		private static void ApplyInternal(Hero hero, float gainedRenown, bool doNotNotify)
		{
			if (gainedRenown > 0f)
			{
				hero.Clan.AddRenown(gainedRenown, true);
				CampaignEventDispatcher.Instance.OnRenownGained(hero, (int)gainedRenown, doNotNotify);
			}
		}

		public static void Apply(Hero hero, float renownValue, bool doNotNotify = false)
		{
			GainRenownAction.ApplyInternal(hero, renownValue, doNotNotify);
		}
	}
}
