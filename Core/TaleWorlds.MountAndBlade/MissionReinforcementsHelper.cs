using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000285 RID: 645
	public static class MissionReinforcementsHelper
	{
		// Token: 0x0600223E RID: 8766 RVA: 0x0007D0C4 File Offset: 0x0007B2C4
		public static void OnMissionStart()
		{
			Mission mission = Mission.Current;
			MissionReinforcementsHelper._reinforcementFormationsData = new MissionReinforcementsHelper.ReinforcementFormationData[mission.Teams.Count, 8];
			foreach (Team team in mission.Teams)
			{
				for (int i = 0; i < 8; i++)
				{
					MissionReinforcementsHelper._reinforcementFormationsData[team.TeamIndex, i] = new MissionReinforcementsHelper.ReinforcementFormationData();
				}
			}
			MissionReinforcementsHelper._localInitTime = 0U;
		}

		// Token: 0x0600223F RID: 8767 RVA: 0x0007D154 File Offset: 0x0007B354
		[return: TupleElementNames(new string[] { "origin", "formationIndex" })]
		public unsafe static List<ValueTuple<IAgentOriginBase, int>> GetReinforcementAssignments(BattleSideEnum battleSide, List<IAgentOriginBase> troopOrigins)
		{
			Mission mission = Mission.Current;
			MissionReinforcementsHelper._localInitTime += 1U;
			List<ValueTuple<IAgentOriginBase, int>> list = new List<ValueTuple<IAgentOriginBase, int>>();
			PriorityQueue<MissionReinforcementsHelper.ReinforcementFormationPriority, Formation> priorityQueue = new PriorityQueue<MissionReinforcementsHelper.ReinforcementFormationPriority, Formation>(new MissionReinforcementsHelper.ReinforcementFormationPreferenceComparer());
			foreach (IAgentOriginBase agentOriginBase in troopOrigins)
			{
				priorityQueue.Clear();
				FormationClass formationClass = agentOriginBase.Troop.GetFormationClass().FallbackClass();
				if (mission.IsSiegeBattle)
				{
					formationClass = formationClass.SiegeClass();
				}
				bool flag = Mission.Current.PlayerTeam.Side == battleSide;
				Team agentTeam = Mission.GetAgentTeam(agentOriginBase, flag);
				foreach (Formation formation in agentTeam.FormationsIncludingEmpty)
				{
					int formationIndex = (int)formation.FormationIndex;
					if (formation.GetReadonlyMovementOrderReference()->OrderEnum != MovementOrder.MovementOrderEnum.Retreat)
					{
						MissionReinforcementsHelper.ReinforcementFormationData reinforcementFormationData = MissionReinforcementsHelper._reinforcementFormationsData[agentTeam.TeamIndex, formationIndex];
						if (!reinforcementFormationData.IsInitialized(MissionReinforcementsHelper._localInitTime))
						{
							reinforcementFormationData.Initialize(formation, MissionReinforcementsHelper._localInitTime);
						}
						MissionReinforcementsHelper.ReinforcementFormationPriority priority = reinforcementFormationData.GetPriority(formationClass);
						if (priorityQueue.IsEmpty<KeyValuePair<MissionReinforcementsHelper.ReinforcementFormationPriority, Formation>>() || priority >= priorityQueue.Peek().Key)
						{
							priorityQueue.Enqueue(priority, formation);
						}
					}
				}
				Formation formation2 = MissionReinforcementsHelper.FindBestFormationAmong(priorityQueue);
				if (formation2 == null)
				{
					formation2 = agentTeam.GetFormation(formationClass);
				}
				int formationIndex2 = (int)formation2.FormationIndex;
				MissionReinforcementsHelper._reinforcementFormationsData[formation2.Team.TeamIndex, formationIndex2].AddProspectiveTroop(formationClass);
				ValueTuple<IAgentOriginBase, int> valueTuple = new ValueTuple<IAgentOriginBase, int>(agentOriginBase, formationIndex2);
				list.Add(valueTuple);
			}
			return list;
		}

		// Token: 0x06002240 RID: 8768 RVA: 0x0007D33C File Offset: 0x0007B53C
		public static void OnMissionEnd()
		{
			MissionReinforcementsHelper._reinforcementFormationsData = null;
		}

		// Token: 0x06002241 RID: 8769 RVA: 0x0007D344 File Offset: 0x0007B544
		private static Formation FindBestFormationAmong(PriorityQueue<MissionReinforcementsHelper.ReinforcementFormationPriority, Formation> matchingFormations)
		{
			Formation formation = null;
			float num = float.MinValue;
			if (!matchingFormations.IsEmpty<KeyValuePair<MissionReinforcementsHelper.ReinforcementFormationPriority, Formation>>())
			{
				int key = (int)matchingFormations.Peek().Key;
				foreach (KeyValuePair<MissionReinforcementsHelper.ReinforcementFormationPriority, Formation> keyValuePair in matchingFormations)
				{
					int key2 = (int)keyValuePair.Key;
					if (key2 < key)
					{
						break;
					}
					Formation value = keyValuePair.Value;
					if (key2 == 3)
					{
						if (formation == null || value.FormationIndex < formation.FormationIndex)
						{
							formation = value;
						}
					}
					else
					{
						float formationReinforcementScore = MissionReinforcementsHelper.GetFormationReinforcementScore(value);
						if (formationReinforcementScore > num)
						{
							num = formationReinforcementScore;
							formation = value;
						}
					}
				}
			}
			return formation;
		}

		// Token: 0x06002242 RID: 8770 RVA: 0x0007D3F8 File Offset: 0x0007B5F8
		private static float GetFormationReinforcementScore(Formation formation)
		{
			Mission mission = Mission.Current;
			float num = (float)formation.CountOfUnits / (float)Math.Max(1, formation.Team.ActiveAgents.Count);
			float num2 = MathF.Max(0f, 1f - num);
			float num3 = 0f;
			BattleSideEnum side = formation.Team.Side;
			if (formation.HasBeenPositioned && mission.DeploymentPlan.IsPlanMadeForBattleSide(side, DeploymentPlanType.Reinforcement))
			{
				Vec2 asVec = mission.DeploymentPlan.GetBattleSideDeploymentFrame(side, DeploymentPlanType.Reinforcement).origin.AsVec2;
				float num4 = formation.CurrentPosition.DistanceSquared(asVec);
				float num5 = MathF.Min(1f, num4 / 62500f);
				num3 = MathF.Max(0f, 1f - num5);
			}
			return 0.4f * num2 + 0.6f * num3;
		}

		// Token: 0x04000CD0 RID: 3280
		private const float DominantClassThreshold = 0.5f;

		// Token: 0x04000CD1 RID: 3281
		private const float CommonClassThreshold = 0.25f;

		// Token: 0x04000CD2 RID: 3282
		private static uint _localInitTime;

		// Token: 0x04000CD3 RID: 3283
		private static MissionReinforcementsHelper.ReinforcementFormationData[,] _reinforcementFormationsData;

		// Token: 0x02000590 RID: 1424
		public enum ReinforcementFormationPriority
		{
			// Token: 0x04001D96 RID: 7574
			Dominant = 5,
			// Token: 0x04001D97 RID: 7575
			Common = 4,
			// Token: 0x04001D98 RID: 7576
			Empty = 3,
			// Token: 0x04001D99 RID: 7577
			AlternativeDominant = 2,
			// Token: 0x04001D9A RID: 7578
			AlternativeCommon = 1,
			// Token: 0x04001D9B RID: 7579
			Default = 0
		}

		// Token: 0x02000591 RID: 1425
		public class ReinforcementFormationPreferenceComparer : IComparer<MissionReinforcementsHelper.ReinforcementFormationPriority>
		{
			// Token: 0x06003B1E RID: 15134 RVA: 0x000EE208 File Offset: 0x000EC408
			public int Compare(MissionReinforcementsHelper.ReinforcementFormationPriority left, MissionReinforcementsHelper.ReinforcementFormationPriority right)
			{
				if (right < left)
				{
					return 1;
				}
				if (right > left)
				{
					return -1;
				}
				return 0;
			}
		}

		// Token: 0x02000592 RID: 1426
		public class ReinforcementFormationData
		{
			// Token: 0x06003B20 RID: 15136 RVA: 0x000EE22E File Offset: 0x000EC42E
			public ReinforcementFormationData()
			{
				this._initTime = 0U;
				this._expectedTroopCountPerClass = new int[4];
				this._expectedTotalTroopCount = 0;
				this._isClassified = false;
				this._troopClasses = new bool[4];
			}

			// Token: 0x06003B21 RID: 15137 RVA: 0x000EE264 File Offset: 0x000EC464
			public void Initialize(Formation formation, uint initTime)
			{
				int countOfUnits = formation.CountOfUnits;
				this._expectedTroopCountPerClass[0] = (int)(formation.QuerySystem.InfantryUnitRatio * (float)countOfUnits);
				this._expectedTroopCountPerClass[1] = (int)(formation.QuerySystem.RangedUnitRatio * (float)countOfUnits);
				this._expectedTroopCountPerClass[2] = (int)(formation.QuerySystem.CavalryUnitRatio * (float)countOfUnits);
				this._expectedTroopCountPerClass[3] = (int)(formation.QuerySystem.RangedCavalryUnitRatio * (float)countOfUnits);
				this._expectedTotalTroopCount = countOfUnits;
				this._isClassified = false;
				this._initTime = initTime;
			}

			// Token: 0x06003B22 RID: 15138 RVA: 0x000EE2EC File Offset: 0x000EC4EC
			public void AddProspectiveTroop(FormationClass troopClass)
			{
				this._expectedTroopCountPerClass[(int)troopClass]++;
				this._expectedTotalTroopCount++;
				this._isClassified = false;
			}

			// Token: 0x06003B23 RID: 15139 RVA: 0x000EE321 File Offset: 0x000EC521
			public bool IsInitialized(uint initTime)
			{
				return initTime == this._initTime;
			}

			// Token: 0x06003B24 RID: 15140 RVA: 0x000EE32C File Offset: 0x000EC52C
			public MissionReinforcementsHelper.ReinforcementFormationPriority GetPriority(FormationClass troopClass)
			{
				if (this._expectedTotalTroopCount == 0)
				{
					return MissionReinforcementsHelper.ReinforcementFormationPriority.Empty;
				}
				if (!this._isClassified)
				{
					this.Classify();
				}
				bool flag;
				if (this.HasTroopClass(troopClass, out flag))
				{
					if (!flag)
					{
						return MissionReinforcementsHelper.ReinforcementFormationPriority.Common;
					}
					return MissionReinforcementsHelper.ReinforcementFormationPriority.Dominant;
				}
				else
				{
					FormationClass formationClass = troopClass.AlternativeClass();
					if (!this.HasTroopClass(formationClass, out flag))
					{
						return MissionReinforcementsHelper.ReinforcementFormationPriority.Default;
					}
					if (!flag)
					{
						return MissionReinforcementsHelper.ReinforcementFormationPriority.AlternativeCommon;
					}
					return MissionReinforcementsHelper.ReinforcementFormationPriority.AlternativeDominant;
				}
			}

			// Token: 0x06003B25 RID: 15141 RVA: 0x000EE380 File Offset: 0x000EC580
			private void Classify()
			{
				if (this._expectedTotalTroopCount > 0)
				{
					int num = -1;
					int num2 = 4;
					for (int i = 0; i < num2; i++)
					{
						float num3 = (float)this._expectedTroopCountPerClass[i] / (float)this._expectedTotalTroopCount;
						this._troopClasses[i] = num3 >= 0.25f;
						if (num3 > 0.5f)
						{
							num = i;
							break;
						}
					}
					if (num >= 0)
					{
						this.ResetClassAssignments();
						this._troopClasses[num] = true;
					}
				}
				else
				{
					this.ResetClassAssignments();
				}
				this._isClassified = true;
			}

			// Token: 0x06003B26 RID: 15142 RVA: 0x000EE3FC File Offset: 0x000EC5FC
			private bool HasTroopClass(FormationClass troopClass, out bool isDominant)
			{
				int num = 0;
				for (int i = 0; i < 4; i++)
				{
					if (i == (int)troopClass && this._troopClasses[i])
					{
						num++;
					}
				}
				isDominant = num == 1;
				return num >= 1;
			}

			// Token: 0x06003B27 RID: 15143 RVA: 0x000EE438 File Offset: 0x000EC638
			private void ResetClassAssignments()
			{
				int num = 4;
				for (int i = 0; i < num; i++)
				{
					this._troopClasses[i] = false;
				}
			}

			// Token: 0x04001D9C RID: 7580
			private uint _initTime;

			// Token: 0x04001D9D RID: 7581
			private bool _isClassified;

			// Token: 0x04001D9E RID: 7582
			private int[] _expectedTroopCountPerClass;

			// Token: 0x04001D9F RID: 7583
			private int _expectedTotalTroopCount;

			// Token: 0x04001DA0 RID: 7584
			private bool[] _troopClasses;
		}
	}
}
