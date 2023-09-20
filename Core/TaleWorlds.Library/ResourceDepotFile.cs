using System;

namespace TaleWorlds.Library
{
	// Token: 0x0200007E RID: 126
	public class ResourceDepotFile
	{
		// Token: 0x17000077 RID: 119
		// (get) Token: 0x0600045E RID: 1118 RVA: 0x0000E8EE File Offset: 0x0000CAEE
		// (set) Token: 0x0600045F RID: 1119 RVA: 0x0000E8F6 File Offset: 0x0000CAF6
		public ResourceDepotLocation ResourceDepotLocation { get; private set; }

		// Token: 0x17000078 RID: 120
		// (get) Token: 0x06000460 RID: 1120 RVA: 0x0000E8FF File Offset: 0x0000CAFF
		public string BasePath
		{
			get
			{
				return this.ResourceDepotLocation.BasePath;
			}
		}

		// Token: 0x17000079 RID: 121
		// (get) Token: 0x06000461 RID: 1121 RVA: 0x0000E90C File Offset: 0x0000CB0C
		public string Location
		{
			get
			{
				return this.ResourceDepotLocation.Path;
			}
		}

		// Token: 0x1700007A RID: 122
		// (get) Token: 0x06000462 RID: 1122 RVA: 0x0000E919 File Offset: 0x0000CB19
		// (set) Token: 0x06000463 RID: 1123 RVA: 0x0000E921 File Offset: 0x0000CB21
		public string FileName { get; private set; }

		// Token: 0x1700007B RID: 123
		// (get) Token: 0x06000464 RID: 1124 RVA: 0x0000E92A File Offset: 0x0000CB2A
		// (set) Token: 0x06000465 RID: 1125 RVA: 0x0000E932 File Offset: 0x0000CB32
		public string FullPath { get; private set; }

		// Token: 0x1700007C RID: 124
		// (get) Token: 0x06000466 RID: 1126 RVA: 0x0000E93B File Offset: 0x0000CB3B
		// (set) Token: 0x06000467 RID: 1127 RVA: 0x0000E943 File Offset: 0x0000CB43
		public string FullPathLowerCase { get; private set; }

		// Token: 0x06000468 RID: 1128 RVA: 0x0000E94C File Offset: 0x0000CB4C
		public ResourceDepotFile(ResourceDepotLocation resourceDepotLocation, string fileName, string fullPath)
		{
			this.ResourceDepotLocation = resourceDepotLocation;
			this.FileName = fileName;
			this.FullPath = fullPath;
			this.FullPathLowerCase = fullPath.ToLower();
		}
	}
}
