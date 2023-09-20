using System;
using System.Numerics;

namespace TaleWorlds.TwoDimension
{
	// Token: 0x02000029 RID: 41
	public static class Mathf
	{
		// Token: 0x060001B5 RID: 437 RVA: 0x00007601 File Offset: 0x00005801
		public static float Sqrt(float f)
		{
			return (float)Math.Sqrt((double)f);
		}

		// Token: 0x060001B6 RID: 438 RVA: 0x0000760B File Offset: 0x0000580B
		public static float Abs(float f)
		{
			return Math.Abs(f);
		}

		// Token: 0x060001B7 RID: 439 RVA: 0x00007614 File Offset: 0x00005814
		public static float Floor(float f)
		{
			return (float)Math.Floor((double)f);
		}

		// Token: 0x060001B8 RID: 440 RVA: 0x0000761E File Offset: 0x0000581E
		public static float Cos(float radian)
		{
			return (float)Math.Cos((double)radian);
		}

		// Token: 0x060001B9 RID: 441 RVA: 0x00007628 File Offset: 0x00005828
		public static float Sin(float radian)
		{
			return (float)Math.Sin((double)radian);
		}

		// Token: 0x060001BA RID: 442 RVA: 0x00007632 File Offset: 0x00005832
		public static float Acos(float f)
		{
			return (float)Math.Acos((double)f);
		}

		// Token: 0x060001BB RID: 443 RVA: 0x0000763C File Offset: 0x0000583C
		public static float Atan2(float y, float x)
		{
			return (float)Math.Atan2((double)y, (double)x);
		}

		// Token: 0x060001BC RID: 444 RVA: 0x00007648 File Offset: 0x00005848
		public static float Clamp(float value, float min, float max)
		{
			if (value > max)
			{
				return max;
			}
			if (value >= min)
			{
				return value;
			}
			return min;
		}

		// Token: 0x060001BD RID: 445 RVA: 0x00007657 File Offset: 0x00005857
		public static int Clamp(int value, int min, int max)
		{
			if (value > max)
			{
				return max;
			}
			if (value >= min)
			{
				return value;
			}
			return min;
		}

		// Token: 0x060001BE RID: 446 RVA: 0x00007666 File Offset: 0x00005866
		public static float Min(float a, float b)
		{
			if (a <= b)
			{
				return a;
			}
			return b;
		}

		// Token: 0x060001BF RID: 447 RVA: 0x0000766F File Offset: 0x0000586F
		public static float Max(float a, float b)
		{
			if (a <= b)
			{
				return b;
			}
			return a;
		}

		// Token: 0x060001C0 RID: 448 RVA: 0x00007678 File Offset: 0x00005878
		public static bool IsZero(float f)
		{
			return f < 1E-05f && f > -1E-05f;
		}

		// Token: 0x060001C1 RID: 449 RVA: 0x0000768C File Offset: 0x0000588C
		public static bool IsZero(Vector2 vector2)
		{
			return Mathf.IsZero(vector2.X) && Mathf.IsZero(vector2.Y);
		}

		// Token: 0x060001C2 RID: 450 RVA: 0x000076A8 File Offset: 0x000058A8
		public static float Sign(float f)
		{
			return (float)Math.Sign(f);
		}

		// Token: 0x060001C3 RID: 451 RVA: 0x000076B1 File Offset: 0x000058B1
		public static float Ceil(float f)
		{
			return (float)Math.Ceiling((double)f);
		}

		// Token: 0x060001C4 RID: 452 RVA: 0x000076BB File Offset: 0x000058BB
		public static float Round(float f)
		{
			return (float)Math.Round((double)f);
		}

		// Token: 0x060001C5 RID: 453 RVA: 0x000076C5 File Offset: 0x000058C5
		public static float Lerp(float start, float end, float amount)
		{
			return (end - start) * amount + start;
		}

		// Token: 0x060001C6 RID: 454 RVA: 0x000076D0 File Offset: 0x000058D0
		private static float PingPong(float min, float max, float time)
		{
			int num = (int)(min * 100f);
			int num2 = (int)(max * 100f);
			int num3 = (int)(time * 100f);
			int num4 = num2 - num;
			bool flag = num3 / num4 % 2 == 0;
			int num5 = num3 % num4;
			return (float)(flag ? (num5 + num) : (num2 - num5)) / 100f;
		}

		// Token: 0x040000E4 RID: 228
		public const float PI = 3.1415927f;

		// Token: 0x040000E5 RID: 229
		public const float Deg2Rad = 0.017453292f;

		// Token: 0x040000E6 RID: 230
		public const float Rad2Deg = 57.295776f;

		// Token: 0x040000E7 RID: 231
		public const float Epsilon = 1E-05f;
	}
}
