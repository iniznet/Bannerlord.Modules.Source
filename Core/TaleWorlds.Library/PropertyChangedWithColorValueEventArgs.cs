using System;

namespace TaleWorlds.Library
{
	// Token: 0x0200004A RID: 74
	public class PropertyChangedWithColorValueEventArgs
	{
		// Token: 0x06000242 RID: 578 RVA: 0x00006B73 File Offset: 0x00004D73
		public PropertyChangedWithColorValueEventArgs(string propertyName, Color value)
		{
			this.PropertyName = propertyName;
			this.Value = value;
		}

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x06000243 RID: 579 RVA: 0x00006B89 File Offset: 0x00004D89
		public string PropertyName { get; }

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x06000244 RID: 580 RVA: 0x00006B91 File Offset: 0x00004D91
		public Color Value { get; }
	}
}
