using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.ObjectSystem;
using TaleWorlds.PlatformService;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020001B4 RID: 436
	public abstract class MBGameManager : GameManagerBase
	{
		// Token: 0x17000504 RID: 1284
		// (get) Token: 0x06001946 RID: 6470 RVA: 0x0005AFED File Offset: 0x000591ED
		// (set) Token: 0x06001947 RID: 6471 RVA: 0x0005AFF5 File Offset: 0x000591F5
		public bool IsEnding { get; private set; }

		// Token: 0x17000505 RID: 1285
		// (get) Token: 0x06001948 RID: 6472 RVA: 0x0005AFFE File Offset: 0x000591FE
		public new static MBGameManager Current
		{
			get
			{
				return (MBGameManager)GameManagerBase.Current;
			}
		}

		// Token: 0x17000506 RID: 1286
		// (get) Token: 0x06001949 RID: 6473 RVA: 0x0005B00A File Offset: 0x0005920A
		// (set) Token: 0x0600194A RID: 6474 RVA: 0x0005B012 File Offset: 0x00059212
		public bool IsLoaded { get; protected set; }

		// Token: 0x0600194B RID: 6475 RVA: 0x0005B01B File Offset: 0x0005921B
		protected MBGameManager()
		{
			this.IsEnding = false;
			NativeConfig.OnConfigChanged();
		}

		// Token: 0x0600194C RID: 6476 RVA: 0x0005B03A File Offset: 0x0005923A
		protected static void StartNewGame()
		{
			MBAPI.IMBGame.StartNew();
		}

		// Token: 0x0600194D RID: 6477 RVA: 0x0005B046 File Offset: 0x00059246
		protected static void LoadModuleData(bool isLoadGame)
		{
			MBAPI.IMBGame.LoadModuleData(isLoadGame);
		}

		// Token: 0x0600194E RID: 6478 RVA: 0x0005B054 File Offset: 0x00059254
		public static void StartNewGame(MBGameManager gameLoader)
		{
			GameLoadingState gameLoadingState = GameStateManager.Current.CreateState<GameLoadingState>();
			gameLoadingState.SetLoadingParameters(gameLoader);
			GameStateManager.Current.CleanAndPushState(gameLoadingState, 0);
		}

		// Token: 0x0600194F RID: 6479 RVA: 0x0005B080 File Offset: 0x00059280
		public override void BeginGameStart(Game game)
		{
			foreach (MBSubModuleBase mbsubModuleBase in Module.CurrentModule.SubModules)
			{
				mbsubModuleBase.BeginGameStart(game);
			}
		}

		// Token: 0x06001950 RID: 6480 RVA: 0x0005B0D8 File Offset: 0x000592D8
		public override void OnNewCampaignStart(Game game, object starterObject)
		{
			foreach (MBSubModuleBase mbsubModuleBase in Module.CurrentModule.SubModules)
			{
				mbsubModuleBase.OnCampaignStart(game, starterObject);
			}
		}

		// Token: 0x06001951 RID: 6481 RVA: 0x0005B130 File Offset: 0x00059330
		public override void RegisterSubModuleObjects(bool isSavedCampaign)
		{
			foreach (MBSubModuleBase mbsubModuleBase in Module.CurrentModule.SubModules)
			{
				mbsubModuleBase.RegisterSubModuleObjects(isSavedCampaign);
			}
		}

		// Token: 0x06001952 RID: 6482 RVA: 0x0005B188 File Offset: 0x00059388
		public override void AfterRegisterSubModuleObjects(bool isSavedCampaign)
		{
			foreach (MBSubModuleBase mbsubModuleBase in Module.CurrentModule.SubModules)
			{
				mbsubModuleBase.AfterRegisterSubModuleObjects(isSavedCampaign);
			}
		}

		// Token: 0x06001953 RID: 6483 RVA: 0x0005B1E0 File Offset: 0x000593E0
		public override void InitializeGameStarter(Game game, IGameStarter starterObject)
		{
			foreach (MBSubModuleBase mbsubModuleBase in Module.CurrentModule.SubModules)
			{
				mbsubModuleBase.InitializeGameStarter(game, starterObject);
			}
		}

		// Token: 0x06001954 RID: 6484 RVA: 0x0005B238 File Offset: 0x00059438
		public override void OnGameInitializationFinished(Game game)
		{
			foreach (MBSubModuleBase mbsubModuleBase in Module.CurrentModule.SubModules)
			{
				mbsubModuleBase.OnGameInitializationFinished(game);
			}
			foreach (SkeletonScale skeletonScale in Game.Current.ObjectManager.GetObjectTypeList<SkeletonScale>())
			{
				sbyte[] array = new sbyte[skeletonScale.BoneNames.Count];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = Skeleton.GetBoneIndexFromName(skeletonScale.SkeletonModel, skeletonScale.BoneNames[i]);
				}
				skeletonScale.SetBoneIndices(array);
			}
		}

		// Token: 0x06001955 RID: 6485 RVA: 0x0005B318 File Offset: 0x00059518
		public override void OnAfterGameInitializationFinished(Game game, object initializerObject)
		{
			foreach (MBSubModuleBase mbsubModuleBase in Module.CurrentModule.SubModules)
			{
				mbsubModuleBase.OnAfterGameInitializationFinished(game, initializerObject);
			}
		}

		// Token: 0x06001956 RID: 6486 RVA: 0x0005B370 File Offset: 0x00059570
		public override void OnGameLoaded(Game game, object initializerObject)
		{
			NetworkMain.Initialize();
			foreach (MBSubModuleBase mbsubModuleBase in Module.CurrentModule.SubModules)
			{
				mbsubModuleBase.OnGameLoaded(game, initializerObject);
			}
		}

		// Token: 0x06001957 RID: 6487 RVA: 0x0005B3CC File Offset: 0x000595CC
		public override void OnNewGameCreated(Game game, object initializerObject)
		{
			foreach (MBSubModuleBase mbsubModuleBase in Module.CurrentModule.SubModules)
			{
				mbsubModuleBase.OnNewGameCreated(game, initializerObject);
			}
		}

		// Token: 0x06001958 RID: 6488 RVA: 0x0005B424 File Offset: 0x00059624
		public override void OnGameStart(Game game, IGameStarter gameStarter)
		{
			Game.Current.MonsterMissionDataCreator = new MonsterMissionDataCreator();
			foreach (MBSubModuleBase mbsubModuleBase in Module.CurrentModule.SubModules)
			{
				mbsubModuleBase.OnGameStart(game, gameStarter);
			}
			Game.Current.AddGameModelsManager<MissionGameModels>(gameStarter.Models);
			Monster.GetBoneIndexWithId = new Func<string, string, sbyte>(MBActionSet.GetBoneIndexWithId);
			Monster.GetBoneHasParentBone = new Func<string, sbyte, bool>(MBActionSet.GetBoneHasParentBone);
		}

		// Token: 0x06001959 RID: 6489 RVA: 0x0005B4BC File Offset: 0x000596BC
		public override void OnGameEnd(Game game)
		{
			foreach (MBSubModuleBase mbsubModuleBase in Module.CurrentModule.SubModules)
			{
				mbsubModuleBase.OnGameEnd(game);
			}
			MissionGameModels.Clear();
			base.OnGameEnd(game);
		}

		// Token: 0x0600195A RID: 6490 RVA: 0x0005B520 File Offset: 0x00059720
		public static async void EndGame()
		{
			for (;;)
			{
				MBGameManager mbgameManager = MBGameManager.Current;
				if (mbgameManager == null || mbgameManager.IsLoaded)
				{
					break;
				}
				await Task.Delay(100);
			}
			MBGameManager mbgameManager2 = MBGameManager.Current;
			if (mbgameManager2 == null || mbgameManager2.CheckAndSetEnding())
			{
				if (Game.Current.GameStateManager != null)
				{
					while (Mission.Current != null && !(Game.Current.GameStateManager.ActiveState is MissionState))
					{
						Game.Current.GameStateManager.PopState(0);
					}
					if (Game.Current.GameStateManager.ActiveState is MissionState)
					{
						((MissionState)Game.Current.GameStateManager.ActiveState).CurrentMission.EndMission();
						while (Mission.Current != null)
						{
							await Task.Delay(1);
						}
					}
					else
					{
						Game.Current.GameStateManager.CleanStates(0);
					}
				}
			}
		}

		// Token: 0x0600195B RID: 6491 RVA: 0x0005B551 File Offset: 0x00059751
		public override void OnAfterCampaignStart(Game game)
		{
			NetworkMain.Initialize();
		}

		// Token: 0x0600195C RID: 6492 RVA: 0x0005B558 File Offset: 0x00059758
		public override void OnLoadFinished()
		{
			this.IsLoaded = true;
		}

		// Token: 0x0600195D RID: 6493 RVA: 0x0005B564 File Offset: 0x00059764
		public bool CheckAndSetEnding()
		{
			object lockObject = this._lockObject;
			bool flag2;
			lock (lockObject)
			{
				if (this.IsEnding)
				{
					flag2 = false;
				}
				else
				{
					this.IsEnding = true;
					flag2 = true;
				}
			}
			return flag2;
		}

		// Token: 0x0600195E RID: 6494 RVA: 0x0005B5B4 File Offset: 0x000597B4
		public virtual void OnSessionInvitationAccepted(SessionInvitationType targetGameType)
		{
			if (targetGameType != SessionInvitationType.None)
			{
				MBGameManager.EndGame();
			}
		}

		// Token: 0x0600195F RID: 6495 RVA: 0x0005B5BE File Offset: 0x000597BE
		public virtual void OnPlatformRequestedMultiplayer()
		{
			MBGameManager.EndGame();
		}

		// Token: 0x06001960 RID: 6496 RVA: 0x0005B5C5 File Offset: 0x000597C5
		protected List<MbObjectXmlInformation> GetXmlInformationFromModule()
		{
			return XmlResource.XmlInformationList;
		}

		// Token: 0x17000507 RID: 1287
		// (get) Token: 0x06001961 RID: 6497 RVA: 0x0005B5CC File Offset: 0x000597CC
		public override float ApplicationTime
		{
			get
			{
				return MBCommon.GetApplicationTime();
			}
		}

		// Token: 0x17000508 RID: 1288
		// (get) Token: 0x06001962 RID: 6498 RVA: 0x0005B5D3 File Offset: 0x000597D3
		public override bool CheatMode
		{
			get
			{
				return NativeConfig.CheatMode;
			}
		}

		// Token: 0x17000509 RID: 1289
		// (get) Token: 0x06001963 RID: 6499 RVA: 0x0005B5DA File Offset: 0x000597DA
		public override bool IsDevelopmentMode
		{
			get
			{
				return NativeConfig.IsDevelopmentMode;
			}
		}

		// Token: 0x1700050A RID: 1290
		// (get) Token: 0x06001964 RID: 6500 RVA: 0x0005B5E1 File Offset: 0x000597E1
		public override bool IsEditModeOn
		{
			get
			{
				return MBEditor.IsEditModeOn;
			}
		}

		// Token: 0x1700050B RID: 1291
		// (get) Token: 0x06001965 RID: 6501 RVA: 0x0005B5E8 File Offset: 0x000597E8
		public override UnitSpawnPrioritizations UnitSpawnPrioritization
		{
			get
			{
				return (UnitSpawnPrioritizations)BannerlordConfig.UnitSpawnPrioritization;
			}
		}

		// Token: 0x040007BF RID: 1983
		private readonly object _lockObject = new object();
	}
}
