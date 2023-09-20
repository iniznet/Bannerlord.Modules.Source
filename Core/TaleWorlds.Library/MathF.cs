using System;

namespace TaleWorlds.Library
{
	public static class MathF
	{
		public static float Sqrt(float x)
		{
			return (float)Math.Sqrt((double)x);
		}

		public static float Sin(float x)
		{
			return (float)Math.Sin((double)x);
		}

		public static float Asin(float x)
		{
			return (float)Math.Asin((double)x);
		}

		public static float Cos(float x)
		{
			return (float)Math.Cos((double)x);
		}

		public static float Acos(float x)
		{
			return (float)Math.Acos((double)x);
		}

		public static float Tan(float x)
		{
			return (float)Math.Tan((double)x);
		}

		public static float Tanh(float x)
		{
			return (float)Math.Tanh((double)x);
		}

		public static float Atan(float x)
		{
			return (float)Math.Atan((double)x);
		}

		public static float Atan2(float y, float x)
		{
			return (float)Math.Atan2((double)y, (double)x);
		}

		public static double Pow(double x, double y)
		{
			return Math.Pow(x, y);
		}

		[Obsolete("Types must match!", true)]
		public static double Pow(float x, double y)
		{
			return Math.Pow((double)x, y);
		}

		public static float Pow(float x, float y)
		{
			return (float)Math.Pow((double)x, (double)y);
		}

		public static int PowTwo32(int x)
		{
			return 1 << x;
		}

		public static ulong PowTwo64(int x)
		{
			return 1UL << x;
		}

		public static bool IsValidValue(float f)
		{
			return !float.IsNaN(f) && !float.IsInfinity(f);
		}

		public static float Clamp(float value, float minValue, float maxValue)
		{
			return MathF.Max(MathF.Min(value, maxValue), minValue);
		}

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

		public static float Lerp(float valueFrom, float valueTo, float amount, float minimumDifference = 1E-05f)
		{
			if (Math.Abs(valueFrom - valueTo) <= minimumDifference)
			{
				return valueTo;
			}
			return valueFrom + (valueTo - valueFrom) * amount;
		}

		public static float AngleLerp(float angleFrom, float angleTo, float amount, float minimumDifference = 1E-05f)
		{
			float num = (angleTo - angleFrom) % 6.2831855f;
			float num2 = 2f * num % 6.2831855f - num;
			return MathF.AngleClamp(angleFrom + num2 * amount);
		}

		public static int Round(double f)
		{
			return (int)Math.Round(f);
		}

		public static int Round(float f)
		{
			return (int)Math.Round((double)f);
		}

		public static float Round(float f, int digits)
		{
			return (float)Math.Round((double)f, digits);
		}

		[Obsolete("Type is already int!", true)]
		public static int Round(int f)
		{
			return (int)Math.Round((double)((float)f));
		}

		public static int Floor(double f)
		{
			return (int)Math.Floor(f);
		}

		public static int Floor(float f)
		{
			return (int)Math.Floor((double)f);
		}

		[Obsolete("Type is already int!", true)]
		public static int Floor(int f)
		{
			return (int)Math.Floor((double)((float)f));
		}

		public static int Ceiling(double f)
		{
			return (int)Math.Ceiling(f);
		}

		public static int Ceiling(float f)
		{
			return (int)Math.Ceiling((double)f);
		}

		[Obsolete("Type is already int!", true)]
		public static int Ceiling(int f)
		{
			return (int)Math.Ceiling((double)((float)f));
		}

		public static double Abs(double f)
		{
			if (f < 0.0)
			{
				return -f;
			}
			return f;
		}

		public static float Abs(float f)
		{
			if (f < 0f)
			{
				return -f;
			}
			return f;
		}

		public static int Abs(int f)
		{
			if ((float)f < 0f)
			{
				return -f;
			}
			return f;
		}

		public static double Max(double a, double b)
		{
			if (a <= b)
			{
				return b;
			}
			return a;
		}

		public static float Max(float a, float b)
		{
			if (a <= b)
			{
				return b;
			}
			return a;
		}

		[Obsolete("Types must match!", true)]
		public static float Max(float a, int b)
		{
			if (a <= (float)b)
			{
				return (float)b;
			}
			return a;
		}

		[Obsolete("Types must match!", true)]
		public static float Max(int a, float b)
		{
			if ((float)a <= b)
			{
				return b;
			}
			return (float)a;
		}

		public static int Max(int a, int b)
		{
			if (a <= b)
			{
				return b;
			}
			return a;
		}

		public static uint Max(uint a, uint b)
		{
			if (a <= b)
			{
				return b;
			}
			return a;
		}

		public static float Max(float a, float b, float c)
		{
			return Math.Max(a, Math.Max(b, c));
		}

		public static double Min(double a, double b)
		{
			if (a >= b)
			{
				return b;
			}
			return a;
		}

		public static float Min(float a, float b)
		{
			if (a >= b)
			{
				return b;
			}
			return a;
		}

		public static short Min(short a, short b)
		{
			if (a >= b)
			{
				return b;
			}
			return a;
		}

		public static int Min(int a, int b)
		{
			if (a >= b)
			{
				return b;
			}
			return a;
		}

		public static uint Min(uint a, uint b)
		{
			if (a >= b)
			{
				return b;
			}
			return a;
		}

		[Obsolete("Types must match!", true)]
		public static int Min(int a, float b)
		{
			if ((float)a >= b)
			{
				return (int)b;
			}
			return a;
		}

		[Obsolete("Types must match!", true)]
		public static int Min(float a, int b)
		{
			if (a >= (float)b)
			{
				return b;
			}
			return (int)a;
		}

		public static float Min(float a, float b, float c)
		{
			return Math.Min(a, Math.Min(b, c));
		}

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

		public static float Log(float a)
		{
			return (float)Math.Log((double)a);
		}

		public static float Log(float a, float newBase)
		{
			return (float)Math.Log((double)a, (double)newBase);
		}

		public static int Sign(float f)
		{
			return Math.Sign(f);
		}

		public static int Sign(int f)
		{
			return Math.Sign(f);
		}

		public static void SinCos(float a, out float sa, out float ca)
		{
			sa = MathF.Sin(a);
			ca = MathF.Cos(a);
		}

		public static float Log10(float val)
		{
			return (float)Math.Log10((double)val);
		}

		public const float DegToRad = 0.017453292f;

		public const float RadToDeg = 57.29578f;

		public const float TwoPI = 6.2831855f;

		public const float PI = 3.1415927f;

		public const float HalfPI = 1.5707964f;

		public const float E = 2.7182817f;

		public const float Epsilon = 1E-05f;
	}
}
