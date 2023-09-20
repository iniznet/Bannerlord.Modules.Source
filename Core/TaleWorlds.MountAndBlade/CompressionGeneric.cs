using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020002FE RID: 766
	public static class CompressionGeneric
	{
		// Token: 0x04000F84 RID: 3972
		public static CompressionInfo.UnsignedInteger ColorCompressionInfo = new CompressionInfo.UnsignedInteger(0U, 32);

		// Token: 0x04000F85 RID: 3973
		public static CompressionInfo.Integer ItemDataValueCompressionInfo = new CompressionInfo.Integer(0, 16);

		// Token: 0x04000F86 RID: 3974
		public static CompressionInfo.Integer RandomSeedCompressionInfo = new CompressionInfo.Integer(0, 2000, true);
	}
}
