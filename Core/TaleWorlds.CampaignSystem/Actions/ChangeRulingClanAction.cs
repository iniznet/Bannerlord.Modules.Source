using System;

namespace TaleWorlds.CampaignSystem.Actions
{
	internal class ChangeRulingClanAction
	{
		private static void ApplyInternal(Kingdom kingdom, Clan clan)
		{
			kingdom.RulingClan = clan;
			CampaignEventDispatcher.Instance.OnRulingClanChanged(kingdom, clan);
		}

		public static void Apply(Kingdom kingdom, Clan clan)
		{
			ChangeRulingClanAction.ApplyInternal(kingdom, clan);
		}
	}
}
