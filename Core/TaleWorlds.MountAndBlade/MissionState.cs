using System;
using System.Collections.Generic;
using System.Linq;
using NetworkMessages.FromClient;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Source.Missions.Handlers;

namespace TaleWorlds.MountAndBlade
{
	public class MissionState : GameState
	{
		public IMissionSystemHandler Handler { get; set; }

		public override bool IsMission
		{
			get
			{
				return true;
			}
		}

		public static MissionState Current { get; private set; }

		public Mission CurrentMission { get; private set; }

		public string MissionName { get; private set; }

		public bool Paused { get; set; }

		protected override void OnInitialize()
		{
			base.OnInitialize();
			MissionState.Current = this;
			LoadingWindow.EnableGlobalLoadingWindow();
		}

		protected override void OnFinalize()
		{
			base.OnFinalize();
			this.CurrentMission.OnMissionStateFinalize(this.CurrentMission.NeedsMemoryCleanup);
			this.CurrentMission = null;
			MissionState.Current = null;
		}

		protected override void OnActivate()
		{
			base.OnActivate();
			this.CurrentMission.OnMissionStateActivate();
		}

		protected override void OnDeactivate()
		{
			base.OnDeactivate();
			this.CurrentMission.OnMissionStateDeactivate();
		}

		protected override void OnIdleTick(float dt)
		{
			base.OnIdleTick(dt);
			if (this.CurrentMission != null && this.CurrentMission.CurrentState == Mission.State.Continuing)
			{
				this.CurrentMission.IdleTick(dt);
			}
		}

		protected override void OnTick(float realDt)
		{
			base.OnTick(realDt);
			if (this._isDelayedDisconnecting && this.CurrentMission != null && this.CurrentMission.CurrentState == Mission.State.Continuing)
			{
				BannerlordNetwork.EndMultiplayerLobbyMission();
			}
			if (this.CurrentMission == null)
			{
				return;
			}
			if (this.CurrentMission.CurrentState == Mission.State.NewlyCreated || this.CurrentMission.CurrentState == Mission.State.Initializing)
			{
				if (this.CurrentMission.CurrentState == Mission.State.NewlyCreated)
				{
					this.CurrentMission.ClearUnreferencedResources(this.CurrentMission.NeedsMemoryCleanup);
				}
				this.TickLoading(realDt);
			}
			else if (this.CurrentMission.CurrentState == Mission.State.Continuing || this.CurrentMission.MissionEnded)
			{
				if (this.MissionFastForwardAmount != 0f)
				{
					this.CurrentMission.FastForwardMission(this.MissionFastForwardAmount, 0.033f);
					this.MissionFastForwardAmount = 0f;
				}
				bool flag = false;
				if (this.MissionEndTime != 0f && this.CurrentMission.CurrentTime > this.MissionEndTime)
				{
					this.CurrentMission.EndMission();
					flag = true;
				}
				if (!flag && (this.Handler == null || this.Handler.RenderIsReady()))
				{
					this.TickMission(realDt);
				}
				if (flag && MBEditor._isEditorMissionOn)
				{
					MBEditor.LeaveEditMissionMode();
					this.TickMission(realDt);
				}
			}
			if (this.CurrentMission.CurrentState == Mission.State.Over)
			{
				if (MBGameManager.Current.IsEnding)
				{
					Game.Current.GameStateManager.CleanStates(0);
					return;
				}
				Game.Current.GameStateManager.PopState(0);
			}
		}

		private void TickMission(float realDt)
		{
			if (this._firstMissionTickAfterLoading && this.CurrentMission != null && this.CurrentMission.CurrentState == Mission.State.Continuing)
			{
				if (GameNetwork.IsClient)
				{
					int currentBattleIndex = GameNetwork.GetNetworkComponent<BaseNetworkComponent>().CurrentBattleIndex;
					MBDebug.Print(string.Format("Client: I finished loading battle with index: {0}. Sending confirmation to server.", currentBattleIndex), 0, Debug.DebugColor.White, 17179869184UL);
					GameNetwork.BeginModuleEventAsClient();
					GameNetwork.WriteMessage(new FinishedLoading(currentBattleIndex));
					GameNetwork.EndModuleEventAsClient();
					GameNetwork.SyncRelevantGameOptionsToServer();
				}
				this._firstMissionTickAfterLoading = false;
			}
			IMissionSystemHandler handler = this.Handler;
			if (handler != null)
			{
				handler.BeforeMissionTick(this.CurrentMission, realDt);
			}
			this.CurrentMission.PauseAITick = false;
			if (GameNetwork.IsSessionActive && this.CurrentMission.ClearSceneTimerElapsedTime < 0f)
			{
				this.CurrentMission.PauseAITick = true;
			}
			float num = realDt;
			if (this.Paused || MBCommon.IsPaused)
			{
				num = 0f;
			}
			else if (this.CurrentMission.FixedDeltaTimeMode)
			{
				num = this.CurrentMission.FixedDeltaTime;
			}
			if (!GameNetwork.IsSessionActive)
			{
				this.CurrentMission.UpdateSceneTimeSpeed();
				float timeSpeed = this.CurrentMission.Scene.TimeSpeed;
				num *= timeSpeed;
			}
			if (this.CurrentMission.ClearSceneTimerElapsedTime < -0.3f && !GameNetwork.IsClientOrReplay)
			{
				this.CurrentMission.ClearAgentActions();
			}
			if (this.CurrentMission.CurrentState == Mission.State.Continuing || this.CurrentMission.MissionEnded)
			{
				if (this.CurrentMission.IsFastForward)
				{
					float num2 = num * 9f;
					while (num2 > 1E-06f)
					{
						if (num2 > 0.1f)
						{
							this.TickMissionAux(0.1f, 0.1f, false, false);
							if (this.CurrentMission.CurrentState == Mission.State.Over)
							{
								break;
							}
							num2 -= 0.1f;
						}
						else
						{
							if (num2 > 0.0033333334f)
							{
								this.TickMissionAux(num2, num2, false, false);
							}
							num2 = 0f;
						}
					}
					if (this.CurrentMission.CurrentState != Mission.State.Over)
					{
						this.TickMissionAux(num, realDt, true, false);
					}
				}
				else
				{
					this.TickMissionAux(num, realDt, true, true);
				}
			}
			if (this.Handler != null)
			{
				this.Handler.AfterMissionTick(this.CurrentMission, realDt);
			}
			this._missionTickCount++;
		}

		private void TickMissionAux(float dt, float realDt, bool updateCamera, bool asyncAITick)
		{
			this.CurrentMission.Tick(dt);
			if (this._missionTickCount > 2)
			{
				this.CurrentMission.OnTick(dt, realDt, updateCamera, asyncAITick);
			}
		}

		private void TickLoading(float realDt)
		{
			this._tickCountBeforeLoad++;
			if (!this._missionInitializing && this._tickCountBeforeLoad > 0)
			{
				this.LoadMission();
				Utilities.SetLoadingScreenPercentage(0.01f);
				return;
			}
			if (this._missionInitializing && this.CurrentMission.IsLoadingFinished)
			{
				this.FinishMissionLoading();
			}
		}

		private void LoadMission()
		{
			foreach (MissionBehavior missionBehavior in this.CurrentMission.MissionBehaviors)
			{
				missionBehavior.OnMissionScreenPreLoad();
			}
			Utilities.ClearOldResourcesAndObjects();
			this._missionInitializing = true;
			this.CurrentMission.Initialize();
		}

		private void CreateMission(MissionInitializerRecord rec)
		{
			this.CurrentMission = new Mission(rec, this);
		}

		private Mission HandleOpenNew(string missionName, MissionInitializerRecord rec, InitializeMissionBehaviorsDelegate handler, bool addDefaultMissionBehaviors)
		{
			this.MissionName = missionName;
			this.CreateMission(rec);
			IEnumerable<MissionBehavior> enumerable = handler(this.CurrentMission);
			if (addDefaultMissionBehaviors)
			{
				enumerable = MissionState.AddDefaultMissionBehaviorsTo(this.CurrentMission, enumerable);
			}
			foreach (MissionBehavior missionBehavior in enumerable)
			{
				missionBehavior.OnAfterMissionCreated();
			}
			this.AddBehaviorsToMission(enumerable);
			if (this.Handler != null)
			{
				enumerable = new MissionBehavior[0];
				enumerable = this.Handler.OnAddBehaviors(enumerable, this.CurrentMission, missionName, addDefaultMissionBehaviors);
				this.AddBehaviorsToMission(enumerable);
			}
			return this.CurrentMission;
		}

		private void AddBehaviorsToMission(IEnumerable<MissionBehavior> behaviors)
		{
			MissionLogic[] array = (from behavior in behaviors.OfType<MissionLogic>()
				where !(behavior is MissionNetwork)
				select behavior).ToArray<MissionLogic>();
			MissionBehavior[] array2 = behaviors.Where((MissionBehavior behavior) => behavior != null && !(behavior is MissionNetwork) && !(behavior is MissionLogic)).ToArray<MissionBehavior>();
			MissionNetwork[] array3 = behaviors.OfType<MissionNetwork>().ToArray<MissionNetwork>();
			this.CurrentMission.InitializeStartingBehaviors(array, array2, array3);
		}

		private static bool IsRecordingActive()
		{
			if (GameNetwork.IsServer)
			{
				return MultiplayerOptions.OptionType.EnableMissionRecording.GetBoolValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
			}
			return MissionState.RecordMission && Game.Current.GameType.IsCoreOnlyGameMode;
		}

		public static Mission OpenNew(string missionName, MissionInitializerRecord rec, InitializeMissionBehaviorsDelegate handler, bool addDefaultMissionBehaviors = true, bool needsMemoryCleanup = true)
		{
			Debug.Print(string.Concat(new string[] { "Opening new mission ", missionName, " ", rec.SceneLevels, ".\n" }), 0, Debug.DebugColor.White, 17592186044416UL);
			if (!GameNetwork.IsClientOrReplay && !GameNetwork.IsServer)
			{
				MBCommon.CurrentGameType = (MissionState.IsRecordingActive() ? MBCommon.GameType.SingleRecord : MBCommon.GameType.Single);
			}
			Game.Current.OnMissionIsStarting(missionName, rec);
			MissionState missionState = Game.Current.GameStateManager.CreateState<MissionState>();
			Mission mission = missionState.HandleOpenNew(missionName, rec, handler, addDefaultMissionBehaviors);
			Game.Current.GameStateManager.PushState(missionState, 0);
			mission.NeedsMemoryCleanup = needsMemoryCleanup;
			return mission;
		}

		private static IEnumerable<MissionBehavior> AddDefaultMissionBehaviorsTo(Mission mission, IEnumerable<MissionBehavior> behaviors)
		{
			List<MissionBehavior> list = new List<MissionBehavior>();
			if (GameNetwork.IsSessionActive || GameNetwork.IsReplay)
			{
				list.Add(new MissionNetworkComponent());
			}
			if (MissionState.IsRecordingActive() && !GameNetwork.IsReplay)
			{
				list.Add(new RecordMissionLogic());
			}
			list.Add(new BasicMissionHandler());
			list.Add(new CasualtyHandler());
			list.Add(new AgentCommonAILogic());
			list.Add(new MissionGamepadHapticEffectsHandler());
			return list.Concat(behaviors);
		}

		private void FinishMissionLoading()
		{
			this._missionInitializing = false;
			this.CurrentMission.Scene.SetOwnerThread();
			Utilities.SetLoadingScreenPercentage(0.4f);
			for (int i = 0; i < 2; i++)
			{
				this.CurrentMission.Tick(0.001f);
			}
			Utilities.SetLoadingScreenPercentage(0.42f);
			IMissionSystemHandler handler = this.Handler;
			if (handler != null)
			{
				handler.OnMissionAfterStarting(this.CurrentMission);
			}
			Utilities.SetLoadingScreenPercentage(0.48f);
			this.CurrentMission.AfterStart();
			Utilities.SetLoadingScreenPercentage(0.56f);
			IMissionSystemHandler handler2 = this.Handler;
			if (handler2 != null)
			{
				handler2.OnMissionLoadingFinished(this.CurrentMission);
			}
			Utilities.SetLoadingScreenPercentage(0.62f);
			this.CurrentMission.Scene.ResumeLoadingRenderings();
		}

		public void BeginDelayedDisconnectFromMission()
		{
			this._isDelayedDisconnecting = true;
		}

		private const int MissionFastForwardSpeedMultiplier = 10;

		private bool _missionInitializing;

		private bool _firstMissionTickAfterLoading = true;

		private int _tickCountBeforeLoad;

		public static bool RecordMission;

		public float MissionFastForwardAmount;

		public float MissionEndTime;

		private bool _isDelayedDisconnecting;

		private int _missionTickCount;
	}
}
