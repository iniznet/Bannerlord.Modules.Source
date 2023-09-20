using System;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.ObjectSystem
{
	public struct MBGUID : IComparable, IEquatable<MBGUID>
	{
		public MBGUID(uint id)
		{
			this._internalValue = id;
		}

		public MBGUID(uint objType, uint subId)
		{
			if (subId < 0U || subId > 67108863U)
			{
				throw new MBOutOfRangeException("subId");
			}
			this._internalValue = (objType << 26) | subId;
		}

		public uint InternalValue
		{
			get
			{
				return this._internalValue;
			}
		}

		public uint SubId
		{
			get
			{
				return this._internalValue & 67108863U;
			}
		}

		public static bool operator ==(MBGUID id1, MBGUID id2)
		{
			return id1._internalValue == id2._internalValue;
		}

		public static bool operator !=(MBGUID id1, MBGUID id2)
		{
			return id1._internalValue != id2._internalValue;
		}

		public static bool operator <(MBGUID id1, MBGUID id2)
		{
			return id1._internalValue < id2._internalValue;
		}

		public static bool operator >(MBGUID id1, MBGUID id2)
		{
			return id1._internalValue > id2._internalValue;
		}

		public static bool operator <=(MBGUID id1, MBGUID id2)
		{
			return id1._internalValue <= id2._internalValue;
		}

		public static bool operator >=(MBGUID id1, MBGUID id2)
		{
			return id1._internalValue >= id2._internalValue;
		}

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

		public uint GetTypeIndex()
		{
			return this._internalValue >> 26;
		}

		public override int GetHashCode()
		{
			return (int)this._internalValue;
		}

		public override string ToString()
		{
			return this.InternalValue.ToString();
		}

		public override bool Equals(object obj)
		{
			if (obj is MBGUID)
			{
				MBGUID mbguid = (MBGUID)obj;
				return this._internalValue == mbguid._internalValue;
			}
			return false;
		}

		public bool Equals(MBGUID other)
		{
			return this._internalValue == other._internalValue;
		}

		private const int ObjectIdNumBits = 26;

		private const int ObjectIdBitFlag = 67108863;

		[CachedData]
		[SaveableField(1)]
		private readonly uint _internalValue;
	}
}
