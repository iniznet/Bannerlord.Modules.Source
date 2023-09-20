using System;
using System.Text;

namespace TaleWorlds.Library
{
	public class BinaryReader : IReader
	{
		public byte[] Data { get; private set; }

		public BinaryReader(byte[] data)
		{
			this.Data = data;
			this._cursor = 0;
			this._buffer = new byte[4];
		}

		public int UnreadByteCount
		{
			get
			{
				return this.Data.Length - this._cursor;
			}
		}

		public ISerializableObject ReadSerializableObject()
		{
			throw new NotImplementedException();
		}

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

		public int ReadInt()
		{
			int num = BitConverter.ToInt32(this.Data, this._cursor);
			this._cursor += 4;
			return num;
		}

		public short ReadShort()
		{
			short num = BitConverter.ToInt16(this.Data, this._cursor);
			this._cursor += 2;
			return num;
		}

		public void ReadFloats(float[] output, int count)
		{
			int num = count * 4;
			Buffer.BlockCopy(this.Data, this._cursor, output, 0, num);
			this._cursor += num;
		}

		public void ReadShorts(short[] output, int count)
		{
			int num = count * 2;
			Buffer.BlockCopy(this.Data, this._cursor, output, 0, num);
			this._cursor += num;
		}

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

		public Color ReadColor()
		{
			float num = this.ReadFloat();
			float num2 = this.ReadFloat();
			float num3 = this.ReadFloat();
			float num4 = this.ReadFloat();
			return new Color(num, num2, num3, num4);
		}

		public bool ReadBool()
		{
			int num = (int)this.Data[this._cursor];
			this._cursor++;
			return num == 1;
		}

		public float ReadFloat()
		{
			float num = BitConverter.ToSingle(this.Data, this._cursor);
			this._cursor += 4;
			return num;
		}

		public uint ReadUInt()
		{
			uint num = BitConverter.ToUInt32(this.Data, this._cursor);
			this._cursor += 4;
			return num;
		}

		public ulong ReadULong()
		{
			ulong num = BitConverter.ToUInt64(this.Data, this._cursor);
			this._cursor += 8;
			return num;
		}

		public long ReadLong()
		{
			long num = BitConverter.ToInt64(this.Data, this._cursor);
			this._cursor += 8;
			return num;
		}

		public byte ReadByte()
		{
			byte b = this.Data[this._cursor];
			this._cursor++;
			return b;
		}

		public byte[] ReadBytes(int length)
		{
			byte[] array = new byte[length];
			Array.Copy(this.Data, this._cursor, array, 0, length);
			this._cursor += length;
			return array;
		}

		public Vec2 ReadVec2()
		{
			float num = this.ReadFloat();
			float num2 = this.ReadFloat();
			return new Vec2(num, num2);
		}

		public Vec3 ReadVec3()
		{
			float num = this.ReadFloat();
			float num2 = this.ReadFloat();
			float num3 = this.ReadFloat();
			float num4 = this.ReadFloat();
			return new Vec3(num, num2, num3, num4);
		}

		public Vec3i ReadVec3Int()
		{
			int num = this.ReadInt();
			int num2 = this.ReadInt();
			int num3 = this.ReadInt();
			return new Vec3i(num, num2, num3);
		}

		public sbyte ReadSByte()
		{
			sbyte b = (sbyte)this.Data[this._cursor];
			this._cursor++;
			return b;
		}

		public ushort ReadUShort()
		{
			ushort num = BitConverter.ToUInt16(this.Data, this._cursor);
			this._cursor += 2;
			return num;
		}

		public double ReadDouble()
		{
			double num = BitConverter.ToDouble(this.Data, this._cursor);
			this._cursor += 8;
			return num;
		}

		private int _cursor;

		private byte[] _buffer;
	}
}
