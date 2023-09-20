using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.Core;
using TaleWorlds.LinQuick;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.CampaignSystem.SceneInformationPopupTypes
{
	// Token: 0x020000AB RID: 171
	public static class CampaignSceneNotificationHelper
	{
		// Token: 0x060011AA RID: 4522 RVA: 0x00050F68 File Offset: 0x0004F168
		public static SceneNotificationData.SceneNotificationCharacter GetBodyguardOfCulture(CultureObject culture)
		{
			string stringId = culture.StringId;
			string text;
			if (!(stringId == "battania"))
			{
				if (!(stringId == "khuzait"))
				{
					if (!(stringId == "vlandia"))
					{
						if (!(stringId == "aserai"))
						{
							if (!(stringId == "sturgia"))
							{
								if (!(stringId == "empire"))
								{
									text = "fighter_sturgia";
								}
								else
								{
									text = "imperial_legionary";
								}
							}
							else
							{
								text = "sturgian_veteran_warrior";
							}
						}
						else
						{
							text = "mamluke_palace_guard";
						}
					}
					else
					{
						text = "vlandian_banner_knight";
					}
				}
				else
				{
					text = "khuzait_khans_guard";
				}
			}
			else
			{
				text = "battanian_fian_champion";
			}
			return new SceneNotificationData.SceneNotificationCharacter(MBObjectManager.Instance.GetObject<CharacterObject>(text), null, default(BodyProperties), false, uint.MaxValue, uint.MaxValue, false);
		}

		// Token: 0x060011AB RID: 4523 RVA: 0x00051020 File Offset: 0x0004F220
		public static void RemoveWeaponsFromEquipment(ref Equipment equipment, bool removeHelmet = false, bool removeShoulder = false)
		{
			for (int i = 0; i < 5; i++)
			{
				equipment[i] = EquipmentElement.Invalid;
			}
			if (removeHelmet)
			{
				equipment[5] = EquipmentElement.Invalid;
			}
			if (removeShoulder)
			{
				equipment[9] = EquipmentElement.Invalid;
			}
		}

		// Token: 0x060011AC RID: 4524 RVA: 0x00051068 File Offset: 0x0004F268
		public static string GetChildStageEquipmentIDFromCulture(CultureObject childCulture)
		{
			string stringId = childCulture.StringId;
			if (stringId == "empire")
			{
				return "comingofage_kid_emp_cutscene_template";
			}
			if (stringId == "aserai")
			{
				return "comingofage_kid_ase_cutscene_template";
			}
			if (stringId == "battania")
			{
				return "comingofage_kid_bat_cutscene_template";
			}
			if (stringId == "khuzait")
			{
				return "comingofage_kid_khu_cutscene_template";
			}
			if (stringId == "sturgia")
			{
				return "comingofage_kid_stu_cutscene_template";
			}
			if (!(stringId == "vlandia"))
			{
				return "comingofage_kid_emp_cutscene_template";
			}
			return "comingofage_kid_vla_cutscene_template";
		}

		// Token: 0x060011AD RID: 4525 RVA: 0x000510F8 File Offset: 0x0004F2F8
		public static CharacterObject GetRandomTroopForCulture(CultureObject culture)
		{
			string text = "imperial_recruit";
			if (culture != null)
			{
				List<CharacterObject> list = new List<CharacterObject>();
				if (culture.BasicTroop != null)
				{
					list.Add(culture.BasicTroop);
				}
				if (culture.EliteBasicTroop != null)
				{
					list.Add(culture.EliteBasicTroop);
				}
				if (culture.MeleeMilitiaTroop != null)
				{
					list.Add(culture.MeleeMilitiaTroop);
				}
				if (culture.MeleeEliteMilitiaTroop != null)
				{
					list.Add(culture.MeleeEliteMilitiaTroop);
				}
				if (culture.RangedMilitiaTroop != null)
				{
					list.Add(culture.RangedMilitiaTroop);
				}
				if (culture.RangedEliteMilitiaTroop != null)
				{
					list.Add(culture.RangedEliteMilitiaTroop);
				}
				if (list.Count > 0)
				{
					return list[MBRandom.RandomInt(list.Count)];
				}
			}
			return Game.Current.ObjectManager.GetObject<CharacterObject>(text);
		}

		// Token: 0x060011AE RID: 4526 RVA: 0x000511BA File Offset: 0x0004F3BA
		public static IEnumerable<Hero> GetMilitaryAudienceForHero(Hero hero, bool includeClanLeader = true, bool onlyClanMembers = false)
		{
			if (hero.Clan != null)
			{
				if (includeClanLeader)
				{
					Hero leader = hero.Clan.Leader;
					if (leader != null && leader.IsAlive && hero != hero.Clan.Leader)
					{
						yield return hero.Clan.Leader;
					}
				}
				IOrderedEnumerable<Hero> orderedEnumerable = hero.Clan.Heroes.OrderBy((Hero h) => h.Level);
				foreach (Hero hero2 in orderedEnumerable)
				{
					if (hero2 != hero.Clan.Leader && hero2.IsAlive && !hero2.IsChild && hero2 != hero)
					{
						yield return hero2;
					}
				}
				IEnumerator<Hero> enumerator = null;
			}
			if (!onlyClanMembers)
			{
				IOrderedEnumerable<Hero> orderedEnumerable2 = Hero.AllAliveHeroes.OrderBy((Hero h) => CharacterRelationManager.GetHeroRelation(hero, h));
				foreach (Hero hero3 in orderedEnumerable2)
				{
					bool flag = hero3 != null && hero3.Clan != hero.Clan;
					if (hero3.IsFriend(hero3) && hero3.IsLord && !hero3.IsChild && hero3 != hero && !flag)
					{
						yield return hero3;
					}
				}
				IEnumerator<Hero> enumerator = null;
			}
			yield break;
			yield break;
		}

		// Token: 0x060011AF RID: 4527 RVA: 0x000511D8 File Offset: 0x0004F3D8
		public static IEnumerable<Hero> GetMilitaryAudienceForKingdom(Kingdom kingdom, bool includeKingdomLeader = true)
		{
			if (includeKingdomLeader)
			{
				Hero leader = kingdom.Leader;
				if (leader != null && leader.IsAlive)
				{
					yield return kingdom.Leader;
				}
			}
			Hero leader2 = kingdom.Leader;
			IOrderedEnumerable<Hero> orderedEnumerable;
			if (leader2 == null)
			{
				orderedEnumerable = null;
			}
			else
			{
				orderedEnumerable = from h in leader2.Clan.Heroes.WhereQ((Hero h) => h != h.Clan.Kingdom.Leader)
					orderby h.GetRelationWithPlayer()
					select h;
			}
			IOrderedEnumerable<Hero> orderedEnumerable2 = orderedEnumerable;
			foreach (Hero hero in orderedEnumerable2)
			{
				if (!hero.IsChild && hero != Hero.MainHero && hero.IsAlive)
				{
					yield return hero;
				}
			}
			IEnumerator<Hero> enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x060011B0 RID: 4528 RVA: 0x000511F0 File Offset: 0x0004F3F0
		public static TextObject GetFormalDayAndSeasonText(CampaignTime time)
		{
			TextObject textObject = new TextObject("{=CpsPq6WD}the {DAY_ORDINAL} day of {SEASON_NAME}", null);
			TextObject textObject2 = GameTexts.FindText("str_season_" + time.GetSeasonOfYear, null);
			TextObject textObject3 = GameTexts.FindText("str_ordinal_number", (time.GetDayOfSeason + 1).ToString());
			textObject.SetTextVariable("SEASON_NAME", textObject2);
			textObject.SetTextVariable("DAY_ORDINAL", textObject3);
			return textObject;
		}

		// Token: 0x060011B1 RID: 4529 RVA: 0x0005125C File Offset: 0x0004F45C
		public static TextObject GetFormalNameForKingdom(Kingdom kingdom)
		{
			TextObject textObject;
			if (kingdom.Culture.StringId.Equals("empire", StringComparison.InvariantCultureIgnoreCase))
			{
				textObject = kingdom.Name;
			}
			else if (kingdom.Leader == Hero.MainHero)
			{
				textObject = kingdom.InformalName;
			}
			else
			{
				textObject = FactionHelper.GetFormalNameForFactionCulture(kingdom.Culture);
			}
			return textObject;
		}

		// Token: 0x060011B2 RID: 4530 RVA: 0x000512B0 File Offset: 0x0004F4B0
		public static SceneNotificationData.SceneNotificationCharacter CreateNotificationCharacterFromHero(Hero hero, Equipment overridenEquipment = null, bool useCivilian = false, BodyProperties overriddenBodyProperties = default(BodyProperties), uint overriddenColor1 = 4294967295U, uint overriddenColor2 = 4294967295U, bool useHorse = false)
		{
			if (overriddenColor1 == 4294967295U)
			{
				IFaction mapFaction = hero.MapFaction;
				overriddenColor1 = ((mapFaction != null) ? mapFaction.Color : hero.CharacterObject.Culture.Color);
			}
			if (overriddenColor2 == 4294967295U)
			{
				IFaction mapFaction2 = hero.MapFaction;
				overriddenColor2 = ((mapFaction2 != null) ? mapFaction2.Color2 : hero.CharacterObject.Culture.Color2);
			}
			if (overridenEquipment == null)
			{
				overridenEquipment = (useCivilian ? hero.CivilianEquipment : hero.BattleEquipment);
			}
			return new SceneNotificationData.SceneNotificationCharacter(hero.CharacterObject, overridenEquipment, overriddenBodyProperties, useCivilian, overriddenColor1, overriddenColor2, useHorse);
		}

		// Token: 0x060011B3 RID: 4531 RVA: 0x00051337 File Offset: 0x0004F537
		public static ItemObject GetDefaultHorseItem()
		{
			return Game.Current.ObjectManager.GetObjectTypeList<ItemObject>().First((ItemObject i) => i.ItemType == ItemObject.ItemTypeEnum.Horse && i.HasHorseComponent && i.HorseComponent.IsMount && i.HorseComponent.Monster.StringId == "horse");
		}
	}
}
