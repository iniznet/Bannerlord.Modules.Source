using System;
using TaleWorlds.Library;
using TaleWorlds.ModuleManager;

namespace TaleWorlds.MountAndBlade.Launcher.Library
{
	public class LauncherSubModule : ViewModel
	{
		public LauncherSubModule(SubModuleInfo subModuleInfo)
		{
			this.Info = subModuleInfo;
			this.Name = subModuleInfo.Name;
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

		public readonly SubModuleInfo Info;

		private string _name;
	}
}
