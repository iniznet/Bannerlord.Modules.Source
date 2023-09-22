using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade;

namespace SandBox
{
	public class EditorSceneMissionManager : MBGameManager
	{
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

		public override void OnAfterCampaignStart(Game game)
		{
		}

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

		private string _missionName;

		private string _sceneName;

		private string _levels;

		private bool _forReplay;

		private string _replayFileName;

		private bool _isRecord;

		private float _startTime;

		private float _endTime;
	}
}
