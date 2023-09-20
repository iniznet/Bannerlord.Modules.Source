using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade;

namespace SandBox
{
	// Token: 0x02000006 RID: 6
	public class EditorSceneMissionManager : MBGameManager
	{
		// Token: 0x0600000C RID: 12 RVA: 0x00003324 File Offset: 0x00001524
		public EditorSceneMissionManager(string missionName, string sceneName, string levels, bool forReplay, string replayFileName, bool isRecord, float startTime, float endTime)
		{
			this._missionName = missionName;
			this._sceneName = sceneName;
			this._levels = levels;
			this._forReplay = forReplay;
			this._replayFileName = replayFileName;
			this._isRecord = isRecord;
			this._startTime = startTime;
			this._endTime = endTime;
		}

		// Token: 0x0600000D RID: 13 RVA: 0x00003374 File Offset: 0x00001574
		protected override void DoLoadingForGameManager(GameManagerLoadingSteps gameManagerLoadingSteps, out GameManagerLoadingSteps nextStep)
		{
			nextStep = -1;
			switch (gameManagerLoadingSteps)
			{
			case 0:
			{
				MBGameManager.LoadModuleData(false);
				MBDebug.Print("Game creating...", 0, 12, 17592186044416UL);
				MBGlobals.InitializeReferences();
				Game game;
				if (this._forReplay)
				{
					game = Game.CreateGame(new EditorGame(), this);
				}
				else
				{
					Campaign campaign = new Campaign(2);
					game = Game.CreateGame(campaign, this);
					campaign.SetLoadingParameters(0);
				}
				game.DoLoading();
				nextStep = 1;
				return;
			}
			case 1:
			{
				bool flag = true;
				foreach (MBSubModuleBase mbsubModuleBase in Module.CurrentModule.SubModules)
				{
					flag = flag && mbsubModuleBase.DoLoading(Game.Current);
				}
				nextStep = (flag ? 2 : 1);
				return;
			}
			case 2:
				MBGameManager.StartNewGame();
				nextStep = 3;
				return;
			case 3:
				nextStep = (Game.Current.DoLoading() ? 4 : 3);
				return;
			case 4:
				nextStep = 5;
				return;
			case 5:
				nextStep = -1;
				return;
			default:
				return;
			}
		}

		// Token: 0x0600000E RID: 14 RVA: 0x00003480 File Offset: 0x00001680
		public override void OnLoadFinished()
		{
			base.OnLoadFinished();
			MBGlobals.InitializeReferences();
			if (!this._forReplay)
			{
				Campaign.Current.InitializeGamePlayReferences();
			}
			Module.CurrentModule.StartMissionForEditorAux(this._missionName, this._sceneName, this._levels, this._forReplay, this._replayFileName, this._isRecord);
			MissionState.Current.MissionFastForwardAmount = this._startTime;
			MissionState.Current.MissionEndTime = this._endTime;
		}

		// Token: 0x0400001F RID: 31
		private string _missionName;

		// Token: 0x04000020 RID: 32
		private string _sceneName;

		// Token: 0x04000021 RID: 33
		private string _levels;

		// Token: 0x04000022 RID: 34
		private bool _forReplay;

		// Token: 0x04000023 RID: 35
		private string _replayFileName;

		// Token: 0x04000024 RID: 36
		private bool _isRecord;

		// Token: 0x04000025 RID: 37
		private float _startTime;

		// Token: 0x04000026 RID: 38
		private float _endTime;
	}
}
