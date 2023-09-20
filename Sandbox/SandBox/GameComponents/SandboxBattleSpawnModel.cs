using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ComponentInterfaces;

namespace SandBox.GameComponents
{
	// Token: 0x0200008C RID: 140
	public class SandboxBattleSpawnModel : BattleSpawnModel
	{
		// Token: 0x060005D6 RID: 1494 RVA: 0x0002C54C File Offset: 0x0002A74C
		public override void OnMissionStart()
		{
			MissionReinforcementsHelper.OnMissionStart();
		}

		// Token: 0x060005D7 RID: 1495 RVA: 0x0002C553 File Offset: 0x0002A753
		public override void OnMissionEnd()
		{
			MissionReinforcementsHelper.OnMissionEnd();
		}

		// Token: 0x060005D8 RID: 1496 RVA: 0x0002C55C File Offset: 0x0002A75C
		[return: TupleElementNames(new string[] { "origin", "formationIndex" })]
		public override List<ValueTuple<IAgentOriginBase, int>> GetInitialSpawnAssignments(BattleSideEnum battleSide, List<IAgentOriginBase> troopOrigins)
		{
			List<ValueTuple<IAgentOriginBase, int>> list = new List<ValueTuple<IAgentOriginBase, int>>();
			SandboxBattleSpawnModel.FormationOrderOfBattleConfiguration[] array;
			if (SandboxBattleSpawnModel.GetOrderOfBattleConfigurationsForFormations(troopOrigins, out array))
			{
				using (List<IAgentOriginBase>.Enumerator enumerator = troopOrigins.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						IAgentOriginBase agentOriginBase = enumerator.Current;
						SandboxBattleSpawnModel.OrderOfBattleInnerClassType orderOfBattleInnerClassType;
						FormationClass formationClass = SandboxBattleSpawnModel.FindBestOrderOfBattleFormationClassAssignmentForTroop(agentOriginBase, array, out orderOfBattleInnerClassType);
						ValueTuple<IAgentOriginBase, int> valueTuple = new ValueTuple<IAgentOriginBase, int>(agentOriginBase, formationClass);
						list.Add(valueTuple);
						if (orderOfBattleInnerClassType == SandboxBattleSpawnModel.OrderOfBattleInnerClassType.PrimaryClass)
						{
							SandboxBattleSpawnModel.FormationOrderOfBattleConfiguration[] array2 = array;
							FormationClass formationClass2 = formationClass;
							array2[formationClass2].PrimaryClassTroopCount = array2[formationClass2].PrimaryClassTroopCount + 1;
						}
						else if (orderOfBattleInnerClassType == SandboxBattleSpawnModel.OrderOfBattleInnerClassType.SecondaryClass)
						{
							SandboxBattleSpawnModel.FormationOrderOfBattleConfiguration[] array3 = array;
							FormationClass formationClass3 = formationClass;
							array3[formationClass3].SecondaryClassTroopCount = array3[formationClass3].SecondaryClassTroopCount + 1;
						}
					}
					return list;
				}
			}
			foreach (IAgentOriginBase agentOriginBase2 in troopOrigins)
			{
				ValueTuple<IAgentOriginBase, int> valueTuple2 = new ValueTuple<IAgentOriginBase, int>(agentOriginBase2, agentOriginBase2.Troop.GetFormationClass());
				list.Add(valueTuple2);
			}
			return list;
		}

		// Token: 0x060005D9 RID: 1497 RVA: 0x0002C658 File Offset: 0x0002A858
		[return: TupleElementNames(new string[] { "origin", "formationIndex" })]
		public override List<ValueTuple<IAgentOriginBase, int>> GetReinforcementAssignments(BattleSideEnum battleSide, List<IAgentOriginBase> troopOrigins)
		{
			return MissionReinforcementsHelper.GetReinforcementAssignments(battleSide, troopOrigins);
		}

		// Token: 0x060005DA RID: 1498 RVA: 0x0002C664 File Offset: 0x0002A864
		private static bool GetOrderOfBattleConfigurationsForFormations(List<IAgentOriginBase> troopOrigins, out SandboxBattleSpawnModel.FormationOrderOfBattleConfiguration[] formationOrderOfBattleConfigurations)
		{
			formationOrderOfBattleConfigurations = new SandboxBattleSpawnModel.FormationOrderOfBattleConfiguration[8];
			Campaign campaign = Campaign.Current;
			OrderOfBattleCampaignBehavior orderOfBattleCampaignBehavior = ((campaign != null) ? campaign.GetCampaignBehavior<OrderOfBattleCampaignBehavior>() : null);
			if (orderOfBattleCampaignBehavior == null)
			{
				return false;
			}
			for (int i = 0; i < 8; i++)
			{
				if (orderOfBattleCampaignBehavior.GetFormationDataAtIndex(i, Mission.Current.IsSiegeBattle) == null)
				{
					return false;
				}
			}
			int[] array = SandboxBattleSpawnModel.CalculateTroopCountsPerDefaultFormation(troopOrigins);
			for (int j = 0; j < 8; j++)
			{
				OrderOfBattleCampaignBehavior.OrderOfBattleFormationData formationDataAtIndex = orderOfBattleCampaignBehavior.GetFormationDataAtIndex(j, Mission.Current.IsSiegeBattle);
				formationOrderOfBattleConfigurations[j].OOBFormationClass = formationDataAtIndex.FormationClass;
				formationOrderOfBattleConfigurations[j].Commander = formationDataAtIndex.Commander;
				FormationClass formationClass = 10;
				FormationClass formationClass2 = 10;
				switch (formationDataAtIndex.FormationClass)
				{
				case 1:
					formationClass = 0;
					break;
				case 2:
					formationClass = 1;
					break;
				case 3:
					formationClass = 2;
					break;
				case 4:
					formationClass = 3;
					break;
				case 5:
					formationClass = 0;
					formationClass2 = 1;
					break;
				case 6:
					formationClass = 2;
					formationClass2 = 3;
					break;
				}
				formationOrderOfBattleConfigurations[j].PrimaryFormationClass = formationClass;
				if (formationClass != 10)
				{
					formationOrderOfBattleConfigurations[j].PrimaryClassDesiredTroopCount = (int)Math.Ceiling((double)((float)array[formationClass] * ((float)formationDataAtIndex.PrimaryClassWeight / 100f)));
				}
				formationOrderOfBattleConfigurations[j].SecondaryFormationClass = formationClass2;
				if (formationClass2 != 10)
				{
					formationOrderOfBattleConfigurations[j].SecondaryClassDesiredTroopCount = (int)Math.Ceiling((double)((float)array[formationClass2] * ((float)formationDataAtIndex.SecondaryClassWeight / 100f)));
				}
			}
			return true;
		}

		// Token: 0x060005DB RID: 1499 RVA: 0x0002C7D8 File Offset: 0x0002A9D8
		private static int[] CalculateTroopCountsPerDefaultFormation(List<IAgentOriginBase> troopOrigins)
		{
			int[] array = new int[4];
			foreach (IAgentOriginBase agentOriginBase in troopOrigins)
			{
				FormationClass formationClass = agentOriginBase.Troop.DefaultFormationClass;
				if (!ModuleExtensions.IsDefaultFormationClass(formationClass))
				{
					Debug.FailedAssert("Found default troop class which is not default for troop " + agentOriginBase.Troop.Name, "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox\\GameComponents\\SandboxBattleSpawnModel.cs", "CalculateTroopCountsPerDefaultFormation", 177);
					formationClass = ModuleExtensions.FallbackClass(formationClass);
				}
				array[formationClass]++;
			}
			return array;
		}

		// Token: 0x060005DC RID: 1500 RVA: 0x0002C878 File Offset: 0x0002AA78
		private static FormationClass FindBestOrderOfBattleFormationClassAssignmentForTroop(IAgentOriginBase origin, SandboxBattleSpawnModel.FormationOrderOfBattleConfiguration[] formationOrderOfBattleConfigurations, out SandboxBattleSpawnModel.OrderOfBattleInnerClassType bestClassInnerClassType)
		{
			FormationClass formationClass = origin.Troop.DefaultFormationClass;
			if (!ModuleExtensions.IsDefaultFormationClass(formationClass))
			{
				Debug.FailedAssert("Found default troop class which is not default for troop " + origin.Troop.Name, "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox\\GameComponents\\SandboxBattleSpawnModel.cs", "FindBestOrderOfBattleFormationClassAssignmentForTroop", 193);
				formationClass = ModuleExtensions.FallbackClass(formationClass);
			}
			FormationClass formationClass2 = formationClass;
			float num = float.MinValue;
			bestClassInnerClassType = SandboxBattleSpawnModel.OrderOfBattleInnerClassType.None;
			for (int i = 0; i < 8; i++)
			{
				CharacterObject characterObject;
				if (origin.Troop.IsHero && (characterObject = origin.Troop as CharacterObject) != null && characterObject.HeroObject == formationOrderOfBattleConfigurations[i].Commander)
				{
					formationClass2 = i;
					bestClassInnerClassType = SandboxBattleSpawnModel.OrderOfBattleInnerClassType.None;
					break;
				}
				if (formationClass == formationOrderOfBattleConfigurations[i].PrimaryFormationClass)
				{
					float num2 = (float)formationOrderOfBattleConfigurations[i].PrimaryClassDesiredTroopCount;
					float num3 = (float)formationOrderOfBattleConfigurations[i].PrimaryClassTroopCount;
					float num4 = 1f - num3 / (num2 + 1f);
					if (num4 > num)
					{
						formationClass2 = i;
						bestClassInnerClassType = SandboxBattleSpawnModel.OrderOfBattleInnerClassType.PrimaryClass;
						num = num4;
					}
				}
				else if (formationClass == formationOrderOfBattleConfigurations[i].SecondaryFormationClass)
				{
					float num5 = (float)formationOrderOfBattleConfigurations[i].SecondaryClassDesiredTroopCount;
					float num6 = (float)formationOrderOfBattleConfigurations[i].SecondaryClassTroopCount;
					float num7 = 1f - num6 / (num5 + 1f);
					if (num7 > num)
					{
						formationClass2 = i;
						bestClassInnerClassType = SandboxBattleSpawnModel.OrderOfBattleInnerClassType.SecondaryClass;
						num = num7;
					}
				}
			}
			return formationClass2;
		}

		// Token: 0x02000167 RID: 359
		private enum OrderOfBattleInnerClassType
		{
			// Token: 0x040006BA RID: 1722
			None,
			// Token: 0x040006BB RID: 1723
			PrimaryClass,
			// Token: 0x040006BC RID: 1724
			SecondaryClass
		}

		// Token: 0x02000168 RID: 360
		private struct FormationOrderOfBattleConfiguration
		{
			// Token: 0x040006BD RID: 1725
			public DeploymentFormationClass OOBFormationClass;

			// Token: 0x040006BE RID: 1726
			public FormationClass PrimaryFormationClass;

			// Token: 0x040006BF RID: 1727
			public int PrimaryClassTroopCount;

			// Token: 0x040006C0 RID: 1728
			public int PrimaryClassDesiredTroopCount;

			// Token: 0x040006C1 RID: 1729
			public FormationClass SecondaryFormationClass;

			// Token: 0x040006C2 RID: 1730
			public int SecondaryClassTroopCount;

			// Token: 0x040006C3 RID: 1731
			public int SecondaryClassDesiredTroopCount;

			// Token: 0x040006C4 RID: 1732
			public Hero Commander;
		}
	}
}
