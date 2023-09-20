using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Library;

namespace TaleWorlds.Core
{
	// Token: 0x020000A9 RID: 169
	public static class MBRandom
	{
		// Token: 0x170002AF RID: 687
		// (get) Token: 0x06000838 RID: 2104 RVA: 0x0001BFF1 File Offset: 0x0001A1F1
		private static MBFastRandom Random
		{
			get
			{
				return Game.Current.RandomGenerator;
			}
		}

		// Token: 0x170002B0 RID: 688
		// (get) Token: 0x06000839 RID: 2105 RVA: 0x0001BFFD File Offset: 0x0001A1FD
		public static float RandomFloat
		{
			get
			{
				return MBRandom.Random.NextFloat();
			}
		}

		// Token: 0x0600083A RID: 2106 RVA: 0x0001C009 File Offset: 0x0001A209
		public static float RandomFloatRanged(float maxVal)
		{
			return MBRandom.RandomFloat * maxVal;
		}

		// Token: 0x0600083B RID: 2107 RVA: 0x0001C012 File Offset: 0x0001A212
		public static float RandomFloatRanged(float minVal, float maxVal)
		{
			return minVal + MBRandom.RandomFloat * (maxVal - minVal);
		}

		// Token: 0x170002B1 RID: 689
		// (get) Token: 0x0600083C RID: 2108 RVA: 0x0001C020 File Offset: 0x0001A220
		public static float RandomFloatNormal
		{
			get
			{
				int num = 4;
				float num2;
				float num4;
				do
				{
					num2 = 2f * MBRandom.RandomFloat - 1f;
					float num3 = 2f * MBRandom.RandomFloat - 1f;
					num4 = num2 * num2 + num3 * num3;
					num--;
				}
				while (num4 >= 1f || (num4 == 0f && num > 0));
				return num2 * num4 * 1f;
			}
		}

		// Token: 0x0600083D RID: 2109 RVA: 0x0001C07C File Offset: 0x0001A27C
		public static int RandomInt()
		{
			return MBRandom.Random.Next();
		}

		// Token: 0x0600083E RID: 2110 RVA: 0x0001C088 File Offset: 0x0001A288
		public static int RandomInt(int maxValue)
		{
			return MBRandom.Random.Next(maxValue);
		}

		// Token: 0x0600083F RID: 2111 RVA: 0x0001C095 File Offset: 0x0001A295
		public static int RandomInt(int minValue, int maxValue)
		{
			return MBRandom.Random.Next(minValue, maxValue);
		}

		// Token: 0x06000840 RID: 2112 RVA: 0x0001C0A4 File Offset: 0x0001A2A4
		public static int RoundRandomized(float f)
		{
			int num = MathF.Floor(f);
			float num2 = f - (float)num;
			if (MBRandom.RandomFloat < num2)
			{
				num++;
			}
			return num;
		}

		// Token: 0x06000841 RID: 2113 RVA: 0x0001C0CC File Offset: 0x0001A2CC
		public static T ChooseWeighted<T>(IReadOnlyList<ValueTuple<T, float>> weightList)
		{
			int num;
			return MBRandom.ChooseWeighted<T>(weightList, out num);
		}

		// Token: 0x06000842 RID: 2114 RVA: 0x0001C0E4 File Offset: 0x0001A2E4
		public static T ChooseWeighted<T>(IReadOnlyList<ValueTuple<T, float>> weightList, out int chosenIndex)
		{
			chosenIndex = -1;
			float num = weightList.Sum((ValueTuple<T, float> x) => x.Item2);
			float num2 = MBRandom.RandomFloat * num;
			for (int i = 0; i < weightList.Count; i++)
			{
				num2 -= weightList[i].Item2;
				if (num2 <= 0f)
				{
					chosenIndex = i;
					return weightList[i].Item1;
				}
			}
			if (weightList.Count > 0)
			{
				chosenIndex = 0;
				return weightList[0].Item1;
			}
			chosenIndex = -1;
			return default(T);
		}

		// Token: 0x06000843 RID: 2115 RVA: 0x0001C17F File Offset: 0x0001A37F
		public static void SetSeed(uint seed, uint seed2)
		{
			MBRandom.Random.SetSeed(seed, seed2);
		}

		// Token: 0x170002B2 RID: 690
		// (get) Token: 0x06000844 RID: 2116 RVA: 0x0001C18D File Offset: 0x0001A38D
		public static float NondeterministicRandomFloat
		{
			get
			{
				return MBRandom.NondeterministicRandom.NextFloat();
			}
		}

		// Token: 0x170002B3 RID: 691
		// (get) Token: 0x06000845 RID: 2117 RVA: 0x0001C199 File Offset: 0x0001A399
		public static int NondeterministicRandomInt
		{
			get
			{
				return MBRandom.NondeterministicRandom.Next();
			}
		}

		// Token: 0x06000846 RID: 2118 RVA: 0x0001C1A5 File Offset: 0x0001A3A5
		public static int RandomIntWithSeed(uint seed, uint seed2)
		{
			return MBFastRandom.GetRandomInt(seed, seed2);
		}

		// Token: 0x06000847 RID: 2119 RVA: 0x0001C1AE File Offset: 0x0001A3AE
		public static float RandomFloatWithSeed(uint seed, uint seed2)
		{
			return MBFastRandom.GetRandomFloat(seed, seed2);
		}

		// Token: 0x040004AE RID: 1198
		public const int MaxSeed = 2000;

		// Token: 0x040004AF RID: 1199
		private static readonly MBFastRandom NondeterministicRandom = new MBFastRandom();
	}
}
