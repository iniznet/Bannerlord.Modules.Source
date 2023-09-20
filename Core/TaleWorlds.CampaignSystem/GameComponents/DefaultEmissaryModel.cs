using System;
using TaleWorlds.CampaignSystem.ComponentInterfaces;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	// Token: 0x02000107 RID: 263
	public class DefaultEmissaryModel : EmissaryModel
	{
		// Token: 0x170005E9 RID: 1513
		// (get) Token: 0x0600157B RID: 5499 RVA: 0x00065414 File Offset: 0x00063614
		public override int EmissaryRelationBonusForMainClan
		{
			get
			{
				return 5;
			}
		}

		// Token: 0x0600157C RID: 5500 RVA: 0x00065418 File Offset: 0x00063618
		public override bool IsEmissary(Hero hero)
		{
			return (hero.CompanionOf == Clan.PlayerClan || hero.Clan == Clan.PlayerClan) && hero.PartyBelongedTo == null && hero.CurrentSettlement != null && hero.CurrentSettlement.IsFortification && !hero.IsPrisoner && hero.Age >= (float)Campaign.Current.Models.AgeModel.HeroComesOfAge;
		}
	}
}
