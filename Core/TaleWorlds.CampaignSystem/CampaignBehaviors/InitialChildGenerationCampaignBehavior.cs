using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	// Token: 0x020003A1 RID: 929
	public class InitialChildGenerationCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x0600373D RID: 14141 RVA: 0x000F86DC File Offset: 0x000F68DC
		public override void RegisterEvents()
		{
			CampaignEvents.OnNewGameCreatedPartialFollowUpEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter, int>(this.OnNewGameCreatedPartialFollowUp));
		}

		// Token: 0x0600373E RID: 14142 RVA: 0x000F86F8 File Offset: 0x000F68F8
		private void OnNewGameCreatedPartialFollowUp(CampaignGameStarter starter, int index)
		{
			if (index == 0)
			{
				using (List<Clan>.Enumerator enumerator = Clan.All.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Clan clan = enumerator.Current;
						if (!clan.IsBanditFaction && !clan.IsMinorFaction && !clan.IsNeutralClan && !clan.IsEliminated && clan != Clan.PlayerClan)
						{
							List<Hero> list = new List<Hero>();
							MBList<Hero> mblist = new MBList<Hero>();
							MBList<Hero> mblist2 = new MBList<Hero>();
							foreach (Hero hero in clan.Lords)
							{
								if (hero.IsAlive)
								{
									if (hero.IsChild)
									{
										list.Add(hero);
									}
									else if (hero.IsFemale)
									{
										mblist.Add(hero);
									}
									else
									{
										mblist2.Add(hero);
									}
								}
							}
							int num = MathF.Ceiling((float)(mblist2.Count + mblist.Count) / 2f) - list.Count;
							float num2 = 0.49f;
							if (mblist2.Count == 0)
							{
								num2 = -1f;
							}
							Func<Clan, bool> <>9__0;
							for (int i = 0; i < num; i++)
							{
								bool isFemale = MBRandom.RandomFloat <= num2;
								Hero hero2 = (isFemale ? mblist.GetRandomElement<Hero>() : mblist2.GetRandomElement<Hero>());
								if (hero2 == null)
								{
									Debug.Print("Searching for a template in other clans for child generation in " + clan.Name, 0, Debug.DebugColor.White, 17592186044416UL);
									IEnumerable<Clan> nonBanditFactions = Clan.NonBanditFactions;
									Func<Clan, bool> func;
									if ((func = <>9__0) == null)
									{
										func = (<>9__0 = (Clan t) => t != clan && t.Culture == clan.Culture);
									}
									MBList<Clan> mblist3 = nonBanditFactions.Where(func).ToMBList<Clan>();
									Func<Hero, bool> <>9__1;
									for (int j = 0; j < 10; j++)
									{
										IEnumerable<Hero> lords = mblist3.GetRandomElement<Clan>().Lords;
										Func<Hero, bool> func2;
										if ((func2 = <>9__1) == null)
										{
											func2 = (<>9__1 = (Hero t) => t.IsAlive && t.IsFemale == isFemale);
										}
										hero2 = lords.Where(func2).ToMBList<Hero>().GetRandomElement<Hero>();
										if (hero2 != null)
										{
											break;
										}
									}
								}
								if (hero2 != null)
								{
									int num3 = MBRandom.RandomInt(2, 18);
									Hero hero3 = HeroCreator.CreateSpecialHero(hero2.CharacterObject, clan.HomeSettlement, clan, null, num3);
									hero3.UpdateHomeSettlement();
									hero3.HeroDeveloper.DeriveSkillsFromTraits(true, null);
									MBEquipmentRoster randomElementInefficiently = Campaign.Current.Models.EquipmentSelectionModel.GetEquipmentRostersForInitialChildrenGeneration(hero3).GetRandomElementInefficiently<MBEquipmentRoster>();
									if (randomElementInefficiently != null)
									{
										Equipment randomCivilianEquipment = randomElementInefficiently.GetRandomCivilianEquipment();
										EquipmentHelper.AssignHeroEquipmentFromEquipment(hero3, randomCivilianEquipment);
										Equipment equipment = new Equipment(false);
										equipment.FillFrom(randomCivilianEquipment, false);
										EquipmentHelper.AssignHeroEquipmentFromEquipment(hero3, equipment);
									}
								}
								if (num2 <= 0f)
								{
									num2 = 0.49f;
								}
							}
							Debug.Print("Generated a child in " + clan.Name, 0, Debug.DebugColor.White, 17592186044416UL);
						}
					}
				}
			}
		}

		// Token: 0x0600373F RID: 14143 RVA: 0x000F8A58 File Offset: 0x000F6C58
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x0400117F RID: 4479
		private const float FemaleChildrenChance = 0.49f;
	}
}
