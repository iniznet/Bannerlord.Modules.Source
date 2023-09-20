using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Engine;

namespace SandBox
{
	// Token: 0x02000010 RID: 16
	public class ModuleManager : IModuleManager
	{
		// Token: 0x1700000D RID: 13
		// (get) Token: 0x060000C1 RID: 193 RVA: 0x0000645B File Offset: 0x0000465B
		public string[] ModuleNames
		{
			get
			{
				return Utilities.GetModulesNames();
			}
		}
	}
}
