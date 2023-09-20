using System;
using TaleWorlds.ModuleManager;

namespace TaleWorlds.MountAndBlade.Launcher.Library
{
	// Token: 0x02000011 RID: 17
	public class LauncherDLLData
	{
		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000087 RID: 135 RVA: 0x00004162 File Offset: 0x00002362
		// (set) Token: 0x06000088 RID: 136 RVA: 0x0000416A File Offset: 0x0000236A
		public SubModuleInfo SubModule { get; private set; }

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x06000089 RID: 137 RVA: 0x00004173 File Offset: 0x00002373
		// (set) Token: 0x0600008A RID: 138 RVA: 0x0000417B File Offset: 0x0000237B
		public bool IsDangerous { get; private set; }

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x0600008B RID: 139 RVA: 0x00004184 File Offset: 0x00002384
		// (set) Token: 0x0600008C RID: 140 RVA: 0x0000418C File Offset: 0x0000238C
		public string VerifyInformation { get; private set; }

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x0600008D RID: 141 RVA: 0x00004195 File Offset: 0x00002395
		// (set) Token: 0x0600008E RID: 142 RVA: 0x0000419D File Offset: 0x0000239D
		public uint Size { get; private set; }

		// Token: 0x0600008F RID: 143 RVA: 0x000041A6 File Offset: 0x000023A6
		public LauncherDLLData(SubModuleInfo subModule, bool isDangerous, string verifyInformation, uint size)
		{
			this.SubModule = subModule;
			this.IsDangerous = isDangerous;
			this.VerifyInformation = verifyInformation;
			this.Size = size;
		}

		// Token: 0x06000090 RID: 144 RVA: 0x000041CB File Offset: 0x000023CB
		public void SetIsDLLDangerous(bool isDangerous)
		{
			this.IsDangerous = isDangerous;
		}

		// Token: 0x06000091 RID: 145 RVA: 0x000041D4 File Offset: 0x000023D4
		public void SetDLLSize(uint size)
		{
			this.Size = size;
		}

		// Token: 0x06000092 RID: 146 RVA: 0x000041DD File Offset: 0x000023DD
		public void SetDLLVerifyInformation(string info)
		{
			this.VerifyInformation = info;
		}
	}
}
