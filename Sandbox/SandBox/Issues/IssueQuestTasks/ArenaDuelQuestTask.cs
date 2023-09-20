using System;
using System.Collections.Generic;
using System.Linq;
using SandBox.Missions.MissionLogics.Arena;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox.Issues.IssueQuestTasks
{
	// Token: 0x02000082 RID: 130
	public class ArenaDuelQuestTask : QuestTaskBase
	{
		// Token: 0x06000581 RID: 1409 RVA: 0x00026F6C File Offset: 0x0002516C
		public ArenaDuelQuestTask(CharacterObject duelOpponentCharacter, Settlement settlement, Action onSucceededAction, Action onFailedAction, DialogFlow dialogFlow = null)
			: base(dialogFlow, onSucceededAction, onFailedAction, null)
		{
			this._opponentCharacter = duelOpponentCharacter;
			this._settlement = settlement;
		}

		// Token: 0x06000582 RID: 1410 RVA: 0x00026F88 File Offset: 0x00025188
		public void AfterStart(IMission mission)
		{
			if (Mission.Current.HasMissionBehavior<ArenaDuelMissionBehavior>() && PlayerEncounter.LocationEncounter.Settlement == this._settlement)
			{
				this.InitializeTeams();
				List<MatrixFrame> list = (from e in Mission.Current.Scene.FindEntitiesWithTag("sp_arena_respawn")
					select e.GetGlobalFrame()).ToList<MatrixFrame>();
				MatrixFrame matrixFrame = list[MBRandom.RandomInt(list.Count)];
				float maxValue = float.MaxValue;
				MatrixFrame matrixFrame2 = matrixFrame;
				foreach (MatrixFrame matrixFrame3 in list)
				{
					if (matrixFrame != matrixFrame3)
					{
						Vec3 origin = matrixFrame3.origin;
						if (origin.DistanceSquared(matrixFrame.origin) < maxValue)
						{
							matrixFrame2 = matrixFrame3;
						}
					}
				}
				matrixFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
				matrixFrame2.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
				this._playerAgent = this.SpawnArenaAgent(CharacterObject.PlayerCharacter, Mission.Current.PlayerTeam, matrixFrame);
				this._opponentAgent = this.SpawnArenaAgent(this._opponentCharacter, Mission.Current.PlayerEnemyTeam, matrixFrame2);
			}
		}

		// Token: 0x06000583 RID: 1411 RVA: 0x000270C8 File Offset: 0x000252C8
		public override void SetReferences()
		{
			CampaignEvents.AfterMissionStarted.AddNonSerializedListener(this, new Action<IMission>(this.AfterStart));
			CampaignEvents.GameMenuOpened.AddNonSerializedListener(this, new Action<MenuCallbackArgs>(this.OnGameMenuOpened));
			CampaignEvents.MissionTickEvent.AddNonSerializedListener(this, new Action<float>(this.MissionTick));
		}

		// Token: 0x06000584 RID: 1412 RVA: 0x0002711A File Offset: 0x0002531A
		public void OnGameMenuOpened(MenuCallbackArgs args)
		{
			if (Hero.MainHero.CurrentSettlement == this._settlement)
			{
				if (this._duelStarted)
				{
					if (this._opponentAgent.IsActive())
					{
						base.Finish(1);
						return;
					}
					base.Finish(0);
					return;
				}
				else
				{
					this.OpenArenaDuelMission();
				}
			}
		}

		// Token: 0x06000585 RID: 1413 RVA: 0x0002715C File Offset: 0x0002535C
		public void MissionTick(float dt)
		{
			if (Mission.Current.HasMissionBehavior<ArenaDuelMissionBehavior>() && PlayerEncounter.LocationEncounter.Settlement == this._settlement && ((this._playerAgent != null && !this._playerAgent.IsActive()) || (this._opponentAgent != null && !this._opponentAgent.IsActive())))
			{
				if (this._missionEndTimer != null && this._missionEndTimer.ElapsedTime > 4f)
				{
					Mission.Current.EndMission();
					return;
				}
				if (this._missionEndTimer == null && ((this._playerAgent != null && !this._playerAgent.IsActive()) || (this._opponentAgent != null && !this._opponentAgent.IsActive())))
				{
					this._missionEndTimer = new BasicMissionTimer();
				}
			}
		}

		// Token: 0x06000586 RID: 1414 RVA: 0x0002721C File Offset: 0x0002541C
		private void OpenArenaDuelMission()
		{
			Location locationWithId = this._settlement.LocationComplex.GetLocationWithId("arena");
			int num = (this._settlement.IsTown ? this._settlement.Town.GetWallLevel() : 1);
			SandBoxMissions.OpenArenaDuelMission(locationWithId.GetSceneName(num), locationWithId);
			this._duelStarted = true;
		}

		// Token: 0x06000587 RID: 1415 RVA: 0x00027278 File Offset: 0x00025478
		private void InitializeTeams()
		{
			Mission.Current.Teams.Add(0, Hero.MainHero.MapFaction.Color, Hero.MainHero.MapFaction.Color2, null, true, false, true);
			Mission.Current.Teams.Add(1, Hero.MainHero.MapFaction.Color2, Hero.MainHero.MapFaction.Color, null, true, false, true);
			Mission.Current.PlayerTeam = Mission.Current.DefenderTeam;
		}

		// Token: 0x06000588 RID: 1416 RVA: 0x00027300 File Offset: 0x00025500
		private Agent SpawnArenaAgent(CharacterObject character, Team team, MatrixFrame frame)
		{
			if (team == Mission.Current.PlayerTeam)
			{
				character = CharacterObject.PlayerCharacter;
			}
			Equipment equipment = this._settlement.Culture.DuelPreset.Equipment;
			Mission mission = Mission.Current;
			AgentBuildData agentBuildData = new AgentBuildData(character).Team(team).ClothingColor1(team.Color).ClothingColor2(team.Color2)
				.InitialPosition(ref frame.origin);
			Vec2 vec = frame.rotation.f.AsVec2;
			vec = vec.Normalized();
			Agent agent = mission.SpawnAgent(agentBuildData.InitialDirection(ref vec).NoHorses(true).Equipment(equipment)
				.TroopOrigin(new SimpleAgentOrigin(character, -1, null, default(UniqueTroopDescriptor)))
				.Controller((character == CharacterObject.PlayerCharacter) ? 2 : 1), false);
			if (agent.IsAIControlled)
			{
				agent.SetWatchState(2);
			}
			return agent;
		}

		// Token: 0x040002AE RID: 686
		private Settlement _settlement;

		// Token: 0x040002AF RID: 687
		private CharacterObject _opponentCharacter;

		// Token: 0x040002B0 RID: 688
		private Agent _playerAgent;

		// Token: 0x040002B1 RID: 689
		private Agent _opponentAgent;

		// Token: 0x040002B2 RID: 690
		private bool _duelStarted;

		// Token: 0x040002B3 RID: 691
		private BasicMissionTimer _missionEndTimer;
	}
}
