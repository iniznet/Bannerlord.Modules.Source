using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.Screens;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.MissionViews.Order;
using TaleWorlds.MountAndBlade.ViewModelCollection;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.MountAndBlade.View.Screens
{
	// Token: 0x02000030 RID: 48
	[GameStateScreen(typeof(MissionState))]
	public class MissionScreen : ScreenBase, IMissionSystemHandler, IGameStateListener, IMissionScreen, IMissionListener
	{
		// Token: 0x17000022 RID: 34
		// (get) Token: 0x060001D0 RID: 464 RVA: 0x0000F3E3 File Offset: 0x0000D5E3
		// (set) Token: 0x060001D1 RID: 465 RVA: 0x0000F3EB File Offset: 0x0000D5EB
		public bool LockCameraMovement { get; private set; }

		// Token: 0x14000001 RID: 1
		// (add) Token: 0x060001D2 RID: 466 RVA: 0x0000F3F4 File Offset: 0x0000D5F4
		// (remove) Token: 0x060001D3 RID: 467 RVA: 0x0000F42C File Offset: 0x0000D62C
		public event MissionScreen.OnSpectateAgentDelegate OnSpectateAgentFocusIn;

		// Token: 0x14000002 RID: 2
		// (add) Token: 0x060001D4 RID: 468 RVA: 0x0000F464 File Offset: 0x0000D664
		// (remove) Token: 0x060001D5 RID: 469 RVA: 0x0000F49C File Offset: 0x0000D69C
		public event MissionScreen.OnSpectateAgentDelegate OnSpectateAgentFocusOut;

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x060001D6 RID: 470 RVA: 0x0000F4D1 File Offset: 0x0000D6D1
		// (set) Token: 0x060001D7 RID: 471 RVA: 0x0000F4D9 File Offset: 0x0000D6D9
		public OrderFlag OrderFlag { get; set; }

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x060001D8 RID: 472 RVA: 0x0000F4E2 File Offset: 0x0000D6E2
		// (set) Token: 0x060001D9 RID: 473 RVA: 0x0000F4EA File Offset: 0x0000D6EA
		public Camera CombatCamera { get; private set; }

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x060001DA RID: 474 RVA: 0x0000F4F3 File Offset: 0x0000D6F3
		// (set) Token: 0x060001DB RID: 475 RVA: 0x0000F4FB File Offset: 0x0000D6FB
		public Camera CustomCamera
		{
			get
			{
				return this._customCamera;
			}
			set
			{
				this._customCamera = value;
			}
		}

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x060001DC RID: 476 RVA: 0x0000F504 File Offset: 0x0000D704
		// (set) Token: 0x060001DD RID: 477 RVA: 0x0000F50C File Offset: 0x0000D70C
		public float CameraBearing { get; private set; }

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x060001DE RID: 478 RVA: 0x0000F515 File Offset: 0x0000D715
		// (set) Token: 0x060001DF RID: 479 RVA: 0x0000F51D File Offset: 0x0000D71D
		public float MaxCameraZoom { get; private set; } = 1f;

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x060001E0 RID: 480 RVA: 0x0000F526 File Offset: 0x0000D726
		// (set) Token: 0x060001E1 RID: 481 RVA: 0x0000F52E File Offset: 0x0000D72E
		public float CameraElevation { get; private set; }

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x060001E2 RID: 482 RVA: 0x0000F537 File Offset: 0x0000D737
		// (set) Token: 0x060001E3 RID: 483 RVA: 0x0000F53F File Offset: 0x0000D73F
		public float CameraResultDistanceToTarget { get; private set; }

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x060001E4 RID: 484 RVA: 0x0000F548 File Offset: 0x0000D748
		// (set) Token: 0x060001E5 RID: 485 RVA: 0x0000F550 File Offset: 0x0000D750
		public float CameraViewAngle { get; private set; }

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x060001E6 RID: 486 RVA: 0x0000F559 File Offset: 0x0000D759
		// (set) Token: 0x060001E7 RID: 487 RVA: 0x0000F561 File Offset: 0x0000D761
		public bool IsPhotoModeEnabled { get; private set; }

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x060001E8 RID: 488 RVA: 0x0000F56A File Offset: 0x0000D76A
		// (set) Token: 0x060001E9 RID: 489 RVA: 0x0000F572 File Offset: 0x0000D772
		public bool IsConversationActive { get; private set; }

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x060001EA RID: 490 RVA: 0x0000F57B File Offset: 0x0000D77B
		public bool IsDeploymentActive
		{
			get
			{
				return this.Mission.Mode == 6;
			}
		}

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x060001EB RID: 491 RVA: 0x0000F58B File Offset: 0x0000D78B
		// (set) Token: 0x060001EC RID: 492 RVA: 0x0000F593 File Offset: 0x0000D793
		public SceneLayer SceneLayer { get; private set; }

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x060001ED RID: 493 RVA: 0x0000F59C File Offset: 0x0000D79C
		public SceneView SceneView
		{
			get
			{
				SceneLayer sceneLayer = this.SceneLayer;
				if (sceneLayer == null)
				{
					return null;
				}
				return sceneLayer.SceneView;
			}
		}

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x060001EE RID: 494 RVA: 0x0000F5AF File Offset: 0x0000D7AF
		// (set) Token: 0x060001EF RID: 495 RVA: 0x0000F5B7 File Offset: 0x0000D7B7
		public Mission Mission { get; private set; }

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x060001F0 RID: 496 RVA: 0x0000F5C0 File Offset: 0x0000D7C0
		// (set) Token: 0x060001F1 RID: 497 RVA: 0x0000F5C8 File Offset: 0x0000D7C8
		public bool IsCheatGhostMode { get; set; }

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x060001F2 RID: 498 RVA: 0x0000F5D1 File Offset: 0x0000D7D1
		// (set) Token: 0x060001F3 RID: 499 RVA: 0x0000F5D9 File Offset: 0x0000D7D9
		public bool IsRadialMenuActive { get; private set; }

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x060001F4 RID: 500 RVA: 0x0000F5E2 File Offset: 0x0000D7E2
		public IInputContext InputManager
		{
			get
			{
				return this.Mission.InputManager;
			}
		}

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x060001F5 RID: 501 RVA: 0x0000F5EF File Offset: 0x0000D7EF
		private bool IsOrderMenuOpen
		{
			get
			{
				return this.Mission.IsOrderMenuOpen;
			}
		}

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x060001F6 RID: 502 RVA: 0x0000F5FC File Offset: 0x0000D7FC
		private bool IsTransferMenuOpen
		{
			get
			{
				return this.Mission.IsTransferMenuOpen;
			}
		}

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x060001F7 RID: 503 RVA: 0x0000F609 File Offset: 0x0000D809
		// (set) Token: 0x060001F8 RID: 504 RVA: 0x0000F614 File Offset: 0x0000D814
		public Agent LastFollowedAgent
		{
			get
			{
				return this._lastFollowedAgent;
			}
			private set
			{
				if (this._lastFollowedAgent != value)
				{
					Agent lastFollowedAgent = this._lastFollowedAgent;
					this._lastFollowedAgent = value;
					NetworkCommunicator myPeer = GameNetwork.MyPeer;
					MissionPeer missionPeer = ((myPeer != null) ? PeerExtensions.GetComponent<MissionPeer>(myPeer) : null);
					if (GameNetwork.IsMyPeerReady)
					{
						if (missionPeer != null)
						{
							missionPeer.FollowedAgent = this._lastFollowedAgent;
						}
						else
						{
							Debug.FailedAssert("MyPeer.IsSynchronized but myMissionPeer == null", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.View\\Screens\\MissionScreen.cs", "LastFollowedAgent", 214);
						}
					}
					if (lastFollowedAgent != null)
					{
						MissionScreen.OnSpectateAgentDelegate onSpectateAgentFocusOut = this.OnSpectateAgentFocusOut;
						if (onSpectateAgentFocusOut != null)
						{
							onSpectateAgentFocusOut(lastFollowedAgent);
						}
					}
					if (this._lastFollowedAgent != null)
					{
						if (this._lastFollowedAgent == this.Mission.MainAgent)
						{
							Agent mainAgent = this.Mission.MainAgent;
							mainAgent.OnMainAgentWieldedItemChange = (Agent.OnMainAgentWieldedItemChangeDelegate)Delegate.Combine(mainAgent.OnMainAgentWieldedItemChange, new Agent.OnMainAgentWieldedItemChangeDelegate(this.OnMainAgentWeaponChanged));
							this.ResetMaxCameraZoom();
						}
						MissionScreen.OnSpectateAgentDelegate onSpectateAgentFocusIn = this.OnSpectateAgentFocusIn;
						if (onSpectateAgentFocusIn != null)
						{
							onSpectateAgentFocusIn(this._lastFollowedAgent);
						}
					}
					if (this._lastFollowedAgent == this._agentToFollowOverride)
					{
						this._agentToFollowOverride = null;
					}
				}
			}
		}

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x060001F9 RID: 505 RVA: 0x0000F70C File Offset: 0x0000D90C
		// (set) Token: 0x060001FA RID: 506 RVA: 0x0000F714 File Offset: 0x0000D914
		public IAgentVisual LastFollowedAgentVisuals { get; set; }

		// Token: 0x060001FB RID: 507 RVA: 0x0000F71D File Offset: 0x0000D91D
		float IMissionScreen.GetCameraElevation()
		{
			return this.CameraElevation;
		}

		// Token: 0x060001FC RID: 508 RVA: 0x0000F725 File Offset: 0x0000D925
		public void SetOrderFlagVisibility(bool value)
		{
			if (this.OrderFlag != null)
			{
				this.OrderFlag.IsVisible = value;
			}
		}

		// Token: 0x060001FD RID: 509 RVA: 0x0000F73B File Offset: 0x0000D93B
		public string GetFollowText()
		{
			if (this.LastFollowedAgent == null)
			{
				return "";
			}
			return this.LastFollowedAgent.Name.ToString();
		}

		// Token: 0x060001FE RID: 510 RVA: 0x0000F75C File Offset: 0x0000D95C
		public string GetFollowPartyText()
		{
			if (this.LastFollowedAgent != null)
			{
				TextObject textObject = new TextObject("{=xsC8Ierj}({BATTLE_COMBATANT})", null);
				textObject.SetTextVariable("BATTLE_COMBATANT", this.LastFollowedAgent.Origin.BattleCombatant.Name);
				return textObject.ToString();
			}
			return "";
		}

		// Token: 0x060001FF RID: 511 RVA: 0x0000F7A8 File Offset: 0x0000D9A8
		public bool SetDisplayDialog(bool value)
		{
			bool flag = this._displayingDialog != value;
			this._displayingDialog = value;
			return flag;
		}

		// Token: 0x06000200 RID: 512 RVA: 0x0000F7BD File Offset: 0x0000D9BD
		bool IMissionScreen.GetDisplayDialog()
		{
			return this._displayingDialog;
		}

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x06000201 RID: 513 RVA: 0x0000F7C5 File Offset: 0x0000D9C5
		public override bool MouseVisible
		{
			get
			{
				return ScreenManager.GetMouseVisibility();
			}
		}

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x06000202 RID: 514 RVA: 0x0000F7CC File Offset: 0x0000D9CC
		public bool IsMissionTickable
		{
			get
			{
				return base.IsActive && this.Mission != null && (this.Mission.CurrentState == 2 || this.Mission.MissionEnded);
			}
		}

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x06000203 RID: 515 RVA: 0x0000F7FB File Offset: 0x0000D9FB
		// (set) Token: 0x06000204 RID: 516 RVA: 0x0000F803 File Offset: 0x0000DA03
		public bool PhotoModeRequiresMouse { get; private set; }

		// Token: 0x06000205 RID: 517 RVA: 0x0000F80C File Offset: 0x0000DA0C
		public MissionScreen(MissionState missionState)
		{
			missionState.Handler = this;
			this._emptyUILayer = new SceneLayer("SceneLayer", true, true);
			((SceneLayer)this._emptyUILayer).SceneView.SetEnable(false);
			this._missionState = missionState;
			this.Mission = missionState.CurrentMission;
			this.CombatCamera = Camera.CreateCamera();
			this._missionViews = new List<MissionView>();
		}

		// Token: 0x06000206 RID: 518 RVA: 0x0000F8D4 File Offset: 0x0000DAD4
		protected override void OnInitialize()
		{
			MBDebug.Print("-------MissionScreen-OnInitialize", 0, 12, 17592186044416UL);
			base.OnInitialize();
			Module.CurrentModule.SkinsXMLHasChanged += this.OnSkinsXMLChanged;
			this.CameraViewAngle = 65f;
			this._cameraTarget = new Vec3(0f, 0f, 10f, -1f);
			this.CameraBearing = 0f;
			this.CameraElevation = -0.2f;
			this._cameraBearingDelta = 0f;
			this._cameraElevationDelta = 0f;
			this._cameraSpecialTargetAddedBearing = 0f;
			this._cameraSpecialCurrentAddedBearing = 0f;
			this._cameraSpecialTargetAddedElevation = 0f;
			this._cameraSpecialCurrentAddedElevation = 0f;
			this._cameraSpecialTargetPositionToAdd = Vec3.Zero;
			this._cameraSpecialCurrentPositionToAdd = Vec3.Zero;
			this._cameraSpecialTargetDistanceToAdd = 0f;
			this._cameraSpecialCurrentDistanceToAdd = 0f;
			this._cameraSpecialCurrentFOV = 65f;
			this._cameraSpecialTargetFOV = 65f;
			this._cameraAddedElevation = 0f;
			this._cameraTargetAddedHeight = 0f;
			this._cameraDeploymentHeightToAdd = 0f;
			this._lastCameraAddedDistance = 0f;
			this.CameraResultDistanceToTarget = 0f;
			this._cameraSpeed = Vec3.Zero;
			this._cameraSpeedMultiplier = 1f;
			this._cameraHeightLimit = 0f;
			this._cameraAddSpecialMovement = false;
			this._cameraAddSpecialPositionalMovement = false;
			this._cameraApplySpecialMovementsInstantly = false;
			this._cameraSmoothMode = false;
			this.CustomCamera = null;
		}

		// Token: 0x06000207 RID: 519 RVA: 0x0000FA54 File Offset: 0x0000DC54
		private void InitializeMissionView()
		{
			this._missionState.Paused = false;
			this.SceneLayer = new SceneLayer("SceneLayer", true, true);
			this.SceneLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("Generic"));
			this.SceneLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
			this.SceneLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericCampaignPanelsGameKeyCategory"));
			this.SceneLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("ScoreboardHotKeyCategory"));
			this.SceneLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("CombatHotKeyCategory"));
			this.SceneLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("Cheats"));
			this.Mission.InputManager = this.SceneLayer.Input;
			base.AddLayer(this.SceneLayer);
			this.SceneView.SetScene(this.Mission.Scene);
			this.SceneView.SetSceneUsesShadows(true);
			this.SceneView.SetAcceptGlobalDebugRenderObjects(true);
			this.SceneView.SetResolutionScaling(true);
			this._missionMainAgentController = this.Mission.GetMissionBehavior<MissionMainAgentController>();
			this._missionLobbyComponent = Mission.Current.GetMissionBehavior<MissionLobbyComponent>();
			this._missionCameraModeLogic = this.Mission.MissionBehaviors.FirstOrDefault((MissionBehavior b) => b is ICameraModeLogic) as ICameraModeLogic;
			using (List<MissionBehavior>.Enumerator enumerator = this.Mission.MissionBehaviors.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					MissionView missionView;
					if ((missionView = enumerator.Current as MissionView) != null)
					{
						missionView.MissionScreen = this;
						missionView.OnMissionScreenInitialize();
					}
				}
			}
			this.Mission.AgentVisualCreator = new AgentVisualsCreator();
			this._mpGameModeBase = this.Mission.GetMissionBehavior<MissionMultiplayerGameModeBaseClient>();
		}

		// Token: 0x06000208 RID: 520 RVA: 0x0000FC4C File Offset: 0x0000DE4C
		protected override void OnActivate()
		{
			base.OnActivate();
			this.ActivateLoadingScreen();
			if (this.Mission != null && this.Mission.MissionEnded)
			{
				MissionScreen missionScreen = ScreenManager.TopScreen as MissionScreen;
				if (missionScreen != null)
				{
					ScreenManager.TopScreen.DeactivateAllLayers();
					missionScreen.SceneView.SetEnable(false);
				}
			}
		}

		// Token: 0x06000209 RID: 521 RVA: 0x0000FCA0 File Offset: 0x0000DEA0
		protected override void OnResume()
		{
			base.OnResume();
			if (this.Mission != null && this.Mission.MissionEnded)
			{
				MissionScreen missionScreen = ScreenManager.TopScreen as MissionScreen;
				if (missionScreen != null)
				{
					ScreenManager.TopScreen.DeactivateAllLayers();
					missionScreen.SceneView.SetEnable(false);
				}
			}
		}

		// Token: 0x0600020A RID: 522 RVA: 0x0000FCEC File Offset: 0x0000DEEC
		public override void OnFocusChangeOnGameWindow(bool focusGained)
		{
			base.OnFocusChangeOnGameWindow(focusGained);
			if (!LoadingWindow.IsLoadingWindowActive)
			{
				Func<bool> isAnyInquiryActive = InformationManager.IsAnyInquiryActive;
				if (isAnyInquiryActive != null && !isAnyInquiryActive())
				{
					Mission mission = this.Mission;
					List<MissionBehavior> list;
					if (mission == null)
					{
						list = null;
					}
					else
					{
						List<MissionBehavior> missionBehaviors = mission.MissionBehaviors;
						if (missionBehaviors == null)
						{
							list = null;
						}
						else
						{
							list = (from v in missionBehaviors
								where v is MissionView
								orderby ((MissionView)v).ViewOrderPriority
								select v).ToList<MissionBehavior>();
						}
					}
					List<MissionBehavior> list2 = list;
					if (list2 != null)
					{
						for (int i = 0; i < list2.Count; i++)
						{
							(list2[i] as MissionView).OnFocusChangeOnGameWindow(focusGained);
						}
					}
				}
			}
			this.IsFocusLost = !focusGained;
		}

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x0600020B RID: 523 RVA: 0x0000FDBA File Offset: 0x0000DFBA
		// (set) Token: 0x0600020C RID: 524 RVA: 0x0000FDC2 File Offset: 0x0000DFC2
		public bool IsFocusLost { get; private set; }

		// Token: 0x0600020D RID: 525 RVA: 0x0000FDCC File Offset: 0x0000DFCC
		public bool IsOpeningEscapeMenuOnFocusChangeAllowed()
		{
			Mission mission = this.Mission;
			List<MissionBehavior> list;
			if (mission == null)
			{
				list = null;
			}
			else
			{
				List<MissionBehavior> missionBehaviors = mission.MissionBehaviors;
				if (missionBehaviors == null)
				{
					list = null;
				}
				else
				{
					list = (from v in missionBehaviors
						where v is MissionView
						orderby ((MissionView)v).ViewOrderPriority
						select v).ToList<MissionBehavior>();
				}
			}
			List<MissionBehavior> list2 = list;
			if (list2 != null)
			{
				using (List<MissionBehavior>.Enumerator enumerator = list2.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (!(enumerator.Current as MissionView).IsOpeningEscapeMenuOnFocusChangeAllowed())
						{
							return false;
						}
					}
				}
				return true;
			}
			return true;
		}

		// Token: 0x0600020E RID: 526 RVA: 0x0000FE90 File Offset: 0x0000E090
		public void SetExtraCameraParameters(bool newForceCanZoom, float newCameraRayCastStartingPointOffset)
		{
			this._forceCanZoom = newForceCanZoom;
			this._cameraRayCastOffset = newCameraRayCastStartingPointOffset;
		}

		// Token: 0x0600020F RID: 527 RVA: 0x0000FEA0 File Offset: 0x0000E0A0
		public void SetCustomAgentListToSpectateGatherer(MissionScreen.GatherCustomAgentListToSpectateDelegate gatherer)
		{
			this._gatherCustomAgentListToSpectate = gatherer;
		}

		// Token: 0x06000210 RID: 528 RVA: 0x0000FEAC File Offset: 0x0000E0AC
		public void UpdateFreeCamera(MatrixFrame frame)
		{
			this.CombatCamera.Frame = frame;
			Vec3 vec = -frame.rotation.u;
			this.CameraBearing = vec.RotationZ;
			Vec3 vec2;
			vec2..ctor(0f, 0f, 1f, -1f);
			this.CameraElevation = MathF.Acos(Vec3.DotProduct(vec2, vec)) - 1.5707964f;
		}

		// Token: 0x06000211 RID: 529 RVA: 0x0000FF18 File Offset: 0x0000E118
		protected override void OnFrameTick(float dt)
		{
			if (this.SceneLayer != null)
			{
				bool flag = MBDebug.IsErrorReportModeActive();
				if (flag)
				{
					this._missionState.Paused = MBDebug.IsErrorReportModePauseMission();
				}
				if (base.DebugInput.IsHotKeyPressed("MissionScreenHotkeyFixCamera"))
				{
					this._fixCamera = !this._fixCamera;
				}
				flag = flag || this._fixCamera;
				if (this.IsPhotoModeEnabled)
				{
					flag = flag || this.PhotoModeRequiresMouse;
				}
				this.SceneLayer.InputRestrictions.SetMouseVisibility(flag);
			}
			if (this.Mission != null)
			{
				if (this.IsMissionTickable)
				{
					foreach (MissionView missionView in this._missionViews)
					{
						missionView.OnMissionScreenTick(dt);
					}
				}
				this.HandleInputs();
			}
		}

		// Token: 0x06000212 RID: 530 RVA: 0x0000FFF4 File Offset: 0x0000E1F4
		private void ActivateMissionView()
		{
			MBDebug.Print("-------MissionScreen-OnActivate", 0, 12, 17592186044416UL);
			this.Mission.OnMainAgentChanged += this.Mission_OnMainAgentChanged;
			this.Mission.OnBeforeAgentRemoved += new Mission.OnBeforeAgentRemovedDelegate(this.Mission_OnBeforeAgentRemoved);
			this._cameraBearingDelta = 0f;
			this._cameraElevationDelta = 0f;
			this.SetCameraFrameToMapView();
			this.CheckForUpdateCamera(1E-05f);
			this.Mission.ResetFirstThirdPersonView();
			if (MBEditor.EditModeEnabled && MBEditor.IsEditModeOn)
			{
				MBEditor.EnterEditMissionMode(this.Mission);
			}
			foreach (MissionView missionView in this._missionViews)
			{
				missionView.OnMissionScreenActivate();
			}
		}

		// Token: 0x06000213 RID: 531 RVA: 0x000100D4 File Offset: 0x0000E2D4
		private void Mission_OnMainAgentChanged(object sender, PropertyChangedEventArgs e)
		{
			if (this.Mission.MainAgent != null)
			{
				this._isPlayerAgentAdded = true;
			}
		}

		// Token: 0x06000214 RID: 532 RVA: 0x000100EA File Offset: 0x0000E2EA
		private void Mission_OnBeforeAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow killingBlow)
		{
			if (affectedAgent == this._agentToFollowOverride)
			{
				this._agentToFollowOverride = null;
				return;
			}
			if (affectedAgent == this.Mission.MainAgent)
			{
				this._agentToFollowOverride = affectorAgent;
			}
		}

		// Token: 0x06000215 RID: 533 RVA: 0x00010112 File Offset: 0x0000E312
		public void OnMainAgentWeaponChanged()
		{
			this.ResetMaxCameraZoom();
		}

		// Token: 0x06000216 RID: 534 RVA: 0x0001011C File Offset: 0x0000E31C
		private void ResetMaxCameraZoom()
		{
			if (this.LastFollowedAgent == null || this.LastFollowedAgent != this.Mission.MainAgent)
			{
				this.MaxCameraZoom = 1f;
				return;
			}
			this.MaxCameraZoom = ((Mission.Current != null) ? MathF.Max(1f, Mission.Current.GetMainAgentMaxCameraZoom()) : 1f);
		}

		// Token: 0x06000217 RID: 535 RVA: 0x00010178 File Offset: 0x0000E378
		protected override void OnDeactivate()
		{
			base.OnDeactivate();
			MBDebug.Print("-------MissionScreen-OnDeactivate", 0, 12, 17592186044416UL);
			if (this.Mission == null)
			{
				return;
			}
			this.Mission.OnMainAgentChanged -= this.Mission_OnMainAgentChanged;
			this.Mission.OnBeforeAgentRemoved -= new Mission.OnBeforeAgentRemovedDelegate(this.Mission_OnBeforeAgentRemoved);
			foreach (MissionView missionView in this._missionViews)
			{
				missionView.OnMissionScreenDeactivate();
			}
			this._isRenderingStarted = false;
			this._loadingScreenFramesLeft = 15;
		}

		// Token: 0x06000218 RID: 536 RVA: 0x0001022C File Offset: 0x0000E42C
		protected override void OnFinalize()
		{
			MBDebug.Print("-------MissionScreen-OnFinalize", 0, 12, 17592186044416UL);
			Module.CurrentModule.SkinsXMLHasChanged -= this.OnSkinsXMLChanged;
			LoadingWindow.EnableGlobalLoadingWindow();
			if (this.Mission != null)
			{
				this.Mission.InputManager = null;
			}
			this.Mission = null;
			this.OrderFlag = null;
			this.SceneLayer = null;
			this._missionMainAgentController = null;
			this.CombatCamera = null;
			this._customCamera = null;
			this._missionState = null;
			base.OnFinalize();
		}

		// Token: 0x06000219 RID: 537 RVA: 0x000102B8 File Offset: 0x0000E4B8
		private static IEnumerable<MissionBehavior> AddDefaultMissionBehaviorsTo(Mission mission, IEnumerable<MissionBehavior> behaviors)
		{
			List<MissionBehavior> list = new List<MissionBehavior>();
			IEnumerable<MissionBehavior> enumerable = ViewCreatorManager.CreateDefaultMissionBehaviors(mission);
			list.AddRange(enumerable);
			return behaviors.Concat(list);
		}

		// Token: 0x0600021A RID: 538 RVA: 0x000102E0 File Offset: 0x0000E4E0
		private void OnSkinsXMLChanged()
		{
			foreach (Agent agent in Mission.Current.Agents)
			{
				agent.EquipItemsFromSpawnEquipment(true);
				agent.UpdateAgentProperties();
				agent.AgentVisuals.UpdateSkeletonScale(agent.SpawnEquipment.BodyDeformType);
			}
		}

		// Token: 0x0600021B RID: 539 RVA: 0x00010354 File Offset: 0x0000E554
		private void OnSceneRenderingStarted()
		{
			LoadingWindow.DisableGlobalLoadingWindow();
			Utilities.SetScreenTextRenderingState(true);
			foreach (MissionView missionView in this._missionViews)
			{
				missionView.OnSceneRenderingStarted();
			}
		}

		// Token: 0x0600021C RID: 540 RVA: 0x000103B0 File Offset: 0x0000E5B0
		[CommandLineFunctionality.CommandLineArgumentFunction("fix_camera_toggle", "mission")]
		public static string ToggleFixedMissionCamera(List<string> strings)
		{
			MissionScreen missionScreen = ScreenManager.TopScreen as MissionScreen;
			if (missionScreen != null)
			{
				MissionScreen.SetFixedMissionCameraActive(!missionScreen._fixCamera);
			}
			return "Done";
		}

		// Token: 0x0600021D RID: 541 RVA: 0x000103E0 File Offset: 0x0000E5E0
		public static void SetFixedMissionCameraActive(bool active)
		{
			MissionScreen missionScreen = ScreenManager.TopScreen as MissionScreen;
			if (missionScreen != null)
			{
				missionScreen._fixCamera = active;
				missionScreen.SceneLayer.InputRestrictions.SetMouseVisibility(missionScreen._fixCamera);
			}
		}

		// Token: 0x0600021E RID: 542 RVA: 0x00010418 File Offset: 0x0000E618
		[CommandLineFunctionality.CommandLineArgumentFunction("set_shift_camera_speed", "mission")]
		public static string SetShiftCameraSpeed(List<string> strings)
		{
			MissionScreen missionScreen = ScreenManager.TopScreen as MissionScreen;
			if (missionScreen == null)
			{
				return "No Mission Available";
			}
			int num;
			if (strings.Count > 0 && int.TryParse(strings[0], out num))
			{
				missionScreen._shiftSpeedMultiplier = num;
				return "Done";
			}
			return "Current multiplier is " + missionScreen._shiftSpeedMultiplier.ToString();
		}

		// Token: 0x0600021F RID: 543 RVA: 0x00010474 File Offset: 0x0000E674
		[CommandLineFunctionality.CommandLineArgumentFunction("set_camera_position", "mission")]
		public static string SetCameraPosition(List<string> strings)
		{
			if (GameNetwork.IsSessionActive)
			{
				return "Does not work on multiplayer.";
			}
			if (strings.Count < 3)
			{
				return "You need to enter 3 arguments.";
			}
			List<float> list = new List<float>();
			for (int i = 0; i < strings.Count; i++)
			{
				float num = 0f;
				if (!float.TryParse(strings[i], out num))
				{
					return "Argument " + (i + 1) + " is not valid.";
				}
				list.Add(num);
			}
			MissionScreen missionScreen = ScreenManager.TopScreen as MissionScreen;
			if (missionScreen != null)
			{
				missionScreen.IsCheatGhostMode = true;
				missionScreen.LastFollowedAgent = null;
				missionScreen.CombatCamera.Position = new Vec3(list[0], list[1], list[2], -1f);
				return string.Concat(new string[]
				{
					"Camera position has been set to: ",
					strings[0],
					", ",
					strings[1],
					", ",
					strings[2]
				});
			}
			return "Mission screen not found.";
		}

		// Token: 0x06000220 RID: 544 RVA: 0x0001057C File Offset: 0x0000E77C
		private void CheckForUpdateCamera(float dt)
		{
			if (this._fixCamera && !this.IsPhotoModeEnabled)
			{
				return;
			}
			if (this.CustomCamera != null)
			{
				if (this._zoomAmount > 0f)
				{
					this._zoomAmount = MBMath.ClampFloat(this._zoomAmount, 0f, 1f);
					float num = 37f / this.MaxCameraZoom;
					this.CameraViewAngle = MBMath.Lerp(Mission.GetFirstPersonFov(), num, this._zoomAmount, 0.005f);
					this.CustomCamera.SetFovVertical(this._cameraSpecialCurrentFOV * (this.CameraViewAngle / 65f) * 0.017453292f, Screen.AspectRatio, 0.065f, 12500f);
				}
				this.CombatCamera.FillParametersFrom(this.CustomCamera);
				if (this.CustomCamera.Entity != null)
				{
					MatrixFrame globalFrame = this.CustomCamera.Entity.GetGlobalFrame();
					globalFrame.rotation.MakeUnit();
					this.CombatCamera.Frame = globalFrame;
				}
				this.SceneView.SetCamera(this.CombatCamera);
				SoundManager.SetListenerFrame(this.CombatCamera.Frame);
				return;
			}
			bool flag = false;
			using (List<MissionBehavior>.Enumerator enumerator = this.Mission.MissionBehaviors.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					MissionView missionView;
					if ((missionView = enumerator.Current as MissionView) != null)
					{
						flag = flag || missionView.UpdateOverridenCamera(dt);
					}
				}
			}
			if (!flag)
			{
				this.UpdateCamera(dt);
			}
		}

		// Token: 0x06000221 RID: 545 RVA: 0x00010708 File Offset: 0x0000E908
		private void UpdateDragData()
		{
			if (this._resetDraggingMode)
			{
				this._rightButtonDraggingMode = false;
				this._resetDraggingMode = false;
				return;
			}
			if (this.SceneLayer.Input.IsKeyReleased(225))
			{
				this._resetDraggingMode = true;
				return;
			}
			if (this.SceneLayer.Input.IsKeyPressed(225))
			{
				this._clickedPositionPixel = this.SceneLayer.Input.GetMousePositionPixel();
				return;
			}
			if (this.SceneLayer.Input.IsKeyDown(225) && !this.SceneLayer.Input.IsKeyReleased(225) && this.SceneLayer.Input.GetMousePositionPixel().DistanceSquared(this._clickedPositionPixel) > 10f && !this._rightButtonDraggingMode)
			{
				this._rightButtonDraggingMode = true;
			}
		}

		// Token: 0x06000222 RID: 546 RVA: 0x000107DC File Offset: 0x0000E9DC
		private void UpdateCamera(float dt)
		{
			Scene scene = this.Mission.Scene;
			bool photoModeOrbit = scene.GetPhotoModeOrbit();
			float num = (this.IsPhotoModeEnabled ? scene.GetPhotoModeFov() : 0f);
			bool flag = this._isGamepadActive && this.PhotoModeRequiresMouse;
			this.UpdateDragData();
			MatrixFrame matrixFrame = MatrixFrame.Identity;
			MissionPeer missionPeer = ((GameNetwork.MyPeer != null) ? PeerExtensions.GetComponent<MissionPeer>(GameNetwork.MyPeer) : null);
			Mission.SpectatorData spectatingData = this.GetSpectatingData(this.CombatCamera.Frame.origin);
			Agent agentToFollow = spectatingData.AgentToFollow;
			IAgentVisual agentVisualToFollow = spectatingData.AgentVisualToFollow;
			SpectatorCameraTypes cameraType = spectatingData.CameraType;
			bool flag2 = this.Mission.CameraIsFirstPerson && agentToFollow != null && agentToFollow == this.Mission.MainAgent;
			float num2 = (flag2 ? Mission.GetFirstPersonFov() : 65f);
			if (this.IsPhotoModeEnabled)
			{
				this.CameraViewAngle = num2;
			}
			else
			{
				this._zoomAmount = MBMath.ClampFloat(this._zoomAmount, 0f, 1f);
				float num3 = 37f / this.MaxCameraZoom;
				this.CameraViewAngle = MBMath.Lerp(num2, num3, this._zoomAmount, 0.005f);
			}
			if (this._missionMainAgentController == null)
			{
				this._missionMainAgentController = this.Mission.GetMissionBehavior<MissionMainAgentController>();
			}
			else
			{
				this._missionMainAgentController.IsDisabled = true;
			}
			if (this._missionMainAgentController != null && this.Mission.Mode != 6 && this.Mission.MainAgent != null && this.Mission.MainAgent.IsCameraAttachable())
			{
				this._missionMainAgentController.IsDisabled = false;
			}
			bool flag3 = this._cameraApplySpecialMovementsInstantly;
			if ((this.IsPhotoModeEnabled && !photoModeOrbit) || (agentToFollow == null && agentVisualToFollow == null))
			{
				float num4 = -scene.GetPhotoModeRoll();
				matrixFrame.rotation.RotateAboutSide(1.5707964f);
				matrixFrame.rotation.RotateAboutForward(this.CameraBearing);
				matrixFrame.rotation.RotateAboutSide(this.CameraElevation);
				matrixFrame.rotation.RotateAboutUp(num4);
				matrixFrame.origin = this.CombatCamera.Frame.origin;
				this._cameraSpeed *= 1f - 5f * dt;
				this._cameraSpeed.x = MBMath.ClampFloat(this._cameraSpeed.x, -20f, 20f);
				this._cameraSpeed.y = MBMath.ClampFloat(this._cameraSpeed.y, -20f, 20f);
				this._cameraSpeed.z = MBMath.ClampFloat(this._cameraSpeed.z, -20f, 20f);
				if (Game.Current.CheatMode)
				{
					if (this.InputManager.IsHotKeyPressed("MissionScreenHotkeyIncreaseCameraSpeed"))
					{
						this._cameraSpeedMultiplier *= 1.5f;
					}
					if (this.InputManager.IsHotKeyPressed("MissionScreenHotkeyDecreaseCameraSpeed"))
					{
						this._cameraSpeedMultiplier *= 0.6666667f;
					}
					if (this.InputManager.IsHotKeyPressed("ResetCameraSpeed"))
					{
						this._cameraSpeedMultiplier = 1f;
					}
					if (this.InputManager.IsControlDown())
					{
						float num5 = this.SceneLayer.Input.GetDeltaMouseScroll() * 0.008333334f;
						if (num5 > 0.01f)
						{
							this._cameraSpeedMultiplier *= 1.25f;
						}
						else if (num5 < -0.01f)
						{
							this._cameraSpeedMultiplier *= 0.8f;
						}
					}
				}
				float num6 = 10f * this._cameraSpeedMultiplier * (this.IsPhotoModeEnabled ? (flag ? 0f : 0.3f) : 1f);
				if (this.Mission.Mode == 6)
				{
					float groundHeightAtPosition = scene.GetGroundHeightAtPosition(matrixFrame.origin, 6402441);
					num6 *= MathF.Max(1f, 1f + (matrixFrame.origin.z - groundHeightAtPosition - 5f) / 10f);
				}
				if ((!this.IsPhotoModeEnabled && this.SceneLayer.Input.IsGameKeyDown(24)) || (this.IsPhotoModeEnabled && !flag && this.SceneLayer.Input.IsHotKeyDown("FasterCamera")))
				{
					num6 *= (float)this._shiftSpeedMultiplier;
				}
				if (!this._cameraSmoothMode)
				{
					this._cameraSpeed.x = 0f;
					this._cameraSpeed.y = 0f;
					this._cameraSpeed.z = 0f;
				}
				if ((!this.InputManager.IsControlDown() || !this.InputManager.IsAltDown()) && !this.LockCameraMovement)
				{
					bool flag4 = !this._isGamepadActive || this.Mission.Mode != 6 || Input.IsKeyDown(254);
					Vec3 vec = Vec3.Zero;
					if (flag4)
					{
						vec.x = this.SceneLayer.Input.GetGameKeyAxis("MovementAxisX");
						vec.y = this.SceneLayer.Input.GetGameKeyAxis("MovementAxisY");
						if (MathF.Abs(vec.x) < 0.2f)
						{
							vec.x = 0f;
						}
						if (MathF.Abs(vec.y) < 0.2f)
						{
							vec.y = 0f;
						}
					}
					if (!this._isGamepadActive || (!this.IsPhotoModeEnabled && this.Mission.Mode != 6 && !this.IsOrderMenuOpen && !this.IsTransferMenuOpen))
					{
						if (this.SceneLayer.Input.IsGameKeyDown(14))
						{
							vec.z += 1f;
						}
						if (this.SceneLayer.Input.IsGameKeyDown(15))
						{
							vec.z -= 1f;
						}
					}
					else if (this.Mission.Mode == 6 && this.SceneLayer.IsHitThisFrame)
					{
						if (this.SceneLayer.Input.IsKeyDown(249))
						{
							vec.z += 1f;
						}
						if (this.SceneLayer.Input.IsKeyDown(248))
						{
							vec.z -= 1f;
						}
					}
					if (vec.IsNonZero)
					{
						float num7 = vec.Normalize();
						vec *= num6 * Math.Min(1f, num7);
						this._cameraSpeed += vec;
					}
				}
				if (this.Mission.Mode == 6 && !this.IsRadialMenuActive)
				{
					matrixFrame.origin += this._cameraSpeed.x * new Vec3(matrixFrame.rotation.s.AsVec2, 0f, -1f).NormalizedCopy() * dt;
					matrixFrame.origin -= this._cameraSpeed.y * new Vec3(matrixFrame.rotation.u.AsVec2, 0f, -1f).NormalizedCopy() * dt;
					matrixFrame.origin.z = matrixFrame.origin.z + this._cameraSpeed.z * dt;
					if (!Game.Current.CheatMode || !this.InputManager.IsControlDown())
					{
						this._cameraDeploymentHeightToAdd += 3f * this.SceneLayer.Input.GetDeltaMouseScroll() / 120f;
						if (this.SceneLayer.Input.IsHotKeyDown("DeploymentCameraIsActive"))
						{
							this._cameraDeploymentHeightToAdd += 0.05f * Input.MouseMoveY;
						}
					}
					if (MathF.Abs(this._cameraDeploymentHeightToAdd) > 0.001f)
					{
						matrixFrame.origin.z = matrixFrame.origin.z + this._cameraDeploymentHeightToAdd * dt * 10f;
						this._cameraDeploymentHeightToAdd *= 1f - dt * 10f;
					}
					else
					{
						matrixFrame.origin.z = matrixFrame.origin.z + this._cameraDeploymentHeightToAdd;
						this._cameraDeploymentHeightToAdd = 0f;
					}
				}
				else
				{
					matrixFrame.origin += this._cameraSpeed.x * matrixFrame.rotation.s * dt;
					matrixFrame.origin -= this._cameraSpeed.y * matrixFrame.rotation.u * dt;
					matrixFrame.origin += this._cameraSpeed.z * matrixFrame.rotation.f * dt;
				}
				if (!MBEditor.IsEditModeOn)
				{
					if (!this.Mission.IsPositionInsideBoundaries(matrixFrame.origin.AsVec2))
					{
						matrixFrame.origin.AsVec2 = this.Mission.GetClosestBoundaryPosition(matrixFrame.origin.AsVec2);
					}
					if (!GameNetwork.IsMultiplayer && this.Mission.Mode == 6)
					{
						BattleSideEnum side = this.Mission.PlayerTeam.Side;
						IMissionDeploymentPlan deploymentPlan = this.Mission.DeploymentPlan;
						if (deploymentPlan.HasDeploymentBoundaries(side, 0))
						{
							IMissionDeploymentPlan missionDeploymentPlan = deploymentPlan;
							BattleSideEnum battleSideEnum = side;
							Vec2 vec2 = matrixFrame.origin.AsVec2;
							if (!missionDeploymentPlan.IsPositionInsideDeploymentBoundaries(battleSideEnum, ref vec2, 0))
							{
								IMissionDeploymentPlan missionDeploymentPlan2 = deploymentPlan;
								BattleSideEnum battleSideEnum2 = side;
								vec2 = matrixFrame.origin.AsVec2;
								matrixFrame.origin.AsVec2 = missionDeploymentPlan2.GetClosestDeploymentBoundaryPosition(battleSideEnum2, ref vec2, 0);
							}
						}
					}
					float groundHeightAtPosition2 = scene.GetGroundHeightAtPosition((this.Mission.Mode == 6) ? (matrixFrame.origin + new Vec3(0f, 0f, 100f, -1f)) : matrixFrame.origin, 6402441);
					if (!this.IsCheatGhostMode && groundHeightAtPosition2 < 9999f)
					{
						matrixFrame.origin.z = MathF.Max(matrixFrame.origin.z, groundHeightAtPosition2 + 0.5f);
					}
					if (matrixFrame.origin.z > groundHeightAtPosition2 + 80f)
					{
						matrixFrame.origin.z = groundHeightAtPosition2 + 80f;
					}
					if (this._cameraHeightLimit > 0f && matrixFrame.origin.z > this._cameraHeightLimit)
					{
						matrixFrame.origin.z = this._cameraHeightLimit;
					}
					if (matrixFrame.origin.z < -100f)
					{
						matrixFrame.origin.z = -100f;
					}
				}
			}
			else if (flag2 && !this.IsPhotoModeEnabled)
			{
				Agent agent = agentToFollow;
				if (agentToFollow.AgentVisuals != null)
				{
					if (this._cameraAddSpecialMovement)
					{
						if (this.Mission.Mode == 1 || this.Mission.Mode == 5)
						{
							MissionMainAgentController missionMainAgentController = this._missionMainAgentController;
							if (((missionMainAgentController != null) ? missionMainAgentController.InteractionComponent.CurrentFocusedObject : null) != null && this._missionMainAgentController.InteractionComponent.CurrentFocusedObject.FocusableObjectType == 3)
							{
								Vec3 vec3 = (this._missionMainAgentController.InteractionComponent.CurrentFocusedObject as Agent).Position - agentToFollow.Position;
								float num9;
								if (65f / this.CameraViewAngle * MathF.Abs(vec3.z) >= 2f)
								{
									float num8 = 160f;
									Vec2 vec2 = vec3.AsVec2;
									num9 = num8 / vec2.Length;
								}
								else
								{
									float num10 = ((this.Mission.Mode == 5) ? 48.75f : 32.5f);
									float num11 = ((this.Mission.Mode == 5) ? 75f : 50f);
									Vec2 vec2 = vec3.AsVec2;
									num9 = MathF.Min(num10, num11 / vec2.Length);
								}
								this._cameraSpecialTargetFOV = num9;
								goto IL_BE2;
							}
						}
						this._cameraSpecialTargetFOV = 65f;
						IL_BE2:
						if (flag3)
						{
							this._cameraSpecialCurrentFOV = this._cameraSpecialTargetFOV;
						}
					}
					MatrixFrame boneEntitialFrame = agentToFollow.AgentVisuals.GetBoneEntitialFrame(agentToFollow.Monster.ThoraxLookDirectionBoneIndex, true);
					MatrixFrame boneEntitialFrame2 = agentToFollow.AgentVisuals.GetBoneEntitialFrame(agentToFollow.Monster.HeadLookDirectionBoneIndex, true);
					boneEntitialFrame2.origin = boneEntitialFrame2.TransformToParent(agent.Monster.FirstPersonCameraOffsetWrtHead);
					MatrixFrame frame = agentToFollow.AgentVisuals.GetFrame();
					bool flag5;
					if (this.Mission.Mode == 1 || this.Mission.Mode == 5)
					{
						MissionMainAgentController missionMainAgentController2 = this._missionMainAgentController;
						if (((missionMainAgentController2 != null) ? missionMainAgentController2.InteractionComponent.CurrentFocusedObject : null) != null)
						{
							flag5 = this._missionMainAgentController.InteractionComponent.CurrentFocusedObject.FocusableObjectType == 3;
							goto IL_CA0;
						}
					}
					flag5 = false;
					IL_CA0:
					bool flag6 = flag5;
					if ((agent.GetCurrentAnimationFlag(0) & 8388608L) != null || (agent.GetCurrentAnimationFlag(1) & 8388608L) != null)
					{
						MatrixFrame matrixFrame2 = frame.TransformToParent(boneEntitialFrame2);
						matrixFrame2.rotation.MakeUnit();
						this.CameraBearing = matrixFrame2.rotation.f.RotationZ;
						this.CameraElevation = matrixFrame2.rotation.f.RotationX;
					}
					else if (flag6 || (agentToFollow.IsMainAgent && this._missionMainAgentController != null && this._missionMainAgentController.CustomLookDir.IsNonZero))
					{
						Vec3 vec4;
						if (flag6)
						{
							Agent agent2 = this._missionMainAgentController.InteractionComponent.CurrentFocusedObject as Agent;
							vec4 = (new Vec3(agent2.Position.AsVec2, agent2.AgentVisuals.GetGlobalStableEyePoint(agent2.IsHuman).z, -1f) - frame.TransformToParent(boneEntitialFrame2.origin)).NormalizedCopy();
							Vec3 vec5 = new Vec3(vec4.y, -vec4.x, 0f, -1f).NormalizedCopy();
							vec4 = vec4.RotateAboutAnArbitraryVector(vec5, ((this.Mission.Mode == 1) ? (-0.003f) : (-0.0045f)) * this._cameraSpecialCurrentFOV);
						}
						else
						{
							vec4 = this._missionMainAgentController.CustomLookDir;
						}
						if (flag3)
						{
							this.CameraBearing = vec4.RotationZ;
							this.CameraElevation = vec4.RotationX;
						}
						else
						{
							Mat3 identity = Mat3.Identity;
							identity.RotateAboutUp(this.CameraBearing);
							identity.RotateAboutSide(this.CameraElevation);
							Vec3 f = identity.f;
							Vec3 vec6 = Vec3.CrossProduct(f, vec4);
							float num12 = vec6.Normalize();
							Vec3 vec7;
							if (num12 < 0.0001f)
							{
								vec7 = vec4;
							}
							else
							{
								vec7 = f;
								vec7 = vec7.RotateAboutAnArbitraryVector(vec6, num12 * dt * 5f);
							}
							this.CameraBearing = vec7.RotationZ;
							this.CameraElevation = vec7.RotationX;
						}
					}
					else
					{
						float num13 = MBMath.WrapAngle(this.CameraBearing);
						float num14 = MBMath.WrapAngle(this.CameraElevation);
						float num15;
						float num16;
						MissionScreen.CalculateNewBearingAndElevation(agentToFollow, num13, num14, out num15, out num16);
						this.CameraBearing = MBMath.LerpRadians(num13, num15, Math.Min(dt * 12f, 1f), 1E-05f, 0.5f);
						this.CameraElevation = MBMath.LerpRadians(num14, num16, Math.Min(dt * 12f, 1f), 1E-05f, 0.5f);
					}
					MatrixFrame matrixFrame3 = matrixFrame;
					matrixFrame3.origin = frame.TransformToParent(boneEntitialFrame.origin);
					matrixFrame3.rotation.RotateAboutSide(1.5707964f);
					matrixFrame3.rotation.RotateAboutForward(this.CameraBearing);
					matrixFrame3.rotation.RotateAboutSide(this.CameraElevation);
					Vec3 vec8 = agentToFollow.AgentVisuals.GetFrame().rotation.u * 0.25f;
					Vec3 vec9 = agentToFollow.AgentVisuals.GetFrame().rotation.u * 0.15f + Vec3.Forward * 0.15f;
					vec9.RotateAboutX(MBMath.ClampFloat(this.CameraElevation, -0.35f, 0.35f));
					vec9.RotateAboutZ(this.CameraBearing);
					matrixFrame3.origin += vec8;
					matrixFrame3.origin += vec9;
					MatrixFrame matrixFrame4 = matrixFrame;
					matrixFrame4.origin = frame.TransformToParent(boneEntitialFrame2.origin);
					matrixFrame4.rotation.RotateAboutSide(1.5707964f);
					matrixFrame4.rotation.RotateAboutForward(this.CameraBearing);
					matrixFrame4.rotation.RotateAboutSide(this.CameraElevation);
					float actionChannelWeight = agentToFollow.GetActionChannelWeight(1);
					matrixFrame.origin.z = MBMath.Lerp(matrixFrame3.origin.z, matrixFrame4.origin.z, actionChannelWeight, 1E-05f);
					float num17 = MBMath.WrapAngle(this.CameraBearing - agentToFollow.MovementDirectionAsAngle);
					float num18 = 1f - (1f - actionChannelWeight) * MBMath.ClampFloat((MathF.Abs(num17) - 1f) * 0.66f, 0f, 1f);
					matrixFrame.origin.x = MBMath.Lerp(matrixFrame3.origin.x, matrixFrame4.origin.x, num18, 1E-05f);
					matrixFrame.origin.y = MBMath.Lerp(matrixFrame3.origin.y, matrixFrame4.origin.y, num18, 1E-05f);
					matrixFrame.rotation = matrixFrame4.rotation;
				}
				else
				{
					matrixFrame = this.CombatCamera.Frame;
				}
			}
			else
			{
				float num19 = 0.6f;
				float num20 = 0f;
				bool flag7 = agentVisualToFollow != null;
				float num21 = 1f;
				bool flag8 = false;
				float num23;
				if (flag7)
				{
					this._cameraSpecialTargetAddedBearing = 0f;
					this._cameraSpecialTargetAddedElevation = 0f;
					this._cameraSpecialTargetPositionToAdd = Vec3.Zero;
					this._cameraSpecialTargetDistanceToAdd = 0f;
					num19 = 1.25f;
					flag3 = flag3 || agentVisualToFollow != this.LastFollowedAgentVisuals;
					if (agentVisualToFollow.GetEquipment().Horse.Item != null)
					{
						float num22 = (float)agentVisualToFollow.GetEquipment().Horse.Item.HorseComponent.BodyLength * 0.01f;
						num19 += 2f;
						num23 = 1f * num22 + 0.9f * num21 - 0.2f;
					}
					else
					{
						num23 = 1f * num21;
					}
					this.CameraBearing = MBMath.WrapAngle(agentVisualToFollow.GetFrame().rotation.f.RotationZ + 3.1415927f);
					this.CameraElevation = 0.15f;
				}
				else
				{
					flag8 = agentToFollow.HasMount;
					num21 = agentToFollow.AgentScale;
					flag3 = flag3 || agentToFollow != this.LastFollowedAgent;
					if (this.Mission.Mode == 1 || this.Mission.Mode == 5)
					{
						MissionMainAgentController missionMainAgentController3 = this._missionMainAgentController;
						if (((missionMainAgentController3 != null) ? missionMainAgentController3.InteractionComponent.CurrentFocusedObject : null) != null && this._missionMainAgentController.InteractionComponent.CurrentFocusedObject.FocusableObjectType == 3)
						{
							MissionMainAgentController missionMainAgentController4 = this._missionMainAgentController;
							Agent agent3 = ((missionMainAgentController4 != null) ? missionMainAgentController4.InteractionComponent.CurrentFocusedObject : null) as Agent;
							num23 = (agent3.AgentVisuals.GetGlobalStableEyePoint(true).z + agentToFollow.AgentVisuals.GetGlobalStableEyePoint(true).z) * 0.5f - agentToFollow.Position.z;
							if (agent3.HasMount)
							{
								num19 += 0.1f;
							}
							if (this.Mission.Mode == 5)
							{
								Vec2 vec10 = agent3.Position.AsVec2 - agentToFollow.Position.AsVec2;
								float length = vec10.Length;
								float num24 = MathF.Max(num19 + Mission.CameraAddedDistance, 0.48f) * num21 + length * 0.5f;
								num23 += -0.004f * num24 * this._cameraSpecialCurrentFOV;
								Vec3 globalStableEyePoint = agent3.AgentVisuals.GetGlobalStableEyePoint(agent3.IsHuman);
								Vec3 globalStableEyePoint2 = agentToFollow.AgentVisuals.GetGlobalStableEyePoint(agentToFollow.IsHuman);
								float num25 = vec10.RotationInRadians - MathF.Min(0.47123894f, 0.4f / length);
								this._cameraSpecialTargetAddedBearing = MBMath.WrapAngle(num25 - this.CameraBearing);
								Vec2 vec11;
								vec11..ctor(globalStableEyePoint.z - globalStableEyePoint2.z, MathF.Max(length, 1f));
								float num26 = (flag8 ? (-0.03f) : 0f) - vec11.RotationInRadians;
								this._cameraSpecialTargetAddedElevation = num26 - this.CameraElevation + MathF.Asin(-0.2f * (num24 - length * 0.5f) / num24);
								goto IL_1590;
							}
							goto IL_1590;
						}
					}
					if (flag8)
					{
						num19 += 0.1f;
						Agent mountAgent = agentToFollow.MountAgent;
						Monster monster = mountAgent.Monster;
						num23 = (monster.RiderCameraHeightAdder + monster.BodyCapsulePoint1.z + monster.BodyCapsuleRadius) * mountAgent.AgentScale + agentToFollow.Monster.CrouchEyeHeight * num21;
					}
					else if (agentToFollow.AgentVisuals.GetCurrentRagdollState() == 3)
					{
						num23 = 0.5f;
					}
					else if ((agentToFollow.GetCurrentAnimationFlag(0) & 1073741824L) != null)
					{
						num23 = 0.5f;
					}
					else if (agentToFollow.CrouchMode || agentToFollow.IsSitting())
					{
						num23 = (agentToFollow.Monster.CrouchEyeHeight + 0.2f) * num21;
					}
					else
					{
						num23 = (agentToFollow.Monster.StandingEyeHeight + 0.2f) * num21;
					}
					IL_1590:
					if ((this.IsViewingCharacter() && (cameraType != 6 || agentToFollow == this.Mission.MainAgent)) || this.IsPhotoModeEnabled)
					{
						num23 *= 0.5f;
						num19 += 0.5f;
					}
					else if (agentToFollow.HasMount && agentToFollow.IsDoingPassiveAttack && (cameraType != 6 || agentToFollow == this.Mission.MainAgent))
					{
						num23 *= 1.1f;
					}
					if (this._cameraAddSpecialMovement)
					{
						if (this.Mission.Mode == 1 || this.Mission.Mode == 5)
						{
							MissionMainAgentController missionMainAgentController5 = this._missionMainAgentController;
							if (((missionMainAgentController5 != null) ? missionMainAgentController5.InteractionComponent.CurrentFocusedObject : null) != null && this._missionMainAgentController.InteractionComponent.CurrentFocusedObject.FocusableObjectType == 3)
							{
								Agent agent4 = this._missionMainAgentController.InteractionComponent.CurrentFocusedObject as Agent;
								Vec3 globalStableEyePoint3 = agent4.AgentVisuals.GetGlobalStableEyePoint(true);
								Vec3 globalStableEyePoint4 = agentToFollow.AgentVisuals.GetGlobalStableEyePoint(true);
								Vec2 vec12 = agent4.Position.AsVec2 - agentToFollow.Position.AsVec2;
								float length2 = vec12.Length;
								this._cameraSpecialTargetPositionToAdd = new Vec3(vec12 * 0.5f, 0f, -1f);
								this._cameraSpecialTargetDistanceToAdd = length2 * (flag8 ? 1.3f : 0.8f) - num19;
								float num27 = vec12.RotationInRadians - MathF.Min(0.47123894f, 0.48f / length2);
								this._cameraSpecialTargetAddedBearing = MBMath.WrapAngle(num27 - this.CameraBearing);
								Vec2 vec13;
								vec13..ctor(globalStableEyePoint3.z - globalStableEyePoint4.z, MathF.Max(length2, 1f));
								float num28 = (flag8 ? (-0.03f) : 0f) - vec13.RotationInRadians;
								this._cameraSpecialTargetAddedElevation = num28 - this.CameraElevation;
								this._cameraSpecialTargetFOV = MathF.Min(32.5f, 50f / length2);
								goto IL_17CC;
							}
						}
						this._cameraSpecialTargetPositionToAdd = Vec3.Zero;
						this._cameraSpecialTargetDistanceToAdd = 0f;
						this._cameraSpecialTargetAddedBearing = 0f;
						this._cameraSpecialTargetAddedElevation = 0f;
						this._cameraSpecialTargetFOV = 65f;
						IL_17CC:
						if (flag3)
						{
							this._cameraSpecialCurrentPositionToAdd = this._cameraSpecialTargetPositionToAdd;
							this._cameraSpecialCurrentDistanceToAdd = this._cameraSpecialTargetDistanceToAdd;
							this._cameraSpecialCurrentAddedBearing = this._cameraSpecialTargetAddedBearing;
							this._cameraSpecialCurrentAddedElevation = this._cameraSpecialTargetAddedElevation;
							this._cameraSpecialCurrentFOV = this._cameraSpecialTargetFOV;
						}
					}
					if (this._cameraSpecialCurrentDistanceToAdd != this._cameraSpecialTargetDistanceToAdd)
					{
						float num29 = this._cameraSpecialTargetDistanceToAdd - this._cameraSpecialCurrentDistanceToAdd;
						if (flag3 || MathF.Abs(num29) < 0.0001f)
						{
							this._cameraSpecialCurrentDistanceToAdd = this._cameraSpecialTargetDistanceToAdd;
						}
						else
						{
							float num30 = num29 * 4f * dt;
							this._cameraSpecialCurrentDistanceToAdd += num30;
						}
					}
					num19 += this._cameraSpecialCurrentDistanceToAdd;
				}
				if (flag3)
				{
					this._cameraTargetAddedHeight = num23;
				}
				else
				{
					this._cameraTargetAddedHeight += (num23 - this._cameraTargetAddedHeight) * dt * 6f * num21;
				}
				if (this._cameraSpecialTargetAddedBearing != this._cameraSpecialCurrentAddedBearing)
				{
					float num31 = this._cameraSpecialTargetAddedBearing - this._cameraSpecialCurrentAddedBearing;
					if (flag3 || MathF.Abs(num31) < 0.0001f)
					{
						this._cameraSpecialCurrentAddedBearing = this._cameraSpecialTargetAddedBearing;
					}
					else
					{
						float num32 = num31 * 10f * dt;
						this._cameraSpecialCurrentAddedBearing += num32;
					}
				}
				if (this._cameraSpecialTargetAddedElevation != this._cameraSpecialCurrentAddedElevation)
				{
					float num33 = this._cameraSpecialTargetAddedElevation - this._cameraSpecialCurrentAddedElevation;
					if (flag3 || MathF.Abs(num33) < 0.0001f)
					{
						this._cameraSpecialCurrentAddedElevation = this._cameraSpecialTargetAddedElevation;
					}
					else
					{
						float num34 = num33 * 8f * dt;
						this._cameraSpecialCurrentAddedElevation += num34;
					}
				}
				matrixFrame.rotation.RotateAboutSide(1.5707964f);
				if (agentToFollow != null && !agentToFollow.IsMine && cameraType == 6)
				{
					Vec3 lookDirection = agentToFollow.LookDirection;
					Vec2 vec2 = lookDirection.AsVec2;
					matrixFrame.rotation.RotateAboutForward(vec2.RotationInRadians);
					matrixFrame.rotation.RotateAboutSide(MathF.Asin(lookDirection.z));
				}
				else
				{
					matrixFrame.rotation.RotateAboutForward(this.CameraBearing + this._cameraSpecialCurrentAddedBearing);
					matrixFrame.rotation.RotateAboutSide(this.CameraElevation + this._cameraSpecialCurrentAddedElevation);
					if (this.IsPhotoModeEnabled)
					{
						float num35 = -scene.GetPhotoModeRoll();
						matrixFrame.rotation.RotateAboutUp(num35);
					}
				}
				MatrixFrame matrixFrame5 = matrixFrame;
				float num36 = MathF.Max(num19 + Mission.CameraAddedDistance, 0.48f) * num21;
				if (this.Mission.Mode != 1 && this.Mission.Mode != 5 && agentToFollow != null && agentToFollow.IsActive() && BannerlordConfig.EnableVerticalAimCorrection)
				{
					WeaponComponentData currentUsageItem = agentToFollow.WieldedWeapon.CurrentUsageItem;
					if (currentUsageItem != null && currentUsageItem.IsRangedWeapon)
					{
						MatrixFrame frame2 = this.CombatCamera.Frame;
						frame2.rotation.RotateAboutSide(-this._cameraAddedElevation);
						float num37;
						if (flag8)
						{
							Agent mountAgent2 = agentToFollow.MountAgent;
							Monster monster2 = mountAgent2.Monster;
							num37 = (monster2.RiderCameraHeightAdder + monster2.BodyCapsulePoint1.z + monster2.BodyCapsuleRadius) * mountAgent2.AgentScale + agentToFollow.Monster.CrouchEyeHeight * num21;
						}
						else
						{
							num37 = agentToFollow.Monster.StandingEyeHeight * num21;
						}
						if (Extensions.HasAnyFlag<WeaponFlags>(currentUsageItem.WeaponFlags, 4294967296L))
						{
							num37 *= 1.25f;
						}
						float num39;
						if (flag3)
						{
							Vec3 vec14 = agentToFollow.Position + matrixFrame.rotation.f * num21 * (0.7f * MathF.Pow(MathF.Cos(1f / ((num36 / num21 - 0.2f) * 30f + 20f)), 3500f));
							vec14.z += this._cameraTargetAddedHeight;
							Vec3 vec15 = vec14 + matrixFrame.rotation.u * num36;
							float z = vec15.z;
							float num38 = -matrixFrame5.rotation.u.z;
							Vec2 vec2 = vec15.AsVec2 - agentToFollow.Position.AsVec2;
							num39 = z + num38 * vec2.Length - (agentToFollow.Position.z + num37);
						}
						else
						{
							float z2 = frame2.origin.z;
							float num40 = -frame2.rotation.u.z;
							Vec2 vec2 = frame2.origin.AsVec2 - agentToFollow.Position.AsVec2;
							num39 = z2 + num40 * vec2.Length - (agentToFollow.Position.z + num37);
						}
						if (num39 > 0f)
						{
							num20 = MathF.Max(-0.15f, -MathF.Asin(MathF.Min(1f, MathF.Sqrt(19.6f * num39) / (float)agentToFollow.WieldedWeapon.GetModifiedMissileSpeedForCurrentUsage())));
						}
						else
						{
							num20 = 0f;
						}
					}
					else
					{
						num20 = ManagedParameters.Instance.GetManagedParameter(2);
					}
				}
				if (flag3 || this.IsPhotoModeEnabled)
				{
					this._cameraAddedElevation = num20;
				}
				else
				{
					this._cameraAddedElevation += (num20 - this._cameraAddedElevation) * dt * 3f;
				}
				if (!this.IsPhotoModeEnabled)
				{
					matrixFrame.rotation.RotateAboutSide(this._cameraAddedElevation);
				}
				bool flag9 = this.IsViewingCharacter() && !GameNetwork.IsSessionActive;
				bool flag10 = agentToFollow != null && agentToFollow.AgentVisuals != null && agentToFollow.AgentVisuals.GetCurrentRagdollState() > 0;
				bool flag11 = agentToFollow != null && agentToFollow.IsActive() && agentToFollow.GetCurrentActionType(0) == 36;
				Vec2 vec16 = Vec2.Zero;
				Vec3 vec17;
				Vec3 vec18;
				if (flag7)
				{
					vec17 = this.GetPlayerAgentVisuals(missionPeer).GetVisuals().GetGlobalFrame()
						.origin;
					vec18 = vec17;
				}
				else
				{
					vec17 = agentToFollow.VisualPosition;
					vec18 = (flag10 ? agentToFollow.AgentVisuals.GetFrame().origin : vec17);
					if (flag8)
					{
						vec16 = agentToFollow.MountAgent.GetMovementDirection() * agentToFollow.MountAgent.Monster.RiderBodyCapsuleForwardAdder;
						vec18 += vec16.ToVec3(0f);
					}
				}
				if (this._cameraAddSpecialPositionalMovement)
				{
					Vec3 vec19 = matrixFrame5.rotation.f * num21 * (0.7f * MathF.Pow(MathF.Cos(1f / ((num36 / num21 - 0.2f) * 30f + 20f)), 3500f));
					if (this.Mission.Mode == 1 || this.Mission.Mode == 5)
					{
						this._cameraSpecialCurrentPositionToAdd += vec19;
					}
					else
					{
						this._cameraSpecialCurrentPositionToAdd -= vec19;
					}
				}
				if (this._cameraSpecialCurrentPositionToAdd != this._cameraSpecialTargetPositionToAdd)
				{
					Vec3 vec20 = this._cameraSpecialTargetPositionToAdd - this._cameraSpecialCurrentPositionToAdd;
					if (flag3 || vec20.LengthSquared < 1.0000001E-06f)
					{
						this._cameraSpecialCurrentPositionToAdd = this._cameraSpecialTargetPositionToAdd;
					}
					else
					{
						this._cameraSpecialCurrentPositionToAdd += vec20 * 4f * dt;
					}
				}
				vec17 += this._cameraSpecialCurrentPositionToAdd;
				vec18 += this._cameraSpecialCurrentPositionToAdd;
				vec18.z += this._cameraTargetAddedHeight;
				int num41 = 0;
				bool flag12 = agentToFollow != null;
				Vec3 vec21 = (flag12 ? (flag8 ? agentToFollow.MountAgent.GetChestGlobalPosition() : agentToFollow.GetChestGlobalPosition()) : Vec3.Invalid);
				bool flag13;
				do
				{
					Vec3 vec22 = vec18;
					if (this.Mission.Mode != 1 && this.Mission.Mode != 5)
					{
						vec22 += matrixFrame5.rotation.f * num21 * (0.7f * MathF.Pow(MathF.Cos(1f / ((num36 / num21 - 0.2f) * 30f + 20f)), 3500f));
					}
					Vec3 vec23 = vec22 + matrixFrame5.rotation.u * num36;
					if (flag10 || flag11)
					{
						float num42 = 0f;
						if (flag11)
						{
							float currentActionProgress = agentToFollow.GetCurrentActionProgress(0);
							num42 = currentActionProgress * currentActionProgress * 20f;
						}
						vec22 = this._cameraTarget + (vec22 - this._cameraTarget) * (5f + num42) * dt;
					}
					flag13 = false;
					MatrixFrame matrixFrame6;
					matrixFrame6..ctor(matrixFrame.rotation, vec23);
					Camera.GetNearPlanePointsStatic(ref matrixFrame6, this.IsPhotoModeEnabled ? (num * 0.017453292f) : (this.CameraViewAngle * 0.017453292f), Screen.AspectRatio, 0.2f, 1f, this._cameraNearPlanePoints);
					Vec3 vec24 = Vec3.Zero;
					for (int i = 0; i < 4; i++)
					{
						vec24 += this._cameraNearPlanePoints[i];
					}
					vec24 *= 0.25f;
					Vec3 vec25;
					vec25..ctor(vec17.AsVec2 + vec16, vec22.z, -1f);
					Vec3 vec26 = vec25 - vec24;
					for (int j = 0; j < 4; j++)
					{
						this._cameraNearPlanePoints[j] += vec26;
					}
					this._cameraBoxPoints[0] = this._cameraNearPlanePoints[3] + matrixFrame6.rotation.u * 0.01f;
					this._cameraBoxPoints[1] = this._cameraNearPlanePoints[0];
					this._cameraBoxPoints[2] = this._cameraNearPlanePoints[3];
					this._cameraBoxPoints[3] = this._cameraNearPlanePoints[2];
					this._cameraBoxPoints[4] = this._cameraNearPlanePoints[1] + matrixFrame6.rotation.u * 0.01f;
					this._cameraBoxPoints[5] = this._cameraNearPlanePoints[0] + matrixFrame6.rotation.u * 0.01f;
					this._cameraBoxPoints[6] = this._cameraNearPlanePoints[1];
					this._cameraBoxPoints[7] = this._cameraNearPlanePoints[2] + matrixFrame6.rotation.u * 0.01f;
					float num43 = ((this.IsPhotoModeEnabled && !flag && photoModeOrbit) ? this._zoomAmount : 0f);
					num36 += num43;
					float num44;
					Vec3 vec27;
					GameEntity gameEntity;
					if (scene.BoxCastOnlyForCamera(this._cameraBoxPoints, vec25, flag12, vec21, matrixFrame6.rotation.u, num36 + 0.5f, ref num44, ref vec27, ref gameEntity, true, true, 6666185))
					{
						num44 = MathF.Max(Vec3.DotProduct(matrixFrame6.rotation.u, vec27 - vec22), 0.48f * num21);
						if (num44 < num36)
						{
							flag13 = true;
							num36 = num44;
						}
					}
					num41++;
				}
				while (!flag9 && num41 < 5 && flag13);
				num19 = num36 - Mission.CameraAddedDistance;
				if (flag3 || (this.CameraResultDistanceToTarget > num36 && num41 > 1))
				{
					this.CameraResultDistanceToTarget = num36;
				}
				else
				{
					float num45 = MathF.Max(MathF.Abs(Mission.CameraAddedDistance - this._lastCameraAddedDistance) * num21, MathF.Abs((num19 - (this.CameraResultDistanceToTarget - this._lastCameraAddedDistance)) * dt * 3f * num21));
					this.CameraResultDistanceToTarget += MBMath.ClampFloat(num36 - this.CameraResultDistanceToTarget, -num45, num45);
				}
				this._lastCameraAddedDistance = Mission.CameraAddedDistance;
				this._cameraTarget = vec18;
				if (this.Mission.Mode != 1 && this.Mission.Mode != 5)
				{
					this._cameraTarget += matrixFrame5.rotation.f * num21 * (0.7f * MathF.Pow(MathF.Cos(1f / ((num36 / num21 - 0.2f) * 30f + 20f)), 3500f));
				}
				matrixFrame.origin = this._cameraTarget + matrixFrame5.rotation.u * this.CameraResultDistanceToTarget;
			}
			if (this._cameraSpecialCurrentFOV != this._cameraSpecialTargetFOV)
			{
				float num46 = this._cameraSpecialTargetFOV - this._cameraSpecialCurrentFOV;
				if (flag3 || MathF.Abs(num46) < 0.001f)
				{
					this._cameraSpecialCurrentFOV = this._cameraSpecialTargetFOV;
				}
				else
				{
					this._cameraSpecialCurrentFOV += num46 * 3f * dt;
				}
			}
			float num47 = (this.Mission.CameraIsFirstPerson ? 0.065f : 0.1f);
			this.CombatCamera.Frame = matrixFrame;
			if (this.IsPhotoModeEnabled)
			{
				float num48 = 0f;
				float num49 = 0f;
				float num50 = 0f;
				float num51 = 0f;
				bool flag14 = false;
				scene.GetPhotoModeFocus(ref num49, ref num50, ref num48, ref num51, ref flag14);
				scene.SetDepthOfFieldFocus(num48);
				scene.SetDepthOfFieldParameters(num49, num50, flag14);
			}
			else
			{
				if (this.Mission.Mode == 1 || this.Mission.Mode == 5)
				{
					MissionMainAgentController missionMainAgentController6 = this._missionMainAgentController;
					if (((missionMainAgentController6 != null) ? missionMainAgentController6.InteractionComponent.CurrentFocusedObject : null) != null && this._missionMainAgentController.InteractionComponent.CurrentFocusedObject.FocusableObjectType == 3)
					{
						MissionMainAgentController missionMainAgentController7 = this._missionMainAgentController;
						Agent agent5 = ((missionMainAgentController7 != null) ? missionMainAgentController7.InteractionComponent.CurrentFocusedObject : null) as Agent;
						scene.SetDepthOfFieldParameters(5f, 5f, false);
						scene.SetDepthOfFieldFocus((matrixFrame.origin - agent5.AgentVisuals.GetGlobalStableEyePoint(true)).Length);
						goto IL_25C5;
					}
				}
				if (!MBMath.ApproximatelyEqualsTo(this._zoomAmount, 1f, 1E-05f))
				{
					scene.SetDepthOfFieldParameters(0f, 0f, false);
					scene.SetDepthOfFieldFocus(0f);
				}
			}
			IL_25C5:
			this.CombatCamera.SetFovVertical(this.IsPhotoModeEnabled ? (num * 0.017453292f) : (this._cameraSpecialCurrentFOV * (this.CameraViewAngle / 65f) * 0.017453292f), Screen.AspectRatio, num47, 12500f);
			this.SceneView.SetCamera(this.CombatCamera);
			Vec3 vec28 = ((agentToFollow != null) ? agentToFollow.GetEyeGlobalPosition() : matrixFrame.origin);
			this.Mission.SetCameraFrame(ref matrixFrame, 65f / this.CameraViewAngle, ref vec28);
			if (this.LastFollowedAgent != null && this.LastFollowedAgent != this.Mission.MainAgent && (agentToFollow == this.Mission.MainAgent || agentToFollow == null))
			{
				MissionScreen.OnSpectateAgentDelegate onSpectateAgentFocusOut = this.OnSpectateAgentFocusOut;
				if (onSpectateAgentFocusOut != null)
				{
					onSpectateAgentFocusOut(this.LastFollowedAgent);
				}
			}
			this.LastFollowedAgent = agentToFollow;
			this.LastFollowedAgentVisuals = agentVisualToFollow;
			this._cameraApplySpecialMovementsInstantly = false;
			this._cameraAddSpecialMovement = false;
			this._cameraAddSpecialPositionalMovement = false;
		}

		// Token: 0x06000223 RID: 547 RVA: 0x00012E9B File Offset: 0x0001109B
		public bool IsViewingCharacter()
		{
			return !this.Mission.CameraIsFirstPerson && !this.IsOrderMenuOpen && this.SceneLayer.Input.IsGameKeyDown(25);
		}

		// Token: 0x06000224 RID: 548 RVA: 0x00012EC8 File Offset: 0x000110C8
		private void SetCameraFrameToMapView()
		{
			MatrixFrame matrixFrame = MatrixFrame.Identity;
			bool flag = false;
			if (GameNetwork.IsMultiplayer)
			{
				GameEntity gameEntity = this.Mission.Scene.FindEntityWithTag("mp_camera_start_pos");
				if (gameEntity != null)
				{
					matrixFrame = gameEntity.GetGlobalFrame();
					matrixFrame.rotation.Orthonormalize();
					this.CameraBearing = matrixFrame.rotation.f.RotationZ;
					this.CameraElevation = matrixFrame.rotation.f.RotationX - 1.5707964f;
				}
				else
				{
					Debug.FailedAssert("Multiplayer scene does not contain a camera frame", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.View\\Screens\\MissionScreen.cs", "SetCameraFrameToMapView", 1995);
					flag = true;
				}
			}
			else if (this.Mission.Mode == 6)
			{
				bool flag2 = this.Mission.PlayerTeam.Side == 1;
				GameEntity gameEntity2;
				if (flag2)
				{
					gameEntity2 = this.Mission.Scene.FindEntityWithTag("strategyCameraAttacker") ?? this.Mission.Scene.FindEntityWithTag("strategyCameraDefender");
				}
				else
				{
					gameEntity2 = this.Mission.Scene.FindEntityWithTag("strategyCameraDefender") ?? this.Mission.Scene.FindEntityWithTag("strategyCameraAttacker");
				}
				if (gameEntity2 != null)
				{
					matrixFrame = gameEntity2.GetGlobalFrame();
					this.CameraBearing = matrixFrame.rotation.f.RotationZ;
					this.CameraElevation = matrixFrame.rotation.f.RotationX - 1.5707964f;
				}
				else if (this.Mission.HasSpawnPath)
				{
					float battleSizeOffset = Mission.GetBattleSizeOffset(100, this.Mission.GetInitialSpawnPath());
					matrixFrame = this.Mission.GetBattleSideInitialSpawnPathFrame(flag2 ? 1 : 0, battleSizeOffset).ToGroundMatrixFrame();
					matrixFrame.origin.z = matrixFrame.origin.z + 25f;
					matrixFrame.origin -= 25f * matrixFrame.rotation.f;
					this.CameraBearing = matrixFrame.rotation.f.RotationZ;
					this.CameraElevation = -0.7853982f;
				}
				else
				{
					flag = true;
				}
			}
			else
			{
				flag = true;
			}
			if (flag)
			{
				Vec3 vec;
				vec..ctor(float.MaxValue, float.MaxValue, 0f, -1f);
				Vec3 vec2;
				vec2..ctor(float.MinValue, float.MinValue, 0f, -1f);
				if (this.Mission.Boundaries.ContainsKey("walk_area"))
				{
					using (IEnumerator<Vec2> enumerator = this.Mission.Boundaries["walk_area"].GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							Vec2 vec3 = enumerator.Current;
							vec.x = MathF.Min(vec.x, vec3.x);
							vec.y = MathF.Min(vec.y, vec3.y);
							vec2.x = MathF.Max(vec2.x, vec3.x);
							vec2.y = MathF.Max(vec2.y, vec3.y);
						}
						goto IL_32A;
					}
				}
				this.Mission.Scene.GetBoundingBox(ref vec, ref vec2);
				IL_32A:
				Vec3 vec4 = (vec + vec2) * 0.5f;
				matrixFrame.origin = vec4;
				matrixFrame.origin.z = matrixFrame.origin.z + 10000f;
				matrixFrame.origin.z = this.Mission.Scene.GetGroundHeightAtPosition(vec4, 6402441) + 10f;
			}
			this.CombatCamera.Frame = matrixFrame;
		}

		// Token: 0x06000225 RID: 549 RVA: 0x00013278 File Offset: 0x00011478
		private bool HandleUserInputDebug()
		{
			bool flag = false;
			if (base.DebugInput.IsHotKeyPressed("MissionScreenHotkeyResetDebugVariables"))
			{
				GameNetwork.ResetDebugVariables();
			}
			if (base.DebugInput.IsHotKeyPressed("MissionScreenHotkeyIncreaseTotalUploadLimit"))
			{
				GameNetwork.IncreaseTotalUploadLimit(1000);
				flag = true;
			}
			if (base.DebugInput.IsHotKeyPressed("MissionScreenIncreaseTotalUploadLimit"))
			{
				GameNetwork.IncreaseTotalUploadLimit(-1000);
				flag = true;
			}
			if (base.DebugInput.IsHotKeyPressed("FixSkeletons"))
			{
				MBCommon.FixSkeletons();
				MessageManager.DisplayMessage("Skeleton models are reloaded...", 4294901760U);
				flag = true;
			}
			return flag;
		}

		// Token: 0x06000226 RID: 550 RVA: 0x00013304 File Offset: 0x00011504
		private void HandleUserInput(float dt)
		{
			bool flag = false;
			bool flag2 = this._isGamepadActive && this.PhotoModeRequiresMouse;
			if (this.Mission == null || this.Mission.CurrentState == 3)
			{
				return;
			}
			if (!flag && Game.Current.CheatMode)
			{
				flag = this.HandleUserInputCheatMode(dt);
			}
			if (flag)
			{
				return;
			}
			float num = this.SceneLayer.Input.GetMouseSensitivity();
			if (!this.MouseVisible && this.Mission.MainAgent != null && this.Mission.MainAgent.State == 1 && this.Mission.MainAgent.IsLookRotationInSlowMotion)
			{
				num *= ManagedParameters.Instance.GetManagedParameter(1);
			}
			float num2 = dt / 0.0009f;
			float num3 = dt / 0.0009f;
			float num4 = 0f;
			float num5 = 0f;
			if ((!MBCommon.IsPaused || this.IsPhotoModeEnabled) && !this.IsRadialMenuActive && this._cameraSpecialTargetFOV > 9f && this.Mission.Mode != 5)
			{
				if (this.MouseVisible && !this.SceneLayer.Input.IsKeyDown(225))
				{
					if (this.Mission.Mode != 1)
					{
						if (this.Mission.Mode == 6)
						{
							num4 = num2 * this.SceneLayer.Input.GetGameKeyAxis("CameraAxisX");
							num5 = -num3 * this.SceneLayer.Input.GetGameKeyAxis("CameraAxisY");
						}
						else
						{
							if (this.SceneLayer.Input.GetMousePositionRanged().x <= 0.01f)
							{
								num4 = -400f * dt;
							}
							else if (this.SceneLayer.Input.GetMousePositionRanged().x >= 0.99f)
							{
								num4 = 400f * dt;
							}
							if (this.SceneLayer.Input.GetMousePositionRanged().y <= 0.01f)
							{
								num5 = -400f * dt;
							}
							else if (this.SceneLayer.Input.GetMousePositionRanged().y >= 0.99f)
							{
								num5 = 400f * dt;
							}
						}
					}
				}
				else if (!this.SceneLayer.Input.GetIsMouseActive())
				{
					float gameKeyAxis = this.SceneLayer.Input.GetGameKeyAxis("CameraAxisX");
					float gameKeyAxis2 = this.SceneLayer.Input.GetGameKeyAxis("CameraAxisY");
					if (gameKeyAxis > 0.9f || gameKeyAxis < -0.9f)
					{
						num2 = dt / 0.00045f;
					}
					if (gameKeyAxis2 > 0.9f || gameKeyAxis2 < -0.9f)
					{
						num3 = dt / 0.00045f;
					}
					if (this._zoomToggled)
					{
						num2 /= 1.5f;
						num3 /= 1.5f;
					}
					num4 = num2 * this.SceneLayer.Input.GetGameKeyAxis("CameraAxisX") + this.SceneLayer.Input.GetMouseMoveX();
					num5 = -num3 * this.SceneLayer.Input.GetGameKeyAxis("CameraAxisY") + this.SceneLayer.Input.GetMouseMoveY();
				}
				else
				{
					num4 = this.SceneLayer.Input.GetMouseMoveX();
					num5 = this.SceneLayer.Input.GetMouseMoveY();
				}
			}
			if (NativeConfig.EnableEditMode && base.DebugInput.IsHotKeyPressed("MissionScreenHotkeySwitchCameraSmooth"))
			{
				this._cameraSmoothMode = !this._cameraSmoothMode;
				if (this._cameraSmoothMode)
				{
					MessageManager.DisplayMessage("Camera smooth mode Enabled.", uint.MaxValue);
				}
				else
				{
					MessageManager.DisplayMessage("Camera smooth mode Disabled.", uint.MaxValue);
				}
			}
			float num6 = 0.0035f;
			float num8;
			if (this._cameraSmoothMode)
			{
				num6 *= 0.02f;
				float num7 = 0.02f + dt - 8f * (dt * dt);
				num8 = MathF.Max(0f, 1f - 2f * num7);
			}
			else
			{
				num8 = 0f;
			}
			this._cameraBearingDelta *= num8;
			this._cameraElevationDelta *= num8;
			bool isSessionActive = GameNetwork.IsSessionActive;
			float num9 = num6 * num;
			float num10 = -num4 * num9;
			float num11 = (NativeConfig.InvertMouse ? num5 : (-num5)) * num9;
			if (isSessionActive)
			{
				float num12 = 0.3f + 10f * dt;
				num10 = MBMath.ClampFloat(num10, -num12, num12);
				num11 = MBMath.ClampFloat(num11, -num12, num12);
			}
			this._cameraBearingDelta += num10;
			this._cameraElevationDelta += num11;
			if (isSessionActive)
			{
				float num13 = 0.3f + 10f * dt;
				this._cameraBearingDelta = MBMath.ClampFloat(this._cameraBearingDelta, -num13, num13);
				this._cameraElevationDelta = MBMath.ClampFloat(this._cameraElevationDelta, -num13, num13);
			}
			Agent agentToFollow = this.GetSpectatingData(this.CombatCamera.Frame.origin).AgentToFollow;
			if (this.Mission.CameraIsFirstPerson && agentToFollow != null && agentToFollow.Controller == 2 && agentToFollow.HasMount && ((ManagedOptions.GetConfig(6) == 1f && !agentToFollow.WieldedWeapon.IsEmpty && agentToFollow.WieldedWeapon.CurrentUsageItem.IsRangedWeapon) || (ManagedOptions.GetConfig(6) == 2f && (agentToFollow.WieldedWeapon.IsEmpty || agentToFollow.WieldedWeapon.CurrentUsageItem.IsMeleeWeapon)) || ManagedOptions.GetConfig(6) == 3f))
			{
				this._cameraBearingDelta += agentToFollow.MountAgent.GetTurnSpeed() * dt;
			}
			if (this.InputManager.IsGameKeyDown(28))
			{
				Mission.CameraAddedDistance -= 2.1f * dt;
			}
			if (this.InputManager.IsGameKeyDown(29))
			{
				Mission.CameraAddedDistance += 2.1f * dt;
			}
			Mission.CameraAddedDistance = MBMath.ClampFloat(Mission.CameraAddedDistance, 0.7f, 2.4f);
			this._isGamepadActive = !Input.IsMouseActive && Input.IsControllerConnected;
			if (this._isGamepadActive)
			{
				Agent mainAgent = this.Mission.MainAgent;
				bool flag3;
				if (mainAgent == null)
				{
					flag3 = false;
				}
				else
				{
					WeaponComponentData currentUsageItem = mainAgent.WieldedWeapon.CurrentUsageItem;
					bool? flag4 = ((currentUsageItem != null) ? new bool?(currentUsageItem.IsRangedWeapon) : null);
					bool flag5 = true;
					flag3 = (flag4.GetValueOrDefault() == flag5) & (flag4 != null);
				}
				if (!flag3)
				{
					goto IL_658;
				}
			}
			bool flag6;
			if (this.CustomCamera == null)
			{
				flag6 = !this.IsRadialMenuActive;
				goto IL_659;
			}
			IL_658:
			flag6 = false;
			IL_659:
			bool flag7 = flag6 || this._forceCanZoom;
			if (flag7)
			{
				float applicationTime = Time.ApplicationTime;
				if (this.SceneLayer.Input.IsHotKeyPressed("ToggleZoom"))
				{
					this._zoomToggleTime = applicationTime;
				}
				if (applicationTime - this._zoomToggleTime > 0.01f && this.SceneLayer.Input.IsHotKeyDown("ToggleZoom"))
				{
					this._zoomToggleTime = float.MaxValue;
					this._zoomToggled = !this._zoomToggled;
				}
			}
			else
			{
				this._zoomToggled = false;
			}
			bool photoModeOrbit = this.Mission.Scene.GetPhotoModeOrbit();
			if (this.IsPhotoModeEnabled)
			{
				if (photoModeOrbit && !flag2)
				{
					this._zoomAmount -= this.SceneLayer.Input.GetDeltaMouseScroll() * 0.002f;
					this._zoomAmount = MBMath.ClampFloat(this._zoomAmount, 0f, 50f);
				}
			}
			else
			{
				if (agentToFollow != null && agentToFollow.IsMine && (this._zoomToggled || (flag7 && this.SceneLayer.Input.IsGameKeyDown(24))))
				{
					this._zoomAmount += 5f * dt;
				}
				else
				{
					this._zoomAmount -= 5f * dt;
				}
				this._zoomAmount = MBMath.ClampFloat(this._zoomAmount, 0f, 1f);
			}
			if (!this.IsPhotoModeEnabled)
			{
				if (MBMath.ApproximatelyEqualsTo(this._zoomAmount, 1f, 1E-05f))
				{
					this.Mission.Scene.SetDepthOfFieldParameters(this._zoomAmount * 160f * 110f, this._zoomAmount * 1500f * 0.3f, false);
				}
				else
				{
					this.Mission.Scene.SetDepthOfFieldParameters(0f, 0f, false);
				}
			}
			float num14;
			this.Mission.Scene.RayCastForClosestEntityOrTerrain(this.CombatCamera.Position + this.CombatCamera.Direction * this._cameraRayCastOffset, this.CombatCamera.Position + this.CombatCamera.Direction * 3000f, ref num14, 0.01f, 79617);
			this.Mission.Scene.SetDepthOfFieldFocus(num14);
			Agent mainAgent2 = this.Mission.MainAgent;
			if (mainAgent2 != null && !this.IsPhotoModeEnabled)
			{
				if (this._isPlayerAgentAdded)
				{
					this._isPlayerAgentAdded = false;
					if (this.Mission.Mode != 6)
					{
						this.CameraBearing = (this.Mission.CameraIsFirstPerson ? mainAgent2.LookDirection.RotationZ : mainAgent2.MovementDirectionAsAngle);
						this.CameraElevation = (this.Mission.CameraIsFirstPerson ? mainAgent2.LookDirection.RotationX : 0f);
						this._cameraSpecialTargetAddedBearing = 0f;
						this._cameraSpecialTargetAddedElevation = 0f;
						this._cameraSpecialCurrentAddedBearing = 0f;
						this._cameraSpecialCurrentAddedElevation = 0f;
					}
				}
				if (this.Mission.ClearSceneTimerElapsedTime >= 0f)
				{
					bool flag8;
					if (!this.IsViewingCharacter() && this.Mission.Mode != 1 && this.Mission.Mode != 5 && !mainAgent2.IsLookDirectionLocked)
					{
						MissionMainAgentController missionMainAgentController = this._missionMainAgentController;
						flag8 = ((missionMainAgentController != null) ? missionMainAgentController.LockedAgent : null) == null;
					}
					else
					{
						flag8 = false;
					}
					if (!flag8)
					{
						if (this.Mission.Mode != 5)
						{
							if (this._missionMainAgentController.LockedAgent != null)
							{
								this.CameraBearing = mainAgent2.LookDirection.RotationZ;
								this.CameraElevation = mainAgent2.LookDirection.RotationX;
							}
							else
							{
								this._cameraSpecialTargetAddedBearing = MBMath.WrapAngle(this._cameraSpecialTargetAddedBearing + this._cameraBearingDelta);
								this._cameraSpecialTargetAddedElevation = MBMath.WrapAngle(this._cameraSpecialTargetAddedElevation + this._cameraElevationDelta);
								this._cameraSpecialCurrentAddedBearing = MBMath.WrapAngle(this._cameraSpecialCurrentAddedBearing + this._cameraBearingDelta);
								this._cameraSpecialCurrentAddedElevation = MBMath.WrapAngle(this._cameraSpecialCurrentAddedElevation + this._cameraElevationDelta);
							}
						}
						float num15 = this.CameraElevation + this._cameraSpecialTargetAddedElevation;
						num15 = MBMath.ClampFloat(num15, -1.3659099f, 1.1219975f);
						this._cameraSpecialTargetAddedElevation = num15 - this.CameraElevation;
						num15 = this.CameraElevation + this._cameraSpecialCurrentAddedElevation;
						num15 = MBMath.ClampFloat(num15, -1.3659099f, 1.1219975f);
						this._cameraSpecialCurrentAddedElevation = num15 - this.CameraElevation;
					}
					else
					{
						this._cameraSpecialTargetAddedBearing = 0f;
						this._cameraSpecialTargetAddedElevation = 0f;
						if (this.Mission.CameraIsFirstPerson && agentToFollow != null && agentToFollow == this.Mission.MainAgent && !this.IsPhotoModeEnabled && !Extensions.HasAnyFlag<AnimFlags>(agentToFollow.GetCurrentAnimationFlag(0), 8388608L) && !Extensions.HasAnyFlag<AnimFlags>(agentToFollow.GetCurrentAnimationFlag(1), 8388608L))
						{
							if (this.Mission.Mode == 1 || this.Mission.Mode == 5)
							{
								MissionMainAgentController missionMainAgentController2 = this._missionMainAgentController;
								if (((missionMainAgentController2 != null) ? missionMainAgentController2.InteractionComponent.CurrentFocusedObject : null) != null && this._missionMainAgentController.InteractionComponent.CurrentFocusedObject.FocusableObjectType == 3)
								{
									goto IL_CC6;
								}
							}
							if (this._missionMainAgentController == null || !this._missionMainAgentController.CustomLookDir.IsNonZero)
							{
								float num16 = MBMath.WrapAngle(this.CameraBearing + this._cameraBearingDelta);
								float num17 = MBMath.WrapAngle(this.CameraElevation + this._cameraElevationDelta);
								float num18;
								float num19;
								MissionScreen.CalculateNewBearingAndElevation(agentToFollow, num16, num17, out num18, out num19);
								if (num18 != num16)
								{
									this._cameraBearingDelta = (MBMath.IsBetween(MBMath.WrapAngle(this._cameraBearingDelta), 0f, 3.1415927f) ? MBMath.ClampFloat(MBMath.WrapAngle(num18 - this.CameraBearing), 0f, this._cameraBearingDelta) : MBMath.ClampFloat(MBMath.WrapAngle(num18 - this.CameraBearing), this._cameraBearingDelta, 0f));
								}
								if (num19 != num17)
								{
									this._cameraElevationDelta = (MBMath.IsBetween(MBMath.WrapAngle(this._cameraElevationDelta), 0f, 3.1415927f) ? MBMath.ClampFloat(MBMath.WrapAngle(num19 - this.CameraElevation), 0f, this._cameraElevationDelta) : MBMath.ClampFloat(MBMath.WrapAngle(num19 - this.CameraElevation), this._cameraElevationDelta, 0f));
								}
							}
						}
						IL_CC6:
						this.CameraBearing += this._cameraBearingDelta;
						this.CameraElevation += this._cameraElevationDelta;
						this.CameraElevation = MBMath.ClampFloat(this.CameraElevation, -1.3659099f, 1.1219975f);
					}
					if (this.LockCameraMovement)
					{
						this._cameraToggleStartTime = float.MaxValue;
						return;
					}
					if (!Input.IsMouseActive)
					{
						float applicationTime2 = Time.ApplicationTime;
						if (this.SceneLayer.Input.IsGameKeyPressed(27))
						{
							if (this.SceneLayer.Input.GetGameKeyAxis("MovementAxisX") <= 0.1f && this.SceneLayer.Input.GetGameKeyAxis("MovementAxisY") <= 0.1f)
							{
								this._cameraToggleStartTime = applicationTime2;
							}
						}
						else if (!this.SceneLayer.Input.IsGameKeyDown(27))
						{
							this._cameraToggleStartTime = float.MaxValue;
						}
						if (this.GetCameraToggleProgress() >= 1f)
						{
							this._cameraToggleStartTime = float.MaxValue;
							this.Mission.CameraIsFirstPerson = !this.Mission.CameraIsFirstPerson;
							this._cameraApplySpecialMovementsInstantly = true;
							return;
						}
					}
					else if (this.SceneLayer.Input.IsGameKeyPressed(27))
					{
						this.Mission.CameraIsFirstPerson = !this.Mission.CameraIsFirstPerson;
						this._cameraApplySpecialMovementsInstantly = true;
						return;
					}
				}
			}
			else
			{
				if (this.IsPhotoModeEnabled && this.Mission.CameraIsFirstPerson)
				{
					this.Mission.CameraIsFirstPerson = false;
				}
				this.CameraBearing += this._cameraBearingDelta;
				this.CameraElevation += this._cameraElevationDelta;
				this.CameraElevation = MBMath.ClampFloat(this.CameraElevation, -1.3659099f, 1.1219975f);
			}
		}

		// Token: 0x06000227 RID: 551 RVA: 0x00014188 File Offset: 0x00012388
		public float GetCameraToggleProgress()
		{
			if (this._cameraToggleStartTime != 3.4028235E+38f && this.SceneLayer.Input.IsGameKeyDown(27))
			{
				return (Time.ApplicationTime - this._cameraToggleStartTime) / 0.5f;
			}
			return 0f;
		}

		// Token: 0x06000228 RID: 552 RVA: 0x000141C4 File Offset: 0x000123C4
		private bool HandleUserInputCheatMode(float dt)
		{
			bool flag = false;
			if (!GameNetwork.IsMultiplayer)
			{
				if (this.InputManager.IsHotKeyPressed("EnterSlowMotion"))
				{
					float num;
					if (this.Mission.GetRequestedTimeSpeed(1121, ref num))
					{
						this.Mission.RemoveTimeSpeedRequest(1121);
					}
					else
					{
						this.Mission.AddTimeSpeedRequest(new Mission.TimeSpeedRequest(0.1f, 1121));
					}
					flag = true;
				}
				float num2;
				if (this.Mission.GetRequestedTimeSpeed(1121, ref num2))
				{
					if (this.InputManager.IsHotKeyDown("MissionScreenHotkeyIncreaseSlowMotionFactor"))
					{
						this.Mission.RemoveTimeSpeedRequest(1121);
						num2 = MBMath.ClampFloat(num2 + 0.5f * dt, 0f, 1f);
						this.Mission.AddTimeSpeedRequest(new Mission.TimeSpeedRequest(num2, 1121));
					}
					if (this.InputManager.IsHotKeyDown("MissionScreenHotkeyDecreaseSlowMotionFactor"))
					{
						this.Mission.RemoveTimeSpeedRequest(1121);
						num2 = MBMath.ClampFloat(num2 - 0.5f * dt, 0f, 1f);
						this.Mission.AddTimeSpeedRequest(new Mission.TimeSpeedRequest(num2, 1121));
					}
				}
				if (this.InputManager.IsHotKeyPressed("Pause"))
				{
					this._missionState.Paused = !this._missionState.Paused;
					flag = true;
				}
				if (this.InputManager.IsHotKeyPressed("MissionScreenHotkeyHealYourSelf") && this.Mission.MainAgent != null)
				{
					this.Mission.MainAgent.Health = this.Mission.MainAgent.HealthLimit;
					flag = true;
				}
				if (this.InputManager.IsHotKeyPressed("MissionScreenHotkeyHealYourHorse"))
				{
					Agent mainAgent = this.Mission.MainAgent;
					if (((mainAgent != null) ? mainAgent.MountAgent : null) != null)
					{
						this.Mission.MainAgent.MountAgent.Health = this.Mission.MainAgent.MountAgent.HealthLimit;
						flag = true;
					}
				}
				if (!this.InputManager.IsShiftDown())
				{
					if (!this.InputManager.IsAltDown())
					{
						if (this.InputManager.IsHotKeyPressed("MissionScreenHotkeyKillEnemyAgent"))
						{
							return Mission.Current.KillCheats(false, true, false, false);
						}
					}
					else if (this.InputManager.IsHotKeyPressed("MissionScreenHotkeyKillAllEnemyAgents"))
					{
						return Mission.Current.KillCheats(true, true, false, false);
					}
				}
				else if (!this.InputManager.IsAltDown())
				{
					if (this.InputManager.IsHotKeyPressed("MissionScreenHotkeyKillEnemyHorse"))
					{
						return Mission.Current.KillCheats(false, true, true, false);
					}
				}
				else if (this.InputManager.IsHotKeyPressed("MissionScreenHotkeyKillAllEnemyHorses"))
				{
					return Mission.Current.KillCheats(true, true, true, false);
				}
				if (!this.InputManager.IsShiftDown())
				{
					if (!this.InputManager.IsAltDown())
					{
						if (this.InputManager.IsHotKeyPressed("MissionScreenHotkeyKillFriendlyAgent"))
						{
							return Mission.Current.KillCheats(false, false, false, false);
						}
					}
					else if (this.InputManager.IsHotKeyPressed("MissionScreenHotkeyKillAllFriendlyAgents"))
					{
						return Mission.Current.KillCheats(true, false, false, false);
					}
				}
				else if (!this.InputManager.IsAltDown())
				{
					if (this.InputManager.IsHotKeyPressed("MissionScreenHotkeyKillFriendlyHorse"))
					{
						return Mission.Current.KillCheats(false, false, true, false);
					}
				}
				else if (this.InputManager.IsHotKeyPressed("MissionScreenHotkeyKillAllFriendlyHorses"))
				{
					return Mission.Current.KillCheats(true, false, true, false);
				}
				if (!this.InputManager.IsShiftDown())
				{
					if (this.InputManager.IsHotKeyPressed("MissionScreenHotkeyKillYourSelf"))
					{
						return Mission.Current.KillCheats(false, false, false, true);
					}
				}
				else if (this.InputManager.IsHotKeyPressed("MissionScreenHotkeyKillYourHorse"))
				{
					return Mission.Current.KillCheats(false, false, true, true);
				}
				if ((GameNetwork.IsServerOrRecorder || !GameNetwork.IsMultiplayer) && this.InputManager.IsHotKeyPressed("MissionScreenHotkeyGhostCam"))
				{
					this.IsCheatGhostMode = !this.IsCheatGhostMode;
				}
			}
			if (!GameNetwork.IsSessionActive)
			{
				if (this.InputManager.IsHotKeyPressed("MissionScreenHotkeySwitchAgentToAi") && this.Mission.MainAgent != null && this.Mission.MainAgent.IsActive())
				{
					this.Mission.MainAgent.Controller = ((this.Mission.MainAgent.Controller == 2) ? 1 : 2);
					flag = true;
				}
				if (this.InputManager.IsHotKeyPressed("MissionScreenHotkeyControlFollowedAgent"))
				{
					if (this.Mission.MainAgent != null)
					{
						if (this.Mission.MainAgent.Controller == 2)
						{
							this.Mission.MainAgent.Controller = 1;
							if (this.LastFollowedAgent != null)
							{
								this.LastFollowedAgent.Controller = 2;
							}
						}
						else
						{
							foreach (Agent agent in this.Mission.Agents)
							{
								if (agent.Controller == 2)
								{
									agent.Controller = 1;
								}
							}
							this.Mission.MainAgent.Controller = 2;
						}
						flag = true;
					}
					else
					{
						if (this.LastFollowedAgent != null)
						{
							this.LastFollowedAgent.Controller = 2;
						}
						flag = true;
					}
				}
			}
			return flag;
		}

		// Token: 0x06000229 RID: 553 RVA: 0x000146CC File Offset: 0x000128CC
		public void AddMissionView(MissionView missionView)
		{
			this.Mission.AddMissionBehavior(missionView);
			missionView.MissionScreen = this;
			missionView.OnMissionScreenInitialize();
			Debug.ReportMemoryBookmark("MissionView Initialized: " + missionView.GetType().Name);
		}

		// Token: 0x0600022A RID: 554 RVA: 0x00014704 File Offset: 0x00012904
		public void ScreenPointToWorldRay(Vec2 screenPoint, out Vec3 rayBegin, out Vec3 rayEnd)
		{
			rayBegin = Vec3.Invalid;
			rayEnd = Vec3.Invalid;
			Vec2 vec = this.SceneView.ScreenPointToViewportPoint(screenPoint);
			this.CombatCamera.ViewportPointToWorldRay(ref rayBegin, ref rayEnd, vec);
			float num = -1f;
			foreach (KeyValuePair<string, ICollection<Vec2>> keyValuePair in this.Mission.Boundaries)
			{
				float boundaryRadius = this.Mission.Boundaries.GetBoundaryRadius(keyValuePair.Key);
				if (num < boundaryRadius)
				{
					num = boundaryRadius;
				}
			}
			if (num < 0f)
			{
				num = 30f;
			}
			Vec3 vec2 = rayEnd - rayBegin;
			float num2 = vec2.Normalize();
			rayEnd = rayBegin + vec2 * MathF.Min(num2, num);
		}

		// Token: 0x0600022B RID: 555 RVA: 0x000147F4 File Offset: 0x000129F4
		public bool GetProjectedMousePositionOnGround(out Vec3 groundPosition, out Vec3 groundNormal, BodyFlags excludeBodyOwnerFlags, bool checkOccludedSurface)
		{
			return this.SceneView.ProjectedMousePositionOnGround(ref groundPosition, ref groundNormal, this.MouseVisible, excludeBodyOwnerFlags, checkOccludedSurface);
		}

		// Token: 0x0600022C RID: 556 RVA: 0x0001480C File Offset: 0x00012A0C
		public void CancelQuickPositionOrder()
		{
			if (this.OrderFlag != null)
			{
				this.OrderFlag.IsVisible = false;
			}
		}

		// Token: 0x0600022D RID: 557 RVA: 0x00014822 File Offset: 0x00012A22
		public bool MissionStartedRendering()
		{
			return this.SceneView != null && this.SceneView.ReadyToRender();
		}

		// Token: 0x0600022E RID: 558 RVA: 0x0001483F File Offset: 0x00012A3F
		public Vec3 GetOrderFlagPosition()
		{
			if (this.OrderFlag != null)
			{
				return this.OrderFlag.Position;
			}
			return Vec3.Invalid;
		}

		// Token: 0x0600022F RID: 559 RVA: 0x0001485A File Offset: 0x00012A5A
		public MatrixFrame GetOrderFlagFrame()
		{
			return this.OrderFlag.Frame;
		}

		// Token: 0x06000230 RID: 560 RVA: 0x00014868 File Offset: 0x00012A68
		private void ActivateLoadingScreen()
		{
			if (this.SceneLayer != null && this.SceneLayer.SceneView != null)
			{
				Scene scene = this.SceneLayer.SceneView.GetScene();
				if (scene != null)
				{
					scene.PreloadForRendering();
				}
			}
		}

		// Token: 0x06000231 RID: 561 RVA: 0x000148B0 File Offset: 0x00012AB0
		public void SetRadialMenuActiveState(bool isActive)
		{
			this.IsRadialMenuActive = isActive;
		}

		// Token: 0x06000232 RID: 562 RVA: 0x000148B9 File Offset: 0x00012AB9
		public void SetPhotoModeRequiresMouse(bool isRequired)
		{
			this.PhotoModeRequiresMouse = isRequired;
		}

		// Token: 0x06000233 RID: 563 RVA: 0x000148C4 File Offset: 0x00012AC4
		public void SetPhotoModeEnabled(bool isEnabled)
		{
			if (this.IsPhotoModeEnabled != isEnabled && !GameNetwork.IsMultiplayer)
			{
				this.IsPhotoModeEnabled = isEnabled;
				if (isEnabled)
				{
					MBCommon.PauseGameEngine();
					using (List<MissionView>.Enumerator enumerator = this._missionViews.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							MissionView missionView = enumerator.Current;
							missionView.OnPhotoModeActivated();
						}
						goto IL_90;
					}
				}
				MBCommon.UnPauseGameEngine();
				foreach (MissionView missionView2 in this._missionViews)
				{
					missionView2.OnPhotoModeDeactivated();
				}
				IL_90:
				this.Mission.Scene.SetPhotoModeOn(this.IsPhotoModeEnabled);
			}
		}

		// Token: 0x06000234 RID: 564 RVA: 0x00014994 File Offset: 0x00012B94
		public void SetConversationActive(bool isActive)
		{
			if (this.IsConversationActive != isActive && !GameNetwork.IsMultiplayer)
			{
				this.IsConversationActive = isActive;
				foreach (MissionView missionView in this._missionViews)
				{
					if (isActive)
					{
						missionView.OnConversationBegin();
					}
					else
					{
						missionView.OnConversationEnd();
					}
				}
			}
		}

		// Token: 0x06000235 RID: 565 RVA: 0x00014A08 File Offset: 0x00012C08
		public void SetCameraLockState(bool isLocked)
		{
			this.LockCameraMovement = isLocked;
		}

		// Token: 0x06000236 RID: 566 RVA: 0x00014A11 File Offset: 0x00012C11
		public void RegisterView(MissionView missionView)
		{
			this._missionViews.Add(missionView);
		}

		// Token: 0x06000237 RID: 567 RVA: 0x00014A1F File Offset: 0x00012C1F
		public void UnregisterView(MissionView missionView)
		{
			this._missionViews.Remove(missionView);
		}

		// Token: 0x06000238 RID: 568 RVA: 0x00014A2E File Offset: 0x00012C2E
		public IAgentVisual GetPlayerAgentVisuals(MissionPeer lobbyPeer)
		{
			return lobbyPeer.GetAgentVisualForPeer(0);
		}

		// Token: 0x06000239 RID: 569 RVA: 0x00014A37 File Offset: 0x00012C37
		public void SetAgentToFollow(Agent agent)
		{
			this._agentToFollowOverride = agent;
		}

		// Token: 0x0600023A RID: 570 RVA: 0x00014A40 File Offset: 0x00012C40
		public Mission.SpectatorData GetSpectatingData(Vec3 currentCameraPosition)
		{
			Agent agent = null;
			IAgentVisual agentVisual = null;
			SpectatorCameraTypes spectatorCameraTypes = -1;
			bool flag = this.Mission.MainAgent != null && this.Mission.MainAgent.IsCameraAttachable() && this.Mission.Mode != 6;
			bool flag2 = flag || (this.LastFollowedAgent != null && this.LastFollowedAgent.Controller == 2 && this.LastFollowedAgent.IsCameraAttachable());
			MissionPeer missionPeer = ((GameNetwork.MyPeer != null) ? PeerExtensions.GetComponent<MissionPeer>(GameNetwork.MyPeer) : null);
			bool flag3 = missionPeer != null && missionPeer.HasSpawnedAgentVisuals;
			bool flag4 = (this._missionLobbyComponent != null && (this._missionLobbyComponent.MissionType == 3 || this._missionLobbyComponent.MissionType == 1)) || this.Mission.Mode == 6;
			SpectatorCameraTypes spectatorCameraTypes2;
			if (!this.IsCheatGhostMode && !flag2 && flag4 && this._agentToFollowOverride != null && this._agentToFollowOverride.IsCameraAttachable() && !flag3)
			{
				agent = this._agentToFollowOverride;
				spectatorCameraTypes2 = 2;
			}
			else
			{
				if (this._missionCameraModeLogic != null)
				{
					spectatorCameraTypes = this._missionCameraModeLogic.GetMissionCameraLockMode(flag2);
				}
				if (this.IsCheatGhostMode)
				{
					spectatorCameraTypes2 = 0;
				}
				else if (spectatorCameraTypes != -1)
				{
					spectatorCameraTypes2 = spectatorCameraTypes;
				}
				else if (this.Mission.Mode == 6)
				{
					spectatorCameraTypes2 = 0;
				}
				else if (flag)
				{
					spectatorCameraTypes2 = 1;
					agent = this.Mission.MainAgent;
				}
				else if (flag2)
				{
					spectatorCameraTypes2 = 1;
					agent = this.LastFollowedAgent;
				}
				else if (missionPeer != null && this.GetPlayerAgentVisuals(missionPeer) != null && spectatorCameraTypes != null)
				{
					spectatorCameraTypes2 = 7;
					agentVisual = this.GetPlayerAgentVisuals(missionPeer);
				}
				else if (!GameNetwork.IsMultiplayer)
				{
					spectatorCameraTypes2 = 0;
				}
				else
				{
					spectatorCameraTypes2 = MultiplayerOptionsExtensions.GetIntValue(25, 0);
				}
				if ((spectatorCameraTypes2 != 1 && spectatorCameraTypes2 != 7 && this.Mission.Mode != 6) || (this.IsCheatGhostMode && !this.IsOrderMenuOpen && !this.IsTransferMenuOpen))
				{
					if (this.LastFollowedAgent != null && this.LastFollowedAgent.IsCameraAttachable())
					{
						agent = this.LastFollowedAgent;
					}
					else if (spectatorCameraTypes2 != null || (this._gatherCustomAgentListToSpectate != null && this.LastFollowedAgent != null))
					{
						agent = this.FindNextCameraAttachableAgent(this.LastFollowedAgent, spectatorCameraTypes2, 1, currentCameraPosition);
					}
					bool flag5 = Game.Current.CheatMode && this.InputManager.IsControlDown();
					if (this.InputManager.IsGameKeyReleased(10) || this.InputManager.IsGameKeyReleased(11))
					{
						if (!flag5)
						{
							agent = this.FindNextCameraAttachableAgent(this.LastFollowedAgent, spectatorCameraTypes2, -1, currentCameraPosition);
						}
					}
					else if ((this.InputManager.IsGameKeyReleased(9) || this.InputManager.IsGameKeyReleased(12)) && !this._rightButtonDraggingMode)
					{
						if (!flag5)
						{
							agent = this.FindNextCameraAttachableAgent(this.LastFollowedAgent, spectatorCameraTypes2, 1, currentCameraPosition);
						}
					}
					else if ((this.InputManager.IsGameKeyDown(0) || this.InputManager.IsGameKeyDown(1) || this.InputManager.IsGameKeyDown(2) || this.InputManager.IsGameKeyDown(3) || (this.InputManager.GetIsControllerConnected() && (Input.GetKeyState(222).y != 0f || Input.GetKeyState(222).x != 0f))) && spectatorCameraTypes2 == null)
					{
						agent = null;
						agentVisual = null;
					}
				}
			}
			return new Mission.SpectatorData(agent, agentVisual, spectatorCameraTypes2);
		}

		// Token: 0x0600023B RID: 571 RVA: 0x00014D88 File Offset: 0x00012F88
		private Agent FindNextCameraAttachableAgent(Agent currentAgent, SpectatorCameraTypes cameraLockMode, int iterationDirection, Vec3 currentCameraPosition)
		{
			if (this.Mission.AllAgents == null || this.Mission.AllAgents.Count == 0)
			{
				return null;
			}
			if (MBDebug.IsErrorReportModeActive())
			{
				return null;
			}
			MissionPeer missionPeer = (GameNetwork.IsMyPeerReady ? PeerExtensions.GetComponent<MissionPeer>(GameNetwork.MyPeer) : null);
			List<Agent> list;
			if (this._gatherCustomAgentListToSpectate != null)
			{
				list = this._gatherCustomAgentListToSpectate(currentAgent);
			}
			else
			{
				switch (cameraLockMode)
				{
				case 2:
					list = this.Mission.AllAgents.Where((Agent x) => x.IsCameraAttachable() || x == currentAgent).ToList<Agent>();
					break;
				case 3:
					list = this.Mission.AllAgents.Where((Agent x) => (x.MissionPeer != null && x.IsCameraAttachable()) || x == currentAgent).ToList<Agent>();
					break;
				case 4:
					list = this.Mission.AllAgents.Where(delegate(Agent x)
					{
						if (x.Formation != null)
						{
							Formation formation = x.Formation;
							MissionPeer missionPeer2 = missionPeer;
							if (formation == ((missionPeer2 != null) ? missionPeer2.ControlledFormation : null) && x.IsCameraAttachable())
							{
								return true;
							}
						}
						return x == currentAgent;
					}).ToList<Agent>();
					break;
				case 5:
				case 6:
					list = this.Mission.AllAgents.Where((Agent x) => (x.Team == this.Mission.PlayerTeam && x.MissionPeer != null && x.IsCameraAttachable()) || x == currentAgent).ToList<Agent>();
					break;
				default:
					list = this.Mission.AllAgents.Where((Agent x) => x.IsCameraAttachable() || x == currentAgent).ToList<Agent>();
					break;
				}
			}
			Agent agent;
			if (list.Count - ((currentAgent != null && !currentAgent.IsCameraAttachable()) ? 1 : 0) == 0)
			{
				agent = null;
			}
			else if (currentAgent == null)
			{
				Agent agent2 = null;
				float num = float.MaxValue;
				foreach (Agent agent3 in list)
				{
					float lengthSquared = (currentCameraPosition - agent3.Position).LengthSquared;
					if (num > lengthSquared)
					{
						num = lengthSquared;
						agent2 = agent3;
					}
				}
				agent = agent2;
			}
			else
			{
				int num2 = list.IndexOf(currentAgent);
				if (iterationDirection == 1)
				{
					agent = list[(num2 + 1) % list.Count];
				}
				else
				{
					agent = ((num2 < 0) ? list[list.Count - 1] : list[(num2 + list.Count - 1) % list.Count]);
				}
			}
			return agent;
		}

		// Token: 0x0600023C RID: 572 RVA: 0x00014FD8 File Offset: 0x000131D8
		void IGameStateListener.OnInitialize()
		{
		}

		// Token: 0x0600023D RID: 573 RVA: 0x00014FDA File Offset: 0x000131DA
		void IGameStateListener.OnFinalize()
		{
		}

		// Token: 0x0600023E RID: 574 RVA: 0x00014FDC File Offset: 0x000131DC
		void IGameStateListener.OnActivate()
		{
			if (this._isDeactivated)
			{
				this.ActivateMissionView();
			}
			this._isDeactivated = false;
		}

		// Token: 0x0600023F RID: 575 RVA: 0x00014FF4 File Offset: 0x000131F4
		void IGameStateListener.OnDeactivate()
		{
			this._isDeactivated = true;
			Mission mission = this.Mission;
			if (((mission != null) ? mission.MissionBehaviors : null) != null)
			{
				foreach (MissionView missionView in this._missionViews)
				{
					missionView.OnMissionScreenDeactivate();
				}
			}
			this.OnDeactivate();
		}

		// Token: 0x06000240 RID: 576 RVA: 0x00015068 File Offset: 0x00013268
		void IMissionSystemHandler.OnMissionAfterStarting(Mission mission)
		{
			this.Mission = mission;
			this.Mission.AddListener(this);
			using (List<MissionBehavior>.Enumerator enumerator = this.Mission.MissionBehaviors.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					MissionView missionView;
					if ((missionView = enumerator.Current as MissionView) != null)
					{
						missionView.MissionScreen = this;
					}
				}
			}
		}

		// Token: 0x06000241 RID: 577 RVA: 0x000150DC File Offset: 0x000132DC
		void IMissionSystemHandler.OnMissionLoadingFinished(Mission mission)
		{
			this.Mission = mission;
			this.InitializeMissionView();
			this.ActivateMissionView();
		}

		// Token: 0x06000242 RID: 578 RVA: 0x000150F4 File Offset: 0x000132F4
		void IMissionSystemHandler.BeforeMissionTick(Mission mission, float realDt)
		{
			if (MBEditor.EditModeEnabled)
			{
				if (base.DebugInput.IsHotKeyReleased("EnterEditMode") && mission == null)
				{
					if (MBEditor.IsEditModeOn)
					{
						MBEditor.LeaveEditMode();
						this._tickEditor = false;
					}
					else
					{
						MBEditor.EnterEditMode(this.SceneView, this.CombatCamera.Frame, this.CameraElevation, this.CameraBearing);
						this._tickEditor = true;
					}
				}
				if (this._tickEditor && MBEditor.IsEditModeOn)
				{
					MBEditor.TickEditMode(realDt);
					return;
				}
			}
			if (mission == null || mission.Scene == null)
			{
				return;
			}
			mission.Scene.SetOwnerThread();
			mission.Scene.SetDynamicShadowmapCascadesRadiusMultiplier(1f);
			if (MBEditor.EditModeEnabled)
			{
				MBCommon.CheckResourceModifications();
			}
			this.HandleUserInput(realDt);
			if (!this._isRenderingStarted && this.MissionStartedRendering())
			{
				Mission.Current.OnRenderingStarted();
				this._isRenderingStarted = true;
			}
			if (this._isRenderingStarted && this._loadingScreenFramesLeft >= 0 && !this._onSceneRenderingStartedCalled)
			{
				if (this._loadingScreenFramesLeft > 0)
				{
					this._loadingScreenFramesLeft--;
					Mission mission2 = Mission.Current;
					Utilities.SetLoadingScreenPercentage((mission2 != null && mission2.HasMissionBehavior<DeploymentMissionController>()) ? ((this._loadingScreenFramesLeft == 0) ? 1f : (0.92f - (float)this._loadingScreenFramesLeft * 0.005f)) : (1f - (float)this._loadingScreenFramesLeft * 0.02f));
				}
				bool flag = this.AreViewsReady();
				if (this._loadingScreenFramesLeft <= 0 && flag && !MBAnimation.IsAnyAnimationLoadingFromDisk())
				{
					this.OnSceneRenderingStarted();
					this._onSceneRenderingStartedCalled = true;
				}
			}
		}

		// Token: 0x06000243 RID: 579 RVA: 0x00015288 File Offset: 0x00013488
		private bool AreViewsReady()
		{
			bool flag = true;
			foreach (MissionView missionView in this._missionViews)
			{
				bool flag2 = missionView.IsReady();
				flag = flag && flag2;
			}
			return flag;
		}

		// Token: 0x06000244 RID: 580 RVA: 0x000152E0 File Offset: 0x000134E0
		private void CameraTick(Mission mission, float realDt)
		{
			if (mission.CurrentState == 2)
			{
				this.CheckForUpdateCamera(realDt);
			}
		}

		// Token: 0x06000245 RID: 581 RVA: 0x000152F2 File Offset: 0x000134F2
		void IMissionSystemHandler.UpdateCamera(Mission mission, float realDt)
		{
			this.CameraTick(mission, realDt);
			if (mission.CurrentState == 2 && !mission.MissionEnded)
			{
				MBWindowManager.PreDisplay();
			}
		}

		// Token: 0x06000246 RID: 582 RVA: 0x00015314 File Offset: 0x00013514
		void IMissionSystemHandler.AfterMissionTick(Mission mission, float realDt)
		{
			if ((mission.CurrentState == 2 || (mission.MissionEnded && mission.CurrentState != 4)) && Game.Current.CheatMode && this.IsCheatGhostMode && Agent.Main != null && this.InputManager.IsHotKeyPressed("MissionScreenHotkeyTeleportMainAgent"))
			{
				MatrixFrame lastFinalRenderCameraFrame = this.Mission.Scene.LastFinalRenderCameraFrame;
				float num;
				if (this.Mission.Scene.RayCastForClosestEntityOrTerrain(lastFinalRenderCameraFrame.origin, lastFinalRenderCameraFrame.origin + -lastFinalRenderCameraFrame.rotation.u * 100f, ref num, 0.01f, 6402441))
				{
					Vec3 vec = lastFinalRenderCameraFrame.origin + -lastFinalRenderCameraFrame.rotation.u * num;
					Vec2 vec2 = -lastFinalRenderCameraFrame.rotation.u.AsVec2;
					vec2.Normalize();
					MatrixFrame matrixFrame = default(MatrixFrame);
					matrixFrame.origin = vec;
					matrixFrame.rotation.f = new Vec3(vec2.x, vec2.y, 0f, -1f);
					matrixFrame.rotation.u = new Vec3(0f, 0f, 1f, -1f);
					matrixFrame.rotation.Orthonormalize();
					Agent.Main.TeleportToPosition(matrixFrame.origin);
				}
			}
			if (this.SceneLayer.Input.IsGameKeyPressed(4) && !base.DebugInput.IsAltDown() && MBEditor.EditModeEnabled && MBEditor.IsEditModeOn)
			{
				MBEditor.LeaveEditMissionMode();
			}
			if (mission.Scene == null)
			{
				MBDebug.Print("Mission is null on MissionScreen::OnFrameTick second phase", 0, 12, 17592186044416UL);
				return;
			}
		}

		// Token: 0x06000247 RID: 583 RVA: 0x000154E9 File Offset: 0x000136E9
		IEnumerable<MissionBehavior> IMissionSystemHandler.OnAddBehaviors(IEnumerable<MissionBehavior> behaviors, Mission mission, string missionName, bool addDefaultMissionBehaviors)
		{
			if (addDefaultMissionBehaviors)
			{
				behaviors = MissionScreen.AddDefaultMissionBehaviorsTo(mission, behaviors);
			}
			behaviors = ViewCreatorManager.CollectMissionBehaviors(missionName, mission, behaviors);
			return behaviors;
		}

		// Token: 0x06000248 RID: 584 RVA: 0x00015503 File Offset: 0x00013703
		private void HandleInputs()
		{
			if (!MBEditor.IsEditorMissionOn() && this.MissionStartedRendering() && this.SceneLayer.Input.IsHotKeyReleased("ToggleEscapeMenu") && !LoadingWindow.IsLoadingWindowActive)
			{
				this.OnEscape();
			}
		}

		// Token: 0x06000249 RID: 585 RVA: 0x00015538 File Offset: 0x00013738
		public void OnEscape()
		{
			if (this.IsMissionTickable)
			{
				foreach (MissionBehavior missionBehavior in (from v in this.Mission.MissionBehaviors
					where v is MissionView
					orderby ((MissionView)v).ViewOrderPriority
					select v).ToList<MissionBehavior>())
				{
					MissionView missionView = missionBehavior as MissionView;
					if (!this.IsMissionTickable)
					{
						break;
					}
					if (missionView.OnEscape())
					{
						break;
					}
				}
			}
		}

		// Token: 0x0600024A RID: 586 RVA: 0x000155F8 File Offset: 0x000137F8
		bool IMissionSystemHandler.RenderIsReady()
		{
			return this.MissionStartedRendering();
		}

		// Token: 0x0600024B RID: 587 RVA: 0x00015600 File Offset: 0x00013800
		void IMissionListener.OnEndMission()
		{
			this._agentToFollowOverride = null;
			this.LastFollowedAgent = null;
			this.LastFollowedAgentVisuals = null;
			MissionView[] array = this._missionViews.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].OnMissionScreenFinalize();
			}
			CraftedDataViewManager.Clear();
			this.Mission.RemoveListener(this);
		}

		// Token: 0x0600024C RID: 588 RVA: 0x00015655 File Offset: 0x00013855
		void IMissionListener.OnEquipItemsFromSpawnEquipmentBegin(Agent agent, Agent.CreationType creationType)
		{
			agent.ClearEquipment();
			agent.AgentVisuals.ClearVisualComponents(false);
		}

		// Token: 0x0600024D RID: 589 RVA: 0x0001566C File Offset: 0x0001386C
		void IMissionListener.OnEquipItemsFromSpawnEquipment(Agent agent, Agent.CreationType creationType)
		{
			switch (creationType)
			{
			case 1:
			case 3:
			{
				bool flag = false;
				Random random = null;
				bool randomizeColors = agent.RandomizeColors;
				uint num;
				uint num2;
				if (randomizeColors)
				{
					int bodyPropertiesSeed = agent.BodyPropertiesSeed;
					random = new Random(bodyPropertiesSeed);
					Color color;
					Color color2;
					AgentVisuals.GetRandomClothingColors(bodyPropertiesSeed, Color.FromUint(agent.ClothingColor1), Color.FromUint(agent.ClothingColor2), out color, out color2);
					num = color.ToUnsignedInteger();
					num2 = color2.ToUnsignedInteger();
				}
				else
				{
					num = agent.ClothingColor1;
					num2 = agent.ClothingColor2;
				}
				for (EquipmentIndex equipmentIndex = 5; equipmentIndex < 10; equipmentIndex++)
				{
					if (!agent.SpawnEquipment[equipmentIndex].IsVisualEmpty)
					{
						ItemObject itemObject = agent.SpawnEquipment[equipmentIndex].CosmeticItem ?? agent.SpawnEquipment[equipmentIndex].Item;
						bool flag2 = equipmentIndex == 6 && agent.SpawnEquipment[8].Item != null;
						bool flag3 = agent.Age >= 14f && agent.IsFemale;
						MetaMesh multiMesh = agent.SpawnEquipment[equipmentIndex].GetMultiMesh(flag3, flag2, true);
						if (multiMesh != null)
						{
							if (randomizeColors)
							{
								multiMesh.SetGlossMultiplier(AgentVisuals.GetRandomGlossFactor(random));
							}
							if (!itemObject.IsUsingTableau)
							{
								goto IL_212;
							}
							bool flag4;
							if (agent == null)
							{
								flag4 = null != null;
							}
							else
							{
								IAgentOriginBase origin = agent.Origin;
								flag4 = ((origin != null) ? origin.Banner : null) != null;
							}
							if (!flag4)
							{
								goto IL_212;
							}
							for (int i = 0; i < multiMesh.MeshCount; i++)
							{
								Mesh currentMesh = multiMesh.GetMeshAtIndex(i);
								Mesh currentMesh3 = currentMesh;
								if (currentMesh3 != null && !currentMesh3.HasTag("dont_use_tableau"))
								{
									Mesh currentMesh2 = currentMesh;
									if (currentMesh2 != null && currentMesh2.HasTag("banner_replacement_mesh"))
									{
										((BannerVisual)agent.Origin.Banner.BannerVisual).GetTableauTextureLarge(delegate(Texture t)
										{
											MissionScreen.ApplyBannerTextureToMesh(currentMesh, t);
										}, true);
										currentMesh.ManualInvalidate();
										break;
									}
								}
								currentMesh.ManualInvalidate();
							}
							IL_287:
							if (itemObject.UsingFacegenScaling)
							{
								multiMesh.UseHeadBoneFaceGenScaling(agent.AgentVisuals.GetSkeleton(), agent.Monster.HeadLookDirectionBoneIndex, agent.AgentVisuals.GetFacegenScalingMatrix());
							}
							Skeleton skeleton = agent.AgentVisuals.GetSkeleton();
							int num3 = ((skeleton != null) ? skeleton.GetComponentCount(3) : (-1));
							agent.AgentVisuals.AddMultiMesh(multiMesh, MBAgentVisuals.GetBodyMeshIndex(equipmentIndex));
							multiMesh.ManualInvalidate();
							int num4 = ((skeleton != null) ? skeleton.GetComponentCount(3) : (-1));
							if (skeleton != null && equipmentIndex == 9 && num4 > num3)
							{
								GameEntityComponent componentAtIndex = skeleton.GetComponentAtIndex(3, num4 - 1);
								agent.SetCapeClothSimulator(componentAtIndex);
								goto IL_32E;
							}
							goto IL_32E;
							IL_212:
							if (itemObject.IsUsingTeamColor)
							{
								for (int j = 0; j < multiMesh.MeshCount; j++)
								{
									Mesh meshAtIndex = multiMesh.GetMeshAtIndex(j);
									if (!meshAtIndex.HasTag("no_team_color"))
									{
										meshAtIndex.Color = num;
										meshAtIndex.Color2 = num2;
										Material material = meshAtIndex.GetMaterial().CreateCopy();
										material.AddMaterialShaderFlag("use_double_colormap_with_mask_texture", false);
										meshAtIndex.SetMaterial(material);
										flag = true;
									}
									meshAtIndex.ManualInvalidate();
								}
								goto IL_287;
							}
							goto IL_287;
						}
						IL_32E:
						if (equipmentIndex == 6 && !string.IsNullOrEmpty(itemObject.ArmBandMeshName))
						{
							MetaMesh copy = MetaMesh.GetCopy(itemObject.ArmBandMeshName, true, true);
							if (copy != null)
							{
								if (randomizeColors)
								{
									copy.SetGlossMultiplier(AgentVisuals.GetRandomGlossFactor(random));
								}
								if (itemObject.IsUsingTeamColor)
								{
									for (int k = 0; k < copy.MeshCount; k++)
									{
										Mesh meshAtIndex2 = copy.GetMeshAtIndex(k);
										if (!meshAtIndex2.HasTag("no_team_color"))
										{
											meshAtIndex2.Color = num;
											meshAtIndex2.Color2 = num2;
											Material material2 = meshAtIndex2.GetMaterial().CreateCopy();
											material2.AddMaterialShaderFlag("use_double_colormap_with_mask_texture", false);
											meshAtIndex2.SetMaterial(material2);
											flag = true;
										}
										meshAtIndex2.ManualInvalidate();
									}
								}
								agent.AgentVisuals.AddMultiMesh(copy, MBAgentVisuals.GetBodyMeshIndex(equipmentIndex));
								copy.ManualInvalidate();
							}
						}
					}
				}
				ItemObject item = agent.SpawnEquipment[6].Item;
				if (item != null)
				{
					int lodAtlasIndex = item.LodAtlasIndex;
					if (lodAtlasIndex != -1)
					{
						agent.AgentVisuals.SetLodAtlasShadingIndex(lodAtlasIndex, flag, agent.ClothingColor1, agent.ClothingColor2);
					}
				}
				break;
			}
			case 2:
				MountVisualCreator.AddMountMeshToAgentVisual(agent.AgentVisuals, agent.SpawnEquipment[10].Item, agent.SpawnEquipment[11].Item, agent.HorseCreationKey, agent);
				break;
			}
			ArmorComponent.ArmorMaterialTypes armorMaterialTypes = 0;
			ItemObject item2 = agent.SpawnEquipment[6].Item;
			if (item2 != null)
			{
				armorMaterialTypes = item2.ArmorComponent.MaterialType;
			}
			agent.SetBodyArmorMaterialType(armorMaterialTypes);
		}

		// Token: 0x0600024E RID: 590 RVA: 0x00015B44 File Offset: 0x00013D44
		void IMissionListener.OnConversationCharacterChanged()
		{
			this._cameraAddSpecialMovement = true;
		}

		// Token: 0x0600024F RID: 591 RVA: 0x00015B50 File Offset: 0x00013D50
		void IMissionListener.OnMissionModeChange(MissionMode oldMissionMode, bool atStart)
		{
			if (this.Mission.Mode == 1 && oldMissionMode != 1)
			{
				this._cameraAddSpecialMovement = true;
				this._cameraApplySpecialMovementsInstantly = atStart;
			}
			else if (this.Mission.Mode == 2 && oldMissionMode == 6 && this.CombatCamera != null)
			{
				this._cameraAddSpecialMovement = true;
				this._cameraApplySpecialMovementsInstantly = atStart || this._playerDeploymentCancelled;
				Agent agentToFollow = this.GetSpectatingData(this.CombatCamera.Position).AgentToFollow;
				if (!atStart)
				{
					this.LastFollowedAgent = agentToFollow;
				}
				this._cameraSpecialCurrentAddedElevation = this.CameraElevation;
				if (agentToFollow != null)
				{
					this._cameraSpecialCurrentAddedBearing = MBMath.WrapAngle(this.CameraBearing - agentToFollow.LookDirectionAsAngle);
					this._cameraSpecialCurrentPositionToAdd = this.CombatCamera.Position - agentToFollow.VisualPosition;
					this.CameraBearing = agentToFollow.LookDirectionAsAngle;
				}
				else
				{
					this._cameraSpecialCurrentAddedBearing = 0f;
					this._cameraSpecialCurrentPositionToAdd = Vec3.Zero;
					this.CameraBearing = 0f;
				}
				this.CameraElevation = 0f;
			}
			if (((this.Mission.Mode == 1 || this.Mission.Mode == 5) && oldMissionMode != 1 && oldMissionMode != 5) || ((oldMissionMode == 1 || oldMissionMode == 5) && this.Mission.Mode != 1 && this.Mission.Mode != 5))
			{
				this._cameraAddSpecialMovement = true;
				this._cameraAddSpecialPositionalMovement = true;
				this._cameraApplySpecialMovementsInstantly = atStart;
			}
			this._cameraHeightLimit = 0f;
			if (this.Mission.Mode == 6)
			{
				GameEntity gameEntity;
				if (this.Mission.PlayerTeam.Side == 1)
				{
					gameEntity = this.Mission.Scene.FindEntityWithTag("strategyCameraAttacker") ?? this.Mission.Scene.FindEntityWithTag("strategyCameraDefender");
				}
				else
				{
					gameEntity = this.Mission.Scene.FindEntityWithTag("strategyCameraDefender") ?? this.Mission.Scene.FindEntityWithTag("strategyCameraAttacker");
				}
				if (gameEntity != null)
				{
					this._cameraHeightLimit = gameEntity.GetGlobalFrame().origin.z;
					return;
				}
			}
			else
			{
				GameEntity gameEntity2 = this.Mission.Scene.FindEntityWithTag("camera_height_limiter");
				if (gameEntity2 != null)
				{
					this._cameraHeightLimit = gameEntity2.GetGlobalFrame().origin.z;
				}
			}
		}

		// Token: 0x06000250 RID: 592 RVA: 0x00015DA8 File Offset: 0x00013FA8
		void IMissionListener.OnResetMission()
		{
			this._agentToFollowOverride = null;
			this.LastFollowedAgent = null;
			this.LastFollowedAgentVisuals = null;
		}

		// Token: 0x06000251 RID: 593 RVA: 0x00015DC0 File Offset: 0x00013FC0
		void IMissionListener.OnInitialDeploymentPlanMade(BattleSideEnum battleSide, bool isFirstPlan)
		{
			if (!GameNetwork.IsMultiplayer && this.Mission.Mode == 6 && isFirstPlan)
			{
				BattleSideEnum side = this.Mission.PlayerTeam.Side;
				if (side == battleSide)
				{
					DeploymentMissionController missionBehavior = this.Mission.GetMissionBehavior<DeploymentMissionController>();
					bool flag = missionBehavior != null && MissionGameModels.Current.BattleInitializationModel.CanPlayerSideDeployWithOrderOfBattle();
					GameEntity gameEntity;
					if (side == 1)
					{
						gameEntity = this.Mission.Scene.FindEntityWithTag("strategyCameraAttacker") ?? this.Mission.Scene.FindEntityWithTag("strategyCameraDefender");
					}
					else
					{
						gameEntity = this.Mission.Scene.FindEntityWithTag("strategyCameraDefender") ?? this.Mission.Scene.FindEntityWithTag("strategyCameraAttacker");
					}
					if (gameEntity == null && flag)
					{
						MatrixFrame battleSideDeploymentFrame = this.Mission.DeploymentPlan.GetBattleSideDeploymentFrame(side, 0);
						MatrixFrame matrixFrame = battleSideDeploymentFrame;
						float num = Math.Max(0.2f * (float)this.Mission.DeploymentPlan.GetTroopCountForSide(side, 0), 32f);
						matrixFrame.rotation.RotateAboutSide(-0.5235988f);
						matrixFrame.origin -= num * matrixFrame.rotation.f;
						bool flag2 = false;
						if (this.Mission.IsPositionInsideBoundaries(matrixFrame.origin.AsVec2))
						{
							flag2 = true;
						}
						else
						{
							IEnumerable<KeyValuePair<string, ICollection<Vec2>>> enumerable = this.Mission.Boundaries.Where((KeyValuePair<string, ICollection<Vec2>> boundary) => boundary.Key == "walk_area");
							if (!Extensions.IsEmpty<KeyValuePair<string, ICollection<Vec2>>>(enumerable))
							{
								List<Vec2> list = enumerable.First<KeyValuePair<string, ICollection<Vec2>>>().Value as List<Vec2>;
								list = list ?? list.ToList<Vec2>();
								Vec2 vec = matrixFrame.rotation.f.AsVec2.Normalized();
								Vec2 asVec = matrixFrame.origin.AsVec2;
								Vec2 vec2;
								if (MBMath.IntersectRayWithBoundaryList(asVec, vec, list, ref vec2))
								{
									Vec2 asVec2 = battleSideDeploymentFrame.origin.AsVec2;
									float num2 = vec2.Distance(asVec2);
									float num3 = asVec.Distance(asVec2);
									float num4 = num2 / Math.Max(num3, 0.1f) * matrixFrame.origin.z;
									Vec3 vec3;
									vec3..ctor(vec2, num4, -1f);
									matrixFrame.origin = vec3;
									flag2 = true;
								}
							}
						}
						if (!flag2)
						{
							matrixFrame = battleSideDeploymentFrame;
							matrixFrame.origin.z = matrixFrame.origin.z + 20f;
						}
						this.CombatCamera.Frame = matrixFrame;
						this.CameraBearing = matrixFrame.rotation.f.RotationZ;
						this.CameraElevation = matrixFrame.rotation.f.RotationX;
					}
					this._playerDeploymentCancelled = missionBehavior != null && !flag;
				}
			}
			foreach (MissionView missionView in this._missionViews)
			{
				missionView.OnInitialDeploymentPlanMadeForSide(battleSide, isFirstPlan);
			}
		}

		// Token: 0x06000252 RID: 594 RVA: 0x000160D4 File Offset: 0x000142D4
		private static void CalculateNewBearingAndElevation(Agent agentToFollow, float cameraBearing, float cameraElevation, out float newBearing, out float newElevation)
		{
			newBearing = cameraBearing;
			newElevation = cameraElevation;
			AnimFlags currentAnimationFlag = agentToFollow.GetCurrentAnimationFlag(0);
			AnimFlags currentAnimationFlag2 = agentToFollow.GetCurrentAnimationFlag(1);
			if (Extensions.HasAnyFlag<AnimFlags>(currentAnimationFlag, 285212672L) || Extensions.HasAnyFlag<AnimFlags>(currentAnimationFlag2, 285212672L) || agentToFollow.MovementLockedState == 2)
			{
				MatrixFrame boneEntitialFrame = agentToFollow.AgentVisuals.GetBoneEntitialFrame(agentToFollow.Monster.ThoraxLookDirectionBoneIndex, true);
				MatrixFrame frame = agentToFollow.AgentVisuals.GetFrame();
				float rotationZ = boneEntitialFrame.rotation.f.RotationZ;
				float num = rotationZ + frame.rotation.f.RotationZ;
				float num2 = MBMath.ToRadians(66f);
				if (Math.Abs(rotationZ) > num2 * 0.5f - 0.0001f)
				{
					float num3 = Math.Abs(rotationZ) - (num2 * 0.5f - 0.0001f);
					num2 += num3;
					num += num3 * ((rotationZ < 0f) ? 0.5f : (-0.5f));
				}
				if (Math.Abs(rotationZ) > num2 * 0.5f - 0.0001f)
				{
					float num4 = Math.Abs(rotationZ) - (num2 * 0.5f - 0.0001f);
					num2 += num4;
					num += num4 * ((rotationZ < 0f) ? 0.5f : (-0.5f));
				}
				if (num <= -3.1415927f)
				{
					num += 6.2831855f;
				}
				else if (num > 3.1415927f)
				{
					num -= 6.2831855f;
				}
				newBearing = MBMath.ClampAngle(MBMath.WrapAngle(cameraBearing), num, num2);
				float num5 = MBMath.ToRadians(50f);
				newElevation = MBMath.ClampAngle(MBMath.WrapAngle(cameraElevation), frame.rotation.f.RotationX, num5);
			}
		}

		// Token: 0x06000253 RID: 595 RVA: 0x0001627C File Offset: 0x0001447C
		private static void ApplyBannerTextureToMesh(Mesh armorMesh, Texture bannerTexture)
		{
			if (armorMesh != null)
			{
				Material material = armorMesh.GetMaterial().CreateCopy();
				material.SetTexture(1, bannerTexture);
				uint num = (uint)material.GetShader().GetMaterialShaderFlagMask("use_tableau_blending", true);
				ulong shaderFlags = material.GetShaderFlags();
				material.SetShaderFlags(shaderFlags | (ulong)num);
				armorMesh.SetMaterial(material);
			}
		}

		// Token: 0x04000121 RID: 289
		public const int LoadingScreenFramesLeftInitial = 15;

		// Token: 0x04000123 RID: 291
		public Func<BasicCharacterObject> GetSpectatedCharacter;

		// Token: 0x04000126 RID: 294
		private MissionScreen.GatherCustomAgentListToSpectateDelegate _gatherCustomAgentListToSpectate;

		// Token: 0x04000128 RID: 296
		public const float MinCameraAddedDistance = 0.7f;

		// Token: 0x04000129 RID: 297
		public const float MinCameraDistanceHardLimit = 0.48f;

		// Token: 0x0400012A RID: 298
		public const float MaxCameraAddedDistance = 2.4f;

		// Token: 0x0400012B RID: 299
		private const int _cheatTimeSpeedRequestId = 1121;

		// Token: 0x0400012C RID: 300
		private const string AttackerCameraEntityTag = "strategyCameraAttacker";

		// Token: 0x0400012D RID: 301
		private const string DefenderCameraEntityTag = "strategyCameraDefender";

		// Token: 0x0400012E RID: 302
		private const string CameraHeightLimiterTag = "camera_height_limiter";

		// Token: 0x0400012F RID: 303
		private float _cameraRayCastOffset;

		// Token: 0x04000130 RID: 304
		private bool _forceCanZoom;

		// Token: 0x04000131 RID: 305
		private ScreenLayer _emptyUILayer;

		// Token: 0x04000132 RID: 306
		public const float DefaultViewAngle = 65f;

		// Token: 0x04000134 RID: 308
		private Camera _customCamera;

		// Token: 0x04000135 RID: 309
		private Vec3[] _cameraNearPlanePoints = new Vec3[4];

		// Token: 0x04000136 RID: 310
		private Vec3[] _cameraBoxPoints = new Vec3[8];

		// Token: 0x04000137 RID: 311
		private Vec3 _cameraTarget;

		// Token: 0x0400013B RID: 315
		private float _cameraBearingDelta;

		// Token: 0x0400013C RID: 316
		private float _cameraElevationDelta;

		// Token: 0x0400013D RID: 317
		private float _cameraSpecialTargetAddedBearing;

		// Token: 0x0400013E RID: 318
		private float _cameraSpecialCurrentAddedBearing;

		// Token: 0x0400013F RID: 319
		private float _cameraSpecialTargetAddedElevation;

		// Token: 0x04000140 RID: 320
		private float _cameraSpecialCurrentAddedElevation;

		// Token: 0x04000141 RID: 321
		private Vec3 _cameraSpecialTargetPositionToAdd;

		// Token: 0x04000142 RID: 322
		private Vec3 _cameraSpecialCurrentPositionToAdd;

		// Token: 0x04000143 RID: 323
		private float _cameraSpecialTargetDistanceToAdd;

		// Token: 0x04000144 RID: 324
		private float _cameraSpecialCurrentDistanceToAdd;

		// Token: 0x04000145 RID: 325
		private bool _cameraAddSpecialMovement;

		// Token: 0x04000146 RID: 326
		private bool _cameraAddSpecialPositionalMovement;

		// Token: 0x04000147 RID: 327
		private bool _cameraApplySpecialMovementsInstantly;

		// Token: 0x04000148 RID: 328
		private float _cameraSpecialCurrentFOV;

		// Token: 0x04000149 RID: 329
		private float _cameraSpecialTargetFOV;

		// Token: 0x0400014A RID: 330
		private float _cameraTargetAddedHeight;

		// Token: 0x0400014B RID: 331
		private float _cameraDeploymentHeightToAdd;

		// Token: 0x0400014C RID: 332
		private float _lastCameraAddedDistance;

		// Token: 0x0400014E RID: 334
		private float _cameraAddedElevation;

		// Token: 0x0400014F RID: 335
		private float _cameraHeightLimit;

		// Token: 0x04000151 RID: 337
		private Vec3 _cameraSpeed;

		// Token: 0x04000152 RID: 338
		private float _cameraSpeedMultiplier;

		// Token: 0x04000153 RID: 339
		private bool _cameraSmoothMode;

		// Token: 0x04000154 RID: 340
		private bool _fixCamera;

		// Token: 0x04000155 RID: 341
		private int _shiftSpeedMultiplier = 3;

		// Token: 0x04000156 RID: 342
		private bool _tickEditor;

		// Token: 0x04000157 RID: 343
		private bool _playerDeploymentCancelled;

		// Token: 0x0400015C RID: 348
		private const float LookUpLimit = 1.1219975f;

		// Token: 0x0400015D RID: 349
		private const float LookDownLimit = -1.3659099f;

		// Token: 0x0400015E RID: 350
		public const float FirstPersonNearClippingDistance = 0.065f;

		// Token: 0x0400015F RID: 351
		public const float ThirdPersonNearClippingDistance = 0.1f;

		// Token: 0x04000160 RID: 352
		public const float FarClippingDistance = 12500f;

		// Token: 0x04000161 RID: 353
		private const float HoldTimeForCameraToggle = 0.5f;

		// Token: 0x04000162 RID: 354
		private bool _zoomToggled;

		// Token: 0x04000163 RID: 355
		private float _zoomToggleTime = float.MaxValue;

		// Token: 0x04000164 RID: 356
		private float _zoomAmount;

		// Token: 0x04000165 RID: 357
		private float _cameraToggleStartTime = float.MaxValue;

		// Token: 0x04000167 RID: 359
		private bool _displayingDialog;

		// Token: 0x04000168 RID: 360
		private MissionMainAgentController _missionMainAgentController;

		// Token: 0x04000169 RID: 361
		private ICameraModeLogic _missionCameraModeLogic;

		// Token: 0x0400016A RID: 362
		private MissionLobbyComponent _missionLobbyComponent;

		// Token: 0x0400016C RID: 364
		private bool _isPlayerAgentAdded = true;

		// Token: 0x0400016D RID: 365
		private bool _isRenderingStarted;

		// Token: 0x0400016E RID: 366
		private bool _onSceneRenderingStartedCalled;

		// Token: 0x0400016F RID: 367
		private int _loadingScreenFramesLeft = 15;

		// Token: 0x04000170 RID: 368
		private bool _resetDraggingMode;

		// Token: 0x04000171 RID: 369
		private bool _rightButtonDraggingMode;

		// Token: 0x04000172 RID: 370
		private Vec2 _clickedPositionPixel = Vec2.Zero;

		// Token: 0x04000173 RID: 371
		private Agent _agentToFollowOverride;

		// Token: 0x04000174 RID: 372
		private Agent _lastFollowedAgent;

		// Token: 0x04000176 RID: 374
		private MissionMultiplayerGameModeBaseClient _mpGameModeBase;

		// Token: 0x04000177 RID: 375
		private bool _isGamepadActive;

		// Token: 0x04000178 RID: 376
		private List<MissionView> _missionViews;

		// Token: 0x0400017A RID: 378
		private MissionState _missionState;

		// Token: 0x0400017C RID: 380
		private bool _isDeactivated;

		// Token: 0x020000A5 RID: 165
		// (Invoke) Token: 0x06000521 RID: 1313
		public delegate void OnSpectateAgentDelegate(Agent followedAgent);

		// Token: 0x020000A6 RID: 166
		// (Invoke) Token: 0x06000525 RID: 1317
		public delegate List<Agent> GatherCustomAgentListToSpectateDelegate(Agent forcedAgentToInclude);
	}
}
