using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using TaleWorlds.AchievementSystem;
using TaleWorlds.ActivitySystem;
using TaleWorlds.Avatar.PlayerServices;
using TaleWorlds.Core;
using TaleWorlds.Diamond.ClientApplication;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Engine.Options;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ModuleManager;
using TaleWorlds.ObjectSystem;
using TaleWorlds.PlatformService;
using TaleWorlds.SaveSystem;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.MountAndBlade
{
	public sealed class Module : DotNetObject, IGameStateManagerOwner
	{
		public GameTextManager GlobalTextManager { get; private set; }

		public JobManager JobManager { get; private set; }

		public MBReadOnlyList<MBSubModuleBase> SubModules
		{
			get
			{
				return this._submodules;
			}
		}

		public GameStateManager GlobalGameStateManager { get; private set; }

		public bool ReturnToEditorState { get; private set; }

		public bool LoadingFinished { get; private set; }

		public bool IsOnlyCoreContentEnabled { get; private set; }

		public GameStartupInfo StartupInfo { get; private set; }

		private Module()
		{
			MBDebug.Print("Creating module...", 0, Debug.DebugColor.White, 17592186044416UL);
			this.StartupInfo = new GameStartupInfo();
			this._testContext = new TestContext();
			this._loadedSubmoduleTypes = new Dictionary<string, Type>();
			this._submodules = new MBList<MBSubModuleBase>();
			this.GlobalGameStateManager = new GameStateManager(this, GameStateManager.GameStateManagerType.Global);
			GameStateManager.Current = this.GlobalGameStateManager;
			this.GlobalTextManager = new GameTextManager();
			this.JobManager = new JobManager();
		}

		public static Module CurrentModule { get; private set; }

		internal static void CreateModule()
		{
			Module.CurrentModule = new Module();
			Utilities.SetLoadingScreenPercentage(0.4f);
		}

		private void AddSubModule(Assembly subModuleAssembly, string name)
		{
			Type type = subModuleAssembly.GetType(name);
			this._loadedSubmoduleTypes.Add(name, type);
			Managed.AddTypes(this.CollectModuleAssemblyTypes(subModuleAssembly));
		}

		private Dictionary<string, Type> CollectModuleAssemblyTypes(Assembly moduleAssembly)
		{
			Dictionary<string, Type> dictionary2;
			try
			{
				Dictionary<string, Type> dictionary = new Dictionary<string, Type>();
				foreach (Type type in moduleAssembly.GetTypes())
				{
					if (typeof(ManagedObject).IsAssignableFrom(type) || typeof(DotNetObject).IsAssignableFrom(type))
					{
						dictionary.Add(type.Name, type);
					}
				}
				dictionary2 = dictionary;
			}
			catch (Exception ex)
			{
				MBDebug.Print("Error while getting types and loading" + ex.Message, 0, Debug.DebugColor.White, 17592186044416UL);
				ReflectionTypeLoadException ex2;
				if ((ex2 = ex as ReflectionTypeLoadException) != null)
				{
					string text = "";
					foreach (Exception ex3 in ex2.LoaderExceptions)
					{
						MBDebug.Print("Loader Exceptions: " + ex3.Message, 0, Debug.DebugColor.White, 17592186044416UL);
						text = text + ex3.Message + Environment.NewLine;
					}
					Debug.SetCrashReportCustomString(text);
					foreach (Type type2 in ex2.Types)
					{
						if (type2 != null)
						{
							MBDebug.Print("Loaded Types: " + type2.FullName, 0, Debug.DebugColor.White, 17592186044416UL);
						}
					}
				}
				if (ex.InnerException != null)
				{
					MBDebug.Print("Inner excetion: " + ex.StackTrace, 0, Debug.DebugColor.White, 17592186044416UL);
				}
				throw;
			}
			return dictionary2;
		}

		private void InitializeSubModules()
		{
			Managed.AddConstructorDelegateOfClass<SpawnedItemEntity>();
			foreach (Type type in this._loadedSubmoduleTypes.Values)
			{
				MBSubModuleBase mbsubModuleBase = (MBSubModuleBase)type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance, null, new Type[0], null).Invoke(new object[0]);
				this._submodules.Add(mbsubModuleBase);
				mbsubModuleBase.OnSubModuleLoad();
			}
		}

		private void FinalizeSubModules()
		{
			foreach (MBSubModuleBase mbsubModuleBase in this._submodules)
			{
				mbsubModuleBase.OnSubModuleUnloaded();
			}
		}

		public Type GetSubModule(string name)
		{
			return this._loadedSubmoduleTypes[name];
		}

		[MBCallback]
		internal void Initialize()
		{
			MBDebug.Print("Module Initialize begin...", 0, Debug.DebugColor.White, 17592186044416UL);
			TWParallel.InitializeAndSetImplementation(new NativeParallelDriver());
			MBSaveLoad.SetSaveDriver(new AsyncFileSaveDriver());
			this.ProcessApplicationArguments();
			this.SetWindowTitle();
			this._initialStateOptions = new List<InitialStateOption>();
			this.FillMultiplayerGameTypes();
			if (!GameNetwork.IsDedicatedServer && !MBDebug.TestModeEnabled)
			{
				MBDebug.Print("Loading platform services...", 0, Debug.DebugColor.White, 17592186044416UL);
				this.LoadPlatformServices();
			}
			string[] modulesNames = Utilities.GetModulesNames();
			List<string> list = new List<string>();
			string[] array = modulesNames;
			for (int i = 0; i < array.Length; i++)
			{
				ModuleInfo moduleInfo = ModuleHelper.GetModuleInfo(array[i]);
				if (moduleInfo != null)
				{
					list.Add(moduleInfo.FolderPath);
				}
			}
			LocalizedTextManager.LoadLocalizationXmls(list.ToArray());
			this.GlobalTextManager.LoadDefaultTexts();
			this.IsOnlyCoreContentEnabled = Utilities.IsOnlyCoreContentEnabled();
			NativeConfig.OnConfigChanged();
			this.LoadSubModules();
			MBDebug.Print("Adding trace listener...", 0, Debug.DebugColor.White, 17592186044416UL);
			MBDebug.Print("MBModuleBase Initialize begin...", 0, Debug.DebugColor.White, 17592186044416UL);
			MBDebug.Print("MBModuleBase Initialize end...", 0, Debug.DebugColor.White, 17592186044416UL);
			GameNetwork.FindGameNetworkMessages();
			HasTableauCache.CollectTableauCacheTypes();
			MBDebug.Print("Module Initialize end...", 0, Debug.DebugColor.White, 17592186044416UL);
			MBDebug.TestModeEnabled = Utilities.CommandLineArgumentExists("/runTest");
			this.FindMissions();
			NativeOptions.ReadRGLConfigFiles();
			BannerlordConfig.Initialize();
			EngineController.ConfigChange += this.OnConfigChanged;
			EngineController.OnConstrainedStateChanged += this.OnConstrainedStateChange;
			ScreenManager.FocusGained += this.OnFocusGained;
			SaveManager.InitializeGlobalDefinitionContext();
			this.EnsureAsyncJobsAreFinished();
		}

		private void SetWindowTitle()
		{
			string applicationName = Utilities.GetApplicationName();
			string text;
			if (this.StartupInfo.StartupType == GameStartupType.Singleplayer)
			{
				text = applicationName + " - Singleplayer";
			}
			else if (this.StartupInfo.StartupType == GameStartupType.Multiplayer)
			{
				text = applicationName + " - Multiplayer";
			}
			else if (this.StartupInfo.StartupType == GameStartupType.GameServer)
			{
				text = string.Concat(new object[]
				{
					"[",
					Utilities.GetCurrentProcessID(),
					"] ",
					applicationName,
					" Dedicated Server Port:",
					this.StartupInfo.ServerPort
				});
			}
			else
			{
				text = applicationName;
			}
			text = Utilities.ProcessWindowTitle(text);
			Utilities.SetWindowTitle(text);
		}

		private void EnsureAsyncJobsAreFinished()
		{
			if (!GameNetwork.IsDedicatedServer)
			{
				while (!MBMusicManager.IsCreationCompleted())
				{
					Thread.Sleep(1);
				}
			}
			if (!GameNetwork.IsDedicatedServer && !MBDebug.TestModeEnabled)
			{
				while (!AchievementManager.AchievementService.IsInitializationCompleted())
				{
					Thread.Sleep(1);
				}
			}
		}

		private void ProcessApplicationArguments()
		{
			this.StartupInfo.StartupType = GameStartupType.None;
			string[] array = Utilities.GetFullCommandLineString().Split(new char[] { ' ' });
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i].ToLowerInvariant();
				if (text == "/dedicatedmatchmakingserver".ToLower())
				{
					int num = Convert.ToInt32(array[i + 1]);
					string text2 = array[i + 2];
					sbyte b = Convert.ToSByte(array[i + 3]);
					string text3 = array[i + 4];
					i += 4;
					this.StartupInfo.StartupType = GameStartupType.GameServer;
					this.StartupInfo.DedicatedServerType = DedicatedServerType.Matchmaker;
					this.StartupInfo.ServerPort = num;
					this.StartupInfo.ServerRegion = text2;
					this.StartupInfo.ServerPriority = b;
					this.StartupInfo.ServerGameMode = text3;
				}
				else if (text == "/dedicatedcustomserver".ToLower())
				{
					int num2 = Convert.ToInt32(array[i + 1]);
					string text4 = array[i + 2];
					int num3 = Convert.ToInt32(array[i + 3]);
					i += 3;
					this.StartupInfo.StartupType = GameStartupType.GameServer;
					this.StartupInfo.DedicatedServerType = DedicatedServerType.Custom;
					this.StartupInfo.ServerPort = num2;
					this.StartupInfo.ServerRegion = text4;
					this.StartupInfo.Permission = num3;
				}
				else if (text == "/dedicatedcustomserverconfigfile".ToLower())
				{
					string text5 = array[i + 1];
					i++;
					this.StartupInfo.CustomGameServerConfigFile = text5;
				}
				else if (text == "/dedicatedcustomserverauthtoken".ToLower())
				{
					string text6 = array[i + 1];
					i++;
					this.StartupInfo.CustomGameServerAuthToken = text6;
				}
				else if (text == "/dedicatedcustomserverDontAllowOptionalModules".ToLower())
				{
					this.StartupInfo.CustomGameServerAllowsOptionalModules = false;
				}
				else if (text == "/playerHostedDedicatedServer".ToLower())
				{
					this.StartupInfo.PlayerHostedDedicatedServer = true;
				}
				else if (text == "/singleplatform")
				{
					this.StartupInfo.IsSinglePlatformServer = true;
				}
				else if (text == "/customserverhost")
				{
					string text7 = array[i + 1];
					i++;
					this.StartupInfo.CustomServerHostIP = text7;
				}
				else if (text == "/singleplayer".ToLower())
				{
					this.StartupInfo.StartupType = GameStartupType.Singleplayer;
				}
				else if (text == "/multiplayer".ToLower())
				{
					this.StartupInfo.StartupType = GameStartupType.Multiplayer;
				}
				else if (text == "/clientConfigurationCategory".ToLower())
				{
					ClientApplicationConfiguration.SetDefualtConfigurationCategory(array[i + 1]);
					i++;
				}
				else if (text == "/overridenusername".ToLower())
				{
					string text8 = array[i + 1];
					this.StartupInfo.OverridenUserName = text8;
					i++;
				}
				else if (text.StartsWith("-AUTH_PASSWORD".ToLowerInvariant()))
				{
					this.StartupInfo.EpicExchangeCode = text.Split(new char[] { '=' })[1];
				}
				else if (text == "/continuegame".ToLower())
				{
					this.StartupInfo.IsContinueGame = true;
				}
			}
		}

		internal void OnApplicationTick(float dt)
		{
			bool isOnlyCoreContentEnabled = this.IsOnlyCoreContentEnabled;
			this.IsOnlyCoreContentEnabled = Utilities.IsOnlyCoreContentEnabled();
			if (isOnlyCoreContentEnabled != this.IsOnlyCoreContentEnabled && isOnlyCoreContentEnabled)
			{
				InitialState initialState;
				if ((initialState = GameStateManager.Current.ActiveState as InitialState) != null)
				{
					Utilities.DisableCoreGame();
					InformationManager.ShowInquiry(new InquiryData(new TextObject("{=CaSafuAH}Content Download Complete", null).ToString(), new TextObject("{=1nKa4pQX}Rest of the game content has been downloaded.", null).ToString(), true, false, new TextObject("{=yS7PvrTD}OK", null).ToString(), null, delegate
					{
						initialState.RefreshContentState();
					}, null, "", 0f, null, null, null), false, false);
				}
				else
				{
					InformationManager.ShowInquiry(new InquiryData(new TextObject("{=CaSafuAH}Content Download Complete", null).ToString(), new TextObject("{=BFhMw4bl}Rest of the game content has been downloaded. Do you want to return to the main menu?", null).ToString(), true, true, new TextObject("{=aeouhelq}Yes", null).ToString(), new TextObject("{=8OkPHu4f}No", null).ToString(), new Action(this.OnConfirmReturnToMainMenu), null, "", 0f, null, null, null), false, false);
					this._enableCoreContentOnReturnToRoot = true;
				}
			}
			if (this._synchronizationContext == null)
			{
				this._synchronizationContext = new SingleThreadedSynchronizationContext();
				SynchronizationContext.SetSynchronizationContext(this._synchronizationContext);
			}
			this._testContext.OnApplicationTick(dt);
			if (!GameNetwork.MultiplayerDisabled)
			{
				this.OnNetworkTick(dt);
			}
			if (GameStateManager.Current == null)
			{
				GameStateManager.Current = this.GlobalGameStateManager;
			}
			if (GameStateManager.Current == this.GlobalGameStateManager)
			{
				if (this.LoadingFinished && this.GlobalGameStateManager.ActiveState == null)
				{
					if (this.ReturnToEditorState)
					{
						this.ReturnToEditorState = false;
						this.SetEditorScreenAsRootScreen();
					}
					else
					{
						this.SetInitialModuleScreenAsRootScreen();
					}
				}
				this.GlobalGameStateManager.OnTick(dt);
			}
			Utilities.RunJobs();
			IPlatformServices instance = PlatformServices.Instance;
			if (instance != null)
			{
				instance.Tick(dt);
			}
			this._synchronizationContext.Tick();
			if (GameManagerBase.Current != null)
			{
				GameManagerBase.Current.OnTick(dt);
			}
			foreach (MBSubModuleBase mbsubModuleBase in this.SubModules)
			{
				mbsubModuleBase.OnApplicationTick(dt);
			}
			this.JobManager.OnTick(dt);
			AvatarServices.UpdateAvatarServices(dt);
		}

		private void OnConfirmReturnToMainMenu()
		{
			MBGameManager.EndGame();
		}

		private void OnNetworkTick(float dt)
		{
			NetworkMain.Tick(dt);
		}

		[MBCallback]
		internal void RunTest(string commandLine)
		{
			MBDebug.Print(" TEST MODE ENABLED. Command line string: " + commandLine, 0, Debug.DebugColor.White, 17592186044416UL);
			this._testContext.RunTestAux(commandLine);
		}

		[MBCallback]
		internal void TickTest(float dt)
		{
			this._testContext.TickTest(dt);
		}

		[MBCallback]
		internal void OnDumpCreated()
		{
			if (TestCommonBase.BaseInstance != null)
			{
				TestCommonBase.BaseInstance.ToggleTimeoutTimer();
				TestCommonBase.BaseInstance.StartTimeoutTimer();
			}
		}

		[MBCallback]
		internal void OnDumpCreationStarted()
		{
			if (TestCommonBase.BaseInstance != null)
			{
				TestCommonBase.BaseInstance.ToggleTimeoutTimer();
			}
		}

		public static void GetMetaMeshPackageMapping(Dictionary<string, string> metaMeshPackageMappings)
		{
			foreach (ItemObject itemObject in Game.Current.ObjectManager.GetObjectTypeList<ItemObject>())
			{
				if (itemObject.HasArmorComponent)
				{
					string text = ((itemObject.Culture != null) ? itemObject.Culture.StringId : "shared") + "_armor";
					metaMeshPackageMappings[itemObject.MultiMeshName] = text;
					metaMeshPackageMappings[itemObject.MultiMeshName + "_converted"] = text;
					metaMeshPackageMappings[itemObject.MultiMeshName + "_converted_slim"] = text;
					metaMeshPackageMappings[itemObject.MultiMeshName + "_slim"] = text;
				}
				if (itemObject.WeaponComponent != null)
				{
					string text2 = ((itemObject.Culture != null) ? itemObject.Culture.StringId : "shared") + "_weapon";
					metaMeshPackageMappings[itemObject.MultiMeshName] = text2;
					if (itemObject.HolsterMeshName != null)
					{
						metaMeshPackageMappings[itemObject.HolsterMeshName] = text2;
					}
					if (itemObject.HolsterWithWeaponMeshName != null)
					{
						metaMeshPackageMappings[itemObject.HolsterWithWeaponMeshName] = text2;
					}
				}
				if (itemObject.HasHorseComponent)
				{
					string text3 = "horses";
					metaMeshPackageMappings[itemObject.MultiMeshName] = text3;
				}
				if (itemObject.IsFood)
				{
					string text4 = "food";
					metaMeshPackageMappings[itemObject.MultiMeshName] = text4;
				}
			}
			foreach (CraftingPiece craftingPiece in Game.Current.ObjectManager.GetObjectTypeList<CraftingPiece>())
			{
				string text5 = ((craftingPiece.Culture != null) ? craftingPiece.Culture.StringId : "shared") + "_crafting";
				metaMeshPackageMappings[craftingPiece.MeshName] = text5;
			}
		}

		public static void GetItemMeshNames(HashSet<string> itemMeshNames)
		{
			foreach (ItemObject itemObject in Game.Current.ObjectManager.GetObjectTypeList<ItemObject>())
			{
				if (!itemObject.IsCraftedWeapon)
				{
					itemMeshNames.Add(itemObject.MultiMeshName);
				}
				if (itemObject.PrimaryWeapon != null)
				{
					if (itemObject.FlyingMeshName != null && !itemObject.FlyingMeshName.IsEmpty<char>())
					{
						itemMeshNames.Add(itemObject.FlyingMeshName);
					}
					if (itemObject.HolsterMeshName != null && !itemObject.HolsterMeshName.IsEmpty<char>())
					{
						itemMeshNames.Add(itemObject.HolsterMeshName);
					}
					if (itemObject.HolsterWithWeaponMeshName != null && !itemObject.HolsterWithWeaponMeshName.IsEmpty<char>())
					{
						itemMeshNames.Add(itemObject.HolsterWithWeaponMeshName);
					}
				}
				if (itemObject.HasHorseComponent)
				{
					foreach (KeyValuePair<string, bool> keyValuePair in itemObject.HorseComponent.AdditionalMeshesNameList)
					{
						if (keyValuePair.Key != null && !keyValuePair.Key.IsEmpty<char>())
						{
							itemMeshNames.Add(keyValuePair.Key);
						}
					}
				}
			}
		}

		[MBCallback]
		internal string GetMetaMeshPackageMapping()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			Module.GetMetaMeshPackageMapping(dictionary);
			string text = "";
			foreach (string text2 in dictionary.Keys)
			{
				text = string.Concat(new string[]
				{
					text,
					text2,
					"|",
					dictionary[text2],
					","
				});
			}
			return text;
		}

		[MBCallback]
		internal string GetItemMeshNames()
		{
			HashSet<string> hashSet = new HashSet<string>();
			Module.GetItemMeshNames(hashSet);
			foreach (CraftingPiece craftingPiece in MBObjectManager.Instance.GetObjectTypeList<CraftingPiece>())
			{
				hashSet.Add(craftingPiece.MeshName);
				if (craftingPiece.BladeData != null)
				{
					hashSet.Add(craftingPiece.BladeData.HolsterMeshName);
				}
			}
			foreach (BannerIconGroup bannerIconGroup in BannerManager.Instance.BannerIconGroups)
			{
				foreach (KeyValuePair<int, BannerIconData> keyValuePair in bannerIconGroup.AllIcons)
				{
					if (keyValuePair.Value.MaterialName != "")
					{
						hashSet.Add(keyValuePair.Value.MaterialName + keyValuePair.Value.TextureIndex);
					}
				}
			}
			string text = "";
			foreach (string text2 in hashSet)
			{
				if (text2 != null && !text2.IsEmpty<char>())
				{
					text = text + text2 + "#";
				}
			}
			return text;
		}

		[MBCallback]
		internal string GetHorseMaterialNames()
		{
			HashSet<string> hashSet = new HashSet<string>();
			string text = "";
			foreach (ItemObject itemObject in Game.Current.ObjectManager.GetObjectTypeList<ItemObject>())
			{
				if (itemObject.HasHorseComponent && itemObject.HorseComponent.HorseMaterialNames != null && itemObject.HorseComponent.HorseMaterialNames.Count > 0)
				{
					foreach (HorseComponent.MaterialProperty materialProperty in itemObject.HorseComponent.HorseMaterialNames)
					{
						hashSet.Add(materialProperty.Name);
					}
				}
			}
			foreach (string text2 in hashSet)
			{
				if (text2 != null && !text2.IsEmpty<char>())
				{
					text = text + text2 + "#";
				}
			}
			return text;
		}

		public void SetInitialModuleScreenAsRootScreen()
		{
			if (GameStateManager.Current != this.GlobalGameStateManager)
			{
				GameStateManager.Current = this.GlobalGameStateManager;
			}
			foreach (MBSubModuleBase mbsubModuleBase in this.SubModules)
			{
				mbsubModuleBase.OnBeforeInitialModuleScreenSetAsRoot();
			}
			if (GameNetwork.IsDedicatedServer)
			{
				MBGameManager.StartNewGame(new MultiplayerGameManager());
				return;
			}
			string text = ModuleHelper.GetModuleFullPath("Native") + "Videos/TWLogo_and_Partners.ivf";
			string text2 = ModuleHelper.GetModuleFullPath("Native") + "Videos/TWLogo_and_Partners.ogg";
			if (!this._splashScreenPlayed && File.Exists(text) && (text2 == "" || File.Exists(text2)) && !Debugger.IsAttached)
			{
				VideoPlaybackState videoPlaybackState = this.GlobalGameStateManager.CreateState<VideoPlaybackState>();
				videoPlaybackState.SetStartingParameters(text, text2, string.Empty, 30f, true);
				videoPlaybackState.SetOnVideoFinisedDelegate(delegate
				{
					this.OnInitialModuleScreenActivated(true);
				});
				this.GlobalGameStateManager.CleanAndPushState(videoPlaybackState, 0);
				this._splashScreenPlayed = true;
				return;
			}
			this.OnInitialModuleScreenActivated(false);
		}

		private void OnInitialModuleScreenActivated(bool isFromSplashScreenVideo)
		{
			Utilities.EnableGlobalLoadingWindow();
			LoadingWindow.EnableGlobalLoadingWindow();
			if (!this.StartupInfo.IsContinueGame)
			{
				this.StartupInfo.IsContinueGame = PlatformServices.IsPlatformRequestedContinueGame && !this.IsOnlyCoreContentEnabled;
			}
			if (this._enableCoreContentOnReturnToRoot)
			{
				Utilities.DisableCoreGame();
				this._enableCoreContentOnReturnToRoot = false;
			}
			if (this.IsOnlyCoreContentEnabled && PlatformServices.SessionInvitationType == SessionInvitationType.Multiplayer)
			{
				PlatformServices.OnSessionInvitationHandled();
			}
			if (this.IsOnlyCoreContentEnabled && PlatformServices.IsPlatformRequestedMultiplayer)
			{
				PlatformServices.OnPlatformMultiplayerRequestHandled();
			}
			if (!this.IsOnlyCoreContentEnabled && (this.StartupInfo.StartupType == GameStartupType.Multiplayer || PlatformServices.SessionInvitationType == SessionInvitationType.Multiplayer || PlatformServices.IsPlatformRequestedMultiplayer))
			{
				MBGameManager.StartNewGame(new MultiplayerGameManager());
			}
			else
			{
				this.GlobalGameStateManager.CleanAndPushState(this.GlobalGameStateManager.CreateState<InitialState>(), 0);
			}
			foreach (MBSubModuleBase mbsubModuleBase in this.SubModules)
			{
				mbsubModuleBase.OnInitialState();
			}
		}

		private void OnSignInStateUpdated(bool isLoggedIn, TextObject message)
		{
			if (!isLoggedIn && !(this.GlobalGameStateManager.ActiveState is ProfileSelectionState))
			{
				this.GlobalGameStateManager.CleanAndPushState(this.GlobalGameStateManager.CreateState<ProfileSelectionState>(), 0);
			}
		}

		[MBCallback]
		internal bool SetEditorScreenAsRootScreen()
		{
			if (GameStateManager.Current != this.GlobalGameStateManager)
			{
				GameStateManager.Current = this.GlobalGameStateManager;
			}
			if (!(this.GlobalGameStateManager.ActiveState is EditorState))
			{
				this.GlobalGameStateManager.CleanAndPushState(GameStateManager.Current.CreateState<EditorState>(), 0);
				return true;
			}
			return false;
		}

		private bool CheckAssemblyForMissionMethods(Assembly assembly)
		{
			Assembly assembly2 = Assembly.GetAssembly(typeof(MissionMethod));
			if (assembly == assembly2)
			{
				return true;
			}
			AssemblyName[] referencedAssemblies = assembly.GetReferencedAssemblies();
			for (int i = 0; i < referencedAssemblies.Length; i++)
			{
				if (referencedAssemblies[i].FullName == assembly2.FullName)
				{
					return true;
				}
			}
			return false;
		}

		private void FindMissions()
		{
			MBDebug.Print("Searching Mission Methods", 0, Debug.DebugColor.White, 17592186044416UL);
			this._missionInfos = new List<MissionInfo>();
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			List<Type> list = new List<Type>();
			foreach (Assembly assembly in assemblies)
			{
				if (this.CheckAssemblyForMissionMethods(assembly))
				{
					foreach (Type type in assembly.GetTypes())
					{
						object[] customAttributes = type.GetCustomAttributes(typeof(MissionManager), true);
						if (customAttributes != null && customAttributes.Length != 0)
						{
							list.Add(type);
						}
					}
				}
			}
			MBDebug.Print("Found " + list.Count + " mission managers", 0, Debug.DebugColor.White, 17592186044416UL);
			foreach (Type type2 in list)
			{
				foreach (MethodInfo methodInfo in type2.GetMethods(BindingFlags.Static | BindingFlags.Public))
				{
					object[] customAttributes2 = methodInfo.GetCustomAttributes(typeof(MissionMethod), true);
					if (customAttributes2 != null && customAttributes2.Length != 0)
					{
						MissionMethod missionMethod = customAttributes2[0] as MissionMethod;
						MissionInfo missionInfo = new MissionInfo();
						missionInfo.Creator = methodInfo;
						missionInfo.Manager = type2;
						missionInfo.UsableByEditor = missionMethod.UsableByEditor;
						missionInfo.Name = methodInfo.Name;
						if (missionInfo.Name.StartsWith("Open"))
						{
							missionInfo.Name = missionInfo.Name.Substring(4);
						}
						if (missionInfo.Name.EndsWith("Mission"))
						{
							missionInfo.Name = missionInfo.Name.Substring(0, missionInfo.Name.Length - 7);
						}
						MissionInfo missionInfo2 = missionInfo;
						missionInfo2.Name = missionInfo2.Name + "[" + type2.Name + "]";
						this._missionInfos.Add(missionInfo);
					}
				}
			}
			MBDebug.Print("Found " + this._missionInfos.Count + " missions", 0, Debug.DebugColor.White, 17592186044416UL);
		}

		[MBCallback]
		internal string GetMissionControllerClassNames()
		{
			string text = "";
			for (int i = 0; i < this._missionInfos.Count; i++)
			{
				MissionInfo missionInfo = this._missionInfos[i];
				if (missionInfo.UsableByEditor)
				{
					text += missionInfo.Name;
					if (i + 1 != this._missionInfos.Count)
					{
						text += " ";
					}
				}
			}
			return text;
		}

		private void LoadPlatformServices()
		{
			IPlatformServices platformServices = null;
			Assembly assembly = null;
			PlatformInitParams platformInitParams = new PlatformInitParams();
			if (ApplicationPlatform.CurrentPlatform == Platform.WindowsSteam)
			{
				assembly = AssemblyLoader.LoadFrom(ManagedDllFolder.Name + "TaleWorlds.PlatformService.Steam.dll", true);
			}
			else if (ApplicationPlatform.CurrentPlatform == Platform.WindowsEpic)
			{
				assembly = AssemblyLoader.LoadFrom(ManagedDllFolder.Name + "TaleWorlds.PlatformService.Epic.dll", true);
				platformInitParams.Add("ExchangeCode", this.StartupInfo.EpicExchangeCode);
			}
			else if (ApplicationPlatform.CurrentPlatform == Platform.WindowsGOG)
			{
				assembly = AssemblyLoader.LoadFrom(ManagedDllFolder.Name + "TaleWorlds.PlatformService.GOG.dll", true);
				platformInitParams.Add("AchievementDataXmlPath", ModuleHelper.GetModuleFullPath("Native") + "ModuleData/gog_achievement_data.xml");
			}
			else if (ApplicationPlatform.CurrentPlatform == Platform.GDKDesktop || ApplicationPlatform.CurrentPlatform == Platform.Durango)
			{
				assembly = AssemblyLoader.LoadFrom(ManagedDllFolder.Name + "TaleWorlds.PlatformService.GDK.dll", true);
			}
			else if (ApplicationPlatform.CurrentPlatform == Platform.Orbis)
			{
				assembly = AssemblyLoader.LoadFrom(ManagedDllFolder.Name + "TaleWorlds.PlatformService.PS.dll", true);
			}
			else if (ApplicationPlatform.CurrentPlatform == Platform.WindowsNoPlatform)
			{
				string text = "TestUser" + DateTime.Now.Ticks % 10000L;
				if (!string.IsNullOrEmpty(this.StartupInfo.OverridenUserName))
				{
					text = this.StartupInfo.OverridenUserName;
				}
				platformServices = new TestPlatformServices(text);
			}
			if (assembly != null)
			{
				Type[] types = assembly.GetTypes();
				Type type = null;
				foreach (Type type2 in types)
				{
					if (type2.GetInterfaces().Contains(typeof(IPlatformServices)))
					{
						type = type2;
						break;
					}
				}
				platformServices = (IPlatformServices)type.GetConstructor(new Type[] { typeof(PlatformInitParams) }).Invoke(new object[] { platformInitParams });
			}
			if (platformServices != null)
			{
				PlatformServices.Setup(platformServices);
				PlatformServices.OnSessionInvitationAccepted = (Action<SessionInvitationType>)Delegate.Combine(PlatformServices.OnSessionInvitationAccepted, new Action<SessionInvitationType>(this.OnSessionInvitationAccepted));
				PlatformServices.OnPlatformRequestedMultiplayer = (Action)Delegate.Combine(PlatformServices.OnPlatformRequestedMultiplayer, new Action(this.OnPlatformRequestedMultiplayer));
				BannerlordFriendListService bannerlordFriendListService = new BannerlordFriendListService();
				ClanFriendListService clanFriendListService = new ClanFriendListService();
				RecentPlayersFriendListService recentPlayersFriendListService = new RecentPlayersFriendListService();
				PlatformServices.Initialize(new IFriendListService[] { bannerlordFriendListService, clanFriendListService, recentPlayersFriendListService });
				AchievementManager.AchievementService = platformServices.GetAchievementService();
				ActivityManager.ActivityService = platformServices.GetActivityService();
			}
		}

		private void OnSessionInvitationAccepted(SessionInvitationType targetGameType)
		{
			if (targetGameType == SessionInvitationType.Multiplayer)
			{
				if (this.IsOnlyCoreContentEnabled)
				{
					PlatformServices.OnSessionInvitationHandled();
					return;
				}
				this.JobManager.AddJob(new OnSessionInvitationAcceptedJob(targetGameType));
			}
		}

		private void OnPlatformRequestedMultiplayer()
		{
			if (this.IsOnlyCoreContentEnabled)
			{
				PlatformServices.OnPlatformMultiplayerRequestHandled();
				return;
			}
			this.JobManager.AddJob(new OnPlatformRequestedMultiplayerJob());
		}

		private void LoadSubModules()
		{
			MBDebug.Print("Loading submodules...", 0, Debug.DebugColor.White, 17592186044416UL);
			List<ModuleInfo> list = new List<ModuleInfo>();
			string[] modulesNames = Utilities.GetModulesNames();
			for (int i = 0; i < modulesNames.Length; i++)
			{
				ModuleInfo moduleInfo = ModuleHelper.GetModuleInfo(modulesNames[i]);
				if (moduleInfo != null)
				{
					list.Add(moduleInfo);
					XmlResource.GetMbprojxmls(modulesNames[i]);
					XmlResource.GetXmlListAndApply(modulesNames[i]);
				}
			}
			string configName = Common.ConfigName;
			foreach (ModuleInfo moduleInfo2 in list)
			{
				foreach (SubModuleInfo subModuleInfo in moduleInfo2.SubModules)
				{
					if (this.CheckIfSubmoduleCanBeLoadable(subModuleInfo) && !this._loadedSubmoduleTypes.ContainsKey(subModuleInfo.SubModuleClassType))
					{
						string text = Path.Combine(moduleInfo2.FolderPath, "bin", configName);
						string text2 = Path.Combine(text, subModuleInfo.DLLName);
						string text3 = ManagedDllFolder.Name + subModuleInfo.DLLName;
						foreach (string text4 in subModuleInfo.Assemblies)
						{
							string text5 = Path.Combine(text, text4);
							string text6 = ManagedDllFolder.Name + text4;
							if (File.Exists(text5))
							{
								AssemblyLoader.LoadFrom(text5, true);
							}
							else
							{
								AssemblyLoader.LoadFrom(text6, true);
							}
						}
						if (File.Exists(text2))
						{
							Assembly assembly = AssemblyLoader.LoadFrom(text2, true);
							this.AddSubModule(assembly, subModuleInfo.SubModuleClassType);
						}
						else if (File.Exists(text3))
						{
							Assembly assembly2 = AssemblyLoader.LoadFrom(text3, true);
							this.AddSubModule(assembly2, subModuleInfo.SubModuleClassType);
						}
						else
						{
							string text7 = "Cannot find: " + text2;
							string text8 = "Error";
							Debug.ShowMessageBox(text7, text8, 4U);
						}
					}
				}
			}
			this.InitializeSubModules();
		}

		public bool CheckIfSubmoduleCanBeLoadable(SubModuleInfo subModuleInfo)
		{
			if (subModuleInfo.Tags.Count > 0)
			{
				foreach (Tuple<SubModuleInfo.SubModuleTags, string> tuple in subModuleInfo.Tags)
				{
					if (!this.GetSubModuleValiditiy(tuple.Item1, tuple.Item2))
					{
						return false;
					}
				}
				return true;
			}
			return true;
		}

		private bool GetSubModuleValiditiy(SubModuleInfo.SubModuleTags tag, string value)
		{
			switch (tag)
			{
			case SubModuleInfo.SubModuleTags.RejectedPlatform:
			{
				Platform platform;
				if (Enum.TryParse<Platform>(value, out platform))
				{
					return ApplicationPlatform.CurrentPlatform != platform;
				}
				break;
			}
			case SubModuleInfo.SubModuleTags.ExclusivePlatform:
			{
				Platform platform;
				if (Enum.TryParse<Platform>(value, out platform))
				{
					return ApplicationPlatform.CurrentPlatform == platform;
				}
				break;
			}
			case SubModuleInfo.SubModuleTags.DedicatedServerType:
			{
				string text = value.ToLower();
				if (text == "none")
				{
					return this.StartupInfo.DedicatedServerType == DedicatedServerType.None;
				}
				if (text == "both")
				{
					return this.StartupInfo.DedicatedServerType != DedicatedServerType.None;
				}
				if (text == "custom")
				{
					return this.StartupInfo.DedicatedServerType == DedicatedServerType.Custom;
				}
				if (text == "matchmaker")
				{
					return this.StartupInfo.DedicatedServerType == DedicatedServerType.Matchmaker;
				}
				break;
			}
			case SubModuleInfo.SubModuleTags.IsNoRenderModeElement:
				return value.Equals("false");
			case SubModuleInfo.SubModuleTags.DependantRuntimeLibrary:
			{
				Runtime runtime;
				if (Enum.TryParse<Runtime>(value, out runtime))
				{
					return ApplicationPlatform.CurrentRuntimeLibrary == runtime;
				}
				break;
			}
			case SubModuleInfo.SubModuleTags.PlayerHostedDedicatedServer:
			{
				string text2 = value.ToLower();
				if (this.StartupInfo.PlayerHostedDedicatedServer)
				{
					return text2.Equals("true");
				}
				return text2.Equals("false");
			}
			}
			return true;
		}

		[MBCallback]
		internal static void MBThrowException()
		{
			Debug.FailedAssert("MBThrowException", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Module.cs", "MBThrowException", 1364);
		}

		[MBCallback]
		internal void OnEnterEditMode(bool isFirstTime)
		{
		}

		[MBCallback]
		internal static Module GetInstance()
		{
			return Module.CurrentModule;
		}

		[MBCallback]
		internal static string GetGameStatus()
		{
			if (TestCommonBase.BaseInstance != null)
			{
				return TestCommonBase.BaseInstance.GetGameStatus();
			}
			return "";
		}

		private void FinalizeModule()
		{
			if (Game.Current != null)
			{
				Game.Current.OnFinalize();
			}
			if (TestCommonBase.BaseInstance != null)
			{
				TestCommonBase.BaseInstance.OnFinalize();
			}
			this._testContext.FinalizeContext();
			MBInformationManager.Clear();
			InformationManager.Clear();
			ScreenManager.OnFinalize();
			BannerlordConfig.Save();
			this.FinalizeSubModules();
			Common.MemoryCleanupGC(false);
			GC.WaitForPendingFinalizers();
		}

		internal static void FinalizeCurrentModule()
		{
			Module.CurrentModule.FinalizeModule();
			Module.CurrentModule = null;
		}

		[MBCallback]
		internal void SetLoadingFinished()
		{
			this.LoadingFinished = true;
		}

		[MBCallback]
		internal void OnCloseSceneEditorPresentation()
		{
			GameStateManager.Current.PopState(0);
		}

		[MBCallback]
		internal void OnSceneEditorModeOver()
		{
			GameStateManager.Current.PopState(0);
		}

		private void OnConfigChanged()
		{
			foreach (MBSubModuleBase mbsubModuleBase in this.SubModules)
			{
				mbsubModuleBase.OnConfigChanged();
			}
		}

		private void OnConstrainedStateChange(bool isConstrained)
		{
			if (!isConstrained)
			{
				PlatformServices.Instance.OnFocusGained();
			}
		}

		private void OnFocusGained()
		{
			PlatformServices.Instance.OnFocusGained();
		}

		[MBCallback]
		internal void OnSkinsXMLHasChanged()
		{
			if (this.SkinsXMLHasChanged != null)
			{
				this.SkinsXMLHasChanged();
			}
		}

		public event Action SkinsXMLHasChanged;

		[MBCallback]
		internal void OnImguiProfilerTick()
		{
			if (this.ImguiProfilerTick != null)
			{
				this.ImguiProfilerTick();
			}
		}

		[MBCallback]
		internal static string CreateProcessedSkinsXMLForNative(out string baseSkinsXmlPath)
		{
			List<string> list;
			XDocument xdocument = MBObjectManager.ToXDocument(MBObjectManager.GetMergedXmlForNative("soln_skins", out list));
			for (int i = 0; i < xdocument.Descendants("race").Count<XElement>(); i++)
			{
				for (int j = i + 1; j < xdocument.Descendants("race").Count<XElement>(); j++)
				{
					if (xdocument.Descendants("race").ElementAt(i).FirstAttribute.ToString() == xdocument.Descendants("race").ElementAt(j).FirstAttribute.ToString())
					{
						xdocument.Descendants("race").ElementAt(i).Add(xdocument.Descendants("race").ElementAt(j).Descendants());
						xdocument.Descendants("race").ElementAt(j).Remove();
						j--;
					}
				}
			}
			XmlNode xmlNode = MBObjectManager.ToXmlDocument(xdocument);
			StringWriter stringWriter = new StringWriter();
			XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
			xmlNode.WriteTo(xmlTextWriter);
			baseSkinsXmlPath = list[0];
			return stringWriter.ToString();
		}

		[MBCallback]
		internal static string CreateProcessedActionSetsXMLForNative()
		{
			List<string> list;
			XmlDocument xmlDocument = MBObjectManager.GetMergedXmlForNative("soln_action_sets", out list);
			XDocument xdocument = MBObjectManager.ToXDocument(xmlDocument);
			for (int i = 0; i < xdocument.Descendants("action_set").Count<XElement>(); i++)
			{
				for (int j = i + 1; j < xdocument.Descendants("action_set").Count<XElement>(); j++)
				{
					if (xdocument.Descendants("action_set").ElementAt(i).FirstAttribute.ToString() == xdocument.Descendants("action_set").ElementAt(j).FirstAttribute.ToString())
					{
						xdocument.Descendants("action_set").ElementAt(i).Add(xdocument.Descendants("action_set").ElementAt(j).Descendants());
						xdocument.Descendants("action_set").ElementAt(j).Remove();
						j--;
					}
				}
			}
			xmlDocument = MBObjectManager.ToXmlDocument(xdocument);
			StringWriter stringWriter = new StringWriter();
			XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
			xmlDocument.WriteTo(xmlTextWriter);
			return stringWriter.ToString();
		}

		[MBCallback]
		internal static string CreateProcessedActionTypesXMLForNative()
		{
			List<string> list;
			XmlDocument mergedXmlForNative = MBObjectManager.GetMergedXmlForNative("soln_action_types", out list);
			StringWriter stringWriter = new StringWriter();
			XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
			mergedXmlForNative.WriteTo(xmlTextWriter);
			return stringWriter.ToString();
		}

		[MBCallback]
		internal static string CreateProcessedAnimationsXMLForNative(out string animationsXmlPaths)
		{
			List<string> list;
			XmlNode mergedXmlForNative = MBObjectManager.GetMergedXmlForNative("soln_animations", out list);
			StringWriter stringWriter = new StringWriter();
			XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
			mergedXmlForNative.WriteTo(xmlTextWriter);
			animationsXmlPaths = "";
			for (int i = 0; i < list.Count; i++)
			{
				animationsXmlPaths += list[i];
				if (i != list.Count - 1)
				{
					animationsXmlPaths += "\n";
				}
			}
			return stringWriter.ToString();
		}

		[MBCallback]
		internal static string CreateProcessedVoiceDefinitionsXMLForNative()
		{
			List<string> list;
			XmlDocument xmlDocument = MBObjectManager.GetMergedXmlForNative("soln_voice_definitions", out list);
			XDocument xdocument = MBObjectManager.ToXDocument(xmlDocument);
			XElement xelement = xdocument.Descendants("voice_type_declarations").First<XElement>();
			for (int i = 1; i < xdocument.Descendants("voice_type_declarations").Count<XElement>(); i++)
			{
				xelement.Add(xdocument.Descendants("voice_type_declarations").ElementAt(i).Descendants());
				xdocument.Descendants("voice_type_declarations").ElementAt(i).Remove();
				i--;
			}
			for (int j = 0; j < xdocument.Descendants("voice_definition").Count<XElement>(); j++)
			{
				for (int k = j + 1; k < xdocument.Descendants("voice_definition").Count<XElement>(); k++)
				{
					if (xdocument.Descendants("voice_definition").ElementAt(j).FirstAttribute.ToString() == xdocument.Descendants("voice_definition").ElementAt(k).FirstAttribute.ToString())
					{
						xdocument.Descendants("voice_definition").ElementAt(j).Add(xdocument.Descendants("voice_definition").ElementAt(k).Descendants());
						xdocument.Descendants("voice_definition").ElementAt(k).Remove();
						k--;
					}
				}
			}
			xmlDocument = MBObjectManager.ToXmlDocument(xdocument);
			StringWriter stringWriter = new StringWriter();
			XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
			xmlDocument.WriteTo(xmlTextWriter);
			return stringWriter.ToString();
		}

		[MBCallback]
		internal static string CreateProcessedModuleDataXMLForNative(string xmlType)
		{
			List<string> list;
			XmlDocument xmlDocument = MBObjectManager.GetMergedXmlForNative("soln_" + xmlType, out list);
			if (xmlType == "full_movement_sets")
			{
				XDocument xdocument = MBObjectManager.ToXDocument(xmlDocument);
				for (int i = 0; i < xdocument.Descendants("full_movement_set").Count<XElement>(); i++)
				{
					for (int j = i + 1; j < xdocument.Descendants("full_movement_set").Count<XElement>(); j++)
					{
						if (xdocument.Descendants("full_movement_set").ElementAt(i).FirstAttribute.ToString() == xdocument.Descendants("full_movement_set").ElementAt(j).FirstAttribute.ToString())
						{
							xdocument.Descendants("full_movement_set").ElementAt(i).Add(xdocument.Descendants("full_movement_set").ElementAt(j).Descendants());
							xdocument.Descendants("full_movement_set").ElementAt(j).Remove();
							j--;
						}
					}
				}
				xmlDocument = MBObjectManager.ToXmlDocument(xdocument);
			}
			StringWriter stringWriter = new StringWriter();
			XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
			xmlDocument.WriteTo(xmlTextWriter);
			return stringWriter.ToString();
		}

		public event Action ImguiProfilerTick;

		public void ClearStateOptions()
		{
			this._initialStateOptions.Clear();
		}

		public void AddInitialStateOption(InitialStateOption initialStateOption)
		{
			this._initialStateOptions.Add(initialStateOption);
		}

		public IEnumerable<InitialStateOption> GetInitialStateOptions()
		{
			return this._initialStateOptions.OrderBy((InitialStateOption s) => s.OrderIndex);
		}

		public InitialStateOption GetInitialStateOptionWithId(string id)
		{
			foreach (InitialStateOption initialStateOption in this._initialStateOptions)
			{
				if (initialStateOption.Id == id)
				{
					return initialStateOption;
				}
			}
			return null;
		}

		public void ExecuteInitialStateOptionWithId(string id)
		{
			InitialStateOption initialStateOptionWithId = this.GetInitialStateOptionWithId(id);
			if (initialStateOptionWithId != null)
			{
				initialStateOptionWithId.DoAction();
			}
		}

		void IGameStateManagerOwner.OnStateStackEmpty()
		{
		}

		void IGameStateManagerOwner.OnStateChanged(GameState oldState)
		{
		}

		public void SetEditorMissionTester(IEditorMissionTester editorMissionTester)
		{
			this._editorMissionTester = editorMissionTester;
		}

		[MBCallback]
		internal void StartMissionForEditor(string missionName, string sceneName, string levels)
		{
			if (this._editorMissionTester != null)
			{
				this._editorMissionTester.StartMissionForEditor(missionName, sceneName, levels);
			}
		}

		[MBCallback]
		internal void StartMissionForReplayEditor(string missionName, string sceneName, string levels, string fileName, bool record, float startTime, float endTime)
		{
			if (this._editorMissionTester != null)
			{
				this._editorMissionTester.StartMissionForReplayEditor(missionName, sceneName, levels, fileName, record, startTime, endTime);
			}
		}

		public void StartMissionForEditorAux(string missionName, string sceneName, string levels, bool forReplay, string replayFileName, bool isRecord)
		{
			GameStateManager.Current = Game.Current.GameStateManager;
			this.ReturnToEditorState = true;
			MissionInfo missionInfo = this._missionInfos.Find((MissionInfo mi) => mi.Name == missionName);
			if (missionInfo == null)
			{
				missionInfo = this._missionInfos.Find((MissionInfo mi) => mi.Name.Contains(missionName));
			}
			if (forReplay)
			{
				missionInfo.Creator.Invoke(null, new object[] { replayFileName, isRecord });
				return;
			}
			missionInfo.Creator.Invoke(null, new object[] { sceneName, levels });
		}

		private void FillMultiplayerGameTypes()
		{
			this._multiplayerGameModesWithNames = new Dictionary<string, MultiplayerGameMode>();
			this._multiplayerGameTypes = new List<MultiplayerGameTypeInfo>();
			this.AddMultiplayerGameMode(new MissionBasedMultiplayerGameMode("FreeForAll"));
			this.AddMultiplayerGameMode(new MissionBasedMultiplayerGameMode("TeamDeathmatch"));
			this.AddMultiplayerGameMode(new MissionBasedMultiplayerGameMode("Duel"));
			this.AddMultiplayerGameMode(new MissionBasedMultiplayerGameMode("Siege"));
			this.AddMultiplayerGameMode(new MissionBasedMultiplayerGameMode("Captain"));
			this.AddMultiplayerGameMode(new MissionBasedMultiplayerGameMode("Skirmish"));
			this.AddMultiplayerGameMode(new MissionBasedMultiplayerGameMode("Battle"));
		}

		public MultiplayerGameMode GetMultiplayerGameMode(string gameType)
		{
			MultiplayerGameMode multiplayerGameMode;
			if (this._multiplayerGameModesWithNames.TryGetValue(gameType, out multiplayerGameMode))
			{
				return multiplayerGameMode;
			}
			return null;
		}

		public void AddMultiplayerGameMode(MultiplayerGameMode multiplayerGameMode)
		{
			this._multiplayerGameModesWithNames.Add(multiplayerGameMode.Name, multiplayerGameMode);
			this._multiplayerGameTypes.Add(new MultiplayerGameTypeInfo("Native", multiplayerGameMode.Name));
		}

		public List<MultiplayerGameTypeInfo> GetMultiplayerGameTypes()
		{
			return this._multiplayerGameTypes;
		}

		public bool StartMultiplayerGame(string multiplayerGameType, string scene)
		{
			MultiplayerGameMode multiplayerGameMode;
			if (this._multiplayerGameModesWithNames.TryGetValue(multiplayerGameType, out multiplayerGameMode))
			{
				multiplayerGameMode.StartMultiplayerGame(scene);
				return true;
			}
			return false;
		}

		public async void ShutDownWithDelay(string reason, int seconds)
		{
			if (!this._isShuttingDown)
			{
				this._isShuttingDown = true;
				for (int i = 0; i < seconds; i++)
				{
					int num = seconds - i;
					string text = string.Concat(new object[] { "Shutting down in ", num, " seconds with reason '", reason, "'" });
					Debug.Print(text, 0, Debug.DebugColor.White, 17592186044416UL);
					Console.WriteLine(text);
					await Task.Delay(1000);
				}
				if (Game.Current != null)
				{
					Debug.Print("Active game exist during ShutDownWithDelay", 0, Debug.DebugColor.White, 17592186044416UL);
					MBGameManager.EndGame();
				}
				Utilities.QuitGame();
			}
		}

		private TestContext _testContext;

		private List<MissionInfo> _missionInfos;

		private Dictionary<string, Type> _loadedSubmoduleTypes;

		private readonly MBList<MBSubModuleBase> _submodules;

		private SingleThreadedSynchronizationContext _synchronizationContext;

		private bool _enableCoreContentOnReturnToRoot;

		private bool _splashScreenPlayed;

		private List<InitialStateOption> _initialStateOptions;

		private IEditorMissionTester _editorMissionTester;

		private Dictionary<string, MultiplayerGameMode> _multiplayerGameModesWithNames;

		private List<MultiplayerGameTypeInfo> _multiplayerGameTypes = new List<MultiplayerGameTypeInfo>();

		private bool _isShuttingDown;

		public enum XmlInformationType
		{
			Parameters,
			MbObjectType
		}

		private enum StartupType
		{
			None,
			TestMode,
			GameServer,
			Singleplayer,
			Multiplayer,
			Count
		}
	}
}
