using System;

namespace TaleWorlds.Library
{
	// Token: 0x0200004C RID: 76
	public class PropertyChangedWithVec2ValueEventArgs
	{
		// Token: 0x06000248 RID: 584 RVA: 0x00006BBF File Offset: 0x00004DBF
		public PropertyChangedWithVec2ValueEventArgs(string propertyName, Vec2 value)
		{
			this.PropertyName = propertyName;
			this.Value = value;
		}

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x06000249 RID: 585 RVA: 0x00006BD5 File Offset: 0x00004DD5
		public string PropertyName { get; }

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x0600024A RID: 586 RVA: 0x00006BDD File Offset: 0x00004DDD
		public Vec2 Value { get; }
	}
}
