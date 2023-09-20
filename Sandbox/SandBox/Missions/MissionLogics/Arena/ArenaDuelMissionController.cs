using System;
using System.Linq;
using SandBox.Tournaments.MissionLogics;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.MissionLogics.Arena
{
	// Token: 0x02000064 RID: 100
	public class ArenaDuelMissionController : MissionLogic
	{
		// Token: 0x06000439 RID: 1081 RVA: 0x0001F1E2 File Offset: 0x0001D3E2
		public ArenaDuelMissionController(CharacterObject duelCharacter, bool requireCivilianEquipment, bool spawnBothSideWithHorses, Action<CharacterObject> onDuelEnd, float customAgentHealth)
		{
			this._duelCharacter = duelCharacter;
			this._requireCivilianEquipment = requireCivilianEquipment;
			this._spawnBothSideWithHorses = spawnBothSideWithHorses;
			this._customAgentHealth = customAgentHealth;
			ArenaDuelMissionController._onDuelEnd = onDuelEnd;
		}

		// Token: 0x0600043A RID: 1082 RVA: 0x0001F210 File Offset: 0x0001D410
		public override void AfterStart()
		{
			this._duelHasEnded = false;
			this._duelEndTimer = new BasicMissionTimer();
			this.DeactivateOtherTournamentSets();
			this.InitializeMissionTeams();
			this._initialSpawnFrames = Extensions.ToMBList<MatrixFrame>(from e in base.Mission.Scene.FindEntitiesWithTag("sp_arena")
				select e.GetGlobalFrame());
			for (int i = 0; i < this._initialSpawnFrames.Count; i++)
			{
				MatrixFrame matrixFrame = this._initialSpawnFrames[i];
				matrixFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
				this._initialSpawnFrames[i] = matrixFrame;
			}
			MatrixFrame randomElement = Extensions.GetRandomElement<MatrixFrame>(this._initialSpawnFrames);
			this._initialSpawnFrames.Remove(randomElement);
			MatrixFrame randomElement2 = Extensions.GetRandomElement<MatrixFrame>(this._initialSpawnFrames);
			this.SpawnAgent(CharacterObject.PlayerCharacter, randomElement);
			this._duelAgent = this.SpawnAgent(this._duelCharacter, randomElement2);
			this._duelAgent.Defensiveness = 1f;
		}

		// Token: 0x0600043B RID: 1083 RVA: 0x0001F310 File Offset: 0x0001D510
		private void InitializeMissionTeams()
		{
			base.Mission.Teams.Add(0, Hero.MainHero.MapFaction.Color, Hero.MainHero.MapFaction.Color2, null, true, false, true);
			base.Mission.Teams.Add(1, this._duelCharacter.Culture.Color, this._duelCharacter.Culture.Color2, null, true, false, true);
			base.Mission.PlayerTeam = base.Mission.Teams.Defender;
		}

		// Token: 0x0600043C RID: 1084 RVA: 0x0001F3A2 File Offset: 0x0001D5A2
		private void DeactivateOtherTournamentSets()
		{
			TournamentBehavior.DeleteTournamentSetsExcept(base.Mission.Scene.FindEntityWithTag("tournament_fight"));
		}

		// Token: 0x0600043D RID: 1085 RVA: 0x0001F3C0 File Offset: 0x0001D5C0
		private Agent SpawnAgent(CharacterObject character, MatrixFrame spawnFrame)
		{
			AgentBuildData agentBuildData = new AgentBuildData(character);
			agentBuildData.BodyProperties(character.GetBodyPropertiesMax());
			Mission mission = base.Mission;
			AgentBuildData agentBuildData2 = agentBuildData.Team((character == CharacterObject.PlayerCharacter) ? base.Mission.PlayerTeam : base.Mission.PlayerEnemyTeam).InitialPosition(ref spawnFrame.origin);
			Vec2 vec = spawnFrame.rotation.f.AsVec2;
			vec = vec.Normalized();
			Agent agent = mission.SpawnAgent(agentBuildData2.InitialDirection(ref vec).NoHorses(!this._spawnBothSideWithHorses).Equipment(this._requireCivilianEquipment ? character.FirstCivilianEquipment : character.FirstBattleEquipment)
				.TroopOrigin(new SimpleAgentOrigin(character, -1, null, default(UniqueTroopDescriptor))), false);
			agent.FadeIn();
			if (character == CharacterObject.PlayerCharacter)
			{
				agent.Controller = 2;
			}
			if (agent.IsAIControlled)
			{
				agent.SetWatchState(2);
			}
			agent.Health = this._customAgentHealth;
			agent.BaseHealthLimit = this._customAgentHealth;
			agent.HealthLimit = this._customAgentHealth;
			return agent;
		}

		// Token: 0x0600043E RID: 1086 RVA: 0x0001F4CC File Offset: 0x0001D6CC
		public override void OnMissionTick(float dt)
		{
			if (this._duelHasEnded && this._duelEndTimer.ElapsedTime > 4f)
			{
				GameTexts.SetVariable("leave_key", HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("Generic", 4)));
				MBInformationManager.AddQuickInformation(GameTexts.FindText("str_duel_has_ended", null), 0, null, "");
				this._duelEndTimer.Reset();
			}
		}

		// Token: 0x0600043F RID: 1087 RVA: 0x0001F530 File Offset: 0x0001D730
		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow killingBlow)
		{
			if (ArenaDuelMissionController._onDuelEnd != null)
			{
				ArenaDuelMissionController._onDuelEnd((affectedAgent == this._duelAgent) ? CharacterObject.PlayerCharacter : this._duelCharacter);
				ArenaDuelMissionController._onDuelEnd = null;
				this._duelHasEnded = true;
				this._duelEndTimer.Reset();
			}
		}

		// Token: 0x06000440 RID: 1088 RVA: 0x0001F57C File Offset: 0x0001D77C
		public override InquiryData OnEndMissionRequest(out bool canPlayerLeave)
		{
			canPlayerLeave = true;
			if (!this._duelHasEnded)
			{
				canPlayerLeave = false;
				MBInformationManager.AddQuickInformation(GameTexts.FindText("str_can_not_retreat_duel_ongoing", null), 0, null, "");
			}
			return null;
		}

		// Token: 0x040001FC RID: 508
		private CharacterObject _duelCharacter;

		// Token: 0x040001FD RID: 509
		private bool _requireCivilianEquipment;

		// Token: 0x040001FE RID: 510
		private bool _spawnBothSideWithHorses;

		// Token: 0x040001FF RID: 511
		private bool _duelHasEnded;

		// Token: 0x04000200 RID: 512
		private Agent _duelAgent;

		// Token: 0x04000201 RID: 513
		private float _customAgentHealth;

		// Token: 0x04000202 RID: 514
		private BasicMissionTimer _duelEndTimer;

		// Token: 0x04000203 RID: 515
		private MBList<MatrixFrame> _initialSpawnFrames;

		// Token: 0x04000204 RID: 516
		private static Action<CharacterObject> _onDuelEnd;
	}
}
