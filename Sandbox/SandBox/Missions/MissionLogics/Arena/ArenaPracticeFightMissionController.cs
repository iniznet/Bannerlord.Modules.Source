using System;
using System.Collections.Generic;
using System.Linq;
using SandBox.Conversation.MissionLogics;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.MissionLogics.Arena
{
	// Token: 0x02000065 RID: 101
	public class ArenaPracticeFightMissionController : MissionLogic
	{
		// Token: 0x17000055 RID: 85
		// (get) Token: 0x06000441 RID: 1089 RVA: 0x0001F5A4 File Offset: 0x0001D7A4
		private int AISpawnIndex
		{
			get
			{
				return this._spawnedOpponentAgentCount;
			}
		}

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x06000442 RID: 1090 RVA: 0x0001F5AC File Offset: 0x0001D7AC
		// (set) Token: 0x06000443 RID: 1091 RVA: 0x0001F5B4 File Offset: 0x0001D7B4
		public int RemainingOpponentCountFromLastPractice { get; private set; }

		// Token: 0x17000057 RID: 87
		// (get) Token: 0x06000444 RID: 1092 RVA: 0x0001F5BD File Offset: 0x0001D7BD
		// (set) Token: 0x06000445 RID: 1093 RVA: 0x0001F5C5 File Offset: 0x0001D7C5
		public bool IsPlayerPracticing { get; private set; }

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x06000446 RID: 1094 RVA: 0x0001F5CE File Offset: 0x0001D7CE
		// (set) Token: 0x06000447 RID: 1095 RVA: 0x0001F5D6 File Offset: 0x0001D7D6
		public int OpponentCountBeatenByPlayer { get; private set; }

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x06000448 RID: 1096 RVA: 0x0001F5DF File Offset: 0x0001D7DF
		public int RemainingOpponentCount
		{
			get
			{
				return 30 - this._spawnedOpponentAgentCount + this._aliveOpponentCount;
			}
		}

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x06000449 RID: 1097 RVA: 0x0001F5F1 File Offset: 0x0001D7F1
		// (set) Token: 0x0600044A RID: 1098 RVA: 0x0001F5F9 File Offset: 0x0001D7F9
		public bool IsPlayerSurvived { get; private set; }

		// Token: 0x1700005B RID: 91
		// (get) Token: 0x0600044B RID: 1099 RVA: 0x0001F602 File Offset: 0x0001D802
		// (set) Token: 0x0600044C RID: 1100 RVA: 0x0001F60A File Offset: 0x0001D80A
		public bool AfterPractice { get; set; }

		// Token: 0x0600044D RID: 1101 RVA: 0x0001F614 File Offset: 0x0001D814
		public override void AfterStart()
		{
			this._settlement = PlayerEncounter.LocationEncounter.Settlement;
			this.InitializeTeams();
			GameEntity gameEntity = base.Mission.Scene.FindEntityWithTag("tournament_practice") ?? base.Mission.Scene.FindEntityWithTag("tournament_fight");
			List<GameEntity> list = Mission.Current.Scene.FindEntitiesWithTag("arena_set").ToList<GameEntity>();
			list.Remove(gameEntity);
			foreach (GameEntity gameEntity2 in list)
			{
				gameEntity2.Remove(88);
			}
			this._initialSpawnFrames = (from e in base.Mission.Scene.FindEntitiesWithTag("sp_arena")
				select e.GetGlobalFrame()).ToList<MatrixFrame>();
			this._spawnFrames = (from e in base.Mission.Scene.FindEntitiesWithTag("sp_arena_respawn")
				select e.GetGlobalFrame()).ToList<MatrixFrame>();
			for (int i = 0; i < this._initialSpawnFrames.Count; i++)
			{
				MatrixFrame matrixFrame = this._initialSpawnFrames[i];
				matrixFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
				this._initialSpawnFrames[i] = matrixFrame;
			}
			for (int j = 0; j < this._spawnFrames.Count; j++)
			{
				MatrixFrame matrixFrame2 = this._spawnFrames[j];
				matrixFrame2.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
				this._spawnFrames[j] = matrixFrame2;
			}
			this.IsPlayerPracticing = false;
			this._participantAgents = new List<Agent>();
			this.StartPractice();
			MissionAgentHandler missionBehavior = base.Mission.GetMissionBehavior<MissionAgentHandler>();
			missionBehavior.SpawnPlayer(true, true, false, false, false, "");
			missionBehavior.SpawnLocationCharacters(null);
		}

		// Token: 0x0600044E RID: 1102 RVA: 0x0001F808 File Offset: 0x0001DA08
		private void SpawnPlayerNearTournamentMaster()
		{
			GameEntity gameEntity = base.Mission.Scene.FindEntityWithTag("sp_player_near_arena_master");
			base.Mission.SpawnAgent(new AgentBuildData(CharacterObject.PlayerCharacter).Team(base.Mission.PlayerTeam).InitialFrameFromSpawnPointEntity(gameEntity).NoHorses(true)
				.CivilianEquipment(true)
				.TroopOrigin(new SimpleAgentOrigin(CharacterObject.PlayerCharacter, -1, null, default(UniqueTroopDescriptor)))
				.Controller(2), false);
			Mission.Current.SetMissionMode(0, false);
		}

		// Token: 0x0600044F RID: 1103 RVA: 0x0001F890 File Offset: 0x0001DA90
		private Agent SpawnArenaAgent(Team team, MatrixFrame frame)
		{
			CharacterObject characterObject;
			int num;
			if (team == base.Mission.PlayerTeam)
			{
				characterObject = CharacterObject.PlayerCharacter;
				num = 0;
			}
			else
			{
				characterObject = this._participantCharacters[this.AISpawnIndex];
				num = this.AISpawnIndex;
			}
			Equipment equipment = new Equipment();
			this.AddRandomWeapons(equipment, num);
			this.AddRandomClothes(characterObject, equipment);
			Mission mission = base.Mission;
			AgentBuildData agentBuildData = new AgentBuildData(characterObject).Team(team).InitialPosition(ref frame.origin);
			Vec2 vec = frame.rotation.f.AsVec2;
			vec = vec.Normalized();
			Agent agent = mission.SpawnAgent(agentBuildData.InitialDirection(ref vec).NoHorses(true).Equipment(equipment)
				.TroopOrigin(new SimpleAgentOrigin(characterObject, -1, null, default(UniqueTroopDescriptor)))
				.Controller((characterObject == CharacterObject.PlayerCharacter) ? 2 : 1), false);
			agent.FadeIn();
			if (characterObject != CharacterObject.PlayerCharacter)
			{
				this._aliveOpponentCount++;
				this._spawnedOpponentAgentCount++;
			}
			if (agent.IsAIControlled)
			{
				agent.SetWatchState(2);
			}
			return agent;
		}

		// Token: 0x06000450 RID: 1104 RVA: 0x0001F99C File Offset: 0x0001DB9C
		public override void OnScoreHit(Agent affectedAgent, Agent affectorAgent, WeaponComponentData attackerWeapon, bool isBlocked, bool isSiegeEngineHit, in Blow blow, in AttackCollisionData collisionData, float damagedHp, float hitDistance, float shotDifficulty)
		{
			if (affectorAgent == null)
			{
				return;
			}
			if (affectorAgent.IsMount && affectorAgent.RiderAgent != null)
			{
				affectorAgent = affectorAgent.RiderAgent;
			}
			if (affectorAgent.Character == null || affectedAgent.Character == null)
			{
				return;
			}
			float num = (float)blow.InflictedDamage;
			if (num > affectedAgent.HealthLimit)
			{
				num = affectedAgent.HealthLimit;
			}
			float num2 = num / affectedAgent.HealthLimit;
			this.EnemyHitReward(affectedAgent, affectorAgent, blow.MovementSpeedDamageModifier, shotDifficulty, attackerWeapon, blow.AttackType, 0.5f * num2, num);
		}

		// Token: 0x06000451 RID: 1105 RVA: 0x0001FA1C File Offset: 0x0001DC1C
		private void EnemyHitReward(Agent affectedAgent, Agent affectorAgent, float lastSpeedBonus, float lastShotDifficulty, WeaponComponentData attackerWeapon, AgentAttackType attackType, float hitpointRatio, float damageAmount)
		{
			CharacterObject characterObject = (CharacterObject)affectedAgent.Character;
			CharacterObject characterObject2 = (CharacterObject)affectorAgent.Character;
			if (affectedAgent.Origin != null && affectorAgent != null && affectorAgent.Origin != null)
			{
				bool flag = affectorAgent.MountAgent != null;
				bool flag2 = flag && attackType == 3;
				SkillLevelingManager.OnCombatHit(characterObject2, characterObject, null, null, lastSpeedBonus, lastShotDifficulty, attackerWeapon, hitpointRatio, 1, flag, affectorAgent.Team == affectedAgent.Team, false, damageAmount, affectedAgent.Health < 1f, false, flag2);
			}
		}

		// Token: 0x06000452 RID: 1106 RVA: 0x0001FA9C File Offset: 0x0001DC9C
		public override void OnMissionTick(float dt)
		{
			base.OnMissionTick(dt);
			if (this._aliveOpponentCount < 6 && this._spawnedOpponentAgentCount < 30 && (this._aliveOpponentCount == 2 || this._nextSpawnTime < base.Mission.CurrentTime))
			{
				Team team = this.SelectRandomAiTeam();
				Agent agent = this.SpawnArenaAgent(team, this.GetSpawnFrame(true, false));
				this._participantAgents.Add(agent);
				this._nextSpawnTime = base.Mission.CurrentTime + 14f - (float)this._spawnedOpponentAgentCount / 3f;
				if (this._spawnedOpponentAgentCount == 30 && !this.IsPlayerPracticing)
				{
					this._spawnedOpponentAgentCount = 0;
				}
			}
			if (this._teleportTimer == null && this.IsPlayerPracticing && this.CheckPracticeEndedForPlayer())
			{
				this._teleportTimer = new BasicMissionTimer();
				this.IsPlayerSurvived = base.Mission.MainAgent != null && base.Mission.MainAgent.IsActive();
				if (this.IsPlayerSurvived)
				{
					MBInformationManager.AddQuickInformation(new TextObject("{=seyti8xR}Victory!", null), 0, null, "event:/ui/mission/arena_victory");
				}
				this.AfterPractice = true;
			}
			if (this._teleportTimer != null && this._teleportTimer.ElapsedTime > (float)this.TeleportTime)
			{
				this._teleportTimer = null;
				this.RemainingOpponentCountFromLastPractice = this.RemainingOpponentCount;
				this.IsPlayerPracticing = false;
				this.StartPractice();
				this.SpawnPlayerNearTournamentMaster();
				Agent agent2 = base.Mission.Agents.FirstOrDefault((Agent x) => x.Character != null && ((CharacterObject)x.Character).Occupation == 5);
				MissionConversationLogic.Current.StartConversation(agent2, true, false);
			}
		}

		// Token: 0x06000453 RID: 1107 RVA: 0x0001FC34 File Offset: 0x0001DE34
		private Team SelectRandomAiTeam()
		{
			Team team = null;
			foreach (Team team2 in this._AIParticipantTeams)
			{
				if (!team2.HasBots)
				{
					team = team2;
					break;
				}
			}
			if (team == null)
			{
				team = this._AIParticipantTeams[MBRandom.RandomInt(this._AIParticipantTeams.Count - 1) + 1];
			}
			return team;
		}

		// Token: 0x06000454 RID: 1108 RVA: 0x0001FCB4 File Offset: 0x0001DEB4
		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow killingBlow)
		{
			if (affectedAgent != null && affectedAgent.IsHuman)
			{
				if (affectedAgent != Agent.Main)
				{
					this._aliveOpponentCount--;
				}
				if (affectorAgent != null && affectorAgent.IsHuman && affectorAgent == Agent.Main && affectedAgent != Agent.Main)
				{
					int opponentCountBeatenByPlayer = this.OpponentCountBeatenByPlayer;
					this.OpponentCountBeatenByPlayer = opponentCountBeatenByPlayer + 1;
				}
			}
			if (this._participantAgents.Contains(affectedAgent))
			{
				this._participantAgents.Remove(affectedAgent);
			}
		}

		// Token: 0x06000455 RID: 1109 RVA: 0x0001FD28 File Offset: 0x0001DF28
		public override bool MissionEnded(ref MissionResult missionResult)
		{
			return false;
		}

		// Token: 0x06000456 RID: 1110 RVA: 0x0001FD2C File Offset: 0x0001DF2C
		public override InquiryData OnEndMissionRequest(out bool canPlayerLeave)
		{
			canPlayerLeave = true;
			if (!this.IsPlayerPracticing)
			{
				return null;
			}
			return new InquiryData(new TextObject("{=zv49qE35}Practice Fight", null).ToString(), GameTexts.FindText("str_give_up_fight", null).ToString(), true, true, GameTexts.FindText("str_ok", null).ToString(), GameTexts.FindText("str_cancel", null).ToString(), new Action(base.Mission.OnEndMissionResult), null, "", 0f, null, null, null);
		}

		// Token: 0x06000457 RID: 1111 RVA: 0x0001FDAC File Offset: 0x0001DFAC
		public void StartPlayerPractice()
		{
			this.IsPlayerPracticing = true;
			this.AfterPractice = false;
			this.StartPractice();
		}

		// Token: 0x06000458 RID: 1112 RVA: 0x0001FDC4 File Offset: 0x0001DFC4
		private void StartPractice()
		{
			this.InitializeParticipantCharacters();
			SandBoxHelpers.MissionHelper.FadeOutAgents(base.Mission.Agents.Where((Agent agent) => this._participantAgents.Contains(agent) || agent.IsMount || agent.IsPlayerControlled), true, false);
			this._spawnedOpponentAgentCount = 0;
			this._aliveOpponentCount = 0;
			this._participantAgents.Clear();
			Mission.Current.ClearCorpses(false);
			base.Mission.RemoveSpawnedItemsAndMissiles();
			this.ArrangePlayerTeamEnmity();
			if (this.IsPlayerPracticing)
			{
				Agent agent2 = this.SpawnArenaAgent(base.Mission.PlayerTeam, this.GetSpawnFrame(false, true));
				agent2.WieldInitialWeapons(2);
				this.OpponentCountBeatenByPlayer = 0;
				this._participantAgents.Add(agent2);
			}
			int count = this._AIParticipantTeams.Count;
			int num = 0;
			while (this._spawnedOpponentAgentCount < 6)
			{
				this._participantAgents.Add(this.SpawnArenaAgent(this._AIParticipantTeams[num % count], this.GetSpawnFrame(false, true)));
				num++;
			}
			this._nextSpawnTime = base.Mission.CurrentTime + 14f;
		}

		// Token: 0x06000459 RID: 1113 RVA: 0x0001FEC6 File Offset: 0x0001E0C6
		private bool CheckPracticeEndedForPlayer()
		{
			return base.Mission.MainAgent == null || !base.Mission.MainAgent.IsActive() || this.RemainingOpponentCount == 0;
		}

		// Token: 0x0600045A RID: 1114 RVA: 0x0001FEF4 File Offset: 0x0001E0F4
		private void AddRandomWeapons(Equipment equipment, int spawnIndex)
		{
			int num = 1 + spawnIndex * 3 / 30;
			List<Equipment> list = (Game.Current.ObjectManager.GetObject<CharacterObject>(string.Concat(new object[]
			{
				"weapon_practice_stage_",
				num,
				"_",
				this._settlement.MapFaction.Culture.StringId
			})) ?? Game.Current.ObjectManager.GetObject<CharacterObject>("weapon_practice_stage_" + num + "_empire")).BattleEquipments.ToList<Equipment>();
			int num2 = MBRandom.RandomInt(list.Count);
			for (int i = 0; i <= 3; i++)
			{
				EquipmentElement equipmentFromSlot = list[num2].GetEquipmentFromSlot(i);
				if (equipmentFromSlot.Item != null)
				{
					equipment.AddEquipmentToSlotWithoutAgent(i, equipmentFromSlot);
				}
			}
		}

		// Token: 0x0600045B RID: 1115 RVA: 0x0001FFC4 File Offset: 0x0001E1C4
		private void AddRandomClothes(CharacterObject troop, Equipment equipment)
		{
			Equipment participantArmor = Campaign.Current.Models.TournamentModel.GetParticipantArmor(troop);
			for (int i = 0; i < 12; i++)
			{
				if (i > 4 && i != 10 && i != 11)
				{
					EquipmentElement equipmentFromSlot = participantArmor.GetEquipmentFromSlot(i);
					if (equipmentFromSlot.Item != null)
					{
						equipment.AddEquipmentToSlotWithoutAgent(i, equipmentFromSlot);
					}
				}
			}
		}

		// Token: 0x0600045C RID: 1116 RVA: 0x0002001C File Offset: 0x0001E21C
		private void InitializeTeams()
		{
			this._AIParticipantTeams = new List<Team>();
			base.Mission.Teams.Add(0, Hero.MainHero.MapFaction.Color, Hero.MainHero.MapFaction.Color2, null, true, false, true);
			base.Mission.PlayerTeam = base.Mission.DefenderTeam;
			this._tournamentMasterTeam = base.Mission.Teams.Add(-1, this._settlement.MapFaction.Color, this._settlement.MapFaction.Color2, null, true, false, true);
			while (this._AIParticipantTeams.Count < 6)
			{
				this._AIParticipantTeams.Add(base.Mission.Teams.Add(1, uint.MaxValue, uint.MaxValue, null, true, false, true));
			}
			for (int i = 0; i < this._AIParticipantTeams.Count; i++)
			{
				this._AIParticipantTeams[i].SetIsEnemyOf(this._tournamentMasterTeam, false);
				for (int j = i + 1; j < this._AIParticipantTeams.Count; j++)
				{
					this._AIParticipantTeams[i].SetIsEnemyOf(this._AIParticipantTeams[j], true);
				}
			}
		}

		// Token: 0x0600045D RID: 1117 RVA: 0x00020150 File Offset: 0x0001E350
		private void InitializeParticipantCharacters()
		{
			List<CharacterObject> participantCharacters = ArenaPracticeFightMissionController.GetParticipantCharacters(this._settlement);
			this._participantCharacters = participantCharacters.OrderBy((CharacterObject x) => x.Level).ToList<CharacterObject>();
		}

		// Token: 0x0600045E RID: 1118 RVA: 0x0002019C File Offset: 0x0001E39C
		public static List<CharacterObject> GetParticipantCharacters(Settlement settlement)
		{
			int num = 30;
			List<CharacterObject> list = new List<CharacterObject>();
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			if (list.Count < num && settlement.Town.GarrisonParty != null)
			{
				foreach (TroopRosterElement troopRosterElement in settlement.Town.GarrisonParty.MemberRoster.GetTroopRoster())
				{
					int num5 = num - list.Count;
					if (!list.Contains(troopRosterElement.Character) && troopRosterElement.Character.Tier == 3 && (float)num5 * 0.4f > (float)num2)
					{
						list.Add(troopRosterElement.Character);
						num2++;
					}
					else if (!list.Contains(troopRosterElement.Character) && troopRosterElement.Character.Tier == 4 && (float)num5 * 0.4f > (float)num3)
					{
						list.Add(troopRosterElement.Character);
						num3++;
					}
					else if (!list.Contains(troopRosterElement.Character) && troopRosterElement.Character.Tier == 5 && (float)num5 * 0.2f > (float)num4)
					{
						list.Add(troopRosterElement.Character);
						num4++;
					}
					if (list.Count >= num)
					{
						break;
					}
				}
			}
			if (list.Count < num)
			{
				List<CharacterObject> list2 = new List<CharacterObject>();
				ArenaPracticeFightMissionController.GetUpgradeTargets(((settlement != null) ? settlement.Culture : Game.Current.ObjectManager.GetObject<CultureObject>("empire")).BasicTroop, ref list2);
				int num6 = num - list.Count;
				using (List<CharacterObject>.Enumerator enumerator2 = list2.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						CharacterObject characterObject = enumerator2.Current;
						if (!list.Contains(characterObject) && characterObject.Tier == 3 && (float)num6 * 0.4f > (float)num2)
						{
							list.Add(characterObject);
							num2++;
						}
						else if (!list.Contains(characterObject) && characterObject.Tier == 4 && (float)num6 * 0.4f > (float)num3)
						{
							list.Add(characterObject);
							num3++;
						}
						else if (!list.Contains(characterObject) && characterObject.Tier == 5 && (float)num6 * 0.2f > (float)num4)
						{
							list.Add(characterObject);
							num4++;
						}
						if (list.Count >= num)
						{
							break;
						}
					}
					goto IL_284;
				}
				IL_256:
				int num7 = 0;
				while (num7 < list2.Count && list.Count < num)
				{
					list.Add(list2[num7]);
					num7++;
				}
				IL_284:
				if (list.Count < num)
				{
					goto IL_256;
				}
			}
			return list;
		}

		// Token: 0x0600045F RID: 1119 RVA: 0x00020454 File Offset: 0x0001E654
		private static void GetUpgradeTargets(CharacterObject troop, ref List<CharacterObject> list)
		{
			if (!list.Contains(troop) && troop.Tier >= 3)
			{
				list.Add(troop);
			}
			CharacterObject[] upgradeTargets = troop.UpgradeTargets;
			for (int i = 0; i < upgradeTargets.Length; i++)
			{
				ArenaPracticeFightMissionController.GetUpgradeTargets(upgradeTargets[i], ref list);
			}
		}

		// Token: 0x06000460 RID: 1120 RVA: 0x0002049C File Offset: 0x0001E69C
		private void ArrangePlayerTeamEnmity()
		{
			foreach (Team team in this._AIParticipantTeams)
			{
				team.SetIsEnemyOf(base.Mission.PlayerTeam, this.IsPlayerPracticing);
			}
		}

		// Token: 0x06000461 RID: 1121 RVA: 0x00020500 File Offset: 0x0001E700
		private Team GetStrongestTeamExceptPlayerTeam()
		{
			Team team = null;
			int num = -1;
			foreach (Team team2 in this._AIParticipantTeams)
			{
				int num2 = this.CalculateTeamPower(team2);
				if (num2 > num)
				{
					team = team2;
					num = num2;
				}
			}
			return team;
		}

		// Token: 0x06000462 RID: 1122 RVA: 0x00020564 File Offset: 0x0001E764
		private int CalculateTeamPower(Team team)
		{
			int num = 0;
			foreach (Agent agent in team.ActiveAgents)
			{
				num += agent.Character.Level * agent.KillCount + (int)MathF.Sqrt(agent.Health);
			}
			return num;
		}

		// Token: 0x06000463 RID: 1123 RVA: 0x000205D8 File Offset: 0x0001E7D8
		private MatrixFrame GetSpawnFrame(bool considerPlayerDistance, bool isInitialSpawn)
		{
			List<MatrixFrame> list = ((isInitialSpawn || Extensions.IsEmpty<MatrixFrame>(this._spawnFrames)) ? this._initialSpawnFrames : this._spawnFrames);
			if (list.Count == 1)
			{
				Debug.FailedAssert("Spawn point count is wrong! Arena practice spawn point set should be used in arena scenes.", "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox\\Missions\\MissionLogics\\Arena\\ArenaPracticeFightMissionController.cs", "GetSpawnFrame", 615);
				return list[0];
			}
			MatrixFrame matrixFrame;
			if (considerPlayerDistance && Agent.Main != null && Agent.Main.IsActive())
			{
				int num = MBRandom.RandomInt(list.Count);
				matrixFrame = list[num];
				float num2 = float.MinValue;
				for (int i = num + 1; i < num + list.Count; i++)
				{
					MatrixFrame matrixFrame2 = list[i % list.Count];
					float num3 = this.CalculateLocationScore(matrixFrame2);
					if (num3 >= 100f)
					{
						matrixFrame = matrixFrame2;
						break;
					}
					if (num3 > num2)
					{
						matrixFrame = matrixFrame2;
						num2 = num3;
					}
				}
			}
			else
			{
				int num4 = this._spawnedOpponentAgentCount;
				if (this.IsPlayerPracticing && Agent.Main != null)
				{
					num4++;
				}
				matrixFrame = list[num4 % list.Count];
			}
			return matrixFrame;
		}

		// Token: 0x06000464 RID: 1124 RVA: 0x000206DC File Offset: 0x0001E8DC
		private float CalculateLocationScore(MatrixFrame matrixFrame)
		{
			float num = 100f;
			float num2 = 0.25f;
			float num3 = 0.75f;
			if (matrixFrame.origin.DistanceSquared(Agent.Main.Position) < 144f)
			{
				num *= num2;
			}
			for (int i = 0; i < this._participantAgents.Count; i++)
			{
				if (this._participantAgents[i].Position.DistanceSquared(matrixFrame.origin) < 144f)
				{
					num *= num3;
				}
			}
			return num;
		}

		// Token: 0x04000205 RID: 517
		private const int AIParticipantCount = 30;

		// Token: 0x04000206 RID: 518
		private const int MaxAliveAgentCount = 6;

		// Token: 0x04000207 RID: 519
		private const int MaxSpawnInterval = 14;

		// Token: 0x04000208 RID: 520
		private const int MinSpawnDistanceSquared = 144;

		// Token: 0x04000209 RID: 521
		private const int TotalStageCount = 3;

		// Token: 0x0400020A RID: 522
		private const int PracticeFightTroopTierLimit = 3;

		// Token: 0x0400020B RID: 523
		public int TeleportTime = 5;

		// Token: 0x0400020C RID: 524
		private Settlement _settlement;

		// Token: 0x0400020D RID: 525
		private int _spawnedOpponentAgentCount;

		// Token: 0x0400020E RID: 526
		private int _aliveOpponentCount;

		// Token: 0x0400020F RID: 527
		private float _nextSpawnTime;

		// Token: 0x04000210 RID: 528
		private List<MatrixFrame> _initialSpawnFrames;

		// Token: 0x04000211 RID: 529
		private List<MatrixFrame> _spawnFrames;

		// Token: 0x04000212 RID: 530
		private List<Team> _AIParticipantTeams;

		// Token: 0x04000213 RID: 531
		private List<Agent> _participantAgents;

		// Token: 0x04000214 RID: 532
		private Team _tournamentMasterTeam;

		// Token: 0x04000215 RID: 533
		private BasicMissionTimer _teleportTimer;

		// Token: 0x04000216 RID: 534
		private List<CharacterObject> _participantCharacters;

		// Token: 0x0400021C RID: 540
		private const float XpShareForKill = 0.5f;

		// Token: 0x0400021D RID: 541
		private const float XpShareForDamage = 0.5f;
	}
}
