using System;

namespace SandBox.ViewModelCollection.GameOver
{
	// Token: 0x02000039 RID: 57
	public class StatItem
	{
		// Token: 0x0600041C RID: 1052 RVA: 0x00012834 File Offset: 0x00010A34
		public StatItem(string id, string value, StatItem.StatType type = StatItem.StatType.None)
		{
			this.ID = id;
			this.Value = value;
			this.Type = type;
		}

		// Token: 0x04000220 RID: 544
		public readonly string ID;

		// Token: 0x04000221 RID: 545
		public readonly string Value;

		// Token: 0x04000222 RID: 546
		public readonly StatItem.StatType Type;

		// Token: 0x020000A2 RID: 162
		public enum StatType
		{
			// Token: 0x04000363 RID: 867
			None,
			// Token: 0x04000364 RID: 868
			Influence,
			// Token: 0x04000365 RID: 869
			Issue,
			// Token: 0x04000366 RID: 870
			Tournament,
			// Token: 0x04000367 RID: 871
			Gold,
			// Token: 0x04000368 RID: 872
			Crime,
			// Token: 0x04000369 RID: 873
			Kill
		}
	}
}
