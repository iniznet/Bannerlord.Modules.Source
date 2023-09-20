using System;
using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.ModuleManager;

namespace TaleWorlds.MountAndBlade.Launcher.Library
{
	// Token: 0x0200000B RID: 11
	public class LauncherConfirmStartVM : ViewModel
	{
		// Token: 0x06000054 RID: 84 RVA: 0x00002E5D File Offset: 0x0000105D
		public LauncherConfirmStartVM(Action onConfirm)
		{
			this._onConfirm = onConfirm;
			this.Title = "CAUTION";
		}

		// Token: 0x06000055 RID: 85 RVA: 0x00002E78 File Offset: 0x00001078
		public void EnableWith(List<SubModuleInfo> unverifiedSubModules, List<DependentVersionMissmatchItem> missmatchedDependentModules)
		{
			this.IsEnabled = true;
			this.Description = string.Empty;
			if (unverifiedSubModules.Count > 0)
			{
				this.Description += "You're loading unverified code from: \n";
				for (int i = 0; i < unverifiedSubModules.Count; i++)
				{
					this.Description += unverifiedSubModules[i].Name;
					if (i == unverifiedSubModules.Count - 1)
					{
						this.Description += "\n";
					}
					else
					{
						this.Description += ", ";
					}
				}
				this.Description += "\n";
			}
			if (missmatchedDependentModules.Count > 0)
			{
				for (int j = 0; j < missmatchedDependentModules.Count; j++)
				{
					for (int k = 0; k < missmatchedDependentModules[j].MissmatchedDependencies.Count; k++)
					{
						string missmatchedModuleId = missmatchedDependentModules[j].MissmatchedModuleId;
						string moduleId = missmatchedDependentModules[j].MissmatchedDependencies[k].Item1.ModuleId;
						string text = missmatchedDependentModules[j].MissmatchedDependencies[k].Item1.Version.ToString();
						string text2 = missmatchedDependentModules[j].MissmatchedDependencies[k].Item2.ToString();
						this.Description = string.Concat(new string[]
						{
							this.Description, missmatchedModuleId, " depends on ", moduleId, "(", text, "), current version is  ", moduleId, "(", text2,
							")\n"
						});
					}
				}
				this.Description += "\n";
			}
			this.Description += "TaleWorlds is not responsible for an unstable experience if it occurs.\n";
			this.Description += "Are you sure?";
		}

		// Token: 0x06000056 RID: 86 RVA: 0x000030B6 File Offset: 0x000012B6
		private void ExecuteConfirm()
		{
			Action onConfirm = this._onConfirm;
			if (onConfirm != null)
			{
				onConfirm();
			}
			this.IsEnabled = false;
		}

		// Token: 0x06000057 RID: 87 RVA: 0x000030D0 File Offset: 0x000012D0
		private void ExecuteCancel()
		{
			this.IsEnabled = false;
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000058 RID: 88 RVA: 0x000030D9 File Offset: 0x000012D9
		// (set) Token: 0x06000059 RID: 89 RVA: 0x000030E1 File Offset: 0x000012E1
		[DataSourceProperty]
		public bool IsEnabled
		{
			get
			{
				return this._isEnabled;
			}
			set
			{
				if (this._isEnabled != value)
				{
					this._isEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsEnabled");
				}
			}
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600005A RID: 90 RVA: 0x000030FF File Offset: 0x000012FF
		// (set) Token: 0x0600005B RID: 91 RVA: 0x00003107 File Offset: 0x00001307
		[DataSourceProperty]
		public string Description
		{
			get
			{
				return this._description;
			}
			set
			{
				if (this._description != value)
				{
					this._description = value;
					base.OnPropertyChangedWithValue<string>(value, "Description");
				}
			}
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600005C RID: 92 RVA: 0x0000312A File Offset: 0x0000132A
		// (set) Token: 0x0600005D RID: 93 RVA: 0x00003132 File Offset: 0x00001332
		[DataSourceProperty]
		public string Title
		{
			get
			{
				return this._title;
			}
			set
			{
				if (this._title != value)
				{
					this._title = value;
					base.OnPropertyChangedWithValue<string>(value, "Title");
				}
			}
		}

		// Token: 0x04000034 RID: 52
		private readonly Action _onConfirm;

		// Token: 0x04000035 RID: 53
		private bool _isEnabled;

		// Token: 0x04000036 RID: 54
		private string _description;

		// Token: 0x04000037 RID: 55
		private string _title;
	}
}
