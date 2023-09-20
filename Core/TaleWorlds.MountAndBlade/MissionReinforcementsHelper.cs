using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public static class MissionReinforcementsHelper
	{
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
				FormationClass agentTroopClass = Mission.Current.GetAgentTroopClass(battleSide, agentOriginBase.Troop);
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
						MissionReinforcementsHelper.ReinforcementFormationPriority priority = reinforcementFormationData.GetPriority(agentTroopClass);
						if (priorityQueue.IsEmpty<KeyValuePair<MissionReinforcementsHelper.ReinforcementFormationPriority, Formation>>() || priority >= priorityQueue.Peek().Key)
						{
							priorityQueue.Enqueue(priority, formation);
						}
					}
				}
				Formation formation2 = MissionReinforcementsHelper.FindBestFormationAmong(priorityQueue);
				if (formation2 == null)
				{
					formation2 = agentTeam.GetFormation(agentTroopClass);
				}
				int formationIndex2 = (int)formation2.FormationIndex;
				MissionReinforcementsHelper._reinforcementFormationsData[formation2.Team.TeamIndex, formationIndex2].AddProspectiveTroop(agentTroopClass);
				ValueTuple<IAgentOriginBase, int> valueTuple = new ValueTuple<IAgentOriginBase, int>(agentOriginBase, formationIndex2);
				list.Add(valueTuple);
			}
			return list;
		}

		public static void OnMissionEnd()
		{
			MissionReinforcementsHelper._reinforcementFormationsData = null;
		}

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
					if (key2 == 3 || key2 == 4)
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
			return 0.6f * num2 + 0.4f * num3;
		}

		private const float DominantClassThreshold = 0.5f;

		private const float CommonClassThreshold = 0.25f;

		private static uint _localInitTime;

		private static MissionReinforcementsHelper.ReinforcementFormationData[,] _reinforcementFormationsData;

		public enum ReinforcementFormationPriority
		{
			Dominant = 6,
			Common = 5,
			EmptyRepresentativeMatch = 4,
			EmptyNoMatch = 3,
			AlternativeDominant = 2,
			AlternativeCommon = 1,
			Default = 0
		}

		public class ReinforcementFormationPreferenceComparer : IComparer<MissionReinforcementsHelper.ReinforcementFormationPriority>
		{
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

		public class ReinforcementFormationData
		{
			public ReinforcementFormationData()
			{
				this._initTime = 0U;
				this._expectedTroopCountPerClass = new int[4];
				this._expectedTotalTroopCount = 0;
				this._isClassified = false;
				this._representativeClass = FormationClass.NumberOfAllFormations;
				this._troopClasses = new bool[4];
			}

			public void Initialize(Formation formation, uint initTime)
			{
				int countOfUnits = formation.CountOfUnits;
				this._expectedTroopCountPerClass[0] = (int)(formation.QuerySystem.InfantryUnitRatio * (float)countOfUnits);
				this._expectedTroopCountPerClass[1] = (int)(formation.QuerySystem.RangedUnitRatio * (float)countOfUnits);
				this._expectedTroopCountPerClass[2] = (int)(formation.QuerySystem.CavalryUnitRatio * (float)countOfUnits);
				this._expectedTroopCountPerClass[3] = (int)(formation.QuerySystem.RangedCavalryUnitRatio * (float)countOfUnits);
				this._expectedTotalTroopCount = countOfUnits;
				this._isClassified = false;
				this._representativeClass = formation.RepresentativeClass;
				this._initTime = initTime;
			}

			public void AddProspectiveTroop(FormationClass troopClass)
			{
				this._expectedTroopCountPerClass[(int)troopClass]++;
				this._expectedTotalTroopCount++;
				this._isClassified = false;
			}

			public bool IsInitialized(uint initTime)
			{
				return initTime == this._initTime;
			}

			public MissionReinforcementsHelper.ReinforcementFormationPriority GetPriority(FormationClass troopClass)
			{
				if (this._expectedTotalTroopCount == 0)
				{
					if (this._representativeClass == troopClass)
					{
						return MissionReinforcementsHelper.ReinforcementFormationPriority.EmptyRepresentativeMatch;
					}
					return MissionReinforcementsHelper.ReinforcementFormationPriority.EmptyNoMatch;
				}
				else
				{
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
			}

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

			private void ResetClassAssignments()
			{
				int num = 4;
				for (int i = 0; i < num; i++)
				{
					this._troopClasses[i] = false;
				}
			}

			private uint _initTime;

			private bool _isClassified;

			private int[] _expectedTroopCountPerClass;

			private int _expectedTotalTroopCount;

			private bool[] _troopClasses;

			private FormationClass _representativeClass;
		}
	}
}
