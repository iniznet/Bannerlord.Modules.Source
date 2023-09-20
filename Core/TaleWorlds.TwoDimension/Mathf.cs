using System;
using System.Numerics;

namespace TaleWorlds.TwoDimension
{
	public static class Mathf
	{
		public static float Sqrt(float f)
		{
			return (float)Math.Sqrt((double)f);
		}

		public static float Abs(float f)
		{
			return Math.Abs(f);
		}

		public static float Floor(float f)
		{
			return (float)Math.Floor((double)f);
		}

		public static float Cos(float radian)
		{
			return (float)Math.Cos((double)radian);
		}

		public static float Sin(float radian)
		{
			return (float)Math.Sin((double)radian);
		}

		public static float Acos(float f)
		{
			return (float)Math.Acos((double)f);
		}

		public static float Atan2(float y, float x)
		{
			return (float)Math.Atan2((double)y, (double)x);
		}

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

		public static float Min(float a, float b)
		{
			if (a <= b)
			{
				return a;
			}
			return b;
		}

		public static float Max(float a, float b)
		{
			if (a <= b)
			{
				return b;
			}
			return a;
		}

		public static bool IsZero(float f)
		{
			return f < 1E-05f && f > -1E-05f;
		}

		public static bool IsZero(Vector2 vector2)
		{
			return Mathf.IsZero(vector2.X) && Mathf.IsZero(vector2.Y);
		}

		public static float Sign(float f)
		{
			return (float)Math.Sign(f);
		}

		public static float Ceil(float f)
		{
			return (float)Math.Ceiling((double)f);
		}

		public static float Round(float f)
		{
			return (float)Math.Round((double)f);
		}

		public static float Lerp(float start, float end, float amount)
		{
			return (end - start) * amount + start;
		}

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

		public const float PI = 3.1415927f;

		public const float Deg2Rad = 0.017453292f;

		public const float Rad2Deg = 57.295776f;

		public const float Epsilon = 1E-05f;
	}
}
