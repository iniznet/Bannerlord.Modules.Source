using System;

namespace TaleWorlds.Library
{
	// Token: 0x0200004B RID: 75
	public class PropertyChangedWithDoubleValueEventArgs
	{
		// Token: 0x06000245 RID: 581 RVA: 0x00006B99 File Offset: 0x00004D99
		public PropertyChangedWithDoubleValueEventArgs(string propertyName, double value)
		{
			this.PropertyName = propertyName;
			this.Value = value;
		}

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x06000246 RID: 582 RVA: 0x00006BAF File Offset: 0x00004DAF
		public string PropertyName { get; }

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x06000247 RID: 583 RVA: 0x00006BB7 File Offset: 0x00004DB7
		public double Value { get; }
	}
}
