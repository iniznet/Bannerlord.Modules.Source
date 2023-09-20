using System;
using TaleWorlds.CampaignSystem.ComponentInterfaces;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	public class DefaultEmissaryModel : EmissaryModel
	{
		public override int EmissaryRelationBonusForMainClan
		{
			get
			{
				return 5;
			}
		}

		public override bool IsEmissary(Hero hero)
		{
			return (hero.CompanionOf == Clan.PlayerClan || hero.Clan == Clan.PlayerClan) && hero.PartyBelongedTo == null && hero.CurrentSettlement != null && hero.CurrentSettlement.IsFortification && !hero.IsPrisoner && hero.Age >= (float)Campaign.Current.Models.AgeModel.HeroComesOfAge;
		}
	}
}
