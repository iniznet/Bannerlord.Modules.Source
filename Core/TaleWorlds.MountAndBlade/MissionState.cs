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
	// Token: 0x02000231 RID: 561
	public class MissionState : GameState
	{
		// Token: 0x1700061A RID: 1562
		// (get) Token: 0x06001F02 RID: 7938 RVA: 0x0006E457 File Offset: 0x0006C657
		// (set) Token: 0x06001F03 RID: 7939 RVA: 0x0006E45F File Offset: 0x0006C65F
		public IMissionSystemHandler Handler { get; set; }

		// Token: 0x1700061B RID: 1563
		// (get) Token: 0x06001F04 RID: 7940 RVA: 0x0006E468 File Offset: 0x0006C668
		public override bool IsMission
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700061C RID: 1564
		// (get) Token: 0x06001F05 RID: 7941 RVA: 0x0006E46B File Offset: 0x0006C66B
		// (set) Token: 0x06001F06 RID: 7942 RVA: 0x0006E472 File Offset: 0x0006C672
		public static MissionState Current { get; private set; }

		// Token: 0x1700061D RID: 1565
		// (get) Token: 0x06001F07 RID: 7943 RVA: 0x0006E47A File Offset: 0x0006C67A
		// (set) Token: 0x06001F08 RID: 7944 RVA: 0x0006E482 File Offset: 0x0006C682
		public Mission CurrentMission { get; private set; }

		// Token: 0x1700061E RID: 1566
		// (get) Token: 0x06001F09 RID: 7945 RVA: 0x0006E48B File Offset: 0x0006C68B
		// (set) Token: 0x06001F0A RID: 7946 RVA: 0x0006E493 File Offset: 0x0006C693
		public string MissionName { get; private set; }

		// Token: 0x1700061F RID: 1567
		// (get) Token: 0x06001F0B RID: 7947 RVA: 0x0006E49C File Offset: 0x0006C69C
		// (set) Token: 0x06001F0C RID: 7948 RVA: 0x0006E4A4 File Offset: 0x0006C6A4
		public bool Paused { get; set; }

		// Token: 0x06001F0D RID: 7949 RVA: 0x0006E4AD File Offset: 0x0006C6AD
		protected override void OnInitialize()
		{
			base.OnInitialize();
			MissionState.Current = this;
			LoadingWindow.EnableGlobalLoadingWindow();
		}

		// Token: 0x06001F0E RID: 7950 RVA: 0x0006E4C0 File Offset: 0x0006C6C0
		protected override void OnFinalize()
		{
			base.OnFinalize();
			this.CurrentMission.OnMissionStateFinalize(this.CurrentMission.NeedsMemoryCleanup);
			this.CurrentMission = null;
			MissionState.Current = null;
		}

		// Token: 0x06001F0F RID: 7951 RVA: 0x0006E4EB File Offset: 0x0006C6EB
		protected override void OnActivate()
		{
			base.OnActivate();
			this.CurrentMission.OnMissionStateActivate();
		}

		// Token: 0x06001F10 RID: 7952 RVA: 0x0006E4FE File Offset: 0x0006C6FE
		protected override void OnDeactivate()
		{
			base.OnDeactivate();
			this.CurrentMission.OnMissionStateDeactivate();
		}

		// Token: 0x06001F11 RID: 7953 RVA: 0x0006E511 File Offset: 0x0006C711
		protected override void OnIdleTick(float dt)
		{
			base.OnIdleTick(dt);
			if (this.CurrentMission != null && this.CurrentMission.CurrentState == Mission.State.Continuing)
			{
				this.CurrentMission.IdleTick(dt);
			}
		}

		// Token: 0x06001F12 RID: 7954 RVA: 0x0006E53C File Offset: 0x0006C73C
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

		// Token: 0x06001F13 RID: 7955 RVA: 0x0006E6B4 File Offset: 0x0006C8B4
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

		// Token: 0x06001F14 RID: 7956 RVA: 0x0006E8CF File Offset: 0x0006CACF
		private void TickMissionAux(float dt, float realDt, bool updateCamera, bool asyncAITick)
		{
			this.CurrentMission.Tick(dt);
			if (this._missionTickCount > 2)
			{
				this.CurrentMission.OnTick(dt, realDt, updateCamera, asyncAITick);
			}
		}

		// Token: 0x06001F15 RID: 7957 RVA: 0x0006E8F8 File Offset: 0x0006CAF8
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

		// Token: 0x06001F16 RID: 7958 RVA: 0x0006E950 File Offset: 0x0006CB50
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

		// Token: 0x06001F17 RID: 7959 RVA: 0x0006E9BC File Offset: 0x0006CBBC
		private void CreateMission(MissionInitializerRecord rec)
		{
			this.CurrentMission = new Mission(rec, this);
		}

		// Token: 0x06001F18 RID: 7960 RVA: 0x0006E9CC File Offset: 0x0006CBCC
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

		// Token: 0x06001F19 RID: 7961 RVA: 0x0006EA78 File Offset: 0x0006CC78
		private void AddBehaviorsToMission(IEnumerable<MissionBehavior> behaviors)
		{
			MissionLogic[] array = (from behavior in behaviors.OfType<MissionLogic>()
				where !(behavior is MissionNetwork)
				select behavior).ToArray<MissionLogic>();
			MissionBehavior[] array2 = behaviors.Where((MissionBehavior behavior) => behavior != null && !(behavior is MissionNetwork) && !(behavior is MissionLogic)).ToArray<MissionBehavior>();
			MissionNetwork[] array3 = behaviors.OfType<MissionNetwork>().ToArray<MissionNetwork>();
			this.CurrentMission.InitializeStartingBehaviors(array, array2, array3);
		}

		// Token: 0x06001F1A RID: 7962 RVA: 0x0006EAFA File Offset: 0x0006CCFA
		private static bool IsRecordingActive()
		{
			if (GameNetwork.IsServer)
			{
				return MultiplayerOptions.OptionType.EnableMissionRecording.GetBoolValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
			}
			return MissionState.RecordMission && Game.Current.GameType.IsCoreOnlyGameMode;
		}

		// Token: 0x06001F1B RID: 7963 RVA: 0x0006EB24 File Offset: 0x0006CD24
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

		// Token: 0x06001F1C RID: 7964 RVA: 0x0006EBD0 File Offset: 0x0006CDD0
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

		// Token: 0x06001F1D RID: 7965 RVA: 0x0006EC48 File Offset: 0x0006CE48
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

		// Token: 0x06001F1E RID: 7966 RVA: 0x0006ED03 File Offset: 0x0006CF03
		public void BeginDelayedDisconnectFromMission()
		{
			this._isDelayedDisconnecting = true;
		}

		// Token: 0x04000B4E RID: 2894
		private const int MissionFastForwardSpeedMultiplier = 10;

		// Token: 0x04000B4F RID: 2895
		private bool _missionInitializing;

		// Token: 0x04000B50 RID: 2896
		private bool _firstMissionTickAfterLoading = true;

		// Token: 0x04000B51 RID: 2897
		private int _tickCountBeforeLoad;

		// Token: 0x04000B52 RID: 2898
		public static bool RecordMission;

		// Token: 0x04000B54 RID: 2900
		public float MissionFastForwardAmount;

		// Token: 0x04000B55 RID: 2901
		public float MissionEndTime;

		// Token: 0x04000B5A RID: 2906
		private bool _isDelayedDisconnecting;

		// Token: 0x04000B5B RID: 2907
		private int _missionTickCount;
	}
}
