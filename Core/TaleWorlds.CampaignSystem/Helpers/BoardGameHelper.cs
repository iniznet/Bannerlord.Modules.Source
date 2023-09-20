using System;

namespace Helpers
{
	// Token: 0x02000015 RID: 21
	public static class BoardGameHelper
	{
		// Token: 0x0200046D RID: 1133
		public enum AIDifficulty
		{
			// Token: 0x0400131F RID: 4895
			Easy,
			// Token: 0x04001320 RID: 4896
			Normal,
			// Token: 0x04001321 RID: 4897
			Hard,
			// Token: 0x04001322 RID: 4898
			NumTypes
		}

		// Token: 0x0200046E RID: 1134
		public enum BoardGameState
		{
			// Token: 0x04001324 RID: 4900
			None,
			// Token: 0x04001325 RID: 4901
			Win,
			// Token: 0x04001326 RID: 4902
			Loss,
			// Token: 0x04001327 RID: 4903
			Draw
		}
	}
}
