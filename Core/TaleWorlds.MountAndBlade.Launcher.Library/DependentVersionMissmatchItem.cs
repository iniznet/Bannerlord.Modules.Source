using System;
using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.ModuleManager;

namespace TaleWorlds.MountAndBlade.Launcher.Library
{
	// Token: 0x0200000C RID: 12
	public struct DependentVersionMissmatchItem
	{
		// Token: 0x1700000F RID: 15
		// (get) Token: 0x0600005E RID: 94 RVA: 0x00003155 File Offset: 0x00001355
		// (set) Token: 0x0600005F RID: 95 RVA: 0x0000315D File Offset: 0x0000135D
		public string MissmatchedModuleId { get; private set; }

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000060 RID: 96 RVA: 0x00003166 File Offset: 0x00001366
		// (set) Token: 0x06000061 RID: 97 RVA: 0x0000316E File Offset: 0x0000136E
		public List<Tuple<DependedModule, ApplicationVersion>> MissmatchedDependencies { get; private set; }

		// Token: 0x06000062 RID: 98 RVA: 0x00003177 File Offset: 0x00001377
		public DependentVersionMissmatchItem(string missmatchedModuleId, List<Tuple<DependedModule, ApplicationVersion>> missmatchedDependencies)
		{
			this.MissmatchedModuleId = missmatchedModuleId;
			this.MissmatchedDependencies = missmatchedDependencies;
		}
	}
}
