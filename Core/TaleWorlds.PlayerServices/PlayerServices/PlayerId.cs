using System;
using TaleWorlds.Library;

namespace TaleWorlds.PlayerServices
{
	[Serializable]
	public struct PlayerId : IComparable<PlayerId>
	{
		public ulong Id1
		{
			get
			{
				return this._id1;
			}
		}

		public ulong Id2
		{
			get
			{
				return this._id2;
			}
		}

		public bool IsValid
		{
			get
			{
				return this._id1 != 0UL || this._id2 > 0UL;
			}
		}

		public PlayerIdProvidedTypes ProvidedType
		{
			get
			{
				return (PlayerIdProvidedTypes)this._providedType;
			}
		}

		public ulong Part1
		{
			get
			{
				return BitConverter.ToUInt64(new byte[] { this._providedType, this._reserved1, this._reserved2, this._reserved3, this._reserved4, this._reserved5, this._reserved6, this._reserved7 }, 0);
			}
		}

		public ulong Part2
		{
			get
			{
				return this._reservedBig;
			}
		}

		public ulong Part3
		{
			get
			{
				return this._id1;
			}
		}

		public ulong Part4
		{
			get
			{
				return this._id2;
			}
		}

		public static PlayerId Empty
		{
			get
			{
				return new PlayerId(0, 0UL, 0UL, 0UL);
			}
		}

		public PlayerId(byte providedType, ulong id1, ulong id2)
		{
			this._providedType = providedType;
			this._reserved1 = 0;
			this._reserved2 = 0;
			this._reserved3 = 0;
			this._reserved4 = 0;
			this._reserved5 = 0;
			this._reserved6 = 0;
			this._reserved7 = 0;
			this._reservedBig = 0UL;
			this._id1 = id1;
			this._id2 = id2;
		}

		public PlayerId(byte providedType, ulong reservedBig, ulong id1, ulong id2)
		{
			this._providedType = providedType;
			this._reserved1 = 0;
			this._reserved2 = 0;
			this._reserved3 = 0;
			this._reserved4 = 0;
			this._reserved5 = 0;
			this._reserved6 = 0;
			this._reserved7 = 0;
			this._reservedBig = reservedBig;
			this._id1 = id1;
			this._id2 = id2;
		}

		public PlayerId(byte providedType, string guid)
		{
			byte[] array = Guid.Parse(guid).ToByteArray();
			this._providedType = providedType;
			this._reserved1 = 0;
			this._reserved2 = 0;
			this._reserved3 = 0;
			this._reserved4 = 0;
			this._reserved5 = 0;
			this._reserved6 = 0;
			this._reserved7 = 0;
			this._reservedBig = 0UL;
			this._id1 = BitConverter.ToUInt64(array, 0);
			this._id2 = BitConverter.ToUInt64(array, 8);
		}

		public PlayerId(ulong part1, ulong part2, ulong part3, ulong part4)
		{
			byte[] bytes = BitConverter.GetBytes(part1);
			this._providedType = bytes[0];
			this._reserved1 = bytes[1];
			this._reserved2 = bytes[2];
			this._reserved3 = bytes[3];
			this._reserved4 = bytes[4];
			this._reserved5 = bytes[5];
			this._reserved6 = bytes[6];
			this._reserved7 = bytes[7];
			this._reservedBig = part2;
			this._id1 = part3;
			this._id2 = part4;
		}

		public PlayerId(byte[] data)
		{
			this._providedType = data[0];
			this._reserved1 = data[1];
			this._reserved2 = data[2];
			this._reserved3 = data[3];
			this._reserved4 = data[4];
			this._reserved5 = data[5];
			this._reserved6 = data[6];
			this._reserved7 = data[7];
			this._reservedBig = BitConverter.ToUInt64(data, 8);
			this._id1 = BitConverter.ToUInt64(data, 16);
			this._id2 = BitConverter.ToUInt64(data, 24);
		}

		public PlayerId(Guid guid)
		{
			byte[] array = guid.ToByteArray();
			this._providedType = 0;
			this._reserved1 = 0;
			this._reserved2 = 0;
			this._reserved3 = 0;
			this._reserved4 = 0;
			this._reserved5 = 0;
			this._reserved6 = 0;
			this._reserved7 = 0;
			this._reservedBig = 0UL;
			this._id1 = BitConverter.ToUInt64(array, 0);
			this._id2 = BitConverter.ToUInt64(array, 8);
		}

		public byte[] ToByteArray()
		{
			byte[] array = new byte[32];
			array[0] = this._providedType;
			array[1] = this._reserved1;
			array[2] = this._reserved2;
			array[3] = this._reserved3;
			array[4] = this._reserved4;
			array[5] = this._reserved5;
			array[6] = this._reserved6;
			array[7] = this._reserved7;
			byte[] bytes = BitConverter.GetBytes(this._reservedBig);
			byte[] bytes2 = BitConverter.GetBytes(this._id1);
			byte[] bytes3 = BitConverter.GetBytes(this._id2);
			for (int i = 0; i < 8; i++)
			{
				array[8 + i] = bytes[i];
				array[16 + i] = bytes2[i];
				array[24 + i] = bytes3[i];
			}
			return array;
		}

		public void Serialize(IWriter writer)
		{
			writer.WriteULong(this.Part1);
			writer.WriteULong(this.Part2);
			writer.WriteULong(this.Part3);
			writer.WriteULong(this.Part4);
		}

		public void Deserialize(IReader reader)
		{
			ulong num = reader.ReadULong();
			ulong num2 = reader.ReadULong();
			ulong num3 = reader.ReadULong();
			ulong num4 = reader.ReadULong();
			byte[] bytes = BitConverter.GetBytes(num);
			this._providedType = bytes[0];
			this._reserved1 = bytes[1];
			this._reserved2 = bytes[2];
			this._reserved3 = bytes[3];
			this._reserved4 = bytes[4];
			this._reserved5 = bytes[5];
			this._reserved6 = bytes[6];
			this._reserved7 = bytes[7];
			this._reservedBig = num2;
			this._id1 = num3;
			this._id2 = num4;
		}

		public override string ToString()
		{
			return string.Concat(new object[] { this.Part1, ".", this.Part2, ".", this.Part3, ".", this.Part4 });
		}

		public static bool operator ==(PlayerId a, PlayerId b)
		{
			return a.Part1 == b.Part1 && a.Part2 == b.Part2 && a.Part3 == b.Part3 && a.Part4 == b.Part4;
		}

		public static bool operator !=(PlayerId a, PlayerId b)
		{
			return a.Part1 != b.Part1 || a.Part2 != b.Part2 || a.Part3 != b.Part3 || a.Part4 != b.Part4;
		}

		public override bool Equals(object o)
		{
			if (o != null && o is PlayerId)
			{
				PlayerId playerId = (PlayerId)o;
				if (this == playerId)
				{
					return true;
				}
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.Part1.GetHashCode();
			int hashCode2 = this.Part2.GetHashCode();
			int hashCode3 = this.Part3.GetHashCode();
			int hashCode4 = this.Part4.GetHashCode();
			return hashCode ^ hashCode2 ^ hashCode3 ^ hashCode4;
		}

		public static PlayerId FromString(string id)
		{
			if (!string.IsNullOrEmpty(id))
			{
				string[] array = id.Split(new char[] { '.' });
				ulong num = Convert.ToUInt64(array[0]);
				ulong num2 = Convert.ToUInt64(array[1]);
				ulong num3 = Convert.ToUInt64(array[2]);
				ulong num4 = Convert.ToUInt64(array[3]);
				return new PlayerId(num, num2, num3, num4);
			}
			return default(PlayerId);
		}

		public int CompareTo(PlayerId other)
		{
			if (this.Part1 != other.Part1)
			{
				return this.Part1.CompareTo(other.Part1);
			}
			if (this.Part2 != other.Part2)
			{
				return this.Part2.CompareTo(other.Part2);
			}
			if (this.Part3 != other.Part3)
			{
				return this.Part3.CompareTo(other.Part3);
			}
			return this.Part4.CompareTo(other.Part4);
		}

		private byte _providedType;

		private byte _reserved1;

		private byte _reserved2;

		private byte _reserved3;

		private byte _reserved4;

		private byte _reserved5;

		private byte _reserved6;

		private byte _reserved7;

		private ulong _reservedBig;

		private ulong _id1;

		private ulong _id2;
	}
}
