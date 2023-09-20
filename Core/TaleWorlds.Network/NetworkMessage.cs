using System;
using System.Text;

namespace TaleWorlds.Network
{
	// Token: 0x0200001E RID: 30
	public class NetworkMessage : INetworkMessageWriter, INetworkMessageReader
	{
		// Token: 0x17000020 RID: 32
		// (get) Token: 0x060000A0 RID: 160 RVA: 0x0000337E File Offset: 0x0000157E
		private byte[] Buffer
		{
			get
			{
				return this.MessageBuffer.Buffer;
			}
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x060000A1 RID: 161 RVA: 0x0000338B File Offset: 0x0000158B
		// (set) Token: 0x060000A2 RID: 162 RVA: 0x00003393 File Offset: 0x00001593
		internal MessageBuffer MessageBuffer { get; private set; }

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x060000A3 RID: 163 RVA: 0x0000339C File Offset: 0x0000159C
		// (set) Token: 0x060000A4 RID: 164 RVA: 0x000033AB File Offset: 0x000015AB
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

		// Token: 0x060000A5 RID: 165 RVA: 0x000033BB File Offset: 0x000015BB
		private NetworkMessage()
		{
			this._writeCursor = 4;
			this._readCursor = 4;
			this._finalized = false;
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x000033D8 File Offset: 0x000015D8
		internal static NetworkMessage CreateForReading(MessageBuffer messageBuffer)
		{
			return new NetworkMessage
			{
				MessageBuffer = messageBuffer
			};
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x000033E6 File Offset: 0x000015E6
		internal static NetworkMessage CreateForWriting()
		{
			NetworkMessage networkMessage = new NetworkMessage();
			networkMessage.MessageBuffer = new MessageBuffer(new byte[16777216]);
			networkMessage.Reset();
			return networkMessage;
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x00003408 File Offset: 0x00001608
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

		// Token: 0x060000A9 RID: 169 RVA: 0x00003498 File Offset: 0x00001698
		public void Write(int data)
		{
			byte[] bytes = BitConverter.GetBytes(data);
			this.Buffer[this._writeCursor] = bytes[0];
			this.Buffer[this._writeCursor + 1] = bytes[1];
			this.Buffer[this._writeCursor + 2] = bytes[2];
			this.Buffer[this._writeCursor + 3] = bytes[3];
			this._writeCursor += 4;
		}

		// Token: 0x060000AA RID: 170 RVA: 0x00003500 File Offset: 0x00001700
		public void Write(short data)
		{
			byte[] bytes = BitConverter.GetBytes(data);
			this.Buffer[this._writeCursor] = bytes[0];
			this.Buffer[this._writeCursor + 1] = bytes[1];
			this._writeCursor += 2;
		}

		// Token: 0x060000AB RID: 171 RVA: 0x00003544 File Offset: 0x00001744
		public void Write(bool data)
		{
			byte[] bytes = BitConverter.GetBytes(data);
			this.Buffer[this._writeCursor] = bytes[0];
			this._writeCursor++;
		}

		// Token: 0x060000AC RID: 172 RVA: 0x00003576 File Offset: 0x00001776
		public void Write(byte data)
		{
			this.Buffer[this._writeCursor] = data;
			this._writeCursor++;
		}

		// Token: 0x060000AD RID: 173 RVA: 0x00003594 File Offset: 0x00001794
		public void Write(float data)
		{
			byte[] bytes = BitConverter.GetBytes(data);
			this.Buffer[this._writeCursor] = bytes[0];
			this.Buffer[this._writeCursor + 1] = bytes[1];
			this.Buffer[this._writeCursor + 2] = bytes[2];
			this.Buffer[this._writeCursor + 3] = bytes[3];
			this._writeCursor += 4;
		}

		// Token: 0x060000AE RID: 174 RVA: 0x000035FC File Offset: 0x000017FC
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

		// Token: 0x060000AF RID: 175 RVA: 0x000036AC File Offset: 0x000018AC
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

		// Token: 0x060000B0 RID: 176 RVA: 0x0000375C File Offset: 0x0000195C
		public void Write(Guid data)
		{
			byte[] array = data.ToByteArray();
			for (int i = 0; i < array.Length; i++)
			{
				this.Buffer[this._writeCursor + i] = array[i];
			}
			this._writeCursor += array.Length;
		}

		// Token: 0x060000B1 RID: 177 RVA: 0x000037A4 File Offset: 0x000019A4
		public void Write(byte[] data)
		{
			this.Write(data.Length);
			for (int i = 0; i < data.Length; i++)
			{
				this.Buffer[this._writeCursor + i] = data[i];
			}
			this._writeCursor += data.Length;
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x000037EA File Offset: 0x000019EA
		internal void Write(MessageContract messageContract)
		{
			this.Write(messageContract.MessageId);
			messageContract.SerializeToNetworkMessage(this);
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x000037FF File Offset: 0x000019FF
		public int ReadInt32()
		{
			int num = BitConverter.ToInt32(this.Buffer, this._readCursor);
			this._readCursor += 4;
			return num;
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x00003820 File Offset: 0x00001A20
		public short ReadInt16()
		{
			short num = BitConverter.ToInt16(this.Buffer, this._readCursor);
			this._readCursor += 2;
			return num;
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x00003841 File Offset: 0x00001A41
		public bool ReadBoolean()
		{
			bool flag = BitConverter.ToBoolean(this.Buffer, this._readCursor);
			this._readCursor++;
			return flag;
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x00003862 File Offset: 0x00001A62
		public byte ReadByte()
		{
			byte b = this.Buffer[this._readCursor];
			this._readCursor++;
			return b;
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x00003880 File Offset: 0x00001A80
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

		// Token: 0x060000B8 RID: 184 RVA: 0x000038C1 File Offset: 0x00001AC1
		public float ReadFloat()
		{
			float num = BitConverter.ToSingle(this.Buffer, this._readCursor);
			this._readCursor += 4;
			return num;
		}

		// Token: 0x060000B9 RID: 185 RVA: 0x000038E2 File Offset: 0x00001AE2
		public long ReadInt64()
		{
			long num = BitConverter.ToInt64(this.Buffer, this._readCursor);
			this._readCursor += 8;
			return num;
		}

		// Token: 0x060000BA RID: 186 RVA: 0x00003903 File Offset: 0x00001B03
		public ulong ReadUInt64()
		{
			ulong num = BitConverter.ToUInt64(this.Buffer, this._readCursor);
			this._readCursor += 8;
			return num;
		}

		// Token: 0x060000BB RID: 187 RVA: 0x00003924 File Offset: 0x00001B24
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

		// Token: 0x060000BC RID: 188 RVA: 0x00003970 File Offset: 0x00001B70
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

		// Token: 0x060000BD RID: 189 RVA: 0x000039BA File Offset: 0x00001BBA
		internal void Reset()
		{
			this._writeCursor = 4;
			this._readCursor = 4;
			this._finalized = false;
			this.DataLength = 0;
		}

		// Token: 0x060000BE RID: 190 RVA: 0x000039D8 File Offset: 0x00001BD8
		internal void ResetRead()
		{
			this._readCursor = 4;
		}

		// Token: 0x060000BF RID: 191 RVA: 0x000039E1 File Offset: 0x00001BE1
		internal void BeginRead()
		{
			this._readCursor = 4;
		}

		// Token: 0x060000C0 RID: 192 RVA: 0x000039EA File Offset: 0x00001BEA
		internal void BeginWrite()
		{
			this._writeCursor = 4;
			this._finalized = false;
		}

		// Token: 0x060000C1 RID: 193 RVA: 0x000039FA File Offset: 0x00001BFA
		internal void FinalizeWrite()
		{
			if (!this._finalized)
			{
				this._finalized = true;
				this.DataLength = this._writeCursor - 4;
			}
		}

		// Token: 0x060000C2 RID: 194 RVA: 0x00003A1C File Offset: 0x00001C1C
		internal void UpdateHeader()
		{
			this.Buffer[0] = (byte)(this.DataLength & 255);
			this.Buffer[1] = (byte)((this.DataLength >> 8) & 255);
			this.Buffer[2] = (byte)((this.DataLength >> 16) & 255);
			this.Buffer[3] = (byte)((this.DataLength >> 24) & 255);
		}

		// Token: 0x060000C3 RID: 195 RVA: 0x00003A85 File Offset: 0x00001C85
		internal string GetDebugText()
		{
			return BitConverter.ToString(this.Buffer, 0, this.DataLength);
		}

		// Token: 0x0400003B RID: 59
		private int _readCursor;

		// Token: 0x0400003C RID: 60
		private int _writeCursor;

		// Token: 0x0400003D RID: 61
		private bool _finalized;
	}
}
