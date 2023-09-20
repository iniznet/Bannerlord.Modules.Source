using System;

namespace TaleWorlds.Network
{
	internal class MessageBuffer
	{
		internal MessageBuffer(byte[] buffer, int dataLength)
		{
			this.Buffer = buffer;
			this.DataLength = dataLength;
		}

		internal MessageBuffer(byte[] buffer)
		{
			this.Buffer = buffer;
		}

		internal byte[] Buffer { get; private set; }

		internal int DataLength { get; set; }

		internal string GetDebugText()
		{
			return BitConverter.ToString(this.Buffer, 0, this.DataLength);
		}

		internal const int MessageHeaderSize = 4;
	}
}
