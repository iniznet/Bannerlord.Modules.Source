using System;
using System.Numerics;
using System.Xml.Serialization;

namespace TaleWorlds.Library
{
	[Serializable]
	public struct Vec3
	{
		public float X
		{
			get
			{
				return this.x;
			}
		}

		public float Y
		{
			get
			{
				return this.y;
			}
		}

		public float Z
		{
			get
			{
				return this.z;
			}
		}

		public Vec3(float x = 0f, float y = 0f, float z = 0f, float w = -1f)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}

		public Vec3(Vec3 c, float w = -1f)
		{
			this.x = c.x;
			this.y = c.y;
			this.z = c.z;
			this.w = w;
		}

		public Vec3(Vec2 xy, float z = 0f, float w = -1f)
		{
			this.x = xy.x;
			this.y = xy.y;
			this.z = z;
			this.w = w;
		}

		public Vec3(Vector3 vector3)
		{
			this = new Vec3(vector3.X, vector3.Y, vector3.Z, -1f);
		}

		public static Vec3 Abs(Vec3 vec)
		{
			return new Vec3(MathF.Abs(vec.x), MathF.Abs(vec.y), MathF.Abs(vec.z), -1f);
		}

		public static explicit operator Vector3(Vec3 vec3)
		{
			return new Vector3(vec3.x, vec3.y, vec3.z);
		}

		public float this[int i]
		{
			get
			{
				switch (i)
				{
				case 0:
					return this.x;
				case 1:
					return this.y;
				case 2:
					return this.z;
				case 3:
					return this.w;
				default:
					throw new IndexOutOfRangeException("Vec3 out of bounds.");
				}
			}
			set
			{
				switch (i)
				{
				case 0:
					this.x = value;
					return;
				case 1:
					this.y = value;
					return;
				case 2:
					this.z = value;
					return;
				case 3:
					this.w = value;
					return;
				default:
					throw new IndexOutOfRangeException("Vec3 out of bounds.");
				}
			}
		}

		public static float DotProduct(Vec3 v1, Vec3 v2)
		{
			return v1.x * v2.x + v1.y * v2.y + v1.z * v2.z;
		}

		public static Vec3 Lerp(Vec3 v1, Vec3 v2, float alpha)
		{
			return v1 * (1f - alpha) + v2 * alpha;
		}

		public static Vec3 Slerp(Vec3 start, Vec3 end, float percent)
		{
			float num = Vec3.DotProduct(start, end);
			num = MBMath.ClampFloat(num, -1f, 1f);
			float num2 = MathF.Acos(num) * percent;
			Vec3 vec = end - start * num;
			vec.Normalize();
			return start * MathF.Cos(num2) + vec * MathF.Sin(num2);
		}

		public static Vec3 Vec3Max(Vec3 v1, Vec3 v2)
		{
			return new Vec3(MathF.Max(v1.x, v2.x), MathF.Max(v1.y, v2.y), MathF.Max(v1.z, v2.z), -1f);
		}

		public static Vec3 Vec3Min(Vec3 v1, Vec3 v2)
		{
			return new Vec3(MathF.Min(v1.x, v2.x), MathF.Min(v1.y, v2.y), MathF.Min(v1.z, v2.z), -1f);
		}

		public static Vec3 CrossProduct(Vec3 va, Vec3 vb)
		{
			return new Vec3(va.y * vb.z - va.z * vb.y, va.z * vb.x - va.x * vb.z, va.x * vb.y - va.y * vb.x, -1f);
		}

		public static Vec3 operator -(Vec3 v)
		{
			return new Vec3(-v.x, -v.y, -v.z, -1f);
		}

		public static Vec3 operator +(Vec3 v1, Vec3 v2)
		{
			return new Vec3(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z, -1f);
		}

		public static Vec3 operator -(Vec3 v1, Vec3 v2)
		{
			return new Vec3(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z, -1f);
		}

		public static Vec3 operator *(Vec3 v, float f)
		{
			return new Vec3(v.x * f, v.y * f, v.z * f, -1f);
		}

		public static Vec3 operator *(float f, Vec3 v)
		{
			return new Vec3(v.x * f, v.y * f, v.z * f, -1f);
		}

		public static Vec3 operator *(Vec3 v, MatrixFrame frame)
		{
			return new Vec3(frame.rotation.s.x * v.x + frame.rotation.f.x * v.y + frame.rotation.u.x * v.z + frame.origin.x * v.w, frame.rotation.s.y * v.x + frame.rotation.f.y * v.y + frame.rotation.u.y * v.z + frame.origin.y * v.w, frame.rotation.s.z * v.x + frame.rotation.f.z * v.y + frame.rotation.u.z * v.z + frame.origin.z * v.w, frame.rotation.s.w * v.x + frame.rotation.f.w * v.y + frame.rotation.u.w * v.z + frame.origin.w * v.w);
		}

		public static Vec3 operator /(Vec3 v, float f)
		{
			f = 1f / f;
			return new Vec3(v.x * f, v.y * f, v.z * f, -1f);
		}

		public static bool operator ==(Vec3 v1, Vec3 v2)
		{
			return v1.x == v2.x && v1.y == v2.y && v1.z == v2.z;
		}

		public static bool operator !=(Vec3 v1, Vec3 v2)
		{
			return v1.x != v2.x || v1.y != v2.y || v1.z != v2.z;
		}

		public float Length
		{
			get
			{
				return MathF.Sqrt(this.x * this.x + this.y * this.y + this.z * this.z);
			}
		}

		public float LengthSquared
		{
			get
			{
				return this.x * this.x + this.y * this.y + this.z * this.z;
			}
		}

		public override bool Equals(object obj)
		{
			return obj != null && !(base.GetType() != obj.GetType()) && (((Vec3)obj).x == this.x && ((Vec3)obj).y == this.y) && ((Vec3)obj).z == this.z;
		}

		public override int GetHashCode()
		{
			return (int)(1001f * this.x + 10039f * this.y + 117f * this.z);
		}

		public bool IsValid
		{
			get
			{
				return !float.IsNaN(this.x) && !float.IsNaN(this.y) && !float.IsNaN(this.z) && !float.IsInfinity(this.x) && !float.IsInfinity(this.y) && !float.IsInfinity(this.z);
			}
		}

		public bool IsValidXYZW
		{
			get
			{
				return !float.IsNaN(this.x) && !float.IsNaN(this.y) && !float.IsNaN(this.z) && !float.IsNaN(this.w) && !float.IsInfinity(this.x) && !float.IsInfinity(this.y) && !float.IsInfinity(this.z) && !float.IsInfinity(this.w);
			}
		}

		public bool IsUnit
		{
			get
			{
				float lengthSquared = this.LengthSquared;
				return lengthSquared > 0.98010004f && lengthSquared < 1.0201f;
			}
		}

		public bool IsNonZero
		{
			get
			{
				return this.x != 0f || this.y != 0f || this.z != 0f;
			}
		}

		public Vec3 NormalizedCopy()
		{
			Vec3 vec = this;
			vec.Normalize();
			return vec;
		}

		public float Normalize()
		{
			float length = this.Length;
			if (length > 1E-05f)
			{
				float num = 1f / length;
				this.x *= num;
				this.y *= num;
				this.z *= num;
			}
			else
			{
				this.x = 0f;
				this.y = 1f;
				this.z = 0f;
			}
			return length;
		}

		public Vec3 ClampedCopy(float min, float max)
		{
			Vec3 vec = this;
			vec.x = MathF.Clamp(vec.x, min, max);
			vec.y = MathF.Clamp(vec.y, min, max);
			vec.z = MathF.Clamp(vec.z, min, max);
			return vec;
		}

		public Vec3 ClampedCopy(float min, float max, out bool valueClamped)
		{
			Vec3 vec = this;
			valueClamped = false;
			for (int i = 0; i < 3; i++)
			{
				if (vec[i] < min)
				{
					vec[i] = min;
					valueClamped = true;
				}
				else if (vec[i] > max)
				{
					vec[i] = max;
					valueClamped = true;
				}
			}
			return vec;
		}

		public void NormalizeWithoutChangingZ()
		{
			this.z = MBMath.ClampFloat(this.z, -0.99999f, 0.99999f);
			float length = this.AsVec2.Length;
			float num = MathF.Sqrt(1f - this.z * this.z);
			if (length < num - 1E-07f || length > num + 1E-07f)
			{
				if (length > 1E-09f)
				{
					float num2 = num / length;
					this.x *= num2;
					this.y *= num2;
					return;
				}
				this.x = 0f;
				this.y = num;
			}
		}

		public bool NearlyEquals(Vec3 v, float epsilon = 1E-05f)
		{
			return MathF.Abs(this.x - v.x) < epsilon && MathF.Abs(this.y - v.y) < epsilon && MathF.Abs(this.z - v.z) < epsilon;
		}

		public void RotateAboutX(float a)
		{
			float num;
			float num2;
			MathF.SinCos(a, out num, out num2);
			float num3 = this.y * num2 - this.z * num;
			this.z = this.z * num2 + this.y * num;
			this.y = num3;
		}

		public void RotateAboutY(float a)
		{
			float num;
			float num2;
			MathF.SinCos(a, out num, out num2);
			float num3 = this.x * num2 + this.z * num;
			this.z = this.z * num2 - this.x * num;
			this.x = num3;
		}

		public void RotateAboutZ(float a)
		{
			float num;
			float num2;
			MathF.SinCos(a, out num, out num2);
			float num3 = this.x * num2 - this.y * num;
			this.y = this.y * num2 + this.x * num;
			this.x = num3;
		}

		public Vec3 RotateAboutAnArbitraryVector(Vec3 vec, float a)
		{
			float num = vec.x;
			float num2 = vec.y;
			float num3 = vec.z;
			float num4 = num * this.x;
			float num5 = num * this.y;
			float num6 = num * this.z;
			float num7 = num2 * this.x;
			float num8 = num2 * this.y;
			float num9 = num2 * this.z;
			float num10 = num3 * this.x;
			float num11 = num3 * this.y;
			float num12 = num3 * this.z;
			float num13;
			float num14;
			MathF.SinCos(a, out num13, out num14);
			return new Vec3
			{
				x = num * (num4 + num8 + num12) + (this.x * (num2 * num2 + num3 * num3) - num * (num8 + num12)) * num14 + (-num11 + num9) * num13,
				y = num2 * (num4 + num8 + num12) + (this.y * (num * num + num3 * num3) - num2 * (num4 + num12)) * num14 + (num10 - num6) * num13,
				z = num3 * (num4 + num8 + num12) + (this.z * (num * num + num2 * num2) - num3 * (num4 + num8)) * num14 + (-num7 + num5) * num13
			};
		}

		public Vec3 Reflect(Vec3 normal)
		{
			return this - normal * (2f * Vec3.DotProduct(this, normal));
		}

		public Vec3 ProjectOnUnitVector(Vec3 ov)
		{
			return ov * (this.x * ov.x + this.y * ov.y + this.z * ov.z);
		}

		public float DistanceSquared(Vec3 v)
		{
			return (v.x - this.x) * (v.x - this.x) + (v.y - this.y) * (v.y - this.y) + (v.z - this.z) * (v.z - this.z);
		}

		public float Distance(Vec3 v)
		{
			return MathF.Sqrt((v.x - this.x) * (v.x - this.x) + (v.y - this.y) * (v.y - this.y) + (v.z - this.z) * (v.z - this.z));
		}

		public static float AngleBetweenTwoVectors(Vec3 v1, Vec3 v2)
		{
			return MathF.Acos(MathF.Clamp(Vec3.DotProduct(v1, v2) / (v1.Length * v2.Length), -1f, 1f));
		}

		public Vec2 AsVec2
		{
			get
			{
				return new Vec2(this.x, this.y);
			}
			set
			{
				this.x = value.x;
				this.y = value.y;
			}
		}

		public override string ToString()
		{
			return string.Concat(new object[] { "(", this.x, ", ", this.y, ", ", this.z, ")" });
		}

		public uint ToARGB
		{
			get
			{
				uint num = (uint)(this.w * 256f);
				uint num2 = (uint)(this.x * 256f);
				uint num3 = (uint)(this.y * 256f);
				uint num4 = (uint)(this.z * 256f);
				return (MathF.Min(num, 255U) << 24) | (MathF.Min(num2, 255U) << 16) | (MathF.Min(num3, 255U) << 8) | MathF.Min(num4, 255U);
			}
		}

		public float RotationZ
		{
			get
			{
				return MathF.Atan2(-this.x, this.y);
			}
		}

		public float RotationX
		{
			get
			{
				return MathF.Atan2(this.z, MathF.Sqrt(this.x * this.x + this.y * this.y));
			}
		}

		public static Vec3 Parse(string input)
		{
			input = input.Replace(" ", "");
			string[] array = input.Split(new char[] { ',' });
			if (array.Length < 3 || array.Length > 4)
			{
				throw new ArgumentOutOfRangeException();
			}
			float num = float.Parse(array[0]);
			float num2 = float.Parse(array[1]);
			float num3 = float.Parse(array[2]);
			float num4 = ((array.Length == 4) ? float.Parse(array[3]) : (-1f));
			return new Vec3(num, num2, num3, num4);
		}

		[XmlAttribute]
		public float x;

		[XmlAttribute]
		public float y;

		[XmlAttribute]
		public float z;

		[XmlAttribute]
		public float w;

		public static readonly Vec3 Side = new Vec3(1f, 0f, 0f, -1f);

		public static readonly Vec3 Forward = new Vec3(0f, 1f, 0f, -1f);

		public static readonly Vec3 Up = new Vec3(0f, 0f, 1f, -1f);

		public static readonly Vec3 One = new Vec3(1f, 1f, 1f, -1f);

		public static readonly Vec3 Zero = new Vec3(0f, 0f, 0f, -1f);

		public static readonly Vec3 Invalid = new Vec3(float.NaN, float.NaN, float.NaN, -1f);

		public struct StackArray8Vec3
		{
			public Vec3 this[int index]
			{
				get
				{
					switch (index)
					{
					case 0:
						return this._element0;
					case 1:
						return this._element1;
					case 2:
						return this._element2;
					case 3:
						return this._element3;
					case 4:
						return this._element4;
					case 5:
						return this._element5;
					case 6:
						return this._element6;
					case 7:
						return this._element7;
					default:
						Debug.FailedAssert("Index out of range.", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\Base\\TaleWorlds.Library\\Vec3.cs", "Item", 41);
						return Vec3.Zero;
					}
				}
				set
				{
					switch (index)
					{
					case 0:
						this._element0 = value;
						return;
					case 1:
						this._element1 = value;
						return;
					case 2:
						this._element2 = value;
						return;
					case 3:
						this._element3 = value;
						return;
					case 4:
						this._element4 = value;
						return;
					case 5:
						this._element5 = value;
						return;
					case 6:
						this._element6 = value;
						return;
					case 7:
						this._element7 = value;
						return;
					default:
						Debug.FailedAssert("Index out of range.", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\Base\\TaleWorlds.Library\\Vec3.cs", "Item", 59);
						return;
					}
				}
			}

			private Vec3 _element0;

			private Vec3 _element1;

			private Vec3 _element2;

			private Vec3 _element3;

			private Vec3 _element4;

			private Vec3 _element5;

			private Vec3 _element6;

			private Vec3 _element7;

			public const int Length = 8;
		}
	}
}
