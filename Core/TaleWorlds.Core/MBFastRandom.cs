﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.Core
{
	public class MBFastRandom
	{
		internal static void AutoGeneratedStaticCollectObjectsMBFastRandom(object o, List<object> collectedObjects)
		{
			((MBFastRandom)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected virtual void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
		}

		internal static object AutoGeneratedGetMemberValue_x(object o)
		{
			return ((MBFastRandom)o)._x;
		}

		internal static object AutoGeneratedGetMemberValue_y(object o)
		{
			return ((MBFastRandom)o)._y;
		}

		internal static object AutoGeneratedGetMemberValue_z(object o)
		{
			return ((MBFastRandom)o)._z;
		}

		internal static object AutoGeneratedGetMemberValue_w(object o)
		{
			return ((MBFastRandom)o)._w;
		}

		public MBFastRandom()
			: this((uint)Environment.TickCount)
		{
		}

		public MBFastRandom(uint seed)
		{
			MBFastRandom.GenerateState(seed, out this._x, out this._y, out this._z, out this._w);
		}

		public void SetSeed(uint seed, uint seed2)
		{
			MBFastRandom.GenerateState(seed, seed2, out this._x, out this._y, out this._z, out this._w);
		}

		public int Next()
		{
			uint num;
			do
			{
				num = MBFastRandom.XorShift(ref this._x, ref this._y, ref this._z, ref this._w);
				num &= 2147483647U;
			}
			while (num == 2147483647U);
			return (int)num;
		}

		public int Next(int maxValue)
		{
			if (maxValue < 0)
			{
				throw new ArgumentOutOfRangeException("maxValue", "Maximum value must be non-negative.");
			}
			uint num = MBFastRandom.XorShift(ref this._x, ref this._y, ref this._z, ref this._w);
			return (int)(4.656612873077393E-10 * (double)(2147483647U & num) * (double)maxValue);
		}

		public int Next(int minValue, int maxValue)
		{
			if (minValue > maxValue)
			{
				throw new ArgumentOutOfRangeException("maxValue", maxValue, "Maximum value must be greater than or equal to minimum value");
			}
			uint num = MBFastRandom.XorShift(ref this._x, ref this._y, ref this._z, ref this._w);
			int num2 = maxValue - minValue;
			if (num2 < 0)
			{
				return minValue + (int)(2.3283064365386963E-10 * num * (double)((long)maxValue - (long)minValue));
			}
			return minValue + (int)(4.656612873077393E-10 * (double)(2147483647U & num) * (double)num2);
		}

		public double NextDouble()
		{
			uint num = MBFastRandom.XorShift(ref this._x, ref this._y, ref this._z, ref this._w);
			return 4.656612873077393E-10 * (double)(2147483647U & num);
		}

		public float NextFloat()
		{
			uint num = MBFastRandom.XorShift(ref this._x, ref this._y, ref this._z, ref this._w);
			return 5.9604645E-08f * (float)(16777215U & num);
		}

		public void NextBytes(byte[] buffer)
		{
			int i = 0;
			int num = buffer.Length - 3;
			while (i < num)
			{
				uint num2 = MBFastRandom.XorShift(ref this._x, ref this._y, ref this._z, ref this._w);
				buffer[i++] = (byte)num2;
				buffer[i++] = (byte)(num2 >> 8);
				buffer[i++] = (byte)(num2 >> 16);
				buffer[i++] = (byte)(num2 >> 24);
			}
			if (i < buffer.Length)
			{
				uint num3 = MBFastRandom.XorShift(ref this._x, ref this._y, ref this._z, ref this._w);
				buffer[i++] = (byte)num3;
				if (i < buffer.Length)
				{
					buffer[i++] = (byte)(num3 >> 8);
					if (i < buffer.Length)
					{
						buffer[i++] = (byte)(num3 >> 16);
						if (i < buffer.Length)
						{
							buffer[i] = (byte)(num3 >> 24);
						}
					}
				}
			}
		}

		internal static int GetRandomInt(uint seed, uint seed2)
		{
			uint num;
			uint num2;
			uint num3;
			uint num4;
			MBFastRandom.GenerateState(seed, seed2, out num, out num2, out num3, out num4);
			uint num5;
			do
			{
				num5 = MBFastRandom.XorShift(ref num, ref num2, ref num3, ref num4);
				num5 &= 2147483647U;
			}
			while (num5 == 2147483647U);
			return (int)num5;
		}

		internal static float GetRandomFloat(uint seed, uint seed2)
		{
			uint num;
			uint num2;
			uint num3;
			uint num4;
			MBFastRandom.GenerateState(seed, seed2, out num, out num2, out num3, out num4);
			uint num5 = MBFastRandom.XorShift(ref num, ref num2, ref num3, ref num4);
			return 5.9604645E-08f * (float)(16777215U & num5);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static uint XorShift(ref uint x, ref uint y, ref uint z, ref uint w)
		{
			uint num = x ^ (x << 11);
			x = y;
			y = z;
			z = w;
			w = w ^ (w >> 19) ^ (num ^ (num >> 8));
			return w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void GenerateState(uint seed, out uint x, out uint y, out uint z, out uint w)
		{
			x = MBFastRandom.CalculateHashFromSeed(14695981039346656037UL, seed);
			y = MBFastRandom.CalculateHashFromSeed(14695981039346656037UL, x);
			z = MBFastRandom.CalculateHashFromSeed((ulong)x, y);
			w = MBFastRandom.CalculateHashFromSeed((ulong)y, z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void GenerateState(uint seed, uint seed2, out uint x, out uint y, out uint z, out uint w)
		{
			uint num = MBFastRandom.CalculateHashFromSeed((ulong)MBFastRandom.CalculateHashFromSeed(14695981039346656037UL, seed), seed2);
			x = MBFastRandom.CalculateHashFromSeed(14695981039346656037UL, num);
			y = MBFastRandom.CalculateHashFromSeed(14695981039346656037UL, x);
			z = MBFastRandom.CalculateHashFromSeed((ulong)x, y);
			w = MBFastRandom.CalculateHashFromSeed((ulong)y, z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static uint CalculateHashFromSeed(ulong inputHash, uint seed)
		{
			return (uint)((inputHash ^ (ulong)seed) * 1099511628211UL >> 17);
		}

		private const ulong InitialHash = 14695981039346656037UL;

		private const ulong Prime = 1099511628211UL;

		private const double RealUnitInt = 4.656612873077393E-10;

		private const double RealUnitUint = 2.3283064365386963E-10;

		private const int FloatUnitRangeMax = 16777215;

		private const float FloatUnitInt = 5.9604645E-08f;

		[SaveableField(1)]
		private uint _x;

		[SaveableField(2)]
		private uint _y;

		[SaveableField(3)]
		private uint _z;

		[SaveableField(4)]
		private uint _w;
	}
}