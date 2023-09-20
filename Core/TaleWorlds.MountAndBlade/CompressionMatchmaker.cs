using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020002FF RID: 767
	public static class CompressionMatchmaker
	{
		// Token: 0x04000F87 RID: 3975
		public static CompressionInfo.Integer KillDeathAssistCountCompressionInfo = new CompressionInfo.Integer(-1000, 100000, true);

		// Token: 0x04000F88 RID: 3976
		public static CompressionInfo.Float MissionTimeCompressionInfo = new CompressionInfo.Float(-5f, 86400f, 20);

		// Token: 0x04000F89 RID: 3977
		public static CompressionInfo.Float MissionTimeLowPrecisionCompressionInfo = new CompressionInfo.Float(-5f, 12, 4f);

		// Token: 0x04000F8A RID: 3978
		public static CompressionInfo.Integer MissionCurrentStateCompressionInfo = new CompressionInfo.Integer(0, 6);

		// Token: 0x04000F8B RID: 3979
		public static CompressionInfo.Integer ScoreCompressionInfo = new CompressionInfo.Integer(-1000000, 21);
	}
}
