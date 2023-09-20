using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	public class DefaultExecutionRelationModel : ExecutionRelationModel
	{
		public override int HeroKillingHeroClanRelationPenalty
		{
			get
			{
				return -40;
			}
		}

		public override int HeroKillingHeroFriendRelationPenalty
		{
			get
			{
				return -10;
			}
		}

		public override int PlayerExecutingHeroFactionRelationPenaltyDishonorable
		{
			get
			{
				return -5;
			}
		}

		public override int PlayerExecutingHeroClanRelationPenaltyDishonorable
		{
			get
			{
				return -30;
			}
		}

		public override int PlayerExecutingHeroFriendRelationPenaltyDishonorable
		{
			get
			{
				return -15;
			}
		}

		public override int PlayerExecutingHeroHonorPenalty
		{
			get
			{
				return -1000;
			}
		}

		public override int PlayerExecutingHeroFactionRelationPenalty
		{
			get
			{
				return -10;
			}
		}

		public override int PlayerExecutingHeroHonorableNobleRelationPenalty
		{
			get
			{
				return -10;
			}
		}

		public override int PlayerExecutingHeroClanRelationPenalty
		{
			get
			{
				return -60;
			}
		}

		public override int PlayerExecutingHeroFriendRelationPenalty
		{
			get
			{
				return -30;
			}
		}

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
