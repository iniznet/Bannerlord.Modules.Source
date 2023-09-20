using System;

namespace TaleWorlds.Library
{
	// Token: 0x02000048 RID: 72
	public class PropertyChangedWithFloatValueEventArgs
	{
		// Token: 0x0600023C RID: 572 RVA: 0x00006B27 File Offset: 0x00004D27
		public PropertyChangedWithFloatValueEventArgs(string propertyName, float value)
		{
			this.PropertyName = propertyName;
			this.Value = value;
		}

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x0600023D RID: 573 RVA: 0x00006B3D File Offset: 0x00004D3D
		public string PropertyName { get; }

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x0600023E RID: 574 RVA: 0x00006B45 File Offset: 0x00004D45
		public float Value { get; }
	}
}
