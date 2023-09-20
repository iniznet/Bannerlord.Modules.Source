using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	public class CharacterDevelopmentCampaignBehavior : CampaignBehaviorBase
	{
		public override void RegisterEvents()
		{
			CampaignEvents.DailyTickHeroEvent.AddNonSerializedListener(this, new Action<Hero>(this.DailyTickHero));
			CampaignEvents.OnNewGameCreatedPartialFollowUpEndEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnNewGameCreatedPartialFollowUpEnd));
		}

		private void OnNewGameCreatedPartialFollowUpEnd(CampaignGameStarter starter)
		{
			foreach (Hero hero in Hero.AllAliveHeroes)
			{
				this.InitializeHeroCharacterDeveloper(hero);
			}
			foreach (Hero hero2 in Hero.DeadOrDisabledHeroes)
			{
				this.InitializeHeroCharacterDeveloper(hero2);
			}
		}

		private void InitializeHeroCharacterDeveloper(Hero hero)
		{
			hero.HeroDeveloper.CheckInitialLevel();
			if (!hero.IsChild && (hero.Clan != Clan.PlayerClan || (hero != Hero.MainHero && CampaignOptions.AutoAllocateClanMemberPerks)))
			{
				this.DevelopCharacterStats(hero);
			}
		}

		public override void SyncData(IDataStore dataStore)
		{
		}

		private void DailyTickHero(Hero hero)
		{
			if (!hero.IsChild && hero.IsAlive && (hero.Clan != Clan.PlayerClan || (hero != Hero.MainHero && CampaignOptions.AutoAllocateClanMemberPerks)))
			{
				if (hero.HeroDeveloper.UnspentFocusPoints > 0)
				{
					this.DistributeUnspentFocusPoints(hero);
				}
				if (hero.HeroDeveloper.GetOneAvailablePerkForEachPerkPair().Count > 0)
				{
					this.SelectPerks(hero);
				}
				if (hero.HeroDeveloper.UnspentAttributePoints > 0)
				{
					this.DistributeUnspentAttributePoints(hero);
				}
			}
		}

		public void DevelopCharacterStats(Hero hero)
		{
			this.DistributeUnspentAttributePoints(hero);
			this.DistributeUnspentFocusPoints(hero);
			this.SelectPerks(hero);
		}

		private void DistributeUnspentAttributePoints(Hero hero)
		{
			while (hero.HeroDeveloper.UnspentAttributePoints > 0)
			{
				CharacterAttribute characterAttribute = null;
				float num = float.MinValue;
				foreach (CharacterAttribute characterAttribute2 in Attributes.All)
				{
					int attributeValue = hero.GetAttributeValue(characterAttribute2);
					if (attributeValue < Campaign.Current.Models.CharacterDevelopmentModel.MaxAttribute)
					{
						float num2 = 0f;
						if (attributeValue == 0)
						{
							num2 = float.MaxValue;
						}
						else
						{
							foreach (SkillObject skillObject in characterAttribute2.Skills)
							{
								float num3 = MathF.Max(0f, (float)(75 + hero.GetSkillValue(skillObject)) - Campaign.Current.Models.CharacterDevelopmentModel.CalculateLearningLimit(attributeValue, hero.HeroDeveloper.GetFocus(skillObject), null, false).ResultNumber);
								num2 += num3;
							}
							int num4 = 1;
							foreach (CharacterAttribute characterAttribute3 in Attributes.All)
							{
								if (characterAttribute3 != characterAttribute2)
								{
									int attributeValue2 = hero.GetAttributeValue(characterAttribute3);
									if (num4 < attributeValue2)
									{
										num4 = attributeValue2;
									}
								}
							}
							float num5 = MathF.Sqrt((float)num4 / (float)attributeValue);
							num2 *= num5;
						}
						if (num2 > num)
						{
							num = num2;
							characterAttribute = characterAttribute2;
						}
					}
				}
				if (characterAttribute == null)
				{
					break;
				}
				hero.HeroDeveloper.AddAttribute(characterAttribute, 1, true);
			}
		}

		private void DistributeUnspentFocusPoints(Hero hero)
		{
			while (hero.HeroDeveloper.UnspentFocusPoints > 0)
			{
				SkillObject skillObject = null;
				float num = float.MinValue;
				foreach (SkillObject skillObject2 in Skills.All)
				{
					if (hero.HeroDeveloper.CanAddFocusToSkill(skillObject2))
					{
						int attributeValue = hero.GetAttributeValue(skillObject2.CharacterAttribute);
						int focus = hero.HeroDeveloper.GetFocus(skillObject2);
						float num2 = (float)hero.GetSkillValue(skillObject2) - Campaign.Current.Models.CharacterDevelopmentModel.CalculateLearningLimit(attributeValue, focus, null, false).ResultNumber;
						if (num2 > num)
						{
							num = num2;
							skillObject = skillObject2;
						}
					}
				}
				if (skillObject == null)
				{
					break;
				}
				hero.HeroDeveloper.AddFocus(skillObject, 1, true);
			}
		}

		private void SelectPerks(Hero hero)
		{
			foreach (PerkObject perkObject in hero.HeroDeveloper.GetOneAvailablePerkForEachPerkPair())
			{
				if (perkObject.AlternativePerk != null)
				{
					if (MBRandom.RandomFloat < 0.5f)
					{
						hero.HeroDeveloper.AddPerk(perkObject);
					}
					else
					{
						hero.HeroDeveloper.AddPerk(perkObject.AlternativePerk);
					}
				}
				else
				{
					hero.HeroDeveloper.AddPerk(perkObject);
				}
			}
		}
	}
}
