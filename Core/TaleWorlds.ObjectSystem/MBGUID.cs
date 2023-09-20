using System;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.ObjectSystem
{
	// Token: 0x02000004 RID: 4
	public struct MBGUID : IComparable, IEquatable<MBGUID>
	{
		// Token: 0x06000004 RID: 4 RVA: 0x00002055 File Offset: 0x00000255
		public MBGUID(uint id)
		{
			this._internalValue = id;
		}

		// Token: 0x06000005 RID: 5 RVA: 0x0000205E File Offset: 0x0000025E
		public MBGUID(uint objType, uint subId)
		{
			if (subId < 0U || subId > 67108863U)
			{
				throw new MBOutOfRangeException("subId");
			}
			this._internalValue = (objType << 26) | subId;
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000006 RID: 6 RVA: 0x00002083 File Offset: 0x00000283
		public uint InternalValue
		{
			get
			{
				return this._internalValue;
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000007 RID: 7 RVA: 0x0000208B File Offset: 0x0000028B
		public uint SubId
		{
			get
			{
				return this._internalValue & 67108863U;
			}
		}

		// Token: 0x06000008 RID: 8 RVA: 0x00002099 File Offset: 0x00000299
		public static bool operator ==(MBGUID id1, MBGUID id2)
		{
			return id1._internalValue == id2._internalValue;
		}

		// Token: 0x06000009 RID: 9 RVA: 0x000020A9 File Offset: 0x000002A9
		public static bool operator !=(MBGUID id1, MBGUID id2)
		{
			return id1._internalValue != id2._internalValue;
		}

		// Token: 0x0600000A RID: 10 RVA: 0x000020BC File Offset: 0x000002BC
		public static bool operator <(MBGUID id1, MBGUID id2)
		{
			return id1._internalValue < id2._internalValue;
		}

		// Token: 0x0600000B RID: 11 RVA: 0x000020CC File Offset: 0x000002CC
		public static bool operator >(MBGUID id1, MBGUID id2)
		{
			return id1._internalValue > id2._internalValue;
		}

		// Token: 0x0600000C RID: 12 RVA: 0x000020DC File Offset: 0x000002DC
		public static bool operator <=(MBGUID id1, MBGUID id2)
		{
			return id1._internalValue <= id2._internalValue;
		}

		// Token: 0x0600000D RID: 13 RVA: 0x000020EF File Offset: 0x000002EF
		public static bool operator >=(MBGUID id1, MBGUID id2)
		{
			return id1._internalValue >= id2._internalValue;
		}

		// Token: 0x0600000E RID: 14 RVA: 0x00002104 File Offset: 0x00000304
		public static long GetHash2(MBGUID id1, MBGUID id2)
		{
			if (id1 > id2)
			{
				MBGUID mbguid = id1;
				id1 = id2;
				id2 = mbguid;
			}
			long num = (long)id1.GetHashCode();
			long num2 = (long)id2.GetHashCode();
			return num * 1046527L + num2;
		}

		// Token: 0x0600000F RID: 15 RVA: 0x00002146 File Offset: 0x00000346
		public int CompareTo(object a)
		{
			if (!(a is MBGUID))
			{
				throw new MBTypeMismatchException("CompareTo function called with an invalid argument!");
			}
			if (this._internalValue == ((MBGUID)a)._internalValue)
			{
				return 0;
			}
			if (this._internalValue > ((MBGUID)a)._internalValue)
			{
				return 1;
			}
			return -1;
		}

		// Token: 0x06000010 RID: 16 RVA: 0x00002186 File Offset: 0x00000386
		public uint GetTypeIndex()
		{
			return this._internalValue >> 26;
		}

		// Token: 0x06000011 RID: 17 RVA: 0x00002191 File Offset: 0x00000391
		public override int GetHashCode()
		{
			return (int)this._internalValue;
		}

		// Token: 0x06000012 RID: 18 RVA: 0x0000219C File Offset: 0x0000039C
		public override string ToString()
		{
			return this.InternalValue.ToString();
		}

		// Token: 0x06000013 RID: 19 RVA: 0x000021B8 File Offset: 0x000003B8
		public override bool Equals(object obj)
		{
			if (obj is MBGUID)
			{
				MBGUID mbguid = (MBGUID)obj;
				return this._internalValue == mbguid._internalValue;
			}
			return false;
		}

		// Token: 0x06000014 RID: 20 RVA: 0x000021E6 File Offset: 0x000003E6
		public bool Equals(MBGUID other)
		{
			return this._internalValue == other._internalValue;
		}

		// Token: 0x04000001 RID: 1
		private const int ObjectIdNumBits = 26;

		// Token: 0x04000002 RID: 2
		private const int ObjectIdBitFlag = 67108863;

		// Token: 0x04000003 RID: 3
		[CachedData]
		[SaveableField(1)]
		private readonly uint _internalValue;
	}
}
