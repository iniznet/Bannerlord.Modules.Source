using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Library;
using TaleWorlds.ModuleManager;
using TaleWorlds.MountAndBlade.Launcher.Library.UserDatas;

namespace TaleWorlds.MountAndBlade.Launcher.Library
{
	// Token: 0x0200000F RID: 15
	public class LauncherModsVM : ViewModel
	{
		// Token: 0x0600006F RID: 111 RVA: 0x00003278 File Offset: 0x00001478
		public LauncherModsVM(UserDataManager userDataManager)
		{
			this._userDataManager = userDataManager;
			this._userData = this._userDataManager.UserData;
			this._modulesCache = ModuleHelper.GetModules().ToList<ModuleInfo>();
			this._dllManager = new LauncherModsDLLManager(this._userData, this._modulesCache.SelectMany((ModuleInfo m) => m.SubModules).ToList<SubModuleInfo>());
			this.Modules = new MBBindingList<LauncherModuleVM>();
			this.IsDisabled = true;
			this.NameCategoryText = "Name";
			this.VersionCategoryText = "Version";
			if (this._dllManager.ShouldUpdateSaveData)
			{
				this._userDataManager.SaveUserData();
			}
		}

		// Token: 0x06000070 RID: 112 RVA: 0x00003333 File Offset: 0x00001533
		public void Refresh(bool isDisabled, bool isMultiplayer)
		{
			this.Modules.Clear();
			this.IsDisabled = isDisabled;
			this.LoadSubModules(isMultiplayer);
		}

		// Token: 0x06000071 RID: 113 RVA: 0x00003350 File Offset: 0x00001550
		private void LoadSubModules(bool isMultiplayer)
		{
			this.Modules.Clear();
			UserGameTypeData userGameTypeData = (isMultiplayer ? this._userData.MultiplayerData : this._userData.SingleplayerData);
			List<ModuleInfo> unorderedModList = new List<ModuleInfo>();
			using (List<UserModData>.Enumerator enumerator = userGameTypeData.ModDatas.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					UserModData mod = enumerator.Current;
					ModuleInfo moduleInfo = this._modulesCache.Find((ModuleInfo m) => m.Id == mod.Id);
					if (moduleInfo != null && !unorderedModList.Contains(moduleInfo) && this.IsVisible(isMultiplayer, moduleInfo))
					{
						unorderedModList.Add(moduleInfo);
					}
				}
			}
			foreach (ModuleInfo moduleInfo2 in this._modulesCache)
			{
				if (!unorderedModList.Contains(moduleInfo2) && this.IsVisible(isMultiplayer, moduleInfo2))
				{
					unorderedModList.Add(moduleInfo2);
				}
			}
			foreach (ModuleInfo moduleInfo3 in MBMath.TopologySort<ModuleInfo>(unorderedModList, (ModuleInfo module) => ModuleHelper.GetDependentModulesOf(unorderedModList, module)))
			{
				UserModData userModData = this._userData.GetUserModData(isMultiplayer, moduleInfo3.Id);
				bool flag = ((this._userDataManager.HasUserData() && userModData != null) ? (userModData.IsSelected || userModData.IsUpdatedToBeDefault(moduleInfo3)) : (moduleInfo3.IsRequiredOfficial || moduleInfo3.IsDefault));
				moduleInfo3.IsSelected = (flag && this.AreAllDependenciesOfModulePresent(moduleInfo3)) || moduleInfo3.IsNative;
				LauncherModuleVM launcherModuleVM = new LauncherModuleVM(moduleInfo3, new Action<LauncherModuleVM, int, string>(this.ChangeLoadingOrderOf), new Action<LauncherModuleVM>(this.ChangeIsSelectedOf), new Func<ModuleInfo, bool>(this.AreAllDependenciesOfModulePresent), new Func<SubModuleInfo, LauncherDLLData>(this.GetSubModuleVerifyData));
				this.Modules.Add(launcherModuleVM);
			}
		}

		// Token: 0x06000072 RID: 114 RVA: 0x0000358C File Offset: 0x0000178C
		private bool IsVisible(bool isMultiplayer, ModuleInfo moduleInfo)
		{
			return moduleInfo.IsNative || (isMultiplayer && moduleInfo.HasMultiplayerCategory) || (!isMultiplayer && moduleInfo.Category == ModuleCategory.Singleplayer);
		}

		// Token: 0x06000073 RID: 115 RVA: 0x000035B4 File Offset: 0x000017B4
		private void ChangeLoadingOrderOf(LauncherModuleVM targetModule, int insertIndex, string tag)
		{
			if (insertIndex >= this.Modules.IndexOf(targetModule))
			{
				insertIndex--;
			}
			insertIndex = (int)MathF.Clamp((float)insertIndex, 0f, (float)(this.Modules.Count - 1));
			int num = this.Modules.IndexOf(targetModule);
			this.Modules.RemoveAt(num);
			this.Modules.Insert(insertIndex, targetModule);
			IEnumerable<ModuleInfo> modulesTemp = from m in this.Modules.ToList<LauncherModuleVM>()
				select m.Info;
			this.Modules.Clear();
			foreach (ModuleInfo moduleInfo in MBMath.TopologySort<ModuleInfo>(modulesTemp, (ModuleInfo module) => ModuleHelper.GetDependentModulesOf(modulesTemp, module)))
			{
				this.Modules.Add(new LauncherModuleVM(moduleInfo, new Action<LauncherModuleVM, int, string>(this.ChangeLoadingOrderOf), new Action<LauncherModuleVM>(this.ChangeIsSelectedOf), new Func<ModuleInfo, bool>(this.AreAllDependenciesOfModulePresent), new Func<SubModuleInfo, LauncherDLLData>(this.GetSubModuleVerifyData)));
			}
		}

		// Token: 0x06000074 RID: 116 RVA: 0x000036EC File Offset: 0x000018EC
		private void ChangeIsSelectedOf(LauncherModuleVM targetModule)
		{
			if (!this.AreAllDependenciesOfModulePresent(targetModule.Info))
			{
				return;
			}
			targetModule.IsSelected = !targetModule.IsSelected;
			if (targetModule.IsSelected)
			{
				using (IEnumerator<LauncherModuleVM> enumerator = this.Modules.GetEnumerator())
				{
					Func<DependedModule, bool> <>9__1;
					while (enumerator.MoveNext())
					{
						LauncherModuleVM module = enumerator.Current;
						module.IsSelected = module.IsSelected || targetModule.Info.DependedModules.Any((DependedModule d) => d.ModuleId == module.Info.Id && !d.IsOptional);
						IEnumerable<DependedModule> incompatibleModules = module.Info.IncompatibleModules;
						Func<DependedModule, bool> func;
						if ((func = <>9__1) == null)
						{
							func = (<>9__1 = (DependedModule i) => i.ModuleId == targetModule.Info.Id);
						}
						if (incompatibleModules.Any(func))
						{
							module.IsSelected = false;
						}
					}
					return;
				}
			}
			Func<DependedModule, bool> <>9__2;
			foreach (LauncherModuleVM launcherModuleVM in this.Modules)
			{
				LauncherModuleVM launcherModuleVM2 = launcherModuleVM;
				bool isSelected = launcherModuleVM2.IsSelected;
				IEnumerable<DependedModule> dependedModules = launcherModuleVM.Info.DependedModules;
				Func<DependedModule, bool> func2;
				if ((func2 = <>9__2) == null)
				{
					func2 = (<>9__2 = (DependedModule d) => d.ModuleId == targetModule.Info.Id && !d.IsOptional);
				}
				launcherModuleVM2.IsSelected = isSelected & !dependedModules.Any(func2);
			}
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000075 RID: 117 RVA: 0x00003884 File Offset: 0x00001A84
		public string ModuleListCode
		{
			get
			{
				string text = "_MODULES_";
				IEnumerable<ModuleInfo> modulesTemp = from m in this.Modules.ToList<LauncherModuleVM>()
					select m.Info;
				IList<ModuleInfo> list = MBMath.TopologySort<ModuleInfo>(modulesTemp, (ModuleInfo module) => ModuleHelper.GetDependentModulesOf(modulesTemp, module));
				for (int i = 0; i < list.Count; i++)
				{
					ModuleInfo moduleInfo = list[i];
					if (moduleInfo.IsSelected)
					{
						text = text + "*" + moduleInfo.Id;
					}
				}
				return text + "*_MODULES_";
			}
		}

		// Token: 0x06000076 RID: 118 RVA: 0x0000392C File Offset: 0x00001B2C
		private bool AreAllDependenciesOfModulePresent(ModuleInfo info)
		{
			using (List<DependedModule>.Enumerator enumerator = info.DependedModules.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					DependedModule dependentModule = enumerator.Current;
					if (!dependentModule.IsOptional && !this._modulesCache.Any((ModuleInfo m) => m.Id == dependentModule.ModuleId))
					{
						return false;
					}
				}
			}
			for (int i = 0; i < info.IncompatibleModules.Count; i++)
			{
				DependedModule module = info.IncompatibleModules[i];
				LauncherModuleVM launcherModuleVM = this._modules.FirstOrDefault((LauncherModuleVM m) => m.Info.Id == module.ModuleId);
				if (launcherModuleVM != null && launcherModuleVM.IsSelected)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06000077 RID: 119 RVA: 0x00003A08 File Offset: 0x00001C08
		private LauncherDLLData GetSubModuleVerifyData(SubModuleInfo subModule)
		{
			return this._dllManager.GetSubModuleVerifyData(subModule);
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000078 RID: 120 RVA: 0x00003A16 File Offset: 0x00001C16
		// (set) Token: 0x06000079 RID: 121 RVA: 0x00003A1E File Offset: 0x00001C1E
		[DataSourceProperty]
		public bool IsDisabled
		{
			get
			{
				return this._isDisabledOnMultiplayer;
			}
			set
			{
				if (value != this._isDisabledOnMultiplayer)
				{
					this._isDisabledOnMultiplayer = value;
					base.OnPropertyChangedWithValue(value, "IsDisabled");
				}
			}
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x0600007A RID: 122 RVA: 0x00003A3C File Offset: 0x00001C3C
		// (set) Token: 0x0600007B RID: 123 RVA: 0x00003A44 File Offset: 0x00001C44
		[DataSourceProperty]
		public string NameCategoryText
		{
			get
			{
				return this._nameCategoryText;
			}
			set
			{
				if (value != this._nameCategoryText)
				{
					this._nameCategoryText = value;
					base.OnPropertyChangedWithValue<string>(value, "NameCategoryText");
				}
			}
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x0600007C RID: 124 RVA: 0x00003A67 File Offset: 0x00001C67
		// (set) Token: 0x0600007D RID: 125 RVA: 0x00003A6F File Offset: 0x00001C6F
		[DataSourceProperty]
		public string VersionCategoryText
		{
			get
			{
				return this._versionCategoryText;
			}
			set
			{
				if (value != this._versionCategoryText)
				{
					this._versionCategoryText = value;
					base.OnPropertyChangedWithValue<string>(value, "VersionCategoryText");
				}
			}
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x0600007E RID: 126 RVA: 0x00003A92 File Offset: 0x00001C92
		// (set) Token: 0x0600007F RID: 127 RVA: 0x00003A9A File Offset: 0x00001C9A
		[DataSourceProperty]
		public MBBindingList<LauncherModuleVM> Modules
		{
			get
			{
				return this._modules;
			}
			set
			{
				if (value != this._modules)
				{
					this._modules = value;
					base.OnPropertyChangedWithValue<MBBindingList<LauncherModuleVM>>(value, "Modules");
				}
			}
		}

		// Token: 0x0400003D RID: 61
		private UserData _userData;

		// Token: 0x0400003E RID: 62
		private List<ModuleInfo> _modulesCache;

		// Token: 0x0400003F RID: 63
		private UserDataManager _userDataManager;

		// Token: 0x04000040 RID: 64
		private LauncherModsDLLManager _dllManager;

		// Token: 0x04000041 RID: 65
		private MBBindingList<LauncherModuleVM> _modules;

		// Token: 0x04000042 RID: 66
		private bool _isDisabledOnMultiplayer;

		// Token: 0x04000043 RID: 67
		private string _nameCategoryText;

		// Token: 0x04000044 RID: 68
		private string _versionCategoryText;
	}
}
