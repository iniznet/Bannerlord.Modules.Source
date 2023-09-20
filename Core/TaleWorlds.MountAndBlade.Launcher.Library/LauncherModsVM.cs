using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Library;
using TaleWorlds.ModuleManager;
using TaleWorlds.MountAndBlade.Launcher.Library.UserDatas;

namespace TaleWorlds.MountAndBlade.Launcher.Library
{
	public class LauncherModsVM : ViewModel
	{
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

		public void Refresh(bool isDisabled, bool isMultiplayer)
		{
			this.Modules.Clear();
			this.IsDisabled = isDisabled;
			this.LoadSubModules(isMultiplayer);
		}

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

		private bool IsVisible(bool isMultiplayer, ModuleInfo moduleInfo)
		{
			return moduleInfo.IsNative || (isMultiplayer && moduleInfo.HasMultiplayerCategory) || (!isMultiplayer && moduleInfo.Category == ModuleCategory.Singleplayer);
		}

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

		private LauncherDLLData GetSubModuleVerifyData(SubModuleInfo subModule)
		{
			return this._dllManager.GetSubModuleVerifyData(subModule);
		}

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

		private UserData _userData;

		private List<ModuleInfo> _modulesCache;

		private UserDataManager _userDataManager;

		private LauncherModsDLLManager _dllManager;

		private MBBindingList<LauncherModuleVM> _modules;

		private bool _isDisabledOnMultiplayer;

		private string _nameCategoryText;

		private string _versionCategoryText;
	}
}
