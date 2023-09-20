using System;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace TaleWorlds.Diamond
{
	[DataContract]
	[JsonConverter(typeof(PeerIdJsonConverter))]
	[Serializable]
	public struct PeerId
	{
		public bool IsValid
		{
			get
			{
				return this._chunk1 != 0UL || this._chunk2 != 0UL || this._chunk3 != 0UL || this._chunk4 > 0UL;
			}
		}

		public PeerId(Guid guid)
		{
			byte[] array = guid.ToByteArray();
			this._chunk1 = 0UL;
			this._chunk2 = 0UL;
			this._chunk3 = BitConverter.ToUInt64(array, 0);
			this._chunk4 = BitConverter.ToUInt64(array, 8);
		}

		public PeerId(byte[] data)
		{
			this._chunk1 = BitConverter.ToUInt64(data, 0);
			this._chunk2 = BitConverter.ToUInt64(data, 8);
			this._chunk3 = BitConverter.ToUInt64(data, 16);
			this._chunk4 = BitConverter.ToUInt64(data, 24);
		}

		public PeerId(string peerIdAsString)
		{
			int num = peerIdAsString.Length * 2;
			byte[] array = new byte[(num < 32) ? 32 : num];
			Encoding.Unicode.GetBytes(peerIdAsString, 0, peerIdAsString.Length, array, 0);
			this._chunk1 = BitConverter.ToUInt64(array, 0);
			this._chunk2 = BitConverter.ToUInt64(array, 8);
			this._chunk3 = BitConverter.ToUInt64(array, 16);
			this._chunk4 = BitConverter.ToUInt64(array, 24);
		}

		public PeerId(ulong chunk1, ulong chunk2, ulong chunk3, ulong chunk4)
		{
			this._chunk1 = chunk1;
			this._chunk2 = chunk2;
			this._chunk3 = chunk3;
			this._chunk4 = chunk4;
		}

		public byte[] ToByteArray()
		{
			byte[] array = new byte[32];
			byte[] bytes = BitConverter.GetBytes(this._chunk1);
			byte[] bytes2 = BitConverter.GetBytes(this._chunk2);
			byte[] bytes3 = BitConverter.GetBytes(this._chunk3);
			byte[] bytes4 = BitConverter.GetBytes(this._chunk4);
			for (int i = 0; i < 8; i++)
			{
				array[i] = bytes[i];
				array[8 + i] = bytes2[i];
				array[16 + i] = bytes3[i];
				array[24 + i] = bytes4[i];
			}
			return array;
		}

		public override string ToString()
		{
			return string.Concat(new object[] { this._chunk1, ".", this._chunk2, ".", this._chunk3, " .", this._chunk4 });
		}

		public static PeerId FromString(string peerIdAsString)
		{
			string[] array = peerIdAsString.Split(new char[] { '.' });
			return new PeerId(ulong.Parse(array[0]), ulong.Parse(array[1]), ulong.Parse(array[2]), ulong.Parse(array[3]));
		}

		public static bool operator ==(PeerId a, PeerId b)
		{
			return a._chunk1 == b._chunk1 && a._chunk2 == b._chunk2 && a._chunk3 == b._chunk3 && a._chunk4 == b._chunk4;
		}

		public static bool operator !=(PeerId a, PeerId b)
		{
			return a._chunk1 != b._chunk1 || a._chunk2 != b._chunk2 || a._chunk3 != b._chunk3 || a._chunk4 != b._chunk4;
		}

		public override bool Equals(object o)
		{
			if (o != null && o is PeerId)
			{
				PeerId peerId = (PeerId)o;
				return this._chunk1 == peerId._chunk1 && this._chunk2 == peerId._chunk2 && this._chunk3 == peerId._chunk3 && this._chunk4 == peerId._chunk4;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this._chunk1.GetHashCode();
			int hashCode2 = this._chunk2.GetHashCode();
			int hashCode3 = this._chunk3.GetHashCode();
			int hashCode4 = this._chunk4.GetHashCode();
			return hashCode ^ hashCode2 ^ hashCode3 ^ hashCode4;
		}

		public static PeerId Empty
		{
			get
			{
				return new PeerId(0UL, 0UL, 0UL, 0UL);
			}
		}

		[DataMember]
		private readonly ulong _chunk1;

		[DataMember]
		private readonly ulong _chunk2;

		[DataMember]
		private readonly ulong _chunk3;

		[DataMember]
		private readonly ulong _chunk4;
	}
}
