using System;
using System.Runtime.Serialization;
using System.Text;

namespace TaleWorlds.Diamond
{
	// Token: 0x02000019 RID: 25
	[DataContract]
	[Serializable]
	public struct PeerId
	{
		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000068 RID: 104 RVA: 0x000027B5 File Offset: 0x000009B5
		public bool IsValid
		{
			get
			{
				return this._chunk1 != 0UL || this._chunk2 != 0UL || this._chunk3 != 0UL || this._chunk4 > 0UL;
			}
		}

		// Token: 0x06000069 RID: 105 RVA: 0x000027DC File Offset: 0x000009DC
		public PeerId(Guid guid)
		{
			byte[] array = guid.ToByteArray();
			this._chunk1 = 0UL;
			this._chunk2 = 0UL;
			this._chunk3 = BitConverter.ToUInt64(array, 0);
			this._chunk4 = BitConverter.ToUInt64(array, 8);
		}

		// Token: 0x0600006A RID: 106 RVA: 0x0000281B File Offset: 0x00000A1B
		public PeerId(byte[] data)
		{
			this._chunk1 = BitConverter.ToUInt64(data, 0);
			this._chunk2 = BitConverter.ToUInt64(data, 8);
			this._chunk3 = BitConverter.ToUInt64(data, 16);
			this._chunk4 = BitConverter.ToUInt64(data, 24);
		}

		// Token: 0x0600006B RID: 107 RVA: 0x00002854 File Offset: 0x00000A54
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

		// Token: 0x0600006C RID: 108 RVA: 0x000028C5 File Offset: 0x00000AC5
		public PeerId(ulong chunk1, ulong chunk2, ulong chunk3, ulong chunk4)
		{
			this._chunk1 = chunk1;
			this._chunk2 = chunk2;
			this._chunk3 = chunk3;
			this._chunk4 = chunk4;
		}

		// Token: 0x0600006D RID: 109 RVA: 0x000028E4 File Offset: 0x00000AE4
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

		// Token: 0x0600006E RID: 110 RVA: 0x00002964 File Offset: 0x00000B64
		public override string ToString()
		{
			return string.Concat(new object[] { this._chunk1, ".", this._chunk2, ".", this._chunk3, " .", this._chunk4 });
		}

		// Token: 0x0600006F RID: 111 RVA: 0x000029CC File Offset: 0x00000BCC
		public static bool operator ==(PeerId a, PeerId b)
		{
			return a._chunk1 == b._chunk1 && a._chunk2 == b._chunk2 && a._chunk3 == b._chunk3 && a._chunk4 == b._chunk4;
		}

		// Token: 0x06000070 RID: 112 RVA: 0x00002A08 File Offset: 0x00000C08
		public static bool operator !=(PeerId a, PeerId b)
		{
			return a._chunk1 != b._chunk1 || a._chunk2 != b._chunk2 || a._chunk3 != b._chunk3 || a._chunk4 != b._chunk4;
		}

		// Token: 0x06000071 RID: 113 RVA: 0x00002A48 File Offset: 0x00000C48
		public override bool Equals(object o)
		{
			if (o != null && o is PeerId)
			{
				PeerId peerId = (PeerId)o;
				return this._chunk1 == peerId._chunk1 && this._chunk2 == peerId._chunk2 && this._chunk3 == peerId._chunk3 && this._chunk4 == peerId._chunk4;
			}
			return false;
		}

		// Token: 0x06000072 RID: 114 RVA: 0x00002AA4 File Offset: 0x00000CA4
		public override int GetHashCode()
		{
			int hashCode = this._chunk1.GetHashCode();
			int hashCode2 = this._chunk2.GetHashCode();
			int hashCode3 = this._chunk3.GetHashCode();
			int hashCode4 = this._chunk4.GetHashCode();
			return hashCode ^ hashCode2 ^ hashCode3 ^ hashCode4;
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000073 RID: 115 RVA: 0x00002AF2 File Offset: 0x00000CF2
		public static PeerId Empty
		{
			get
			{
				return new PeerId(0UL, 0UL, 0UL, 0UL);
			}
		}

		// Token: 0x0400001B RID: 27
		[DataMember]
		private readonly ulong _chunk1;

		// Token: 0x0400001C RID: 28
		[DataMember]
		private readonly ulong _chunk2;

		// Token: 0x0400001D RID: 29
		[DataMember]
		private readonly ulong _chunk3;

		// Token: 0x0400001E RID: 30
		[DataMember]
		private readonly ulong _chunk4;
	}
}
