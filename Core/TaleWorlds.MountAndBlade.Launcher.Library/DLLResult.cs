using System;

namespace TaleWorlds.MountAndBlade.Launcher.Library
{
	// Token: 0x02000013 RID: 19
	public class DLLResult
	{
		// Token: 0x17000020 RID: 32
		// (get) Token: 0x0600009A RID: 154 RVA: 0x0000428E File Offset: 0x0000248E
		// (set) Token: 0x0600009B RID: 155 RVA: 0x00004296 File Offset: 0x00002496
		public string DLLName { get; set; }

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x0600009C RID: 156 RVA: 0x0000429F File Offset: 0x0000249F
		// (set) Token: 0x0600009D RID: 157 RVA: 0x000042A7 File Offset: 0x000024A7
		public bool IsSafe { get; set; }

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x0600009E RID: 158 RVA: 0x000042B0 File Offset: 0x000024B0
		// (set) Token: 0x0600009F RID: 159 RVA: 0x000042B8 File Offset: 0x000024B8
		public string Information { get; set; }

		// Token: 0x060000A0 RID: 160 RVA: 0x000042C1 File Offset: 0x000024C1
		public DLLResult(string dLLName, bool isSafe, string information)
		{
			this.DLLName = dLLName;
			this.IsSafe = isSafe;
			this.Information = information;
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x000042DE File Offset: 0x000024DE
		public DLLResult()
		{
			this.DLLName = "";
			this.IsSafe = false;
			this.Information = "";
		}
	}
}
