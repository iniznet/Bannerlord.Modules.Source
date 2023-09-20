using System;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class CompressionInfo
	{
		[EngineStruct("Integer_compression_info")]
		public struct Integer
		{
			public Integer(int minimumValue, int maximumValue, bool maximumValueGiven)
			{
				this.maximumValue = maximumValue;
				this.minimumValue = minimumValue;
				uint num = (uint)(maximumValue - minimumValue);
				this.numberOfBits = MBMath.GetNumberOfBitsToRepresentNumber(num);
			}

			public Integer(int minimumValue, int numberOfBits)
			{
				this.minimumValue = minimumValue;
				this.numberOfBits = numberOfBits;
				if (minimumValue == -2147483648 && numberOfBits == 32)
				{
					this.maximumValue = int.MaxValue;
					return;
				}
				this.maximumValue = minimumValue + (1 << numberOfBits) - 1;
			}

			public int GetNumBits()
			{
				return this.numberOfBits;
			}

			public int GetMaximumValue()
			{
				return this.maximumValue;
			}

			private readonly int minimumValue;

			private readonly int maximumValue;

			private readonly int numberOfBits;
		}

		[EngineStruct("Unsigned_integer_compression_info")]
		public struct UnsignedInteger
		{
			public UnsignedInteger(uint minimumValue, uint maximumValue, bool maximumValueGiven)
			{
				this.minimumValue = minimumValue;
				this.maximumValue = maximumValue;
				uint num = maximumValue - minimumValue;
				this.numberOfBits = MBMath.GetNumberOfBitsToRepresentNumber(num);
			}

			public UnsignedInteger(uint minimumValue, int numberOfBits)
			{
				this.minimumValue = minimumValue;
				this.numberOfBits = numberOfBits;
				if (minimumValue == 0U && numberOfBits == 32)
				{
					this.maximumValue = uint.MaxValue;
					return;
				}
				this.maximumValue = (uint)((ulong)minimumValue + (1UL << numberOfBits) - 1UL);
			}

			public int GetNumBits()
			{
				return this.numberOfBits;
			}

			private readonly uint minimumValue;

			private readonly uint maximumValue;

			private readonly int numberOfBits;
		}

		[EngineStruct("Integer64_compression_info")]
		public struct LongInteger
		{
			public LongInteger(long minimumValue, long maximumValue, bool maximumValueGiven)
			{
				this.maximumValue = maximumValue;
				this.minimumValue = minimumValue;
				ulong num = (ulong)(maximumValue - minimumValue);
				this.numberOfBits = MBMath.GetNumberOfBitsToRepresentNumber(num);
			}

			public LongInteger(long minimumValue, int numberOfBits)
			{
				this.minimumValue = minimumValue;
				this.numberOfBits = numberOfBits;
				if (minimumValue == -9223372036854775808L && numberOfBits == 64)
				{
					this.maximumValue = long.MaxValue;
					return;
				}
				this.maximumValue = minimumValue + (1L << numberOfBits) - 1L;
			}

			public int GetNumBits()
			{
				return this.numberOfBits;
			}

			private readonly long minimumValue;

			private readonly long maximumValue;

			private readonly int numberOfBits;
		}

		[EngineStruct("Unsigned_integer64_compression_info")]
		public struct UnsignedLongInteger
		{
			public UnsignedLongInteger(ulong minimumValue, ulong maximumValue, bool maximumValueGiven)
			{
				this.minimumValue = minimumValue;
				this.maximumValue = maximumValue;
				ulong num = maximumValue - minimumValue;
				this.numberOfBits = MBMath.GetNumberOfBitsToRepresentNumber(num);
			}

			public UnsignedLongInteger(ulong minimumValue, int numberOfBits)
			{
				this.minimumValue = minimumValue;
				this.numberOfBits = numberOfBits;
				if (minimumValue == 0UL && numberOfBits == 64)
				{
					this.maximumValue = ulong.MaxValue;
					return;
				}
				this.maximumValue = minimumValue + (1UL << numberOfBits) - 1UL;
			}

			public int GetNumBits()
			{
				return this.numberOfBits;
			}

			private readonly ulong minimumValue;

			private readonly ulong maximumValue;

			private readonly int numberOfBits;
		}

		[EngineStruct("Float_compression_info")]
		public struct Float
		{
			public static CompressionInfo.Float FullPrecision { get; } = new CompressionInfo.Float(true);

			public Float(float minimumValue, float maximumValue, int numberOfBits)
			{
				this.minimumValue = minimumValue;
				this.maximumValue = maximumValue;
				this.numberOfBits = numberOfBits;
				float num = maximumValue - minimumValue;
				int num2 = (1 << numberOfBits) - 1;
				this.precision = num / (float)num2;
			}

			public Float(float minimumValue, int numberOfBits, float precision)
			{
				this.minimumValue = minimumValue;
				this.precision = precision;
				this.numberOfBits = numberOfBits;
				int num = (1 << numberOfBits) - 1;
				float num2 = precision * (float)num;
				this.maximumValue = num2 + minimumValue;
			}

			private Float(bool isFullPrecision)
			{
				this.minimumValue = float.MinValue;
				this.maximumValue = float.MaxValue;
				this.precision = 0f;
				this.numberOfBits = 32;
			}

			public int GetNumBits()
			{
				return this.numberOfBits;
			}

			public float GetMaximumValue()
			{
				return this.maximumValue;
			}

			public float GetMinimumValue()
			{
				return this.minimumValue;
			}

			public float GetPrecision()
			{
				return this.precision;
			}

			private readonly float minimumValue;

			private readonly float maximumValue;

			private readonly float precision;

			private readonly int numberOfBits;
		}
	}
}
