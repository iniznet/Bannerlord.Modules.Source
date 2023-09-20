using System;
using System.Text;

namespace TaleWorlds.Library
{
	// Token: 0x02000018 RID: 24
	public class BinaryReader : IReader
	{
		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000046 RID: 70 RVA: 0x00002B85 File Offset: 0x00000D85
		// (set) Token: 0x06000047 RID: 71 RVA: 0x00002B8D File Offset: 0x00000D8D
		public byte[] Data { get; private set; }

		// Token: 0x06000048 RID: 72 RVA: 0x00002B96 File Offset: 0x00000D96
		public BinaryReader(byte[] data)
		{
			this.Data = data;
			this._cursor = 0;
			this._buffer = new byte[4];
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000049 RID: 73 RVA: 0x00002BB8 File Offset: 0x00000DB8
		public int UnreadByteCount
		{
			get
			{
				return this.Data.Length - this._cursor;
			}
		}

		// Token: 0x0600004A RID: 74 RVA: 0x00002BC9 File Offset: 0x00000DC9
		public ISerializableObject ReadSerializableObject()
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600004B RID: 75 RVA: 0x00002BD0 File Offset: 0x00000DD0
		public int Read3ByteInt()
		{
			this._buffer[0] = this.ReadByte();
			this._buffer[1] = this.ReadByte();
			this._buffer[2] = this.ReadByte();
			if (this._buffer[0] == 255 && this._buffer[1] == 255 && this._buffer[2] == 255)
			{
				this._buffer[3] = byte.MaxValue;
			}
			else
			{
				this._buffer[3] = 0;
			}
			return BitConverter.ToInt32(this._buffer, 0);
		}

		// Token: 0x0600004C RID: 76 RVA: 0x00002C58 File Offset: 0x00000E58
		public int ReadInt()
		{
			int num = BitConverter.ToInt32(this.Data, this._cursor);
			this._cursor += 4;
			return num;
		}

		// Token: 0x0600004D RID: 77 RVA: 0x00002C79 File Offset: 0x00000E79
		public short ReadShort()
		{
			short num = BitConverter.ToInt16(this.Data, this._cursor);
			this._cursor += 2;
			return num;
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00002C9C File Offset: 0x00000E9C
		public void ReadFloats(float[] output, int count)
		{
			int num = count * 4;
			Buffer.BlockCopy(this.Data, this._cursor, output, 0, num);
			this._cursor += num;
		}

		// Token: 0x0600004F RID: 79 RVA: 0x00002CD0 File Offset: 0x00000ED0
		public void ReadShorts(short[] output, int count)
		{
			int num = count * 2;
			Buffer.BlockCopy(this.Data, this._cursor, output, 0, num);
			this._cursor += num;
		}

		// Token: 0x06000050 RID: 80 RVA: 0x00002D04 File Offset: 0x00000F04
		public string ReadString()
		{
			int num = this.ReadInt();
			string text = null;
			if (num >= 0)
			{
				text = Encoding.UTF8.GetString(this.Data, this._cursor, num);
				this._cursor += num;
			}
			return text;
		}

		// Token: 0x06000051 RID: 81 RVA: 0x00002D48 File Offset: 0x00000F48
		public Color ReadColor()
		{
			float num = this.ReadFloat();
			float num2 = this.ReadFloat();
			float num3 = this.ReadFloat();
			float num4 = this.ReadFloat();
			return new Color(num, num2, num3, num4);
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00002D7A File Offset: 0x00000F7A
		public bool ReadBool()
		{
			int num = (int)this.Data[this._cursor];
			this._cursor++;
			return num == 1;
		}

		// Token: 0x06000053 RID: 83 RVA: 0x00002D9A File Offset: 0x00000F9A
		public float ReadFloat()
		{
			float num = BitConverter.ToSingle(this.Data, this._cursor);
			this._cursor += 4;
			return num;
		}

		// Token: 0x06000054 RID: 84 RVA: 0x00002DBB File Offset: 0x00000FBB
		public uint ReadUInt()
		{
			uint num = BitConverter.ToUInt32(this.Data, this._cursor);
			this._cursor += 4;
			return num;
		}

		// Token: 0x06000055 RID: 85 RVA: 0x00002DDC File Offset: 0x00000FDC
		public ulong ReadULong()
		{
			ulong num = BitConverter.ToUInt64(this.Data, this._cursor);
			this._cursor += 8;
			return num;
		}

		// Token: 0x06000056 RID: 86 RVA: 0x00002DFD File Offset: 0x00000FFD
		public long ReadLong()
		{
			long num = BitConverter.ToInt64(this.Data, this._cursor);
			this._cursor += 8;
			return num;
		}

		// Token: 0x06000057 RID: 87 RVA: 0x00002E1E File Offset: 0x0000101E
		public byte ReadByte()
		{
			byte b = this.Data[this._cursor];
			this._cursor++;
			return b;
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00002E3C File Offset: 0x0000103C
		public byte[] ReadBytes(int length)
		{
			byte[] array = new byte[length];
			Array.Copy(this.Data, this._cursor, array, 0, length);
			this._cursor += length;
			return array;
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00002E74 File Offset: 0x00001074
		public Vec2 ReadVec2()
		{
			float num = this.ReadFloat();
			float num2 = this.ReadFloat();
			return new Vec2(num, num2);
		}

		// Token: 0x0600005A RID: 90 RVA: 0x00002E94 File Offset: 0x00001094
		public Vec3 ReadVec3()
		{
			float num = this.ReadFloat();
			float num2 = this.ReadFloat();
			float num3 = this.ReadFloat();
			float num4 = this.ReadFloat();
			return new Vec3(num, num2, num3, num4);
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00002EC4 File Offset: 0x000010C4
		public Vec3i ReadVec3Int()
		{
			int num = this.ReadInt();
			int num2 = this.ReadInt();
			int num3 = this.ReadInt();
			return new Vec3i(num, num2, num3);
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00002EEC File Offset: 0x000010EC
		public sbyte ReadSByte()
		{
			sbyte b = (sbyte)this.Data[this._cursor];
			this._cursor++;
			return b;
		}

		// Token: 0x0600005D RID: 93 RVA: 0x00002F0A File Offset: 0x0000110A
		public ushort ReadUShort()
		{
			ushort num = BitConverter.ToUInt16(this.Data, this._cursor);
			this._cursor += 2;
			return num;
		}

		// Token: 0x0600005E RID: 94 RVA: 0x00002F2B File Offset: 0x0000112B
		public double ReadDouble()
		{
			double num = BitConverter.ToDouble(this.Data, this._cursor);
			this._cursor += 8;
			return num;
		}

		// Token: 0x04000052 RID: 82
		private int _cursor;

		// Token: 0x04000053 RID: 83
		private byte[] _buffer;
	}
}
