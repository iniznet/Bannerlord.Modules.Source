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
	// Token: 0x020002D1 RID: 721
	public sealed class Module : DotNetObject, IGameStateManagerOwner
	{
		// Token: 0x17000736 RID: 1846
		// (get) Token: 0x060027A5 RID: 10149 RVA: 0x00098292 File Offset: 0x00096492
		// (set) Token: 0x060027A6 RID: 10150 RVA: 0x0009829A File Offset: 0x0009649A
		public GameTextManager GlobalTextManager { get; private set; }

		// Token: 0x17000737 RID: 1847
		// (get) Token: 0x060027A7 RID: 10151 RVA: 0x000982A3 File Offset: 0x000964A3
		// (set) Token: 0x060027A8 RID: 10152 RVA: 0x000982AB File Offset: 0x000964AB
		public JobManager JobManager { get; private set; }

		// Token: 0x17000738 RID: 1848
		// (get) Token: 0x060027A9 RID: 10153 RVA: 0x000982B4 File Offset: 0x000964B4
		public MBReadOnlyList<MBSubModuleBase> SubModules
		{
			get
			{
				return this._submodules;
			}
		}

		// Token: 0x17000739 RID: 1849
		// (get) Token: 0x060027AA RID: 10154 RVA: 0x000982BC File Offset: 0x000964BC
		// (set) Token: 0x060027AB RID: 10155 RVA: 0x000982C4 File Offset: 0x000964C4
		public GameStateManager GlobalGameStateManager { get; private set; }

		// Token: 0x1700073A RID: 1850
		// (get) Token: 0x060027AC RID: 10156 RVA: 0x000982CD File Offset: 0x000964CD
		// (set) Token: 0x060027AD RID: 10157 RVA: 0x000982D5 File Offset: 0x000964D5
		public bool ReturnToEditorState { get; private set; }

		// Token: 0x1700073B RID: 1851
		// (get) Token: 0x060027AE RID: 10158 RVA: 0x000982DE File Offset: 0x000964DE
		// (set) Token: 0x060027AF RID: 10159 RVA: 0x000982E6 File Offset: 0x000964E6
		public bool LoadingFinished { get; private set; }

		// Token: 0x1700073C RID: 1852
		// (get) Token: 0x060027B0 RID: 10160 RVA: 0x000982EF File Offset: 0x000964EF
		// (set) Token: 0x060027B1 RID: 10161 RVA: 0x000982F7 File Offset: 0x000964F7
		public bool IsOnlyCoreContentEnabled { get; private set; }

		// Token: 0x1700073D RID: 1853
		// (get) Token: 0x060027B2 RID: 10162 RVA: 0x00098300 File Offset: 0x00096500
		// (set) Token: 0x060027B3 RID: 10163 RVA: 0x00098308 File Offset: 0x00096508
		public GameStartupInfo StartupInfo { get; private set; }

		// Token: 0x060027B4 RID: 10164 RVA: 0x00098314 File Offset: 0x00096514
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

		// Token: 0x1700073E RID: 1854
		// (get) Token: 0x060027B5 RID: 10165 RVA: 0x000983A2 File Offset: 0x000965A2
		// (set) Token: 0x060027B6 RID: 10166 RVA: 0x000983A9 File Offset: 0x000965A9
		public static Module CurrentModule { get; private set; }

		// Token: 0x060027B7 RID: 10167 RVA: 0x000983B1 File Offset: 0x000965B1
		internal static void CreateModule()
		{
			Module.CurrentModule = new Module();
			Utilities.SetLoadingScreenPercentage(0.4f);
		}

		// Token: 0x060027B8 RID: 10168 RVA: 0x000983C8 File Offset: 0x000965C8
		private void AddSubModule(Assembly subModuleAssembly, string name)
		{
			Type type = subModuleAssembly.GetType(name);
			this._loadedSubmoduleTypes.Add(name, type);
			Managed.AddTypes(this.CollectModuleAssemblyTypes(subModuleAssembly));
		}

		// Token: 0x060027B9 RID: 10169 RVA: 0x000983F8 File Offset: 0x000965F8
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

		// Token: 0x060027BA RID: 10170 RVA: 0x00098588 File Offset: 0x00096788
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

		// Token: 0x060027BB RID: 10171 RVA: 0x00098614 File Offset: 0x00096814
		private void FinalizeSubModules()
		{
			foreach (MBSubModuleBase mbsubModuleBase in this._submodules)
			{
				mbsubModuleBase.OnSubModuleUnloaded();
			}
		}

		// Token: 0x060027BC RID: 10172 RVA: 0x00098664 File Offset: 0x00096864
		public Type GetSubModule(string name)
		{
			return this._loadedSubmoduleTypes[name];
		}

		// Token: 0x060027BD RID: 10173 RVA: 0x00098674 File Offset: 0x00096874
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

		// Token: 0x060027BE RID: 10174 RVA: 0x00098810 File Offset: 0x00096A10
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

		// Token: 0x060027BF RID: 10175 RVA: 0x000988C2 File Offset: 0x00096AC2
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

		// Token: 0x060027C0 RID: 10176 RVA: 0x000988FC File Offset: 0x00096AFC
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

		// Token: 0x060027C1 RID: 10177 RVA: 0x00098C1C File Offset: 0x00096E1C
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

		// Token: 0x060027C2 RID: 10178 RVA: 0x00098E5C File Offset: 0x0009705C
		private void OnConfirmReturnToMainMenu()
		{
			MBGameManager.EndGame();
		}

		// Token: 0x060027C3 RID: 10179 RVA: 0x00098E63 File Offset: 0x00097063
		private void OnNetworkTick(float dt)
		{
			NetworkMain.Tick(dt);
		}

		// Token: 0x060027C4 RID: 10180 RVA: 0x00098E6B File Offset: 0x0009706B
		[MBCallback]
		internal void RunTest(string commandLine)
		{
			MBDebug.Print(" TEST MODE ENABLED. Command line string: " + commandLine, 0, Debug.DebugColor.White, 17592186044416UL);
			this._testContext.RunTestAux(commandLine);
		}

		// Token: 0x060027C5 RID: 10181 RVA: 0x00098E95 File Offset: 0x00097095
		[MBCallback]
		internal void TickTest(float dt)
		{
			this._testContext.TickTest(dt);
		}

		// Token: 0x060027C6 RID: 10182 RVA: 0x00098EA3 File Offset: 0x000970A3
		[MBCallback]
		internal void OnDumpCreated()
		{
			if (TestCommonBase.BaseInstance != null)
			{
				TestCommonBase.BaseInstance.ToggleTimeoutTimer();
				TestCommonBase.BaseInstance.StartTimeoutTimer();
			}
		}

		// Token: 0x060027C7 RID: 10183 RVA: 0x00098EC0 File Offset: 0x000970C0
		[MBCallback]
		internal void OnDumpCreationStarted()
		{
			if (TestCommonBase.BaseInstance != null)
			{
				TestCommonBase.BaseInstance.ToggleTimeoutTimer();
			}
		}

		// Token: 0x060027C8 RID: 10184 RVA: 0x00098ED4 File Offset: 0x000970D4
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

		// Token: 0x060027C9 RID: 10185 RVA: 0x000990E4 File Offset: 0x000972E4
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

		// Token: 0x060027CA RID: 10186 RVA: 0x00099230 File Offset: 0x00097430
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

		// Token: 0x060027CB RID: 10187 RVA: 0x000992C0 File Offset: 0x000974C0
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

		// Token: 0x060027CC RID: 10188 RVA: 0x0009946C File Offset: 0x0009766C
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

		// Token: 0x060027CD RID: 10189 RVA: 0x0009959C File Offset: 0x0009779C
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

		// Token: 0x060027CE RID: 10190 RVA: 0x000996BC File Offset: 0x000978BC
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

		// Token: 0x060027CF RID: 10191 RVA: 0x000997C8 File Offset: 0x000979C8
		private void OnSignInStateUpdated(bool isLoggedIn, TextObject message)
		{
			if (!isLoggedIn && !(this.GlobalGameStateManager.ActiveState is ProfileSelectionState))
			{
				this.GlobalGameStateManager.CleanAndPushState(this.GlobalGameStateManager.CreateState<ProfileSelectionState>(), 0);
			}
		}

		// Token: 0x060027D0 RID: 10192 RVA: 0x000997F8 File Offset: 0x000979F8
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

		// Token: 0x060027D1 RID: 10193 RVA: 0x00099848 File Offset: 0x00097A48
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

		// Token: 0x060027D2 RID: 10194 RVA: 0x000998A0 File Offset: 0x00097AA0
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

		// Token: 0x060027D3 RID: 10195 RVA: 0x00099B00 File Offset: 0x00097D00
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

		// Token: 0x060027D4 RID: 10196 RVA: 0x00099B68 File Offset: 0x00097D68
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

		// Token: 0x060027D5 RID: 10197 RVA: 0x00099DC3 File Offset: 0x00097FC3
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

		// Token: 0x060027D6 RID: 10198 RVA: 0x00099DE8 File Offset: 0x00097FE8
		private void OnPlatformRequestedMultiplayer()
		{
			if (this.IsOnlyCoreContentEnabled)
			{
				PlatformServices.OnPlatformMultiplayerRequestHandled();
				return;
			}
			this.JobManager.AddJob(new OnPlatformRequestedMultiplayerJob());
		}

		// Token: 0x060027D7 RID: 10199 RVA: 0x00099E08 File Offset: 0x00098008
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

		// Token: 0x060027D8 RID: 10200 RVA: 0x0009A054 File Offset: 0x00098254
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

		// Token: 0x060027D9 RID: 10201 RVA: 0x0009A0CC File Offset: 0x000982CC
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

		// Token: 0x060027DA RID: 10202 RVA: 0x0009A1F8 File Offset: 0x000983F8
		[MBCallback]
		internal static void MBThrowException()
		{
			Debug.FailedAssert("MBThrowException", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Module.cs", "MBThrowException", 1364);
		}

		// Token: 0x060027DB RID: 10203 RVA: 0x0009A213 File Offset: 0x00098413
		[MBCallback]
		internal void OnEnterEditMode(bool isFirstTime)
		{
		}

		// Token: 0x060027DC RID: 10204 RVA: 0x0009A217 File Offset: 0x00098417
		[MBCallback]
		internal static Module GetInstance()
		{
			return Module.CurrentModule;
		}

		// Token: 0x060027DD RID: 10205 RVA: 0x0009A21E File Offset: 0x0009841E
		[MBCallback]
		internal static string GetGameStatus()
		{
			if (TestCommonBase.BaseInstance != null)
			{
				return TestCommonBase.BaseInstance.GetGameStatus();
			}
			return "";
		}

		// Token: 0x060027DE RID: 10206 RVA: 0x0009A238 File Offset: 0x00098438
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

		// Token: 0x060027DF RID: 10207 RVA: 0x0009A298 File Offset: 0x00098498
		internal static void FinalizeCurrentModule()
		{
			Module.CurrentModule.FinalizeModule();
			Module.CurrentModule = null;
		}

		// Token: 0x060027E0 RID: 10208 RVA: 0x0009A2AA File Offset: 0x000984AA
		[MBCallback]
		internal void SetLoadingFinished()
		{
			this.LoadingFinished = true;
		}

		// Token: 0x060027E1 RID: 10209 RVA: 0x0009A2B3 File Offset: 0x000984B3
		[MBCallback]
		internal void OnCloseSceneEditorPresentation()
		{
			GameStateManager.Current.PopState(0);
		}

		// Token: 0x060027E2 RID: 10210 RVA: 0x0009A2C0 File Offset: 0x000984C0
		[MBCallback]
		internal void OnSceneEditorModeOver()
		{
			GameStateManager.Current.PopState(0);
		}

		// Token: 0x060027E3 RID: 10211 RVA: 0x0009A2D0 File Offset: 0x000984D0
		private void OnConfigChanged()
		{
			foreach (MBSubModuleBase mbsubModuleBase in this.SubModules)
			{
				mbsubModuleBase.OnConfigChanged();
			}
		}

		// Token: 0x060027E4 RID: 10212 RVA: 0x0009A320 File Offset: 0x00098520
		private void OnConstrainedStateChange(bool isConstrained)
		{
			if (!isConstrained)
			{
				PlatformServices.Instance.OnFocusGained();
			}
		}

		// Token: 0x060027E5 RID: 10213 RVA: 0x0009A32F File Offset: 0x0009852F
		private void OnFocusGained()
		{
			PlatformServices.Instance.OnFocusGained();
		}

		// Token: 0x060027E6 RID: 10214 RVA: 0x0009A33B File Offset: 0x0009853B
		[MBCallback]
		internal void OnSkinsXMLHasChanged()
		{
			if (this.SkinsXMLHasChanged != null)
			{
				this.SkinsXMLHasChanged();
			}
		}

		// Token: 0x14000073 RID: 115
		// (add) Token: 0x060027E7 RID: 10215 RVA: 0x0009A350 File Offset: 0x00098550
		// (remove) Token: 0x060027E8 RID: 10216 RVA: 0x0009A388 File Offset: 0x00098588
		public event Action SkinsXMLHasChanged;

		// Token: 0x060027E9 RID: 10217 RVA: 0x0009A3BD File Offset: 0x000985BD
		[MBCallback]
		internal void OnImguiProfilerTick()
		{
			if (this.ImguiProfilerTick != null)
			{
				this.ImguiProfilerTick();
			}
		}

		// Token: 0x060027EA RID: 10218 RVA: 0x0009A3D4 File Offset: 0x000985D4
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

		// Token: 0x060027EB RID: 10219 RVA: 0x0009A518 File Offset: 0x00098718
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

		// Token: 0x060027EC RID: 10220 RVA: 0x0009A654 File Offset: 0x00098854
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

		// Token: 0x060027ED RID: 10221 RVA: 0x0009A688 File Offset: 0x00098888
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

		// Token: 0x060027EE RID: 10222 RVA: 0x0009A6FC File Offset: 0x000988FC
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

		// Token: 0x060027EF RID: 10223 RVA: 0x0009A8B8 File Offset: 0x00098AB8
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

		// Token: 0x14000074 RID: 116
		// (add) Token: 0x060027F0 RID: 10224 RVA: 0x0009AA08 File Offset: 0x00098C08
		// (remove) Token: 0x060027F1 RID: 10225 RVA: 0x0009AA40 File Offset: 0x00098C40
		public event Action ImguiProfilerTick;

		// Token: 0x060027F2 RID: 10226 RVA: 0x0009AA75 File Offset: 0x00098C75
		public void ClearStateOptions()
		{
			this._initialStateOptions.Clear();
		}

		// Token: 0x060027F3 RID: 10227 RVA: 0x0009AA82 File Offset: 0x00098C82
		public void AddInitialStateOption(InitialStateOption initialStateOption)
		{
			this._initialStateOptions.Add(initialStateOption);
		}

		// Token: 0x060027F4 RID: 10228 RVA: 0x0009AA90 File Offset: 0x00098C90
		public IEnumerable<InitialStateOption> GetInitialStateOptions()
		{
			return this._initialStateOptions.OrderBy((InitialStateOption s) => s.OrderIndex);
		}

		// Token: 0x060027F5 RID: 10229 RVA: 0x0009AABC File Offset: 0x00098CBC
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

		// Token: 0x060027F6 RID: 10230 RVA: 0x0009AB20 File Offset: 0x00098D20
		public void ExecuteInitialStateOptionWithId(string id)
		{
			InitialStateOption initialStateOptionWithId = this.GetInitialStateOptionWithId(id);
			if (initialStateOptionWithId != null)
			{
				initialStateOptionWithId.DoAction();
			}
		}

		// Token: 0x060027F7 RID: 10231 RVA: 0x0009AB3E File Offset: 0x00098D3E
		void IGameStateManagerOwner.OnStateStackEmpty()
		{
		}

		// Token: 0x060027F8 RID: 10232 RVA: 0x0009AB40 File Offset: 0x00098D40
		void IGameStateManagerOwner.OnStateChanged(GameState oldState)
		{
		}

		// Token: 0x060027F9 RID: 10233 RVA: 0x0009AB42 File Offset: 0x00098D42
		public void SetEditorMissionTester(IEditorMissionTester editorMissionTester)
		{
			this._editorMissionTester = editorMissionTester;
		}

		// Token: 0x060027FA RID: 10234 RVA: 0x0009AB4B File Offset: 0x00098D4B
		[MBCallback]
		internal void StartMissionForEditor(string missionName, string sceneName, string levels)
		{
			if (this._editorMissionTester != null)
			{
				this._editorMissionTester.StartMissionForEditor(missionName, sceneName, levels);
			}
		}

		// Token: 0x060027FB RID: 10235 RVA: 0x0009AB63 File Offset: 0x00098D63
		[MBCallback]
		internal void StartMissionForReplayEditor(string missionName, string sceneName, string levels, string fileName, bool record, float startTime, float endTime)
		{
			if (this._editorMissionTester != null)
			{
				this._editorMissionTester.StartMissionForReplayEditor(missionName, sceneName, levels, fileName, record, startTime, endTime);
			}
		}

		// Token: 0x060027FC RID: 10236 RVA: 0x0009AB84 File Offset: 0x00098D84
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

		// Token: 0x060027FD RID: 10237 RVA: 0x0009AC2C File Offset: 0x00098E2C
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

		// Token: 0x060027FE RID: 10238 RVA: 0x0009ACC0 File Offset: 0x00098EC0
		public MultiplayerGameMode GetMultiplayerGameMode(string gameType)
		{
			MultiplayerGameMode multiplayerGameMode;
			if (this._multiplayerGameModesWithNames.TryGetValue(gameType, out multiplayerGameMode))
			{
				return multiplayerGameMode;
			}
			return null;
		}

		// Token: 0x060027FF RID: 10239 RVA: 0x0009ACE0 File Offset: 0x00098EE0
		public void AddMultiplayerGameMode(MultiplayerGameMode multiplayerGameMode)
		{
			this._multiplayerGameModesWithNames.Add(multiplayerGameMode.Name, multiplayerGameMode);
			this._multiplayerGameTypes.Add(new MultiplayerGameTypeInfo("Native", multiplayerGameMode.Name));
		}

		// Token: 0x06002800 RID: 10240 RVA: 0x0009AD0F File Offset: 0x00098F0F
		public List<MultiplayerGameTypeInfo> GetMultiplayerGameTypes()
		{
			return this._multiplayerGameTypes;
		}

		// Token: 0x06002801 RID: 10241 RVA: 0x0009AD18 File Offset: 0x00098F18
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

		// Token: 0x06002802 RID: 10242 RVA: 0x0009AD40 File Offset: 0x00098F40
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

		// Token: 0x04000EA9 RID: 3753
		private TestContext _testContext;

		// Token: 0x04000EAA RID: 3754
		private List<MissionInfo> _missionInfos;

		// Token: 0x04000EAB RID: 3755
		private Dictionary<string, Type> _loadedSubmoduleTypes;

		// Token: 0x04000EAC RID: 3756
		private readonly MBList<MBSubModuleBase> _submodules;

		// Token: 0x04000EAD RID: 3757
		private SingleThreadedSynchronizationContext _synchronizationContext;

		// Token: 0x04000EB3 RID: 3763
		private bool _enableCoreContentOnReturnToRoot;

		// Token: 0x04000EB6 RID: 3766
		private bool _splashScreenPlayed;

		// Token: 0x04000EB9 RID: 3769
		private List<InitialStateOption> _initialStateOptions;

		// Token: 0x04000EBA RID: 3770
		private IEditorMissionTester _editorMissionTester;

		// Token: 0x04000EBB RID: 3771
		private Dictionary<string, MultiplayerGameMode> _multiplayerGameModesWithNames;

		// Token: 0x04000EBC RID: 3772
		private List<MultiplayerGameTypeInfo> _multiplayerGameTypes = new List<MultiplayerGameTypeInfo>();

		// Token: 0x04000EBD RID: 3773
		private bool _isShuttingDown;

		// Token: 0x020005ED RID: 1517
		public enum XmlInformationType
		{
			// Token: 0x04001EFF RID: 7935
			Parameters,
			// Token: 0x04001F00 RID: 7936
			MbObjectType
		}

		// Token: 0x020005EE RID: 1518
		private enum StartupType
		{
			// Token: 0x04001F02 RID: 7938
			None,
			// Token: 0x04001F03 RID: 7939
			TestMode,
			// Token: 0x04001F04 RID: 7940
			GameServer,
			// Token: 0x04001F05 RID: 7941
			Singleplayer,
			// Token: 0x04001F06 RID: 7942
			Multiplayer,
			// Token: 0x04001F07 RID: 7943
			Count
		}
	}
}
