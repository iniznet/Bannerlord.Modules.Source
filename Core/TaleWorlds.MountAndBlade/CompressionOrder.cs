using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000301 RID: 769
	public static class CompressionOrder
	{
		// Token: 0x04000FD2 RID: 4050
		public static CompressionInfo.Integer OrderTypeCompressionInfo = new CompressionInfo.Integer(0, 45, true);

		// Token: 0x04000FD3 RID: 4051
		public static CompressionInfo.Integer FormationClassCompressionInfo = new CompressionInfo.Integer(-1, 10, true);

		// Token: 0x04000FD4 RID: 4052
		public static CompressionInfo.Float OrderPositionCompressionInfo = new CompressionInfo.Float(-100000f, 100000f, 24);
	}
}
