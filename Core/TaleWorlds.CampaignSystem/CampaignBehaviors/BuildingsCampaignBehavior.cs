using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Buildings;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	// Token: 0x0200037B RID: 891
	public class BuildingsCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x06003410 RID: 13328 RVA: 0x000D9A60 File Offset: 0x000D7C60
		public override void RegisterEvents()
		{
			CampaignEvents.OnNewGameCreatedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnNewGameCreated));
			CampaignEvents.DailyTickSettlementEvent.AddNonSerializedListener(this, new Action<Settlement>(this.DailyTickSettlement));
			CampaignEvents.OnBuildingLevelChangedEvent.AddNonSerializedListener(this, new Action<Town, Building, int>(this.OnBuildingLevelChanged));
		}

		// Token: 0x06003411 RID: 13329 RVA: 0x000D9AB2 File Offset: 0x000D7CB2
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x06003412 RID: 13330 RVA: 0x000D9AB4 File Offset: 0x000D7CB4
		private void OnNewGameCreated(CampaignGameStarter starter)
		{
			BuildingsCampaignBehavior.BuildDevelopmentsAtGameStart();
		}

		// Token: 0x06003413 RID: 13331 RVA: 0x000D9ABC File Offset: 0x000D7CBC
		private void DecideProject(Town town)
		{
			if (town.Owner.Settlement.OwnerClan != Clan.PlayerClan && town.BuildingsInProgress.Count < 3)
			{
				List<Building> list = new List<Building>(town.BuildingsInProgress);
				int num = 100;
				for (int i = 0; i < num; i++)
				{
					Building nextBuilding = Campaign.Current.Models.BuildingScoreCalculationModel.GetNextBuilding(town);
					if (nextBuilding != null)
					{
						list.Add(nextBuilding);
						break;
					}
				}
				BuildingHelper.ChangeCurrentBuildingQueue(list, town);
			}
		}

		// Token: 0x06003414 RID: 13332 RVA: 0x000D9B34 File Offset: 0x000D7D34
		private void DailyTickSettlement(Settlement settlement)
		{
			if (settlement.IsFortification)
			{
				Town town = settlement.Town;
				foreach (Building building in town.Buildings)
				{
					if (town.Owner.Settlement.SiegeEvent == null)
					{
						building.HitPointChanged(10f);
					}
				}
				this.DecideProject(town);
			}
		}

		// Token: 0x06003415 RID: 13333 RVA: 0x000D9BB4 File Offset: 0x000D7DB4
		private void OnBuildingLevelChanged(Town town, Building building, int levelChange)
		{
			if (levelChange > 0)
			{
				if (town.Governor != null)
				{
					if (town.IsTown && town.Governor.GetPerkValue(DefaultPerks.Charm.MoralLeader))
					{
						Hero randomElement = town.Settlement.Notables.GetRandomElement<Hero>();
						if (randomElement != null)
						{
							ChangeRelationAction.ApplyRelationChangeBetweenHeroes(town.Settlement.OwnerClan.Leader, randomElement, MathF.Round(DefaultPerks.Charm.MoralLeader.SecondaryBonus), true);
						}
					}
					if (town.Governor.GetPerkValue(DefaultPerks.Engineering.Foreman))
					{
						town.Settlement.Prosperity += DefaultPerks.Engineering.Foreman.SecondaryBonus;
					}
				}
				SkillLevelingManager.OnSettlementProjectFinished(town.Settlement);
			}
		}

		// Token: 0x06003416 RID: 13334 RVA: 0x000D9C60 File Offset: 0x000D7E60
		private static void BuildDevelopmentsAtGameStart()
		{
			foreach (Settlement settlement in Settlement.All)
			{
				Town town = settlement.Town;
				if (town != null)
				{
					bool flag = false;
					int num = 0;
					if (town.IsTown)
					{
						foreach (BuildingType buildingType4 in BuildingType.All)
						{
							if (buildingType4.BuildingLocation == BuildingLocation.Settlement && buildingType4 != DefaultBuildingTypes.Fortifications)
							{
								BuildingsCampaignBehavior.GetBuildingProbability(out flag, out num);
								if (flag)
								{
									if (num > 3)
									{
										num = 3;
									}
									town.Buildings.Add(new Building(buildingType4, town, 0f, num));
								}
							}
						}
						using (List<BuildingType>.Enumerator enumerator2 = BuildingType.All.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								BuildingType buildingType5 = enumerator2.Current;
								if (!town.Buildings.Any((Building k) => k.BuildingType == buildingType5) && buildingType5.BuildingLocation == BuildingLocation.Settlement)
								{
									town.Buildings.Add(new Building(buildingType5, town, 0f, 0));
								}
							}
							goto IL_220;
						}
						goto IL_124;
					}
					goto IL_124;
					IL_220:
					int num2 = MBRandom.RandomInt(1, 4);
					int num3 = 1;
					foreach (BuildingType buildingType2 in BuildingType.All)
					{
						if (buildingType2.BuildingLocation == BuildingLocation.Daily)
						{
							Building building = new Building(buildingType2, town, 0f, 1);
							town.Buildings.Add(building);
							if (num3 == num2)
							{
								building.IsCurrentlyDefault = true;
							}
							num3++;
						}
					}
					foreach (Building building2 in town.Buildings.OrderByDescending((Building k) => k.CurrentLevel))
					{
						if (building2.CurrentLevel != 3 && building2.CurrentLevel != building2.BuildingType.StartLevel && building2.BuildingType.BuildingLocation != BuildingLocation.Daily)
						{
							town.BuildingsInProgress.Enqueue(building2);
						}
					}
					continue;
					IL_124:
					if (town.IsCastle)
					{
						foreach (BuildingType buildingType3 in BuildingType.All)
						{
							if (buildingType3.BuildingLocation == BuildingLocation.Castle && buildingType3 != DefaultBuildingTypes.Wall)
							{
								BuildingsCampaignBehavior.GetBuildingProbability(out flag, out num);
								if (flag)
								{
									if (num > 3)
									{
										num = 3;
									}
									town.Buildings.Add(new Building(buildingType3, town, 0f, num));
								}
							}
						}
						using (List<BuildingType>.Enumerator enumerator2 = BuildingType.All.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								BuildingType buildingType = enumerator2.Current;
								if (!town.Buildings.Any((Building k) => k.BuildingType == buildingType) && buildingType.BuildingLocation == BuildingLocation.Castle)
								{
									town.Buildings.Add(new Building(buildingType, town, 0f, 0));
								}
							}
						}
						goto IL_220;
					}
					goto IL_220;
				}
			}
		}

		// Token: 0x06003417 RID: 13335 RVA: 0x000DA05C File Offset: 0x000D825C
		private static void GetBuildingProbability(out bool haveBuilding, out int level)
		{
			level = MBRandom.RandomInt(0, 7);
			if (level < 4)
			{
				haveBuilding = false;
				return;
			}
			haveBuilding = true;
			level -= 3;
		}
	}
}
