using System;
using System.Text;

namespace TaleWorlds.Library
{
	public class BinaryWriter : IWriter
	{
		public byte[] Data
		{
			get
			{
				byte[] array = new byte[this._availableIndex];
				Buffer.BlockCopy(this._data, 0, array, 0, this._availableIndex);
				return array;
			}
		}

		public int Length
		{
			get
			{
				return this._availableIndex;
			}
		}

		public BinaryWriter()
		{
			this._data = new byte[4096];
			this._availableIndex = 0;
		}

		public BinaryWriter(int capacity)
		{
			this._data = new byte[capacity];
			this._availableIndex = 0;
		}

		public void Clear()
		{
			this._availableIndex = 0;
		}

		public void EnsureLength(int added)
		{
			int num = this._availableIndex + added;
			if (num > this._data.Length)
			{
				int num2 = this._data.Length * 2;
				if (num > num2)
				{
					num2 = num;
				}
				byte[] array = new byte[num2];
				Buffer.BlockCopy(this._data, 0, array, 0, this._availableIndex);
				this._data = array;
			}
		}

		public void WriteSerializableObject(ISerializableObject serializableObject)
		{
			throw new NotImplementedException();
		}

		public void WriteByte(byte value)
		{
			this.EnsureLength(1);
			this._data[this._availableIndex] = value;
			this._availableIndex++;
		}

		public void WriteBytes(byte[] bytes)
		{
			this.EnsureLength(bytes.Length);
			Buffer.BlockCopy(bytes, 0, this._data, this._availableIndex, bytes.Length);
			this._availableIndex += bytes.Length;
		}

		public void Write3ByteInt(int value)
		{
			this.EnsureLength(3);
			byte[] data = this._data;
			int num = this._availableIndex;
			this._availableIndex = num + 1;
			data[num] = (byte)value;
			byte[] data2 = this._data;
			num = this._availableIndex;
			this._availableIndex = num + 1;
			data2[num] = (byte)(value >> 8);
			byte[] data3 = this._data;
			num = this._availableIndex;
			this._availableIndex = num + 1;
			data3[num] = (byte)(value >> 16);
		}

		public void WriteInt(int value)
		{
			this.EnsureLength(4);
			byte[] data = this._data;
			int num = this._availableIndex;
			this._availableIndex = num + 1;
			data[num] = (byte)value;
			byte[] data2 = this._data;
			num = this._availableIndex;
			this._availableIndex = num + 1;
			data2[num] = (byte)(value >> 8);
			byte[] data3 = this._data;
			num = this._availableIndex;
			this._availableIndex = num + 1;
			data3[num] = (byte)(value >> 16);
			byte[] data4 = this._data;
			num = this._availableIndex;
			this._availableIndex = num + 1;
			data4[num] = (byte)(value >> 24);
		}

		public void WriteShort(short value)
		{
			this.EnsureLength(2);
			byte[] data = this._data;
			int num = this._availableIndex;
			this._availableIndex = num + 1;
			data[num] = (byte)value;
			byte[] data2 = this._data;
			num = this._availableIndex;
			this._availableIndex = num + 1;
			data2[num] = (byte)(value >> 8);
		}

		public void WriteString(string value)
		{
			if (!string.IsNullOrEmpty(value))
			{
				byte[] bytes = Encoding.UTF8.GetBytes(value);
				this.WriteInt(bytes.Length);
				this.WriteBytes(bytes);
				return;
			}
			this.WriteInt(0);
		}

		public void WriteFloats(float[] value, int count)
		{
			int num = count * 4;
			this.EnsureLength(num);
			Buffer.BlockCopy(value, 0, this._data, this._availableIndex, num);
			this._availableIndex += num;
		}

		public void WriteShorts(short[] value, int count)
		{
			int num = count * 2;
			this.EnsureLength(num);
			Buffer.BlockCopy(value, 0, this._data, this._availableIndex, num);
			this._availableIndex += num;
		}

		public void WriteColor(Color value)
		{
			this.WriteFloat(value.Red);
			this.WriteFloat(value.Green);
			this.WriteFloat(value.Blue);
			this.WriteFloat(value.Alpha);
		}

		public void WriteBool(bool value)
		{
			this.EnsureLength(1);
			this._data[this._availableIndex] = (value ? 1 : 0);
			this._availableIndex++;
		}

		public void WriteFloat(float value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			this.EnsureLength(bytes.Length);
			Buffer.BlockCopy(bytes, 0, this._data, this._availableIndex, bytes.Length);
			this._availableIndex += bytes.Length;
		}

		public void WriteUInt(uint value)
		{
			this.EnsureLength(4);
			byte[] data = this._data;
			int num = this._availableIndex;
			this._availableIndex = num + 1;
			data[num] = (byte)value;
			byte[] data2 = this._data;
			num = this._availableIndex;
			this._availableIndex = num + 1;
			data2[num] = (byte)(value >> 8);
			byte[] data3 = this._data;
			num = this._availableIndex;
			this._availableIndex = num + 1;
			data3[num] = (byte)(value >> 16);
			byte[] data4 = this._data;
			num = this._availableIndex;
			this._availableIndex = num + 1;
			data4[num] = (byte)(value >> 24);
		}

		public void WriteULong(ulong value)
		{
			this.EnsureLength(8);
			byte[] data = this._data;
			int num = this._availableIndex;
			this._availableIndex = num + 1;
			data[num] = (byte)value;
			byte[] data2 = this._data;
			num = this._availableIndex;
			this._availableIndex = num + 1;
			data2[num] = (byte)(value >> 8);
			byte[] data3 = this._data;
			num = this._availableIndex;
			this._availableIndex = num + 1;
			data3[num] = (byte)(value >> 16);
			byte[] data4 = this._data;
			num = this._availableIndex;
			this._availableIndex = num + 1;
			data4[num] = (byte)(value >> 24);
			byte[] data5 = this._data;
			num = this._availableIndex;
			this._availableIndex = num + 1;
			data5[num] = (byte)(value >> 32);
			byte[] data6 = this._data;
			num = this._availableIndex;
			this._availableIndex = num + 1;
			data6[num] = (byte)(value >> 40);
			byte[] data7 = this._data;
			num = this._availableIndex;
			this._availableIndex = num + 1;
			data7[num] = (byte)(value >> 48);
			byte[] data8 = this._data;
			num = this._availableIndex;
			this._availableIndex = num + 1;
			data8[num] = (byte)(value >> 56);
		}

		public void WriteLong(long value)
		{
			this.EnsureLength(8);
			byte[] data = this._data;
			int num = this._availableIndex;
			this._availableIndex = num + 1;
			data[num] = (byte)value;
			byte[] data2 = this._data;
			num = this._availableIndex;
			this._availableIndex = num + 1;
			data2[num] = (byte)(value >> 8);
			byte[] data3 = this._data;
			num = this._availableIndex;
			this._availableIndex = num + 1;
			data3[num] = (byte)(value >> 16);
			byte[] data4 = this._data;
			num = this._availableIndex;
			this._availableIndex = num + 1;
			data4[num] = (byte)(value >> 24);
			byte[] data5 = this._data;
			num = this._availableIndex;
			this._availableIndex = num + 1;
			data5[num] = (byte)(value >> 32);
			byte[] data6 = this._data;
			num = this._availableIndex;
			this._availableIndex = num + 1;
			data6[num] = (byte)(value >> 40);
			byte[] data7 = this._data;
			num = this._availableIndex;
			this._availableIndex = num + 1;
			data7[num] = (byte)(value >> 48);
			byte[] data8 = this._data;
			num = this._availableIndex;
			this._availableIndex = num + 1;
			data8[num] = (byte)(value >> 56);
		}

		public void WriteVec2(Vec2 vec2)
		{
			this.WriteFloat(vec2.x);
			this.WriteFloat(vec2.y);
		}

		public void WriteVec3(Vec3 vec3)
		{
			this.WriteFloat(vec3.x);
			this.WriteFloat(vec3.y);
			this.WriteFloat(vec3.z);
			this.WriteFloat(vec3.w);
		}

		public void WriteVec3Int(Vec3i vec3)
		{
			this.WriteInt(vec3.X);
			this.WriteInt(vec3.Y);
			this.WriteInt(vec3.Z);
		}

		public void WriteSByte(sbyte value)
		{
			this.EnsureLength(1);
			this._data[this._availableIndex] = (byte)value;
			this._availableIndex++;
		}

		public void WriteUShort(ushort value)
		{
			this.EnsureLength(2);
			byte[] data = this._data;
			int num = this._availableIndex;
			this._availableIndex = num + 1;
			data[num] = (byte)value;
			byte[] data2 = this._data;
			num = this._availableIndex;
			this._availableIndex = num + 1;
			data2[num] = (byte)(value >> 8);
		}

		public void WriteDouble(double value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			this.EnsureLength(bytes.Length);
			Buffer.BlockCopy(bytes, 0, this._data, this._availableIndex, bytes.Length);
			this._availableIndex += bytes.Length;
		}

		public void AppendData(BinaryWriter writer)
		{
			this.EnsureLength(writer._availableIndex);
			Buffer.BlockCopy(writer._data, 0, this._data, this._availableIndex, writer._availableIndex);
			this._availableIndex += writer._availableIndex;
		}

		private byte[] _data;

		private int _availableIndex;
	}
}
