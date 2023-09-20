using System;

namespace TaleWorlds.Library
{
	// Token: 0x02000063 RID: 99
	public static class MathF
	{
		// Token: 0x06000312 RID: 786 RVA: 0x00009FF3 File Offset: 0x000081F3
		public static float Sqrt(float x)
		{
			return (float)Math.Sqrt((double)x);
		}

		// Token: 0x06000313 RID: 787 RVA: 0x00009FFD File Offset: 0x000081FD
		public static float Sin(float x)
		{
			return (float)Math.Sin((double)x);
		}

		// Token: 0x06000314 RID: 788 RVA: 0x0000A007 File Offset: 0x00008207
		public static float Asin(float x)
		{
			return (float)Math.Asin((double)x);
		}

		// Token: 0x06000315 RID: 789 RVA: 0x0000A011 File Offset: 0x00008211
		public static float Cos(float x)
		{
			return (float)Math.Cos((double)x);
		}

		// Token: 0x06000316 RID: 790 RVA: 0x0000A01B File Offset: 0x0000821B
		public static float Acos(float x)
		{
			return (float)Math.Acos((double)x);
		}

		// Token: 0x06000317 RID: 791 RVA: 0x0000A025 File Offset: 0x00008225
		public static float Tan(float x)
		{
			return (float)Math.Tan((double)x);
		}

		// Token: 0x06000318 RID: 792 RVA: 0x0000A02F File Offset: 0x0000822F
		public static float Tanh(float x)
		{
			return (float)Math.Tanh((double)x);
		}

		// Token: 0x06000319 RID: 793 RVA: 0x0000A039 File Offset: 0x00008239
		public static float Atan(float x)
		{
			return (float)Math.Atan((double)x);
		}

		// Token: 0x0600031A RID: 794 RVA: 0x0000A043 File Offset: 0x00008243
		public static float Atan2(float y, float x)
		{
			return (float)Math.Atan2((double)y, (double)x);
		}

		// Token: 0x0600031B RID: 795 RVA: 0x0000A04F File Offset: 0x0000824F
		public static double Pow(double x, double y)
		{
			return Math.Pow(x, y);
		}

		// Token: 0x0600031C RID: 796 RVA: 0x0000A058 File Offset: 0x00008258
		[Obsolete("Types must match!", true)]
		public static double Pow(float x, double y)
		{
			return Math.Pow((double)x, y);
		}

		// Token: 0x0600031D RID: 797 RVA: 0x0000A062 File Offset: 0x00008262
		public static float Pow(float x, float y)
		{
			return (float)Math.Pow((double)x, (double)y);
		}

		// Token: 0x0600031E RID: 798 RVA: 0x0000A06E File Offset: 0x0000826E
		public static int PowTwo32(int x)
		{
			return 1 << x;
		}

		// Token: 0x0600031F RID: 799 RVA: 0x0000A076 File Offset: 0x00008276
		public static ulong PowTwo64(int x)
		{
			return 1UL << x;
		}

		// Token: 0x06000320 RID: 800 RVA: 0x0000A07F File Offset: 0x0000827F
		public static bool IsValidValue(float f)
		{
			return !float.IsNaN(f) && !float.IsInfinity(f);
		}

		// Token: 0x06000321 RID: 801 RVA: 0x0000A094 File Offset: 0x00008294
		public static float Clamp(float value, float minValue, float maxValue)
		{
			return MathF.Max(MathF.Min(value, maxValue), minValue);
		}

		// Token: 0x06000322 RID: 802 RVA: 0x0000A0A3 File Offset: 0x000082A3
		public static float AngleClamp(float angle)
		{
			while (angle < 0f)
			{
				angle += 6.2831855f;
			}
			while (angle > 6.2831855f)
			{
				angle -= 6.2831855f;
			}
			return angle;
		}

		// Token: 0x06000323 RID: 803 RVA: 0x0000A0CC File Offset: 0x000082CC
		public static float Lerp(float valueFrom, float valueTo, float amount, float minimumDifference = 1E-05f)
		{
			if (Math.Abs(valueFrom - valueTo) <= minimumDifference)
			{
				return valueTo;
			}
			return valueFrom + (valueTo - valueFrom) * amount;
		}

		// Token: 0x06000324 RID: 804 RVA: 0x0000A0E4 File Offset: 0x000082E4
		public static float AngleLerp(float angleFrom, float angleTo, float amount, float minimumDifference = 1E-05f)
		{
			float num = (angleTo - angleFrom) % 6.2831855f;
			float num2 = 2f * num % 6.2831855f - num;
			return MathF.AngleClamp(angleFrom + num2 * amount);
		}

		// Token: 0x06000325 RID: 805 RVA: 0x0000A115 File Offset: 0x00008315
		public static int Round(double f)
		{
			return (int)Math.Round(f);
		}

		// Token: 0x06000326 RID: 806 RVA: 0x0000A11E File Offset: 0x0000831E
		public static int Round(float f)
		{
			return (int)Math.Round((double)f);
		}

		// Token: 0x06000327 RID: 807 RVA: 0x0000A128 File Offset: 0x00008328
		public static float Round(float f, int digits)
		{
			return (float)Math.Round((double)f, digits);
		}

		// Token: 0x06000328 RID: 808 RVA: 0x0000A133 File Offset: 0x00008333
		[Obsolete("Type is already int!", true)]
		public static int Round(int f)
		{
			return (int)Math.Round((double)((float)f));
		}

		// Token: 0x06000329 RID: 809 RVA: 0x0000A13E File Offset: 0x0000833E
		public static int Floor(double f)
		{
			return (int)Math.Floor(f);
		}

		// Token: 0x0600032A RID: 810 RVA: 0x0000A147 File Offset: 0x00008347
		public static int Floor(float f)
		{
			return (int)Math.Floor((double)f);
		}

		// Token: 0x0600032B RID: 811 RVA: 0x0000A151 File Offset: 0x00008351
		[Obsolete("Type is already int!", true)]
		public static int Floor(int f)
		{
			return (int)Math.Floor((double)((float)f));
		}

		// Token: 0x0600032C RID: 812 RVA: 0x0000A15C File Offset: 0x0000835C
		public static int Ceiling(double f)
		{
			return (int)Math.Ceiling(f);
		}

		// Token: 0x0600032D RID: 813 RVA: 0x0000A165 File Offset: 0x00008365
		public static int Ceiling(float f)
		{
			return (int)Math.Ceiling((double)f);
		}

		// Token: 0x0600032E RID: 814 RVA: 0x0000A16F File Offset: 0x0000836F
		[Obsolete("Type is already int!", true)]
		public static int Ceiling(int f)
		{
			return (int)Math.Ceiling((double)((float)f));
		}

		// Token: 0x0600032F RID: 815 RVA: 0x0000A17A File Offset: 0x0000837A
		public static double Abs(double f)
		{
			if (f < 0.0)
			{
				return -f;
			}
			return f;
		}

		// Token: 0x06000330 RID: 816 RVA: 0x0000A18C File Offset: 0x0000838C
		public static float Abs(float f)
		{
			if (f < 0f)
			{
				return -f;
			}
			return f;
		}

		// Token: 0x06000331 RID: 817 RVA: 0x0000A19A File Offset: 0x0000839A
		public static int Abs(int f)
		{
			if ((float)f < 0f)
			{
				return -f;
			}
			return f;
		}

		// Token: 0x06000332 RID: 818 RVA: 0x0000A1A9 File Offset: 0x000083A9
		public static double Max(double a, double b)
		{
			if (a <= b)
			{
				return b;
			}
			return a;
		}

		// Token: 0x06000333 RID: 819 RVA: 0x0000A1B2 File Offset: 0x000083B2
		public static float Max(float a, float b)
		{
			if (a <= b)
			{
				return b;
			}
			return a;
		}

		// Token: 0x06000334 RID: 820 RVA: 0x0000A1BB File Offset: 0x000083BB
		[Obsolete("Types must match!", true)]
		public static float Max(float a, int b)
		{
			if (a <= (float)b)
			{
				return (float)b;
			}
			return a;
		}

		// Token: 0x06000335 RID: 821 RVA: 0x0000A1C6 File Offset: 0x000083C6
		[Obsolete("Types must match!", true)]
		public static float Max(int a, float b)
		{
			if ((float)a <= b)
			{
				return b;
			}
			return (float)a;
		}

		// Token: 0x06000336 RID: 822 RVA: 0x0000A1D1 File Offset: 0x000083D1
		public static int Max(int a, int b)
		{
			if (a <= b)
			{
				return b;
			}
			return a;
		}

		// Token: 0x06000337 RID: 823 RVA: 0x0000A1DA File Offset: 0x000083DA
		public static uint Max(uint a, uint b)
		{
			if (a <= b)
			{
				return b;
			}
			return a;
		}

		// Token: 0x06000338 RID: 824 RVA: 0x0000A1E3 File Offset: 0x000083E3
		public static float Max(float a, float b, float c)
		{
			return Math.Max(a, Math.Max(b, c));
		}

		// Token: 0x06000339 RID: 825 RVA: 0x0000A1F2 File Offset: 0x000083F2
		public static double Min(double a, double b)
		{
			if (a >= b)
			{
				return b;
			}
			return a;
		}

		// Token: 0x0600033A RID: 826 RVA: 0x0000A1FB File Offset: 0x000083FB
		public static float Min(float a, float b)
		{
			if (a >= b)
			{
				return b;
			}
			return a;
		}

		// Token: 0x0600033B RID: 827 RVA: 0x0000A204 File Offset: 0x00008404
		public static short Min(short a, short b)
		{
			if (a >= b)
			{
				return b;
			}
			return a;
		}

		// Token: 0x0600033C RID: 828 RVA: 0x0000A20D File Offset: 0x0000840D
		public static int Min(int a, int b)
		{
			if (a >= b)
			{
				return b;
			}
			return a;
		}

		// Token: 0x0600033D RID: 829 RVA: 0x0000A216 File Offset: 0x00008416
		public static uint Min(uint a, uint b)
		{
			if (a >= b)
			{
				return b;
			}
			return a;
		}

		// Token: 0x0600033E RID: 830 RVA: 0x0000A21F File Offset: 0x0000841F
		[Obsolete("Types must match!", true)]
		public static int Min(int a, float b)
		{
			if ((float)a >= b)
			{
				return (int)b;
			}
			return a;
		}

		// Token: 0x0600033F RID: 831 RVA: 0x0000A22A File Offset: 0x0000842A
		[Obsolete("Types must match!", true)]
		public static int Min(float a, int b)
		{
			if (a >= (float)b)
			{
				return b;
			}
			return (int)a;
		}

		// Token: 0x06000340 RID: 832 RVA: 0x0000A235 File Offset: 0x00008435
		public static float Min(float a, float b, float c)
		{
			return Math.Min(a, Math.Min(b, c));
		}

		// Token: 0x06000341 RID: 833 RVA: 0x0000A244 File Offset: 0x00008444
		public static float PingPong(float min, float max, float time)
		{
			int num = (int)(min * 100f);
			int num2 = (int)(max * 100f);
			int num3 = (int)(time * 100f);
			int num4 = num2 - num;
			bool flag = num3 / num4 % 2 == 0;
			int num5 = num3 % num4;
			return (float)(flag ? (num5 + num) : (num2 - num5)) / 100f;
		}

		// Token: 0x06000342 RID: 834 RVA: 0x0000A290 File Offset: 0x00008490
		public static int GreatestCommonDivisor(int a, int b)
		{
			while (b != 0)
			{
				int num = a % b;
				a = b;
				b = num;
			}
			return a;
		}

		// Token: 0x06000343 RID: 835 RVA: 0x0000A2A0 File Offset: 0x000084A0
		public static float Log(float a)
		{
			return (float)Math.Log((double)a);
		}

		// Token: 0x06000344 RID: 836 RVA: 0x0000A2AA File Offset: 0x000084AA
		public static float Log(float a, float newBase)
		{
			return (float)Math.Log((double)a, (double)newBase);
		}

		// Token: 0x06000345 RID: 837 RVA: 0x0000A2B6 File Offset: 0x000084B6
		public static int Sign(float f)
		{
			return Math.Sign(f);
		}

		// Token: 0x06000346 RID: 838 RVA: 0x0000A2BE File Offset: 0x000084BE
		public static int Sign(int f)
		{
			return Math.Sign(f);
		}

		// Token: 0x06000347 RID: 839 RVA: 0x0000A2C6 File Offset: 0x000084C6
		public static void SinCos(float a, out float sa, out float ca)
		{
			sa = MathF.Sin(a);
			ca = MathF.Cos(a);
		}

		// Token: 0x06000348 RID: 840 RVA: 0x0000A2D8 File Offset: 0x000084D8
		public static float Log10(float val)
		{
			return (float)Math.Log10((double)val);
		}

		// Token: 0x04000103 RID: 259
		public const float DegToRad = 0.017453292f;

		// Token: 0x04000104 RID: 260
		public const float RadToDeg = 57.29578f;

		// Token: 0x04000105 RID: 261
		public const float TwoPI = 6.2831855f;

		// Token: 0x04000106 RID: 262
		public const float PI = 3.1415927f;

		// Token: 0x04000107 RID: 263
		public const float HalfPI = 1.5707964f;

		// Token: 0x04000108 RID: 264
		public const float E = 2.7182817f;

		// Token: 0x04000109 RID: 265
		public const float Epsilon = 1E-05f;
	}
}
