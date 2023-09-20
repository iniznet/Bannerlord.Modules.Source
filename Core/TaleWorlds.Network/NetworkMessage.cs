using System;
using System.Text;

namespace TaleWorlds.Network
{
	public class NetworkMessage : INetworkMessageWriter, INetworkMessageReader
	{
		private byte[] Buffer
		{
			get
			{
				return this.MessageBuffer.Buffer;
			}
		}

		internal MessageBuffer MessageBuffer { get; private set; }

		internal int DataLength
		{
			get
			{
				return this.MessageBuffer.DataLength - 4;
			}
			set
			{
				this.MessageBuffer.DataLength = value + 4;
			}
		}

		private NetworkMessage()
		{
			this._writeCursor = 4;
			this._readCursor = 4;
			this._finalized = false;
		}

		internal static NetworkMessage CreateForReading(MessageBuffer messageBuffer)
		{
			return new NetworkMessage
			{
				MessageBuffer = messageBuffer
			};
		}

		internal static NetworkMessage CreateForWriting()
		{
			NetworkMessage networkMessage = new NetworkMessage();
			networkMessage.MessageBuffer = new MessageBuffer(new byte[16777216]);
			networkMessage.Reset();
			return networkMessage;
		}

		public void Write(string data)
		{
			int writeCursor = this._writeCursor;
			this._writeCursor += 4;
			int num;
			if (data != null)
			{
				num = Encoding.UTF8.GetBytes(data, 0, data.Length, this.Buffer, this._writeCursor);
				this._writeCursor += num;
			}
			else
			{
				num = -1;
			}
			byte[] bytes = BitConverter.GetBytes(num);
			this.Buffer[writeCursor] = bytes[0];
			this.Buffer[writeCursor + 1] = bytes[1];
			this.Buffer[writeCursor + 2] = bytes[2];
			this.Buffer[writeCursor + 3] = bytes[3];
		}

		public void Write(int data)
		{
			byte[] bytes = BitConverter.GetBytes(data);
			this.Buffer[this._writeCursor] = bytes[0];
			this.Buffer[this._writeCursor + 1] = bytes[1];
			this.Buffer[this._writeCursor + 2] = bytes[2];
			this.Buffer[this._writeCursor + 3] = bytes[3];
			this._writeCursor += 4;
		}

		public void Write(short data)
		{
			byte[] bytes = BitConverter.GetBytes(data);
			this.Buffer[this._writeCursor] = bytes[0];
			this.Buffer[this._writeCursor + 1] = bytes[1];
			this._writeCursor += 2;
		}

		public void Write(bool data)
		{
			byte[] bytes = BitConverter.GetBytes(data);
			this.Buffer[this._writeCursor] = bytes[0];
			this._writeCursor++;
		}

		public void Write(byte data)
		{
			this.Buffer[this._writeCursor] = data;
			this._writeCursor++;
		}

		public void Write(float data)
		{
			byte[] bytes = BitConverter.GetBytes(data);
			this.Buffer[this._writeCursor] = bytes[0];
			this.Buffer[this._writeCursor + 1] = bytes[1];
			this.Buffer[this._writeCursor + 2] = bytes[2];
			this.Buffer[this._writeCursor + 3] = bytes[3];
			this._writeCursor += 4;
		}

		public void Write(long data)
		{
			byte[] bytes = BitConverter.GetBytes(data);
			this.Buffer[this._writeCursor] = bytes[0];
			this.Buffer[this._writeCursor + 1] = bytes[1];
			this.Buffer[this._writeCursor + 2] = bytes[2];
			this.Buffer[this._writeCursor + 3] = bytes[3];
			this.Buffer[this._writeCursor + 4] = bytes[4];
			this.Buffer[this._writeCursor + 5] = bytes[5];
			this.Buffer[this._writeCursor + 6] = bytes[6];
			this.Buffer[this._writeCursor + 7] = bytes[7];
			this._writeCursor += 8;
		}

		public void Write(ulong data)
		{
			byte[] bytes = BitConverter.GetBytes(data);
			this.Buffer[this._writeCursor] = bytes[0];
			this.Buffer[this._writeCursor + 1] = bytes[1];
			this.Buffer[this._writeCursor + 2] = bytes[2];
			this.Buffer[this._writeCursor + 3] = bytes[3];
			this.Buffer[this._writeCursor + 4] = bytes[4];
			this.Buffer[this._writeCursor + 5] = bytes[5];
			this.Buffer[this._writeCursor + 6] = bytes[6];
			this.Buffer[this._writeCursor + 7] = bytes[7];
			this._writeCursor += 8;
		}

		public void Write(Guid data)
		{
			byte[] array = data.ToByteArray();
			for (int i = 0; i < array.Length; i++)
			{
				this.Buffer[this._writeCursor + i] = array[i];
			}
			this._writeCursor += array.Length;
		}

		public void Write(byte[] data)
		{
			this.Write(data.Length);
			for (int i = 0; i < data.Length; i++)
			{
				this.Buffer[this._writeCursor + i] = data[i];
			}
			this._writeCursor += data.Length;
		}

		internal void Write(MessageContract messageContract)
		{
			this.Write(messageContract.MessageId);
			messageContract.SerializeToNetworkMessage(this);
		}

		public int ReadInt32()
		{
			int num = BitConverter.ToInt32(this.Buffer, this._readCursor);
			this._readCursor += 4;
			return num;
		}

		public short ReadInt16()
		{
			short num = BitConverter.ToInt16(this.Buffer, this._readCursor);
			this._readCursor += 2;
			return num;
		}

		public bool ReadBoolean()
		{
			bool flag = BitConverter.ToBoolean(this.Buffer, this._readCursor);
			this._readCursor++;
			return flag;
		}

		public byte ReadByte()
		{
			byte b = this.Buffer[this._readCursor];
			this._readCursor++;
			return b;
		}

		public string ReadString()
		{
			int num = this.ReadInt32();
			string text = null;
			if (num >= 0)
			{
				text = Encoding.UTF8.GetString(this.Buffer, this._readCursor, num);
				this._readCursor += num;
			}
			return text;
		}

		public float ReadFloat()
		{
			float num = BitConverter.ToSingle(this.Buffer, this._readCursor);
			this._readCursor += 4;
			return num;
		}

		public long ReadInt64()
		{
			long num = BitConverter.ToInt64(this.Buffer, this._readCursor);
			this._readCursor += 8;
			return num;
		}

		public ulong ReadUInt64()
		{
			ulong num = BitConverter.ToUInt64(this.Buffer, this._readCursor);
			this._readCursor += 8;
			return num;
		}

		public Guid ReadGuid()
		{
			byte[] array = new byte[16];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = this.Buffer[this._readCursor + i];
			}
			this._readCursor += array.Length;
			return new Guid(array);
		}

		public byte[] ReadByteArray()
		{
			byte[] array = new byte[this.ReadInt32()];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = this.Buffer[this._readCursor + i];
			}
			this._readCursor += array.Length;
			return array;
		}

		internal void Reset()
		{
			this._writeCursor = 4;
			this._readCursor = 4;
			this._finalized = false;
			this.DataLength = 0;
		}

		internal void ResetRead()
		{
			this._readCursor = 4;
		}

		internal void BeginRead()
		{
			this._readCursor = 4;
		}

		internal void BeginWrite()
		{
			this._writeCursor = 4;
			this._finalized = false;
		}

		internal void FinalizeWrite()
		{
			if (!this._finalized)
			{
				this._finalized = true;
				this.DataLength = this._writeCursor - 4;
			}
		}

		internal void UpdateHeader()
		{
			this.Buffer[0] = (byte)(this.DataLength & 255);
			this.Buffer[1] = (byte)((this.DataLength >> 8) & 255);
			this.Buffer[2] = (byte)((this.DataLength >> 16) & 255);
			this.Buffer[3] = (byte)((this.DataLength >> 24) & 255);
		}

		internal string GetDebugText()
		{
			return BitConverter.ToString(this.Buffer, 0, this.DataLength);
		}

		private int _readCursor;

		private int _writeCursor;

		private bool _finalized;
	}
}
