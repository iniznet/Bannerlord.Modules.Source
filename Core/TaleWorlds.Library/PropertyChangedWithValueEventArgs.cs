using System;

namespace TaleWorlds.Library
{
	// Token: 0x02000045 RID: 69
	public class PropertyChangedWithValueEventArgs
	{
		// Token: 0x06000233 RID: 563 RVA: 0x00006AB5 File Offset: 0x00004CB5
		public PropertyChangedWithValueEventArgs(string propertyName, object value)
		{
			this.PropertyName = propertyName;
			this.Value = value;
		}

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x06000234 RID: 564 RVA: 0x00006ACB File Offset: 0x00004CCB
		public string PropertyName { get; }

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x06000235 RID: 565 RVA: 0x00006AD3 File Offset: 0x00004CD3
		public object Value { get; }
	}
}
