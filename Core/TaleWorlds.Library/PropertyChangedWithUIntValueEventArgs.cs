using System;

namespace TaleWorlds.Library
{
	// Token: 0x02000049 RID: 73
	public class PropertyChangedWithUIntValueEventArgs
	{
		// Token: 0x0600023F RID: 575 RVA: 0x00006B4D File Offset: 0x00004D4D
		public PropertyChangedWithUIntValueEventArgs(string propertyName, uint value)
		{
			this.PropertyName = propertyName;
			this.Value = value;
		}

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x06000240 RID: 576 RVA: 0x00006B63 File Offset: 0x00004D63
		public string PropertyName { get; }

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x06000241 RID: 577 RVA: 0x00006B6B File Offset: 0x00004D6B
		public uint Value { get; }
	}
}
