using System;

namespace TaleWorlds.MountAndBlade
{
	public static class CompressionOrder
	{
		public static CompressionInfo.Integer OrderTypeCompressionInfo = new CompressionInfo.Integer(0, 45, true);

		public static CompressionInfo.Integer FormationClassCompressionInfo = new CompressionInfo.Integer(-1, 10, true);

		public static CompressionInfo.Float OrderPositionCompressionInfo = new CompressionInfo.Float(-100000f, 100000f, 24);
	}
}
