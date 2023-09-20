using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox.Conversation.MissionLogics
{
	// Token: 0x0200008F RID: 143
	public class ConversationMissionLogic : MissionLogic
	{
		// Token: 0x1700006E RID: 110
		// (get) Token: 0x060005E4 RID: 1508 RVA: 0x0002CADE File Offset: 0x0002ACDE
		private bool IsReadyForConversation
		{
			get
			{
				return this._isRenderingStarted && Agent.Main != null && Agent.Main.IsActive();
			}
		}

		// Token: 0x060005E5 RID: 1509 RVA: 0x0002CAFB File Offset: 0x0002ACFB
		public ConversationMissionLogic(ConversationCharacterData playerCharacterData, ConversationCharacterData otherCharacterData)
		{
			this._playerConversationData = playerCharacterData;
			this._otherSideConversationData = otherCharacterData;
			this._isCivilianEquipmentRequiredForLeader = otherCharacterData.IsCivilianEquipmentRequiredForLeader;
			this._isCivilianEquipmentRequiredForBodyGuards = otherCharacterData.IsCivilianEquipmentRequiredForBodyGuardCharacters;
			this._addBloodToAgents = new List<Agent>();
		}

		// Token: 0x060005E6 RID: 1510 RVA: 0x0002CB34 File Offset: 0x0002AD34
		public override void AfterStart()
		{
			base.AfterStart();
			this._realCameraController = base.Mission.CameraIsFirstPerson;
			base.Mission.CameraIsFirstPerson = true;
			IEnumerable<GameEntity> enumerable = base.Mission.Scene.FindEntitiesWithTag("binary_conversation_point");
			if (enumerable.Any<GameEntity>())
			{
				this._conversationSet = Extensions.GetRandomElement<GameEntity>(Extensions.ToMBList<GameEntity>(enumerable));
			}
			this._usedSpawnPoints = new List<GameEntity>();
			BattleSideEnum battleSideEnum = 1;
			if (PlayerSiege.PlayerSiegeEvent != null)
			{
				battleSideEnum = PlayerSiege.PlayerSide;
			}
			else if (PlayerEncounter.Current != null)
			{
				if (PlayerEncounter.InsideSettlement && PlayerEncounter.Current.OpponentSide != null)
				{
					battleSideEnum = 0;
				}
				else
				{
					battleSideEnum = 1;
				}
				if (PlayerEncounter.Current.EncounterSettlementAux != null && PlayerEncounter.Current.EncounterSettlementAux.MapFaction == Hero.MainHero.MapFaction)
				{
					if (PlayerEncounter.Current.EncounterSettlementAux.IsUnderSiege)
					{
						battleSideEnum = 0;
					}
					else
					{
						battleSideEnum = 1;
					}
				}
			}
			base.Mission.PlayerTeam = base.Mission.Teams.Add(battleSideEnum, Hero.MainHero.MapFaction.Color, Hero.MainHero.MapFaction.Color2, null, true, false, true);
			bool flag = this._otherSideConversationData.Character.Equipment[10].Item != null && this._otherSideConversationData.Character.Equipment[10].Item.HasHorseComponent && battleSideEnum == 0;
			MatrixFrame matrixFrame;
			MatrixFrame matrixFrame2;
			if (this._conversationSet != null)
			{
				if (base.Mission.PlayerTeam.IsDefender)
				{
					matrixFrame = this.GetDefenderSideSpawnFrame();
					matrixFrame2 = this.GetAttackerSideSpawnFrame(flag);
				}
				else
				{
					matrixFrame = this.GetAttackerSideSpawnFrame(flag);
					matrixFrame2 = this.GetDefenderSideSpawnFrame();
				}
			}
			else
			{
				matrixFrame = this.GetPlayerSideSpawnFrameInSettlement();
				matrixFrame2 = this.GetOtherSideSpawnFrameInSettlement(matrixFrame);
			}
			this.SpawnPlayer(this._playerConversationData, matrixFrame);
			this.SpawnOtherSide(this._otherSideConversationData, matrixFrame2, flag, !base.Mission.PlayerTeam.IsDefender);
		}

		// Token: 0x060005E7 RID: 1511 RVA: 0x0002CD20 File Offset: 0x0002AF20
		private void SpawnPlayer(ConversationCharacterData playerConversationData, MatrixFrame initialFrame)
		{
			MatrixFrame matrixFrame;
			matrixFrame..ctor(initialFrame.rotation, initialFrame.origin);
			matrixFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
			this.SpawnCharacter(CharacterObject.PlayerCharacter, playerConversationData, matrixFrame, ConversationMissionLogic.act_conversation_normal_loop);
		}

		// Token: 0x060005E8 RID: 1512 RVA: 0x0002CD60 File Offset: 0x0002AF60
		private void SpawnOtherSide(ConversationCharacterData characterData, MatrixFrame initialFrame, bool spawnWithHorse, bool isDefenderSide)
		{
			MatrixFrame matrixFrame;
			matrixFrame..ctor(initialFrame.rotation, initialFrame.origin);
			if (Agent.Main != null)
			{
				matrixFrame.rotation.f = Agent.Main.Position - matrixFrame.origin;
			}
			matrixFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
			Monster monsterWithSuffix = FaceGen.GetMonsterWithSuffix(characterData.Character.Race, "_settlement");
			AgentBuildData agentBuildData = new AgentBuildData(characterData.Character).TroopOrigin(new SimpleAgentOrigin(characterData.Character, -1, null, default(UniqueTroopDescriptor))).Team(base.Mission.PlayerTeam).Monster(monsterWithSuffix)
				.InitialPosition(ref matrixFrame.origin);
			Vec2 asVec = matrixFrame.rotation.f.AsVec2;
			AgentBuildData agentBuildData2 = agentBuildData.InitialDirection(ref asVec).NoHorses(!spawnWithHorse).CivilianEquipment(this._isCivilianEquipmentRequiredForLeader);
			PartyBase party = characterData.Party;
			bool flag;
			if (party == null)
			{
				flag = null != null;
			}
			else
			{
				Hero leaderHero = party.LeaderHero;
				flag = ((leaderHero != null) ? leaderHero.ClanBanner : null) != null;
			}
			if (flag)
			{
				agentBuildData2.Banner(characterData.Party.LeaderHero.ClanBanner);
			}
			else if (characterData.Party != null && characterData.Party.MapFaction != CampaignData.NeutralFaction)
			{
				AgentBuildData agentBuildData3 = agentBuildData2;
				PartyBase party2 = characterData.Party;
				Banner banner;
				if (party2 == null)
				{
					banner = null;
				}
				else
				{
					IFaction mapFaction = party2.MapFaction;
					banner = ((mapFaction != null) ? mapFaction.Banner : null);
				}
				agentBuildData3.Banner(banner);
			}
			if (spawnWithHorse)
			{
				agentBuildData2.MountKey(MountCreationKey.GetRandomMountKeyString(characterData.Character.Equipment[10].Item, characterData.Character.GetMountKeySeed()));
			}
			if (characterData.Party != null)
			{
				agentBuildData2.TroopOrigin(new PartyAgentOrigin(characterData.Party, characterData.Character, 0, new UniqueTroopDescriptor(FlattenedTroopRoster.GenerateUniqueNoFromParty(characterData.Party.MobileParty, 0)), false));
				agentBuildData2.ClothingColor1(characterData.Party.MapFaction.Color).ClothingColor2(characterData.Party.MapFaction.Color2);
			}
			Agent agent = base.Mission.SpawnAgent(agentBuildData2, false);
			if (characterData.SpawnedAfterFight)
			{
				this._addBloodToAgents.Add(agent);
			}
			if (agent.MountAgent == null)
			{
				agent.SetActionChannel(0, ConversationMissionLogic.act_conversation_normal_loop, false, 0UL, 0f, 1f, 0f, 0.4f, MBRandom.RandomFloat, false, -0.2f, 0, true);
			}
			agent.AgentVisuals.SetAgentLodZeroOrMax(true);
			this._curConversationPartnerAgent = agent;
			bool flag2 = characterData.Character.HeroObject != null && characterData.Character.HeroObject.IsPlayerCompanion;
			if (!characterData.NoBodyguards && !flag2)
			{
				this.SpawnBodyguards(isDefenderSide);
			}
		}

		// Token: 0x060005E9 RID: 1513 RVA: 0x0002CFF8 File Offset: 0x0002B1F8
		private MatrixFrame GetDefenderSideSpawnFrame()
		{
			MatrixFrame matrixFrame = MatrixFrame.Identity;
			foreach (GameEntity gameEntity in this._conversationSet.GetChildren())
			{
				if (gameEntity.HasTag("opponent_infantry_spawn"))
				{
					matrixFrame = gameEntity.GetGlobalFrame();
					break;
				}
			}
			matrixFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
			return matrixFrame;
		}

		// Token: 0x060005EA RID: 1514 RVA: 0x0002D06C File Offset: 0x0002B26C
		private MatrixFrame GetAttackerSideSpawnFrame(bool hasHorse)
		{
			MatrixFrame matrixFrame = MatrixFrame.Identity;
			foreach (GameEntity gameEntity in this._conversationSet.GetChildren())
			{
				if (hasHorse && gameEntity.HasTag("player_cavalry_spawn"))
				{
					matrixFrame = gameEntity.GetGlobalFrame();
					break;
				}
				if (gameEntity.HasTag("player_infantry_spawn"))
				{
					matrixFrame = gameEntity.GetGlobalFrame();
					break;
				}
			}
			matrixFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
			return matrixFrame;
		}

		// Token: 0x060005EB RID: 1515 RVA: 0x0002D0FC File Offset: 0x0002B2FC
		private MatrixFrame GetPlayerSideSpawnFrameInSettlement()
		{
			GameEntity gameEntity;
			if ((gameEntity = base.Mission.Scene.FindEntityWithTag("spawnpoint_player")) == null)
			{
				gameEntity = base.Mission.Scene.FindEntitiesWithTag("sp_player_conversation").FirstOrDefault<GameEntity>() ?? base.Mission.Scene.FindEntityWithTag("spawnpoint_player_outside");
			}
			GameEntity gameEntity2 = gameEntity;
			MatrixFrame matrixFrame = ((gameEntity2 != null) ? gameEntity2.GetFrame() : MatrixFrame.Identity);
			matrixFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
			return matrixFrame;
		}

		// Token: 0x060005EC RID: 1516 RVA: 0x0002D174 File Offset: 0x0002B374
		private MatrixFrame GetOtherSideSpawnFrameInSettlement(MatrixFrame playerFrame)
		{
			MatrixFrame matrixFrame = playerFrame;
			Vec3 vec;
			vec..ctor(playerFrame.rotation.f, -1f);
			vec.Normalize();
			matrixFrame.origin = playerFrame.origin + 4f * vec;
			matrixFrame.rotation.RotateAboutUp(3.1415927f);
			return matrixFrame;
		}

		// Token: 0x060005ED RID: 1517 RVA: 0x0002D1D1 File Offset: 0x0002B3D1
		public override void OnRenderingStarted()
		{
			this._isRenderingStarted = true;
			Debug.Print("\n ConversationMissionLogic::OnRenderingStarted\n", 0, 7, 64UL);
		}

		// Token: 0x060005EE RID: 1518 RVA: 0x0002D1E9 File Offset: 0x0002B3E9
		private void InitializeAfterCreation(Agent conversationPartnerAgent, PartyBase conversationPartnerParty)
		{
			Campaign.Current.ConversationManager.SetupAndStartMapConversation((conversationPartnerParty != null) ? conversationPartnerParty.MobileParty : null, conversationPartnerAgent, Mission.Current.MainAgentServer);
			base.Mission.SetMissionMode(1, true);
		}

		// Token: 0x060005EF RID: 1519 RVA: 0x0002D220 File Offset: 0x0002B420
		public override void OnMissionTick(float dt)
		{
			if (this._addBloodToAgents.Count > 0)
			{
				foreach (Agent agent in this._addBloodToAgents)
				{
					ValueTuple<sbyte, sbyte> randomPairOfRealBloodBurstBoneIndices = agent.GetRandomPairOfRealBloodBurstBoneIndices();
					if (randomPairOfRealBloodBurstBoneIndices.Item1 != -1 && randomPairOfRealBloodBurstBoneIndices.Item2 != -1)
					{
						agent.CreateBloodBurstAtLimb(randomPairOfRealBloodBurstBoneIndices.Item1, 0.1f + MBRandom.RandomFloat * 0.1f);
						agent.CreateBloodBurstAtLimb(randomPairOfRealBloodBurstBoneIndices.Item2, 0.2f + MBRandom.RandomFloat * 0.2f);
					}
				}
				this._addBloodToAgents.Clear();
			}
			if (!this._conversationStarted)
			{
				if (!this.IsReadyForConversation)
				{
					return;
				}
				this.InitializeAfterCreation(this._curConversationPartnerAgent, this._otherSideConversationData.Party);
				this._conversationStarted = true;
			}
			if (base.Mission.InputManager.IsGameKeyPressed(4))
			{
				Campaign.Current.ConversationManager.EndConversation();
			}
			if (!Campaign.Current.ConversationManager.IsConversationInProgress)
			{
				base.Mission.EndMission();
			}
		}

		// Token: 0x060005F0 RID: 1520 RVA: 0x0002D34C File Offset: 0x0002B54C
		private void SpawnBodyguards(bool isDefenderSide)
		{
			int num = 2;
			ConversationCharacterData otherSideConversationData = this._otherSideConversationData;
			if (otherSideConversationData.Party == null)
			{
				return;
			}
			TroopRoster memberRoster = otherSideConversationData.Party.MemberRoster;
			int num2 = memberRoster.TotalManCount;
			if (memberRoster.Contains(CharacterObject.PlayerCharacter))
			{
				num2--;
			}
			if (num2 < num + 1)
			{
				return;
			}
			List<CharacterObject> list = new List<CharacterObject>();
			using (List<TroopRosterElement>.Enumerator enumerator = memberRoster.GetTroopRoster().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TroopRosterElement troopRosterElement = enumerator.Current;
					if (troopRosterElement.Character.IsHero && otherSideConversationData.Character != troopRosterElement.Character && !list.Contains(troopRosterElement.Character) && troopRosterElement.Character.HeroObject.IsWounded && !troopRosterElement.Character.IsPlayerCharacter)
					{
						list.Add(troopRosterElement.Character);
					}
				}
				goto IL_16B;
			}
			IL_D4:
			foreach (TroopRosterElement troopRosterElement2 in from k in memberRoster.GetTroopRoster()
				orderby k.Character.Level descending
				select k)
			{
				if ((!otherSideConversationData.Character.IsHero || otherSideConversationData.Character != troopRosterElement2.Character) && !troopRosterElement2.Character.IsPlayerCharacter)
				{
					list.Add(troopRosterElement2.Character);
				}
				if (list.Count == num)
				{
					break;
				}
			}
			IL_16B:
			if (list.Count >= num)
			{
				List<ActionIndexCache> list2 = new List<ActionIndexCache>
				{
					ActionIndexCache.Create("act_stand_1"),
					ActionIndexCache.Create("act_inventory_idle_start"),
					ActionIndexCache.Create("act_inventory_idle"),
					ConversationMissionLogic.act_conversation_normal_loop,
					ActionIndexCache.Create("act_conversation_warrior_loop"),
					ActionIndexCache.Create("act_conversation_hip_loop"),
					ActionIndexCache.Create("act_conversation_closed_loop"),
					ActionIndexCache.Create("act_conversation_demure_loop")
				};
				for (int i = 0; i < num; i++)
				{
					int num3 = new Random().Next(0, list.Count);
					int num4 = MBRandom.RandomInt(0, list2.Count);
					this.SpawnCharacter(list[num3], otherSideConversationData, this.GetBodyguardSpawnFrame(list[num3].HasMount(), isDefenderSide), list2[num4]);
					list2.RemoveAt(num4);
					list.RemoveAt(num3);
				}
				return;
			}
			goto IL_D4;
		}

		// Token: 0x060005F1 RID: 1521 RVA: 0x0002D5E4 File Offset: 0x0002B7E4
		private void SpawnCharacter(CharacterObject character, ConversationCharacterData characterData, MatrixFrame initialFrame, ActionIndexCache conversationAction)
		{
			Monster monsterWithSuffix = FaceGen.GetMonsterWithSuffix(character.Race, "_settlement");
			AgentBuildData agentBuildData = new AgentBuildData(character).TroopOrigin(new SimpleAgentOrigin(character, -1, null, default(UniqueTroopDescriptor))).Team(base.Mission.PlayerTeam).Monster(monsterWithSuffix)
				.InitialPosition(ref initialFrame.origin);
			Vec2 vec = initialFrame.rotation.f.AsVec2;
			vec = vec.Normalized();
			AgentBuildData agentBuildData2 = agentBuildData.InitialDirection(ref vec).NoHorses(character.HasMount()).NoWeapons(characterData.NoWeapon)
				.CivilianEquipment((character == CharacterObject.PlayerCharacter) ? this._isCivilianEquipmentRequiredForLeader : this._isCivilianEquipmentRequiredForBodyGuards);
			PartyBase party = characterData.Party;
			bool flag;
			if (party == null)
			{
				flag = null != null;
			}
			else
			{
				Hero leaderHero = party.LeaderHero;
				flag = ((leaderHero != null) ? leaderHero.ClanBanner : null) != null;
			}
			if (flag)
			{
				agentBuildData2.Banner(characterData.Party.LeaderHero.ClanBanner);
			}
			else if (characterData.Party != null)
			{
				PartyBase party2 = characterData.Party;
				if (((party2 != null) ? party2.MapFaction : null) != CampaignData.NeutralFaction)
				{
					agentBuildData2.Banner(characterData.Party.MapFaction.Banner);
				}
			}
			if (characterData.Party != null)
			{
				agentBuildData2.ClothingColor1(characterData.Party.MapFaction.Color).ClothingColor2(characterData.Party.MapFaction.Color2);
			}
			if (characterData.Character == CharacterObject.PlayerCharacter)
			{
				agentBuildData2.Controller(2);
			}
			Agent agent = base.Mission.SpawnAgent(agentBuildData2, false);
			agent.AgentVisuals.SetAgentLodZeroOrMax(true);
			agent.SetLookAgent(Agent.Main);
			AnimationSystemData animationSystemData = MonsterExtensions.FillAnimationSystemData(agentBuildData2.AgentMonster, MBGlobals.GetActionSetWithSuffix(agentBuildData2.AgentMonster, agentBuildData2.AgentIsFemale, "_poses"), character.GetStepSize(), false);
			agent.SetActionSet(ref animationSystemData);
			if (characterData.Character == CharacterObject.PlayerCharacter)
			{
				agent.AgentVisuals.GetSkeleton().TickAnimationsAndForceUpdate(0.1f, initialFrame, true);
			}
			if (characterData.SpawnedAfterFight)
			{
				this._addBloodToAgents.Add(agent);
				return;
			}
			if (agent.MountAgent == null)
			{
				agent.SetActionChannel(0, conversationAction, false, 0UL, 0f, 1f, 0f, 0.4f, MBRandom.RandomFloat * 0.8f, false, -0.2f, 0, true);
			}
		}

		// Token: 0x060005F2 RID: 1522 RVA: 0x0002D81C File Offset: 0x0002BA1C
		private MatrixFrame GetBodyguardSpawnFrame(bool spawnWithHorse, bool isDefenderSide)
		{
			MatrixFrame matrixFrame = MatrixFrame.Identity;
			foreach (GameEntity gameEntity in this._conversationSet.GetChildren())
			{
				if (!isDefenderSide)
				{
					if (spawnWithHorse && gameEntity.HasTag("player_bodyguard_cavalry_spawn") && !this._usedSpawnPoints.Contains(gameEntity))
					{
						this._usedSpawnPoints.Add(gameEntity);
						matrixFrame = gameEntity.GetGlobalFrame();
						break;
					}
					if (gameEntity.HasTag("player_bodyguard_infantry_spawn") && !this._usedSpawnPoints.Contains(gameEntity))
					{
						this._usedSpawnPoints.Add(gameEntity);
						matrixFrame = gameEntity.GetGlobalFrame();
						break;
					}
				}
				else
				{
					if (spawnWithHorse && gameEntity.HasTag("opponent_bodyguard_cavalry_spawn") && !this._usedSpawnPoints.Contains(gameEntity))
					{
						this._usedSpawnPoints.Add(gameEntity);
						matrixFrame = gameEntity.GetGlobalFrame();
						break;
					}
					if (gameEntity.HasTag("opponent_bodyguard_infantry_spawn") && !this._usedSpawnPoints.Contains(gameEntity))
					{
						this._usedSpawnPoints.Add(gameEntity);
						matrixFrame = gameEntity.GetGlobalFrame();
						break;
					}
				}
			}
			matrixFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
			return matrixFrame;
		}

		// Token: 0x060005F3 RID: 1523 RVA: 0x0002D950 File Offset: 0x0002BB50
		protected override void OnEndMission()
		{
			this._conversationSet = null;
			base.Mission.CameraIsFirstPerson = this._realCameraController;
		}

		// Token: 0x040002BB RID: 699
		private static readonly ActionIndexCache act_conversation_normal_loop = ActionIndexCache.Create("act_conversation_normal_loop");

		// Token: 0x040002BC RID: 700
		private ConversationCharacterData _otherSideConversationData;

		// Token: 0x040002BD RID: 701
		private ConversationCharacterData _playerConversationData;

		// Token: 0x040002BE RID: 702
		private readonly List<Agent> _addBloodToAgents;

		// Token: 0x040002BF RID: 703
		private Agent _curConversationPartnerAgent;

		// Token: 0x040002C0 RID: 704
		private bool _isRenderingStarted;

		// Token: 0x040002C1 RID: 705
		private bool _conversationStarted;

		// Token: 0x040002C2 RID: 706
		private bool _isCivilianEquipmentRequiredForLeader;

		// Token: 0x040002C3 RID: 707
		private bool _isCivilianEquipmentRequiredForBodyGuards;

		// Token: 0x040002C4 RID: 708
		private List<GameEntity> _usedSpawnPoints;

		// Token: 0x040002C5 RID: 709
		private GameEntity _conversationSet;

		// Token: 0x040002C6 RID: 710
		private bool _realCameraController;
	}
}
