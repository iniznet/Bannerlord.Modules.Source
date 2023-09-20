using System;

namespace TaleWorlds.MountAndBlade.Launcher.Library.UserDatas
{
	// Token: 0x0200001D RID: 29
	public class DLLCheckData
	{
		// Token: 0x17000051 RID: 81
		// (get) Token: 0x0600011F RID: 287 RVA: 0x00005826 File Offset: 0x00003A26
		// (set) Token: 0x06000120 RID: 288 RVA: 0x0000582E File Offset: 0x00003A2E
		public string DLLName { get; set; }

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x06000121 RID: 289 RVA: 0x00005837 File Offset: 0x00003A37
		// (set) Token: 0x06000122 RID: 290 RVA: 0x0000583F File Offset: 0x00003A3F
		public string DLLVerifyInformation { get; set; }

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x06000123 RID: 291 RVA: 0x00005848 File Offset: 0x00003A48
		// (set) Token: 0x06000124 RID: 292 RVA: 0x00005850 File Offset: 0x00003A50
		public uint LatestSizeInBytes { get; set; }

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x06000125 RID: 293 RVA: 0x00005859 File Offset: 0x00003A59
		// (set) Token: 0x06000126 RID: 294 RVA: 0x00005861 File Offset: 0x00003A61
		public bool IsDangerous { get; set; }

		// Token: 0x06000127 RID: 295 RVA: 0x0000586A File Offset: 0x00003A6A
		public DLLCheckData(string dllname)
		{
			this.LatestSizeInBytes = 0U;
			this.IsDangerous = true;
			this.DLLName = dllname;
			this.DLLVerifyInformation = "";
		}

		// Token: 0x06000128 RID: 296 RVA: 0x00005892 File Offset: 0x00003A92
		public DLLCheckData()
		{
		}
	}
}
