using System;
using System.Numerics;

namespace TaleWorlds.TwoDimension
{
	// Token: 0x02000008 RID: 8
	public class RichTextPart
	{
		// Token: 0x1700001E RID: 30
		// (get) Token: 0x0600005C RID: 92 RVA: 0x00004460 File Offset: 0x00002660
		// (set) Token: 0x0600005D RID: 93 RVA: 0x00004468 File Offset: 0x00002668
		public string Style { get; set; }

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x0600005E RID: 94 RVA: 0x00004471 File Offset: 0x00002671
		// (set) Token: 0x0600005F RID: 95 RVA: 0x00004479 File Offset: 0x00002679
		internal TextMeshGenerator TextMeshGenerator { get; set; }

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06000060 RID: 96 RVA: 0x00004482 File Offset: 0x00002682
		// (set) Token: 0x06000061 RID: 97 RVA: 0x0000448A File Offset: 0x0000268A
		public DrawObject2D DrawObject2D { get; set; }

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x06000062 RID: 98 RVA: 0x00004493 File Offset: 0x00002693
		// (set) Token: 0x06000063 RID: 99 RVA: 0x0000449B File Offset: 0x0000269B
		public Font DefaultFont { get; set; }

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x06000064 RID: 100 RVA: 0x000044A4 File Offset: 0x000026A4
		// (set) Token: 0x06000065 RID: 101 RVA: 0x000044AC File Offset: 0x000026AC
		public float WordWidth { get; set; }

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x06000066 RID: 102 RVA: 0x000044B5 File Offset: 0x000026B5
		// (set) Token: 0x06000067 RID: 103 RVA: 0x000044BD File Offset: 0x000026BD
		public Vector2 PartPosition { get; set; }

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x06000068 RID: 104 RVA: 0x000044C6 File Offset: 0x000026C6
		// (set) Token: 0x06000069 RID: 105 RVA: 0x000044CE File Offset: 0x000026CE
		public Sprite Sprite { get; set; }

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x0600006A RID: 106 RVA: 0x000044D7 File Offset: 0x000026D7
		// (set) Token: 0x0600006B RID: 107 RVA: 0x000044DF File Offset: 0x000026DF
		public Vector2 SpritePosition { get; set; }

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x0600006C RID: 108 RVA: 0x000044E8 File Offset: 0x000026E8
		// (set) Token: 0x0600006D RID: 109 RVA: 0x000044F0 File Offset: 0x000026F0
		public RichTextPartType Type { get; set; }

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x0600006E RID: 110 RVA: 0x000044F9 File Offset: 0x000026F9
		// (set) Token: 0x0600006F RID: 111 RVA: 0x00004501 File Offset: 0x00002701
		public float Extend { get; set; }

		// Token: 0x06000070 RID: 112 RVA: 0x0000450A File Offset: 0x0000270A
		internal RichTextPart()
		{
			this.Style = "Default";
		}
	}
}
