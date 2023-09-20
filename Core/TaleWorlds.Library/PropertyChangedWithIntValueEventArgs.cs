using System;

namespace TaleWorlds.Library
{
	// Token: 0x02000047 RID: 71
	public class PropertyChangedWithIntValueEventArgs
	{
		// Token: 0x06000239 RID: 569 RVA: 0x00006B01 File Offset: 0x00004D01
		public PropertyChangedWithIntValueEventArgs(string propertyName, int value)
		{
			this.PropertyName = propertyName;
			this.Value = value;
		}

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x0600023A RID: 570 RVA: 0x00006B17 File Offset: 0x00004D17
		public string PropertyName { get; }

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x0600023B RID: 571 RVA: 0x00006B1F File Offset: 0x00004D1F
		public int Value { get; }
	}
}
