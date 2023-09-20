using System;

namespace TaleWorlds.TwoDimension.Standalone
{
	// Token: 0x02000006 RID: 6
	public class InputData
	{
		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000043 RID: 67 RVA: 0x0000358A File Offset: 0x0000178A
		// (set) Token: 0x06000044 RID: 68 RVA: 0x00003592 File Offset: 0x00001792
		public bool[] KeyData { get; set; }

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000045 RID: 69 RVA: 0x0000359B File Offset: 0x0000179B
		// (set) Token: 0x06000046 RID: 70 RVA: 0x000035A3 File Offset: 0x000017A3
		public bool LeftMouse { get; set; }

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000047 RID: 71 RVA: 0x000035AC File Offset: 0x000017AC
		// (set) Token: 0x06000048 RID: 72 RVA: 0x000035B4 File Offset: 0x000017B4
		public bool RightMouse { get; set; }

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000049 RID: 73 RVA: 0x000035BD File Offset: 0x000017BD
		// (set) Token: 0x0600004A RID: 74 RVA: 0x000035C5 File Offset: 0x000017C5
		public int CursorX { get; set; }

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x0600004B RID: 75 RVA: 0x000035CE File Offset: 0x000017CE
		// (set) Token: 0x0600004C RID: 76 RVA: 0x000035D6 File Offset: 0x000017D6
		public int CursorY { get; set; }

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x0600004D RID: 77 RVA: 0x000035DF File Offset: 0x000017DF
		// (set) Token: 0x0600004E RID: 78 RVA: 0x000035E7 File Offset: 0x000017E7
		public bool MouseMove { get; set; }

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x0600004F RID: 79 RVA: 0x000035F0 File Offset: 0x000017F0
		// (set) Token: 0x06000050 RID: 80 RVA: 0x000035F8 File Offset: 0x000017F8
		public float MouseScrollDelta { get; set; }

		// Token: 0x06000051 RID: 81 RVA: 0x00003604 File Offset: 0x00001804
		public InputData()
		{
			this.KeyData = new bool[256];
			this.CursorX = 0;
			this.CursorY = 0;
			this.LeftMouse = false;
			this.RightMouse = false;
			this.MouseMove = false;
			this.MouseScrollDelta = 0f;
			for (int i = 0; i < 256; i++)
			{
				this.KeyData[i] = false;
			}
		}

		// Token: 0x06000052 RID: 82 RVA: 0x0000366E File Offset: 0x0000186E
		public void Reset()
		{
			this.MouseScrollDelta = 0f;
		}

		// Token: 0x06000053 RID: 83 RVA: 0x0000367C File Offset: 0x0000187C
		public void FillFrom(InputData inputData)
		{
			this.CursorX = inputData.CursorX;
			this.CursorY = inputData.CursorY;
			this.LeftMouse = inputData.LeftMouse;
			this.RightMouse = inputData.RightMouse;
			this.MouseMove = inputData.MouseMove;
			this.MouseScrollDelta = inputData.MouseScrollDelta;
			for (int i = 0; i < 256; i++)
			{
				this.KeyData[i] = inputData.KeyData[i];
			}
		}
	}
}
