using System;

namespace TaleWorlds.MountAndBlade
{
	public static class CompressionGeneric
	{
		public static CompressionInfo.UnsignedInteger ColorCompressionInfo = new CompressionInfo.UnsignedInteger(0U, 32);

		public static CompressionInfo.Integer ItemDataValueCompressionInfo = new CompressionInfo.Integer(0, 16);

		public static CompressionInfo.Integer RandomSeedCompressionInfo = new CompressionInfo.Integer(0, 2000, true);
	}
}
