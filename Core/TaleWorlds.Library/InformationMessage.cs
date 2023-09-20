using System;

namespace TaleWorlds.Library
{
	// Token: 0x02000038 RID: 56
	public class InformationMessage
	{
		// Token: 0x17000022 RID: 34
		// (get) Token: 0x060001C1 RID: 449 RVA: 0x000067C1 File Offset: 0x000049C1
		// (set) Token: 0x060001C2 RID: 450 RVA: 0x000067C9 File Offset: 0x000049C9
		public string Information { get; set; }

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x060001C3 RID: 451 RVA: 0x000067D2 File Offset: 0x000049D2
		// (set) Token: 0x060001C4 RID: 452 RVA: 0x000067DA File Offset: 0x000049DA
		public string Detail { get; set; }

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x060001C5 RID: 453 RVA: 0x000067E3 File Offset: 0x000049E3
		// (set) Token: 0x060001C6 RID: 454 RVA: 0x000067EB File Offset: 0x000049EB
		public Color Color { get; set; }

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x060001C7 RID: 455 RVA: 0x000067F4 File Offset: 0x000049F4
		// (set) Token: 0x060001C8 RID: 456 RVA: 0x000067FC File Offset: 0x000049FC
		public string SoundEventPath { get; set; }

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x060001C9 RID: 457 RVA: 0x00006805 File Offset: 0x00004A05
		// (set) Token: 0x060001CA RID: 458 RVA: 0x0000680D File Offset: 0x00004A0D
		public string Category { get; set; }

		// Token: 0x060001CB RID: 459 RVA: 0x00006816 File Offset: 0x00004A16
		public InformationMessage(string information)
		{
			this.Information = information;
			this.Color = Color.White;
		}

		// Token: 0x060001CC RID: 460 RVA: 0x00006830 File Offset: 0x00004A30
		public InformationMessage(string information, Color color)
		{
			this.Information = information;
			this.Color = color;
		}

		// Token: 0x060001CD RID: 461 RVA: 0x00006846 File Offset: 0x00004A46
		public InformationMessage(string information, Color color, string category)
		{
			this.Information = information;
			this.Color = color;
			this.Category = category;
		}

		// Token: 0x060001CE RID: 462 RVA: 0x00006863 File Offset: 0x00004A63
		public InformationMessage(string information, string soundEventPath)
		{
			this.Information = information;
			this.SoundEventPath = soundEventPath;
			this.Color = Color.White;
		}

		// Token: 0x060001CF RID: 463 RVA: 0x00006884 File Offset: 0x00004A84
		public InformationMessage()
		{
			this.Information = "";
		}
	}
}
