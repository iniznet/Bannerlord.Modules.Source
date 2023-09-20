using System;
using TaleWorlds.Library;

namespace TaleWorlds.ModuleManager
{
	// Token: 0x02000002 RID: 2
	public struct DependedModule
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000001 RID: 1 RVA: 0x00002048 File Offset: 0x00000248
		// (set) Token: 0x06000002 RID: 2 RVA: 0x00002050 File Offset: 0x00000250
		public string ModuleId { get; private set; }

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000003 RID: 3 RVA: 0x00002059 File Offset: 0x00000259
		// (set) Token: 0x06000004 RID: 4 RVA: 0x00002061 File Offset: 0x00000261
		public ApplicationVersion Version { get; private set; }

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000005 RID: 5 RVA: 0x0000206A File Offset: 0x0000026A
		// (set) Token: 0x06000006 RID: 6 RVA: 0x00002072 File Offset: 0x00000272
		public bool IsOptional { get; private set; }

		// Token: 0x06000007 RID: 7 RVA: 0x0000207B File Offset: 0x0000027B
		public DependedModule(string moduleId, ApplicationVersion version, bool isOptional = false)
		{
			this.ModuleId = moduleId;
			this.Version = version;
			this.IsOptional = isOptional;
		}

		// Token: 0x06000008 RID: 8 RVA: 0x00002094 File Offset: 0x00000294
		public void UpdateVersionChangeSet()
		{
			this.Version = new ApplicationVersion(this.Version.ApplicationVersionType, this.Version.Major, this.Version.Minor, this.Version.Revision, 17949);
		}
	}
}
