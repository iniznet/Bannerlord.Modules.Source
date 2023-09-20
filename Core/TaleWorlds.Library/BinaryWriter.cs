using System;
using System.Text;

namespace TaleWorlds.Library
{
	// Token: 0x02000019 RID: 25
	public class BinaryWriter : IWriter
	{
		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600005F RID: 95 RVA: 0x00002F4C File Offset: 0x0000114C
		public byte[] Data
		{
			get
			{
				byte[] array = new byte[this._availableIndex];
				Buffer.BlockCopy(this._data, 0, array, 0, this._availableIndex);
				return array;
			}
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000060 RID: 96 RVA: 0x00002F7A File Offset: 0x0000117A
		public int Length
		{
			get
			{
				return this._availableIndex;
			}
		}

		// Token: 0x06000061 RID: 97 RVA: 0x00002F82 File Offset: 0x00001182
		public BinaryWriter()
		{
			this._data = new byte[4096];
			this._availableIndex = 0;
		}

		// Token: 0x06000062 RID: 98 RVA: 0x00002FA1 File Offset: 0x000011A1
		public BinaryWriter(int capacity)
		{
			this._data = new byte[capacity];
			this._availableIndex = 0;
		}

		// Token: 0x06000063 RID: 99 RVA: 0x00002FBC File Offset: 0x000011BC
		public void Clear()
		{
			this._availableIndex = 0;
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00002FC8 File Offset: 0x000011C8
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

		// Token: 0x06000065 RID: 101 RVA: 0x0000301C File Offset: 0x0000121C
		public void WriteSerializableObject(ISerializableObject serializableObject)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000066 RID: 102 RVA: 0x00003023 File Offset: 0x00001223
		public void WriteByte(byte value)
		{
			this.EnsureLength(1);
			this._data[this._availableIndex] = value;
			this._availableIndex++;
		}

		// Token: 0x06000067 RID: 103 RVA: 0x00003048 File Offset: 0x00001248
		public void WriteBytes(byte[] bytes)
		{
			this.EnsureLength(bytes.Length);
			Buffer.BlockCopy(bytes, 0, this._data, this._availableIndex, bytes.Length);
			this._availableIndex += bytes.Length;
		}

		// Token: 0x06000068 RID: 104 RVA: 0x0000307C File Offset: 0x0000127C
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

		// Token: 0x06000069 RID: 105 RVA: 0x000030E4 File Offset: 0x000012E4
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

		// Token: 0x0600006A RID: 106 RVA: 0x00003168 File Offset: 0x00001368
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

		// Token: 0x0600006B RID: 107 RVA: 0x000031B4 File Offset: 0x000013B4
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

		// Token: 0x0600006C RID: 108 RVA: 0x000031F0 File Offset: 0x000013F0
		public void WriteFloats(float[] value, int count)
		{
			int num = count * 4;
			this.EnsureLength(num);
			Buffer.BlockCopy(value, 0, this._data, this._availableIndex, num);
			this._availableIndex += num;
		}

		// Token: 0x0600006D RID: 109 RVA: 0x0000322C File Offset: 0x0000142C
		public void WriteShorts(short[] value, int count)
		{
			int num = count * 2;
			this.EnsureLength(num);
			Buffer.BlockCopy(value, 0, this._data, this._availableIndex, num);
			this._availableIndex += num;
		}

		// Token: 0x0600006E RID: 110 RVA: 0x00003266 File Offset: 0x00001466
		public void WriteColor(Color value)
		{
			this.WriteFloat(value.Red);
			this.WriteFloat(value.Green);
			this.WriteFloat(value.Blue);
			this.WriteFloat(value.Alpha);
		}

		// Token: 0x0600006F RID: 111 RVA: 0x00003298 File Offset: 0x00001498
		public void WriteBool(bool value)
		{
			this.EnsureLength(1);
			this._data[this._availableIndex] = (value ? 1 : 0);
			this._availableIndex++;
		}

		// Token: 0x06000070 RID: 112 RVA: 0x000032C4 File Offset: 0x000014C4
		public void WriteFloat(float value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			this.EnsureLength(bytes.Length);
			Buffer.BlockCopy(bytes, 0, this._data, this._availableIndex, bytes.Length);
			this._availableIndex += bytes.Length;
		}

		// Token: 0x06000071 RID: 113 RVA: 0x00003308 File Offset: 0x00001508
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

		// Token: 0x06000072 RID: 114 RVA: 0x0000338C File Offset: 0x0000158C
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

		// Token: 0x06000073 RID: 115 RVA: 0x00003484 File Offset: 0x00001684
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

		// Token: 0x06000074 RID: 116 RVA: 0x0000357C File Offset: 0x0000177C
		public void WriteVec2(Vec2 vec2)
		{
			this.WriteFloat(vec2.x);
			this.WriteFloat(vec2.y);
		}

		// Token: 0x06000075 RID: 117 RVA: 0x00003596 File Offset: 0x00001796
		public void WriteVec3(Vec3 vec3)
		{
			this.WriteFloat(vec3.x);
			this.WriteFloat(vec3.y);
			this.WriteFloat(vec3.z);
			this.WriteFloat(vec3.w);
		}

		// Token: 0x06000076 RID: 118 RVA: 0x000035C8 File Offset: 0x000017C8
		public void WriteVec3Int(Vec3i vec3)
		{
			this.WriteInt(vec3.X);
			this.WriteInt(vec3.Y);
			this.WriteInt(vec3.Z);
		}

		// Token: 0x06000077 RID: 119 RVA: 0x000035EE File Offset: 0x000017EE
		public void WriteSByte(sbyte value)
		{
			this.EnsureLength(1);
			this._data[this._availableIndex] = (byte)value;
			this._availableIndex++;
		}

		// Token: 0x06000078 RID: 120 RVA: 0x00003614 File Offset: 0x00001814
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

		// Token: 0x06000079 RID: 121 RVA: 0x00003660 File Offset: 0x00001860
		public void WriteDouble(double value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			this.EnsureLength(bytes.Length);
			Buffer.BlockCopy(bytes, 0, this._data, this._availableIndex, bytes.Length);
			this._availableIndex += bytes.Length;
		}

		// Token: 0x0600007A RID: 122 RVA: 0x000036A3 File Offset: 0x000018A3
		public void AppendData(BinaryWriter writer)
		{
			this.EnsureLength(writer._availableIndex);
			Buffer.BlockCopy(writer._data, 0, this._data, this._availableIndex, writer._availableIndex);
			this._availableIndex += writer._availableIndex;
		}

		// Token: 0x04000055 RID: 85
		private byte[] _data;

		// Token: 0x04000056 RID: 86
		private int _availableIndex;
	}
}
