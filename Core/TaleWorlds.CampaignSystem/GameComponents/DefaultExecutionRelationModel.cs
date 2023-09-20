using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	// Token: 0x0200010B RID: 267
	public class DefaultExecutionRelationModel : ExecutionRelationModel
	{
		// Token: 0x170005EF RID: 1519
		// (get) Token: 0x0600159D RID: 5533 RVA: 0x0006639C File Offset: 0x0006459C
		public override int HeroKillingHeroClanRelationPenalty
		{
			get
			{
				return -40;
			}
		}

		// Token: 0x170005F0 RID: 1520
		// (get) Token: 0x0600159E RID: 5534 RVA: 0x000663A0 File Offset: 0x000645A0
		public override int HeroKillingHeroFriendRelationPenalty
		{
			get
			{
				return -10;
			}
		}

		// Token: 0x170005F1 RID: 1521
		// (get) Token: 0x0600159F RID: 5535 RVA: 0x000663A4 File Offset: 0x000645A4
		public override int PlayerExecutingHeroFactionRelationPenaltyDishonorable
		{
			get
			{
				return -5;
			}
		}

		// Token: 0x170005F2 RID: 1522
		// (get) Token: 0x060015A0 RID: 5536 RVA: 0x000663A8 File Offset: 0x000645A8
		public override int PlayerExecutingHeroClanRelationPenaltyDishonorable
		{
			get
			{
				return -30;
			}
		}

		// Token: 0x170005F3 RID: 1523
		// (get) Token: 0x060015A1 RID: 5537 RVA: 0x000663AC File Offset: 0x000645AC
		public override int PlayerExecutingHeroFriendRelationPenaltyDishonorable
		{
			get
			{
				return -15;
			}
		}

		// Token: 0x170005F4 RID: 1524
		// (get) Token: 0x060015A2 RID: 5538 RVA: 0x000663B0 File Offset: 0x000645B0
		public override int PlayerExecutingHeroHonorPenalty
		{
			get
			{
				return -1000;
			}
		}

		// Token: 0x170005F5 RID: 1525
		// (get) Token: 0x060015A3 RID: 5539 RVA: 0x000663B7 File Offset: 0x000645B7
		public override int PlayerExecutingHeroFactionRelationPenalty
		{
			get
			{
				return -10;
			}
		}

		// Token: 0x170005F6 RID: 1526
		// (get) Token: 0x060015A4 RID: 5540 RVA: 0x000663BB File Offset: 0x000645BB
		public override int PlayerExecutingHeroHonorableNobleRelationPenalty
		{
			get
			{
				return -10;
			}
		}

		// Token: 0x170005F7 RID: 1527
		// (get) Token: 0x060015A5 RID: 5541 RVA: 0x000663BF File Offset: 0x000645BF
		public override int PlayerExecutingHeroClanRelationPenalty
		{
			get
			{
				return -60;
			}
		}

		// Token: 0x170005F8 RID: 1528
		// (get) Token: 0x060015A6 RID: 5542 RVA: 0x000663C3 File Offset: 0x000645C3
		public override int PlayerExecutingHeroFriendRelationPenalty
		{
			get
			{
				return -30;
			}
		}

		// Token: 0x060015A7 RID: 5543 RVA: 0x000663C8 File Offset: 0x000645C8
		public override int GetRelationChangeForExecutingHero(Hero victim, Hero hero, out bool showQuickNotification)
		{
			int num = 0;
			showQuickNotification = false;
			if (victim.GetTraitLevel(DefaultTraits.Honor) < 0)
			{
				if (!hero.IsHumanPlayerCharacter && hero != victim && hero.Clan != null && hero.Clan.Leader == hero)
				{
					if (hero.Clan == victim.Clan)
					{
						num = Campaign.Current.Models.ExecutionRelationModel.PlayerExecutingHeroClanRelationPenaltyDishonorable;
						showQuickNotification = true;
					}
					else if (victim.IsFriend(hero))
					{
						num = Campaign.Current.Models.ExecutionRelationModel.PlayerExecutingHeroFriendRelationPenaltyDishonorable;
						showQuickNotification = true;
					}
					else if (hero.MapFaction == victim.MapFaction && hero.CharacterObject.Occupation == Occupation.Lord)
					{
						num = Campaign.Current.Models.ExecutionRelationModel.PlayerExecutingHeroFactionRelationPenaltyDishonorable;
						showQuickNotification = true;
					}
				}
			}
			else if (!hero.IsHumanPlayerCharacter && hero != victim && hero.Clan != null && hero.Clan.Leader == hero)
			{
				if (hero.Clan == victim.Clan)
				{
					num = Campaign.Current.Models.ExecutionRelationModel.PlayerExecutingHeroClanRelationPenalty;
					showQuickNotification = true;
				}
				else if (victim.IsFriend(hero))
				{
					num = Campaign.Current.Models.ExecutionRelationModel.PlayerExecutingHeroFriendRelationPenalty;
					showQuickNotification = true;
				}
				else if (hero.MapFaction == victim.MapFaction && hero.CharacterObject.Occupation == Occupation.Lord)
				{
					num = Campaign.Current.Models.ExecutionRelationModel.PlayerExecutingHeroFactionRelationPenalty;
					showQuickNotification = false;
				}
				else if (hero.GetTraitLevel(DefaultTraits.Honor) > 0 && !victim.Clan.IsRebelClan)
				{
					num = Campaign.Current.Models.ExecutionRelationModel.PlayerExecutingHeroHonorableNobleRelationPenalty;
					showQuickNotification = true;
				}
			}
			return num;
		}
	}
}
