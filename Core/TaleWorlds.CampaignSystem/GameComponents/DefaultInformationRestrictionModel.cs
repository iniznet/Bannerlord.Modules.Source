using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Workshops;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	public class DefaultInformationRestrictionModel : InformationRestrictionModel
	{
		public override bool DoesPlayerKnowDetailsOf(Settlement settlement)
		{
			if (settlement.MapFaction == PartyBase.MainParty.MapFaction || settlement.IsInspected)
			{
				return true;
			}
			Settlement settlement2 = (settlement.IsVillage ? settlement.Village.Bound : settlement);
			if (!settlement2.IsFortification)
			{
				return true;
			}
			EmissaryModel emissaryModel = Campaign.Current.Models.EmissaryModel;
			foreach (Hero hero in Clan.PlayerClan.Heroes)
			{
				if (emissaryModel.IsEmissary(hero) && hero.CurrentSettlement == settlement2)
				{
					return true;
				}
			}
			using (List<Workshop>.Enumerator enumerator2 = Hero.MainHero.OwnedWorkshops.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					if (enumerator2.Current.Settlement == settlement2)
					{
						return true;
					}
				}
			}
			using (List<Alley>.Enumerator enumerator3 = Hero.MainHero.OwnedAlleys.GetEnumerator())
			{
				while (enumerator3.MoveNext())
				{
					if (enumerator3.Current.Settlement == settlement2)
					{
						return true;
					}
				}
			}
			return this.IsDisabledByCheat;
		}

		public override bool DoesPlayerKnowDetailsOf(Hero hero)
		{
			return hero.Clan == Clan.PlayerClan || hero.IsDead || (hero.MapFaction != null && hero.MapFaction.IsKingdomFaction && hero.MapFaction.Leader == hero) || hero.IsKnownToPlayer || this.IsDisabledByCheat;
		}

		public bool IsDisabledByCheat;
	}
}
