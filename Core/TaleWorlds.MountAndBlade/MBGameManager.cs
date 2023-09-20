using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.ObjectSystem;
using TaleWorlds.PlatformService;

namespace TaleWorlds.MountAndBlade
{
	public abstract class MBGameManager : GameManagerBase
	{
		public bool IsEnding { get; private set; }

		public new static MBGameManager Current
		{
			get
			{
				return (MBGameManager)GameManagerBase.Current;
			}
		}

		public bool IsLoaded { get; protected set; }

		protected MBGameManager()
		{
			this.IsEnding = false;
			NativeConfig.OnConfigChanged();
		}

		protected static void StartNewGame()
		{
			MBAPI.IMBGame.StartNew();
		}

		protected static void LoadModuleData(bool isLoadGame)
		{
			MBAPI.IMBGame.LoadModuleData(isLoadGame);
		}

		public static void StartNewGame(MBGameManager gameLoader)
		{
			GameLoadingState gameLoadingState = GameStateManager.Current.CreateState<GameLoadingState>();
			gameLoadingState.SetLoadingParameters(gameLoader);
			GameStateManager.Current.CleanAndPushState(gameLoadingState, 0);
		}

		public override void BeginGameStart(Game game)
		{
			foreach (MBSubModuleBase mbsubModuleBase in Module.CurrentModule.SubModules)
			{
				mbsubModuleBase.BeginGameStart(game);
			}
		}

		public override void OnNewCampaignStart(Game game, object starterObject)
		{
			foreach (MBSubModuleBase mbsubModuleBase in Module.CurrentModule.SubModules)
			{
				mbsubModuleBase.OnCampaignStart(game, starterObject);
			}
		}

		public override void RegisterSubModuleObjects(bool isSavedCampaign)
		{
			foreach (MBSubModuleBase mbsubModuleBase in Module.CurrentModule.SubModules)
			{
				mbsubModuleBase.RegisterSubModuleObjects(isSavedCampaign);
			}
		}

		public override void AfterRegisterSubModuleObjects(bool isSavedCampaign)
		{
			foreach (MBSubModuleBase mbsubModuleBase in Module.CurrentModule.SubModules)
			{
				mbsubModuleBase.AfterRegisterSubModuleObjects(isSavedCampaign);
			}
		}

		public override void InitializeGameStarter(Game game, IGameStarter starterObject)
		{
			foreach (MBSubModuleBase mbsubModuleBase in Module.CurrentModule.SubModules)
			{
				mbsubModuleBase.InitializeGameStarter(game, starterObject);
			}
		}

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

		public override void OnAfterGameInitializationFinished(Game game, object initializerObject)
		{
			foreach (MBSubModuleBase mbsubModuleBase in Module.CurrentModule.SubModules)
			{
				mbsubModuleBase.OnAfterGameInitializationFinished(game, initializerObject);
			}
		}

		public override void OnGameLoaded(Game game, object initializerObject)
		{
			NetworkMain.Initialize();
			foreach (MBSubModuleBase mbsubModuleBase in Module.CurrentModule.SubModules)
			{
				mbsubModuleBase.OnGameLoaded(game, initializerObject);
			}
		}

		public override void OnNewGameCreated(Game game, object initializerObject)
		{
			foreach (MBSubModuleBase mbsubModuleBase in Module.CurrentModule.SubModules)
			{
				mbsubModuleBase.OnNewGameCreated(game, initializerObject);
			}
		}

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

		public override void OnGameEnd(Game game)
		{
			foreach (MBSubModuleBase mbsubModuleBase in Module.CurrentModule.SubModules)
			{
				mbsubModuleBase.OnGameEnd(game);
			}
			MissionGameModels.Clear();
			base.OnGameEnd(game);
		}

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

		public override void OnAfterCampaignStart(Game game)
		{
			NetworkMain.Initialize();
		}

		public override void OnLoadFinished()
		{
			this.IsLoaded = true;
		}

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

		public virtual void OnSessionInvitationAccepted(SessionInvitationType targetGameType)
		{
			if (targetGameType != SessionInvitationType.None)
			{
				MBGameManager.EndGame();
			}
		}

		public virtual void OnPlatformRequestedMultiplayer()
		{
			MBGameManager.EndGame();
		}

		protected List<MbObjectXmlInformation> GetXmlInformationFromModule()
		{
			return XmlResource.XmlInformationList;
		}

		public override float ApplicationTime
		{
			get
			{
				return MBCommon.GetApplicationTime();
			}
		}

		public override bool CheatMode
		{
			get
			{
				return NativeConfig.CheatMode;
			}
		}

		public override bool IsDevelopmentMode
		{
			get
			{
				return NativeConfig.IsDevelopmentMode;
			}
		}

		public override bool IsEditModeOn
		{
			get
			{
				return MBEditor.IsEditModeOn;
			}
		}

		public override UnitSpawnPrioritizations UnitSpawnPrioritization
		{
			get
			{
				return (UnitSpawnPrioritizations)BannerlordConfig.UnitSpawnPrioritization;
			}
		}

		private readonly object _lockObject = new object();
	}
}
