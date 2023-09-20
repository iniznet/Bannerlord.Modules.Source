using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem
{
	// Token: 0x02000090 RID: 144
	public static class RandomOwnerExtensions
	{
		// Token: 0x060010DD RID: 4317 RVA: 0x0004AAAA File Offset: 0x00048CAA
		public static int RandomIntWithSeed(this IRandomOwner obj, uint seed)
		{
			return MBRandom.RandomIntWithSeed((uint)obj.RandomValue, seed);
		}

		// Token: 0x060010DE RID: 4318 RVA: 0x0004AAB8 File Offset: 0x00048CB8
		public static int RandomIntWithSeed(this IRandomOwner obj, uint seed, int max)
		{
			return obj.RandomIntWithSeed(seed, 0, max);
		}

		// Token: 0x060010DF RID: 4319 RVA: 0x0004AAC3 File Offset: 0x00048CC3
		public static int RandomIntWithSeed(this IRandomOwner obj, uint seed, int min, int max)
		{
			return RandomOwnerExtensions.Random(obj.RandomIntWithSeed(seed), min, max);
		}

		// Token: 0x060010E0 RID: 4320 RVA: 0x0004AAD3 File Offset: 0x00048CD3
		public static float RandomFloatWithSeed(this IRandomOwner obj, uint seed)
		{
			return MBRandom.RandomFloatWithSeed((uint)obj.RandomValue, seed);
		}

		// Token: 0x060010E1 RID: 4321 RVA: 0x0004AAE1 File Offset: 0x00048CE1
		public static float RandomFloatWithSeed(this IRandomOwner obj, uint seed, float max)
		{
			return obj.RandomFloatWithSeed(seed, 0f, max);
		}

		// Token: 0x060010E2 RID: 4322 RVA: 0x0004AAF0 File Offset: 0x00048CF0
		public static float RandomFloatWithSeed(this IRandomOwner obj, uint seed, float min, float max)
		{
			return RandomOwnerExtensions.Random(obj.RandomFloatWithSeed(seed), min, max);
		}

		// Token: 0x060010E3 RID: 4323 RVA: 0x0004AB00 File Offset: 0x00048D00
		public static int RandomInt(this IRandomOwner obj)
		{
			return obj.RandomValue;
		}

		// Token: 0x060010E4 RID: 4324 RVA: 0x0004AB08 File Offset: 0x00048D08
		public static int RandomInt(this IRandomOwner obj, int max)
		{
			return obj.RandomInt(0, max);
		}

		// Token: 0x060010E5 RID: 4325 RVA: 0x0004AB12 File Offset: 0x00048D12
		public static int RandomInt(this IRandomOwner obj, int min, int max)
		{
			return RandomOwnerExtensions.Random(obj.RandomInt(), min, max);
		}

		// Token: 0x060010E6 RID: 4326 RVA: 0x0004AB21 File Offset: 0x00048D21
		public static float RandomFloat(this IRandomOwner obj)
		{
			return (float)obj.RandomValue / 2.1474836E+09f;
		}

		// Token: 0x060010E7 RID: 4327 RVA: 0x0004AB30 File Offset: 0x00048D30
		public static float RandomFloat(this IRandomOwner obj, float max)
		{
			return obj.RandomFloat(0f, max);
		}

		// Token: 0x060010E8 RID: 4328 RVA: 0x0004AB3E File Offset: 0x00048D3E
		public static float RandomFloat(this IRandomOwner obj, float min, float max)
		{
			return RandomOwnerExtensions.Random(obj.RandomFloat(), min, max);
		}

		// Token: 0x060010E9 RID: 4329 RVA: 0x0004AB50 File Offset: 0x00048D50
		private static int Random(int randomValue, int min, int max)
		{
			int num = max - min;
			if (num == 0)
			{
				Debug.FailedAssert("invalid Random parameters", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\IRandomOwner.cs", "Random", 78);
				return 0;
			}
			return min + randomValue % num;
		}

		// Token: 0x060010EA RID: 4330 RVA: 0x0004AB84 File Offset: 0x00048D84
		private static float Random(float randomValue, float min, float max)
		{
			float num = max - min;
			if (num <= 1E-45f)
			{
				Debug.FailedAssert("invalid Random parameters", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\IRandomOwner.cs", "Random", 90);
				return min;
			}
			return min + randomValue * num;
		}
	}
}
