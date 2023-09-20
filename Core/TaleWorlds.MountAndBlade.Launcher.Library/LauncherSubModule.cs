using System;
using TaleWorlds.Library;
using TaleWorlds.ModuleManager;

namespace TaleWorlds.MountAndBlade.Launcher.Library
{
	// Token: 0x02000006 RID: 6
	public class LauncherSubModule : ViewModel
	{
		// Token: 0x0600002D RID: 45 RVA: 0x000024ED File Offset: 0x000006ED
		public LauncherSubModule(SubModuleInfo subModuleInfo)
		{
			this.Info = subModuleInfo;
			this.Name = subModuleInfo.Name;
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600002E RID: 46 RVA: 0x00002508 File Offset: 0x00000708
		// (set) Token: 0x0600002F RID: 47 RVA: 0x00002510 File Offset: 0x00000710
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

		// Token: 0x04000012 RID: 18
		public readonly SubModuleInfo Info;

		// Token: 0x04000013 RID: 19
		private string _name;
	}
}
