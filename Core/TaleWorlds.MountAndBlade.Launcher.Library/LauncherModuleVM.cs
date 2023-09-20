using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TaleWorlds.Library;
using TaleWorlds.ModuleManager;

namespace TaleWorlds.MountAndBlade.Launcher.Library
{
	public class LauncherModuleVM : ViewModel
	{
		public LauncherModuleVM(ModuleInfo moduleInfo, Action<LauncherModuleVM, int, string> onChangeLoadingOrder, Action<LauncherModuleVM> onSelect, Func<ModuleInfo, bool> areAllDependenciesPresent, Func<SubModuleInfo, LauncherDLLData> queryIsSubmoduleDangerous)
		{
			this.Info = moduleInfo;
			this._onSelect = onSelect;
			this._onChangeLoadingOrder = onChangeLoadingOrder;
			this._querySubmoduleVerifyData = queryIsSubmoduleDangerous;
			this._areAllDependenciesPresent = areAllDependenciesPresent;
			this.SubModules = new MBBindingList<LauncherSubModule>();
			this.IsOfficial = this.Info.IsOfficial;
			this.VersionText = this.Info.Version.ToString();
			this.Name = moduleInfo.Name;
			string text = string.Empty;
			if (moduleInfo.DependedModules.Count > 0)
			{
				text += "Depends on: \n";
				foreach (DependedModule dependedModule in moduleInfo.DependedModules)
				{
					text = text + dependedModule.ModuleId + (dependedModule.IsOptional ? " (optional)" : "") + "\n";
				}
				this.AnyDependencyAvailable = true;
			}
			if (moduleInfo.IncompatibleModules.Count > 0)
			{
				if (this.AnyDependencyAvailable)
				{
					text += "\n----\n";
				}
				text += "Incompatible with: \n";
				foreach (DependedModule dependedModule2 in moduleInfo.IncompatibleModules)
				{
					text = text + dependedModule2.ModuleId + "\n";
				}
				this.AnyDependencyAvailable = true;
			}
			if (moduleInfo.ModulesToLoadAfterThis.Count > 0)
			{
				if (this.AnyDependencyAvailable)
				{
					text += "\n----\n";
				}
				text += "Needs to load before: \n";
				foreach (DependedModule dependedModule3 in moduleInfo.ModulesToLoadAfterThis)
				{
					text = text + dependedModule3.ModuleId + "\n";
				}
				this.AnyDependencyAvailable = true;
			}
			this.DependencyHint = new LauncherHintVM(text);
			this.UpdateIsDisabled();
			bool flag = !moduleInfo.SubModules.Any(delegate(SubModuleInfo s)
			{
				LauncherDLLData launcherDLLData2 = this._querySubmoduleVerifyData(s);
				return launcherDLLData2 != null && launcherDLLData2.Size == 0U;
			});
			string text2 = "";
			if (flag)
			{
				text2 = "Dangerous code detected.\n\nTaleWorlds is not responsible for consequences arising from running unverified/unofficial code.";
				using (List<SubModuleInfo>.Enumerator enumerator2 = moduleInfo.SubModules.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						SubModuleInfo subModuleInfo = enumerator2.Current;
						this.SubModules.Add(new LauncherSubModule(subModuleInfo));
						LauncherDLLData launcherDLLData = this._querySubmoduleVerifyData(subModuleInfo);
						if (launcherDLLData != null)
						{
							this.IsDangerous = this.IsDangerous || launcherDLLData.IsDangerous;
						}
					}
					goto IL_296;
				}
			}
			this.IsDangerous = true;
			text2 = "Couldn't verify some or all of the code included in this module.\n\nTaleWorlds is not responsible for consequences arising from running unverified/unofficial code.";
			IL_296:
			this.DangerousHint = new LauncherHintVM(text2);
		}

		private void UpdateIsDisabled()
		{
			this.IsDisabled = !Debugger.IsAttached && ((this.Info.IsRequiredOfficial && this.Info.IsSelected) || !this._areAllDependenciesPresent(this.Info));
		}

		private void ExecuteSelect()
		{
			this._onSelect(this);
		}

		[DataSourceProperty]
		public MBBindingList<LauncherSubModule> SubModules
		{
			get
			{
				return this._subModules;
			}
			set
			{
				if (value != this._subModules)
				{
					this._subModules = value;
					base.OnPropertyChangedWithValue<MBBindingList<LauncherSubModule>>(value, "SubModules");
				}
			}
		}

		[DataSourceProperty]
		public LauncherHintVM DangerousHint
		{
			get
			{
				return this._dangerousHint;
			}
			set
			{
				if (value != this._dangerousHint)
				{
					this._dangerousHint = value;
					base.OnPropertyChangedWithValue<LauncherHintVM>(value, "DangerousHint");
				}
			}
		}

		[DataSourceProperty]
		public LauncherHintVM DependencyHint
		{
			get
			{
				return this._dependencyHint;
			}
			set
			{
				if (value != this._dependencyHint)
				{
					this._dependencyHint = value;
					base.OnPropertyChangedWithValue<LauncherHintVM>(value, "DependencyHint");
				}
			}
		}

		[DataSourceProperty]
		public string VersionText
		{
			get
			{
				return this._versionText;
			}
			set
			{
				if (value != this._versionText)
				{
					this._versionText = value;
					base.OnPropertyChangedWithValue<string>(value, "VersionText");
				}
			}
		}

		[DataSourceProperty]
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				if (value != this._name)
				{
					this._name = value;
					base.OnPropertyChangedWithValue<string>(value, "Name");
				}
			}
		}

		[DataSourceProperty]
		public bool IsDisabled
		{
			get
			{
				return this._isDisabled;
			}
			set
			{
				if (value != this._isDisabled)
				{
					this._isDisabled = value;
					base.OnPropertyChangedWithValue(value, "IsDisabled");
				}
			}
		}

		[DataSourceProperty]
		public bool AnyDependencyAvailable
		{
			get
			{
				return this._anyDependencyAvailable;
			}
			set
			{
				if (value != this._anyDependencyAvailable)
				{
					this._anyDependencyAvailable = value;
					base.OnPropertyChangedWithValue(value, "AnyDependencyAvailable");
				}
			}
		}

		[DataSourceProperty]
		public bool IsDangerous
		{
			get
			{
				return this._isDangerous;
			}
			set
			{
				if (value != this._isDangerous)
				{
					this._isDangerous = value;
					base.OnPropertyChangedWithValue(value, "IsDangerous");
				}
			}
		}

		[DataSourceProperty]
		public bool IsOfficial
		{
			get
			{
				return this._isOfficial;
			}
			set
			{
				if (value != this._isOfficial)
				{
					this._isOfficial = value;
					base.OnPropertyChangedWithValue(value, "IsOfficial");
				}
			}
		}

		[DataSourceProperty]
		public bool IsSelected
		{
			get
			{
				return this.Info.IsSelected;
			}
			set
			{
				if (!value && this.Info.IsNative)
				{
					return;
				}
				if (value != this.Info.IsSelected)
				{
					this.UpdateIsDisabled();
					this.Info.IsSelected = value;
					base.OnPropertyChangedWithValue(value, "IsSelected");
				}
			}
		}

		public readonly ModuleInfo Info;

		private readonly Action<LauncherModuleVM, int, string> _onChangeLoadingOrder;

		private readonly Action<LauncherModuleVM> _onSelect;

		private readonly Func<SubModuleInfo, LauncherDLLData> _querySubmoduleVerifyData;

		private readonly Func<ModuleInfo, bool> _areAllDependenciesPresent;

		private MBBindingList<LauncherSubModule> _subModules;

		private LauncherHintVM _dangerousHint;

		private LauncherHintVM _dependencyHint;

		private string _name;

		private string _versionText;

		private bool _isDisabled;

		private bool _isDangerous;

		private bool _isOfficial;

		private bool _anyDependencyAvailable;
	}
}
