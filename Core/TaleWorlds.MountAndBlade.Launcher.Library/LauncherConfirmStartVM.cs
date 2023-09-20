using System;
using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.ModuleManager;

namespace TaleWorlds.MountAndBlade.Launcher.Library
{
	public class LauncherConfirmStartVM : ViewModel
	{
		public LauncherConfirmStartVM(Action onConfirm)
		{
			this._onConfirm = onConfirm;
			this.Title = "CAUTION";
		}

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

		private void ExecuteConfirm()
		{
			Action onConfirm = this._onConfirm;
			if (onConfirm != null)
			{
				onConfirm();
			}
			this.IsEnabled = false;
		}

		private void ExecuteCancel()
		{
			this.IsEnabled = false;
		}

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

		private readonly Action _onConfirm;

		private bool _isEnabled;

		private string _description;

		private string _title;
	}
}
