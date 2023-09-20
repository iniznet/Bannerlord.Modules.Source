using System;
using System.Globalization;
using System.Numerics;

namespace TaleWorlds.Library
{
	public struct Color
	{
		public Color(float red, float green, float blue, float alpha = 1f)
		{
			this.Red = red;
			this.Green = green;
			this.Blue = blue;
			this.Alpha = alpha;
		}

		public Vector3 ToVector3()
		{
			return new Vector3(this.Red, this.Green, this.Blue);
		}

		public Vec3 ToVec3()
		{
			return new Vec3(this.Red, this.Green, this.Blue, this.Alpha);
		}

		public static Color operator *(Color c, float f)
		{
			float num = c.Red * f;
			float num2 = c.Green * f;
			float num3 = c.Blue * f;
			float num4 = c.Alpha * f;
			return new Color(num, num2, num3, num4);
		}

		public static Color operator *(Color c1, Color c2)
		{
			return new Color(c1.Red * c2.Red, c1.Green * c2.Green, c1.Blue * c2.Blue, c1.Alpha * c2.Alpha);
		}

		public static Color operator +(Color c1, Color c2)
		{
			return new Color(c1.Red + c2.Red, c1.Green + c2.Green, c1.Blue + c2.Blue, c1.Alpha + c2.Alpha);
		}

		public static Color operator -(Color c1, Color c2)
		{
			return new Color(c1.Red - c2.Red, c1.Green - c2.Green, c1.Blue - c2.Blue, c1.Alpha - c2.Alpha);
		}

		public static Color Black
		{
			get
			{
				return new Color(0f, 0f, 0f, 1f);
			}
		}

		public static Color White
		{
			get
			{
				return new Color(1f, 1f, 1f, 1f);
			}
		}

		public static bool operator ==(Color a, Color b)
		{
			return a.Red == b.Red && a.Green == b.Green && a.Blue == b.Blue && a.Alpha == b.Alpha;
		}

		public static bool operator !=(Color a, Color b)
		{
			return !(a == b);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (!(obj is Color))
			{
				return false;
			}
			Color color = (Color)obj;
			return this == color;
		}

		public static Color FromVector3(Vector3 vector3)
		{
			return new Color(vector3.X, vector3.Y, vector3.Z, 1f);
		}

		public static Color FromVector3(Vec3 vector3)
		{
			return new Color(vector3.x, vector3.y, vector3.z, 1f);
		}

		public float Length()
		{
			return MathF.Sqrt(this.Red * this.Red + this.Green * this.Green + this.Blue * this.Blue + this.Alpha * this.Alpha);
		}

		public uint ToUnsignedInteger()
		{
			byte b = (byte)(this.Red * 255f);
			byte b2 = (byte)(this.Green * 255f);
			byte b3 = (byte)(this.Blue * 255f);
			return (uint)(((int)((byte)(this.Alpha * 255f)) << 24) + ((int)b << 16) + ((int)b2 << 8) + (int)b3);
		}

		public static Color FromUint(uint color)
		{
			float num = (float)((byte)(color >> 24));
			byte b = (byte)(color >> 16);
			byte b2 = (byte)(color >> 8);
			byte b3 = (byte)color;
			float num2 = num * 0.003921569f;
			float num3 = (float)b * 0.003921569f;
			float num4 = (float)b2 * 0.003921569f;
			float num5 = (float)b3 * 0.003921569f;
			return new Color(num3, num4, num5, num2);
		}

		public static Color ConvertStringToColor(string color)
		{
			string text = color.Substring(1, 2);
			string text2 = color.Substring(3, 2);
			string text3 = color.Substring(5, 2);
			string text4 = color.Substring(7, 2);
			int num = int.Parse(text, NumberStyles.HexNumber);
			int num2 = int.Parse(text2, NumberStyles.HexNumber);
			int num3 = int.Parse(text3, NumberStyles.HexNumber);
			int num4 = int.Parse(text4, NumberStyles.HexNumber);
			return new Color((float)num * 0.003921569f, (float)num2 * 0.003921569f, (float)num3 * 0.003921569f, (float)num4 * 0.003921569f);
		}

		public static Color Lerp(Color start, Color end, float ratio)
		{
			float num = start.Red * (1f - ratio) + end.Red * ratio;
			float num2 = start.Green * (1f - ratio) + end.Green * ratio;
			float num3 = start.Blue * (1f - ratio) + end.Blue * ratio;
			float num4 = start.Alpha * (1f - ratio) + end.Alpha * ratio;
			return new Color(num, num2, num3, num4);
		}

		public override string ToString()
		{
			byte b = (byte)(this.Red * 255f);
			byte b2 = (byte)(this.Green * 255f);
			byte b3 = (byte)(this.Blue * 255f);
			byte b4 = (byte)(this.Alpha * 255f);
			return string.Concat(new string[]
			{
				"#",
				b.ToString("X2"),
				b2.ToString("X2"),
				b3.ToString("X2"),
				b4.ToString("X2")
			});
		}

		public static string UIntToColorString(uint color)
		{
			string text = (color >> 24).ToString("X");
			if (text.Length == 1)
			{
				text = text.Insert(0, "0");
			}
			string text2 = (color >> 16).ToString("X");
			if (text2.Length == 1)
			{
				text2 = text2.Insert(0, "0");
			}
			text2 = text2.Substring(MathF.Max(0, text2.Length - 2));
			string text3 = (color >> 8).ToString("X");
			if (text3.Length == 1)
			{
				text3 = text3.Insert(0, "0");
			}
			text3 = text3.Substring(MathF.Max(0, text3.Length - 2));
			uint num = color;
			string text4 = num.ToString("X");
			if (text4.Length == 1)
			{
				text4 = text4.Insert(0, "0");
			}
			text4 = text4.Substring(MathF.Max(0, text4.Length - 2));
			return text2 + text3 + text4 + text;
		}

		public float Red;

		public float Green;

		public float Blue;

		public float Alpha;
	}
}
