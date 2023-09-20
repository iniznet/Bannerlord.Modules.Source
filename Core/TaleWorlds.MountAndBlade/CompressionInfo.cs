using System;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020002E6 RID: 742
	public class CompressionInfo
	{
		// Token: 0x020005F7 RID: 1527
		[EngineStruct("Integer_compression_info")]
		public struct Integer
		{
			// Token: 0x06003CF3 RID: 15603 RVA: 0x000F1C44 File Offset: 0x000EFE44
			public Integer(int minimumValue, int maximumValue, bool maximumValueGiven)
			{
				this.maximumValue = maximumValue;
				this.minimumValue = minimumValue;
				uint num = (uint)(maximumValue - minimumValue);
				this.numberOfBits = MBMath.GetNumberOfBitsToRepresentNumber(num);
			}

			// Token: 0x06003CF4 RID: 15604 RVA: 0x000F1C6F File Offset: 0x000EFE6F
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

			// Token: 0x06003CF5 RID: 15605 RVA: 0x000F1CA8 File Offset: 0x000EFEA8
			public int GetNumBits()
			{
				return this.numberOfBits;
			}

			// Token: 0x06003CF6 RID: 15606 RVA: 0x000F1CB0 File Offset: 0x000EFEB0
			public int GetMaximumValue()
			{
				return this.maximumValue;
			}

			// Token: 0x04001F20 RID: 7968
			private readonly int minimumValue;

			// Token: 0x04001F21 RID: 7969
			private readonly int maximumValue;

			// Token: 0x04001F22 RID: 7970
			private readonly int numberOfBits;
		}

		// Token: 0x020005F8 RID: 1528
		[EngineStruct("Unsigned_integer_compression_info")]
		public struct UnsignedInteger
		{
			// Token: 0x06003CF7 RID: 15607 RVA: 0x000F1CB8 File Offset: 0x000EFEB8
			public UnsignedInteger(uint minimumValue, uint maximumValue, bool maximumValueGiven)
			{
				this.minimumValue = minimumValue;
				this.maximumValue = maximumValue;
				uint num = maximumValue - minimumValue;
				this.numberOfBits = MBMath.GetNumberOfBitsToRepresentNumber(num);
			}

			// Token: 0x06003CF8 RID: 15608 RVA: 0x000F1CE3 File Offset: 0x000EFEE3
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

			// Token: 0x06003CF9 RID: 15609 RVA: 0x000F1D17 File Offset: 0x000EFF17
			public int GetNumBits()
			{
				return this.numberOfBits;
			}

			// Token: 0x04001F23 RID: 7971
			private readonly uint minimumValue;

			// Token: 0x04001F24 RID: 7972
			private readonly uint maximumValue;

			// Token: 0x04001F25 RID: 7973
			private readonly int numberOfBits;
		}

		// Token: 0x020005F9 RID: 1529
		[EngineStruct("Integer64_compression_info")]
		public struct LongInteger
		{
			// Token: 0x06003CFA RID: 15610 RVA: 0x000F1D20 File Offset: 0x000EFF20
			public LongInteger(long minimumValue, long maximumValue, bool maximumValueGiven)
			{
				this.maximumValue = maximumValue;
				this.minimumValue = minimumValue;
				ulong num = (ulong)(maximumValue - minimumValue);
				this.numberOfBits = MBMath.GetNumberOfBitsToRepresentNumber(num);
			}

			// Token: 0x06003CFB RID: 15611 RVA: 0x000F1D4C File Offset: 0x000EFF4C
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

			// Token: 0x06003CFC RID: 15612 RVA: 0x000F1D9A File Offset: 0x000EFF9A
			public int GetNumBits()
			{
				return this.numberOfBits;
			}

			// Token: 0x04001F26 RID: 7974
			private readonly long minimumValue;

			// Token: 0x04001F27 RID: 7975
			private readonly long maximumValue;

			// Token: 0x04001F28 RID: 7976
			private readonly int numberOfBits;
		}

		// Token: 0x020005FA RID: 1530
		[EngineStruct("Unsigned_integer64_compression_info")]
		public struct UnsignedLongInteger
		{
			// Token: 0x06003CFD RID: 15613 RVA: 0x000F1DA4 File Offset: 0x000EFFA4
			public UnsignedLongInteger(ulong minimumValue, ulong maximumValue, bool maximumValueGiven)
			{
				this.minimumValue = minimumValue;
				this.maximumValue = maximumValue;
				ulong num = maximumValue - minimumValue;
				this.numberOfBits = MBMath.GetNumberOfBitsToRepresentNumber(num);
			}

			// Token: 0x06003CFE RID: 15614 RVA: 0x000F1DCF File Offset: 0x000EFFCF
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

			// Token: 0x06003CFF RID: 15615 RVA: 0x000F1E02 File Offset: 0x000F0002
			public int GetNumBits()
			{
				return this.numberOfBits;
			}

			// Token: 0x04001F29 RID: 7977
			private readonly ulong minimumValue;

			// Token: 0x04001F2A RID: 7978
			private readonly ulong maximumValue;

			// Token: 0x04001F2B RID: 7979
			private readonly int numberOfBits;
		}

		// Token: 0x020005FB RID: 1531
		[EngineStruct("Float_compression_info")]
		public struct Float
		{
			// Token: 0x170009BD RID: 2493
			// (get) Token: 0x06003D00 RID: 15616 RVA: 0x000F1E0A File Offset: 0x000F000A
			public static CompressionInfo.Float FullPrecision { get; } = new CompressionInfo.Float(true);

			// Token: 0x06003D01 RID: 15617 RVA: 0x000F1E14 File Offset: 0x000F0014
			public Float(float minimumValue, float maximumValue, int numberOfBits)
			{
				this.minimumValue = minimumValue;
				this.maximumValue = maximumValue;
				this.numberOfBits = numberOfBits;
				float num = maximumValue - minimumValue;
				int num2 = (1 << numberOfBits) - 1;
				this.precision = num / (float)num2;
			}

			// Token: 0x06003D02 RID: 15618 RVA: 0x000F1E50 File Offset: 0x000F0050
			public Float(float minimumValue, int numberOfBits, float precision)
			{
				this.minimumValue = minimumValue;
				this.precision = precision;
				this.numberOfBits = numberOfBits;
				int num = (1 << numberOfBits) - 1;
				float num2 = precision * (float)num;
				this.maximumValue = num2 + minimumValue;
			}

			// Token: 0x06003D03 RID: 15619 RVA: 0x000F1E89 File Offset: 0x000F0089
			private Float(bool isFullPrecision)
			{
				this.minimumValue = float.MinValue;
				this.maximumValue = float.MaxValue;
				this.precision = 0f;
				this.numberOfBits = 32;
			}

			// Token: 0x06003D04 RID: 15620 RVA: 0x000F1EB4 File Offset: 0x000F00B4
			public int GetNumBits()
			{
				return this.numberOfBits;
			}

			// Token: 0x06003D05 RID: 15621 RVA: 0x000F1EBC File Offset: 0x000F00BC
			public float GetMaximumValue()
			{
				return this.maximumValue;
			}

			// Token: 0x06003D06 RID: 15622 RVA: 0x000F1EC4 File Offset: 0x000F00C4
			public float GetMinimumValue()
			{
				return this.minimumValue;
			}

			// Token: 0x06003D07 RID: 15623 RVA: 0x000F1ECC File Offset: 0x000F00CC
			public float GetPrecision()
			{
				return this.precision;
			}

			// Token: 0x04001F2D RID: 7981
			private readonly float minimumValue;

			// Token: 0x04001F2E RID: 7982
			private readonly float maximumValue;

			// Token: 0x04001F2F RID: 7983
			private readonly float precision;

			// Token: 0x04001F30 RID: 7984
			private readonly int numberOfBits;
		}
	}
}
