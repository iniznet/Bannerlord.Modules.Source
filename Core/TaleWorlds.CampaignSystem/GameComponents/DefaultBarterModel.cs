using System;
using TaleWorlds.CampaignSystem.BarterSystem.Barterables;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	public class DefaultBarterModel : BarterModel
	{
		public override int BarterCooldownWithHeroInDays
		{
			get
			{
				return 3;
			}
		}

		private int MaximumOverpayRelationBonus
		{
			get
			{
				return 3;
			}
		}

		public override float MaximumPercentageOfNpcGoldToSpendAtBarter
		{
			get
			{
				return 0.25f;
			}
		}

		public override int CalculateOverpayRelationIncreaseCosts(Hero hero, float overpayAmount)
		{
			int num = (int)hero.GetRelationWithPlayer();
			float num2 = MathF.Clamp((float)(num + this.MaximumOverpayRelationBonus), -100f, 100f);
			int num3 = 0;
			int num4 = num;
			while ((float)num4 < num2)
			{
				int num5 = 1000 + 100 * (num4 * num4);
				if (overpayAmount >= (float)num5)
				{
					overpayAmount -= (float)num5;
					num3++;
					num4++;
				}
				else
				{
					if (MBRandom.RandomFloat <= overpayAmount / (float)num5)
					{
						num3++;
						break;
					}
					break;
				}
			}
			if (Hero.MainHero.GetPerkValue(DefaultPerks.Charm.Tribute) && MBRandom.RandomFloat < 0.2f)
			{
				num3 += (int)DefaultPerks.Charm.Tribute.PrimaryBonus;
			}
			return num3;
		}

		public override ExplainedNumber GetBarterPenalty(IFaction faction, ItemBarterable itemBarterable, Hero otherHero, PartyBase otherParty)
		{
			ExplainedNumber explainedNumber;
			if (faction == ((otherHero != null) ? otherHero.Clan : null) || faction == ((otherHero != null) ? otherHero.MapFaction : null) || faction == ((otherParty != null) ? otherParty.MapFaction : null))
			{
				explainedNumber = new ExplainedNumber(0.4f, false, null);
				if (otherHero != null && itemBarterable.OriginalOwner != null && otherHero != itemBarterable.OriginalOwner && otherHero.MapFaction != null && otherHero.IsPartyLeader)
				{
					CultureObject culture = otherHero.Culture;
					Hero originalOwner = itemBarterable.OriginalOwner;
					if (culture == ((originalOwner != null) ? originalOwner.Culture : null))
					{
						if (itemBarterable.OriginalOwner.GetPerkValue(DefaultPerks.Charm.EffortForThePeople))
						{
							explainedNumber.AddFactor(-DefaultPerks.Charm.EffortForThePeople.SecondaryBonus, null);
						}
					}
					else if (itemBarterable.OriginalOwner.GetPerkValue(DefaultPerks.Charm.SlickNegotiator))
					{
						explainedNumber.AddFactor(-DefaultPerks.Charm.SlickNegotiator.SecondaryBonus, null);
					}
					if (itemBarterable.OriginalOwner.GetPerkValue(DefaultPerks.Trade.SelfMadeMan))
					{
						explainedNumber.AddFactor(-DefaultPerks.Trade.SelfMadeMan.PrimaryBonus, null);
					}
				}
			}
			else
			{
				Hero originalOwner2 = itemBarterable.OriginalOwner;
				if (faction != ((originalOwner2 != null) ? originalOwner2.Clan : null))
				{
					Hero originalOwner3 = itemBarterable.OriginalOwner;
					if (faction != ((originalOwner3 != null) ? originalOwner3.MapFaction : null))
					{
						PartyBase originalParty = itemBarterable.OriginalParty;
						if (faction != ((originalParty != null) ? originalParty.MapFaction : null))
						{
							explainedNumber = new ExplainedNumber(0f, false, null);
							return explainedNumber;
						}
					}
				}
				if (itemBarterable.ItemRosterElement.EquipmentElement.Item.IsAnimal || itemBarterable.ItemRosterElement.EquipmentElement.Item.IsMountable)
				{
					explainedNumber = new ExplainedNumber(-8.4f, false, null);
				}
				else if (itemBarterable.ItemRosterElement.EquipmentElement.Item.IsFood)
				{
					explainedNumber = new ExplainedNumber(-12.6f, false, null);
				}
				else
				{
					explainedNumber = new ExplainedNumber(-2.1f, false, null);
				}
				if (otherHero != null && otherHero != itemBarterable.OriginalOwner && otherHero.MapFaction != null && otherHero.IsPartyLeader)
				{
					CultureObject culture2 = otherHero.Culture;
					Hero originalOwner4 = itemBarterable.OriginalOwner;
					if (culture2 == ((originalOwner4 != null) ? originalOwner4.Culture : null))
					{
						if (otherHero.GetPerkValue(DefaultPerks.Charm.EffortForThePeople))
						{
							explainedNumber.AddFactor(DefaultPerks.Charm.EffortForThePeople.SecondaryBonus, null);
						}
					}
					else if (otherHero.GetPerkValue(DefaultPerks.Charm.SlickNegotiator))
					{
						explainedNumber.AddFactor(DefaultPerks.Charm.SlickNegotiator.SecondaryBonus, null);
					}
					if (otherHero.GetPerkValue(DefaultPerks.Trade.SelfMadeMan))
					{
						explainedNumber.AddFactor(DefaultPerks.Trade.SelfMadeMan.PrimaryBonus, null);
					}
				}
			}
			return explainedNumber;
		}
	}
}
