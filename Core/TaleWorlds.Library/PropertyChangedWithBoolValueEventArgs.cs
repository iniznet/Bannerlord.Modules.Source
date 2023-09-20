using System;

namespace TaleWorlds.Library
{
	// Token: 0x02000046 RID: 70
	public class PropertyChangedWithBoolValueEventArgs
	{
		// Token: 0x06000236 RID: 566 RVA: 0x00006ADB File Offset: 0x00004CDB
		public PropertyChangedWithBoolValueEventArgs(string propertyName, bool value)
		{
			this.PropertyName = propertyName;
			this.Value = value;
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x06000237 RID: 567 RVA: 0x00006AF1 File Offset: 0x00004CF1
		public string PropertyName { get; }

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x06000238 RID: 568 RVA: 0x00006AF9 File Offset: 0x00004CF9
		public bool Value { get; }
	}
}
