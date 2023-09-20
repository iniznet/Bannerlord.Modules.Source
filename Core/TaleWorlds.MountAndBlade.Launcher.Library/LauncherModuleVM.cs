using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TaleWorlds.Library;
using TaleWorlds.ModuleManager;

namespace TaleWorlds.MountAndBlade.Launcher.Library
{
	// Token: 0x02000014 RID: 20
	public class LauncherModuleVM : ViewModel
	{
		// Token: 0x060000A2 RID: 162 RVA: 0x00004304 File Offset: 0x00002504
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

		// Token: 0x060000A3 RID: 163 RVA: 0x000045E8 File Offset: 0x000027E8
		private void UpdateIsDisabled()
		{
			this.IsDisabled = !Debugger.IsAttached && ((this.Info.IsRequiredOfficial && this.Info.IsSelected) || !this._areAllDependenciesPresent(this.Info));
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x00004636 File Offset: 0x00002836
		private void ExecuteSelect()
		{
			this._onSelect(this);
		}

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x060000A5 RID: 165 RVA: 0x00004644 File Offset: 0x00002844
		// (set) Token: 0x060000A6 RID: 166 RVA: 0x0000464C File Offset: 0x0000284C
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

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x060000A7 RID: 167 RVA: 0x0000466A File Offset: 0x0000286A
		// (set) Token: 0x060000A8 RID: 168 RVA: 0x00004672 File Offset: 0x00002872
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

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x060000A9 RID: 169 RVA: 0x00004690 File Offset: 0x00002890
		// (set) Token: 0x060000AA RID: 170 RVA: 0x00004698 File Offset: 0x00002898
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

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x060000AB RID: 171 RVA: 0x000046B6 File Offset: 0x000028B6
		// (set) Token: 0x060000AC RID: 172 RVA: 0x000046BE File Offset: 0x000028BE
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

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x060000AD RID: 173 RVA: 0x000046E1 File Offset: 0x000028E1
		// (set) Token: 0x060000AE RID: 174 RVA: 0x000046E9 File Offset: 0x000028E9
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

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x060000AF RID: 175 RVA: 0x0000470C File Offset: 0x0000290C
		// (set) Token: 0x060000B0 RID: 176 RVA: 0x00004714 File Offset: 0x00002914
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

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x060000B1 RID: 177 RVA: 0x00004732 File Offset: 0x00002932
		// (set) Token: 0x060000B2 RID: 178 RVA: 0x0000473A File Offset: 0x0000293A
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

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x060000B3 RID: 179 RVA: 0x00004758 File Offset: 0x00002958
		// (set) Token: 0x060000B4 RID: 180 RVA: 0x00004760 File Offset: 0x00002960
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

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x060000B5 RID: 181 RVA: 0x0000477E File Offset: 0x0000297E
		// (set) Token: 0x060000B6 RID: 182 RVA: 0x00004786 File Offset: 0x00002986
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

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x060000B7 RID: 183 RVA: 0x000047A4 File Offset: 0x000029A4
		// (set) Token: 0x060000B8 RID: 184 RVA: 0x000047B1 File Offset: 0x000029B1
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

		// Token: 0x04000051 RID: 81
		public readonly ModuleInfo Info;

		// Token: 0x04000052 RID: 82
		private readonly Action<LauncherModuleVM, int, string> _onChangeLoadingOrder;

		// Token: 0x04000053 RID: 83
		private readonly Action<LauncherModuleVM> _onSelect;

		// Token: 0x04000054 RID: 84
		private readonly Func<SubModuleInfo, LauncherDLLData> _querySubmoduleVerifyData;

		// Token: 0x04000055 RID: 85
		private readonly Func<ModuleInfo, bool> _areAllDependenciesPresent;

		// Token: 0x04000056 RID: 86
		private MBBindingList<LauncherSubModule> _subModules;

		// Token: 0x04000057 RID: 87
		private LauncherHintVM _dangerousHint;

		// Token: 0x04000058 RID: 88
		private LauncherHintVM _dependencyHint;

		// Token: 0x04000059 RID: 89
		private string _name;

		// Token: 0x0400005A RID: 90
		private string _versionText;

		// Token: 0x0400005B RID: 91
		private bool _isDisabled;

		// Token: 0x0400005C RID: 92
		private bool _isDangerous;

		// Token: 0x0400005D RID: 93
		private bool _isOfficial;

		// Token: 0x0400005E RID: 94
		private bool _anyDependencyAvailable;
	}
}
