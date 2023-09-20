using System;
using System.Numerics;

namespace TaleWorlds.Library
{
	[Serializable]
	public struct Vec2
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

		public Vec2(float a, float b)
		{
			this.x = a;
			this.y = b;
		}

		public Vec2(Vec2 v)
		{
			this.x = v.x;
			this.y = v.y;
		}

		public Vec2(Vector2 v)
		{
			this.x = v.X;
			this.y = v.Y;
		}

		public Vec3 ToVec3(float z = 0f)
		{
			return new Vec3(this.x, this.y, z, -1f);
		}

		public static explicit operator Vector2(Vec2 vec2)
		{
			return new Vector2(vec2.x, vec2.y);
		}

		public static implicit operator Vec2(Vector2 vec2)
		{
			return new Vec2(vec2.X, vec2.Y);
		}

		public float this[int i]
		{
			get
			{
				if (i == 0)
				{
					return this.x;
				}
				if (i != 1)
				{
					throw new IndexOutOfRangeException("Vec2 out of bounds.");
				}
				return this.y;
			}
			set
			{
				if (i == 0)
				{
					this.x = value;
					return;
				}
				if (i != 1)
				{
					return;
				}
				this.y = value;
			}
		}

		public float Normalize()
		{
			float length = this.Length;
			if (length > 1E-05f)
			{
				this.x /= length;
				this.y /= length;
			}
			else
			{
				this.x = 0f;
				this.y = 1f;
			}
			return length;
		}

		public Vec2 Normalized()
		{
			Vec2 vec = this;
			vec.Normalize();
			return vec;
		}

		public static WindingOrder GetWindingOrder(Vec2 first, Vec2 second, Vec2 third)
		{
			Vec2 vec = second - first;
			float num = Vec2.CCW(third - second, vec);
			if (num > 0f)
			{
				return WindingOrder.Ccw;
			}
			if (num < 0f)
			{
				return WindingOrder.Cw;
			}
			return WindingOrder.None;
		}

		public static float CCW(Vec2 va, Vec2 vb)
		{
			return va.x * vb.y - va.y * vb.x;
		}

		public float Length
		{
			get
			{
				return MathF.Sqrt(this.x * this.x + this.y * this.y);
			}
		}

		public float LengthSquared
		{
			get
			{
				return this.x * this.x + this.y * this.y;
			}
		}

		public override bool Equals(object obj)
		{
			return obj != null && !(base.GetType() != obj.GetType()) && ((Vec2)obj).x == this.x && ((Vec2)obj).y == this.y;
		}

		public override int GetHashCode()
		{
			return (int)(1001f * this.x + 10039f * this.y);
		}

		public static bool operator ==(Vec2 v1, Vec2 v2)
		{
			return v1.x == v2.x && v1.y == v2.y;
		}

		public static bool operator !=(Vec2 v1, Vec2 v2)
		{
			return v1.x != v2.x || v1.y != v2.y;
		}

		public static Vec2 operator -(Vec2 v)
		{
			return new Vec2(-v.x, -v.y);
		}

		public static Vec2 operator +(Vec2 v1, Vec2 v2)
		{
			return new Vec2(v1.x + v2.x, v1.y + v2.y);
		}

		public static Vec2 operator -(Vec2 v1, Vec2 v2)
		{
			return new Vec2(v1.x - v2.x, v1.y - v2.y);
		}

		public static Vec2 operator *(Vec2 v, float f)
		{
			return new Vec2(v.x * f, v.y * f);
		}

		public static Vec2 operator *(float f, Vec2 v)
		{
			return new Vec2(v.x * f, v.y * f);
		}

		public static Vec2 operator /(float f, Vec2 v)
		{
			return new Vec2(v.x / f, v.y / f);
		}

		public static Vec2 operator /(Vec2 v, float f)
		{
			return new Vec2(v.x / f, v.y / f);
		}

		public bool IsUnit()
		{
			float length = this.Length;
			return (double)length > 0.95 && (double)length < 1.05;
		}

		public bool IsNonZero()
		{
			float num = 1E-05f;
			return this.x > num || this.x < -num || this.y > num || this.y < -num;
		}

		public bool NearlyEquals(Vec2 v, float epsilon = 1E-05f)
		{
			return MathF.Abs(this.x - v.x) < epsilon && MathF.Abs(this.y - v.y) < epsilon;
		}

		public void RotateCCW(float angleInRadians)
		{
			float num;
			float num2;
			MathF.SinCos(angleInRadians, out num, out num2);
			float num3 = this.x * num2 - this.y * num;
			this.y = this.y * num2 + this.x * num;
			this.x = num3;
		}

		public float DotProduct(Vec2 v)
		{
			return v.x * this.x + v.y * this.y;
		}

		public static float DotProduct(Vec2 va, Vec2 vb)
		{
			return va.x * vb.x + va.y * vb.y;
		}

		public float RotationInRadians
		{
			get
			{
				return MathF.Atan2(-this.x, this.y);
			}
		}

		public static Vec2 FromRotation(float rotation)
		{
			return new Vec2(-MathF.Sin(rotation), MathF.Cos(rotation));
		}

		public Vec2 TransformToLocalUnitF(Vec2 a)
		{
			return new Vec2(this.y * a.x - this.x * a.y, this.x * a.x + this.y * a.y);
		}

		public Vec2 TransformToParentUnitF(Vec2 a)
		{
			return new Vec2(this.y * a.x + this.x * a.y, -this.x * a.x + this.y * a.y);
		}

		public Vec2 TransformToLocalUnitFLeftHanded(Vec2 a)
		{
			return new Vec2(-this.y * a.x + this.x * a.y, this.x * a.x + this.y * a.y);
		}

		public Vec2 TransformToParentUnitFLeftHanded(Vec2 a)
		{
			return new Vec2(-this.y * a.x + this.x * a.y, this.x * a.x + this.y * a.y);
		}

		public Vec2 RightVec()
		{
			return new Vec2(this.y, -this.x);
		}

		public Vec2 LeftVec()
		{
			return new Vec2(-this.y, this.x);
		}

		public static Vec2 Max(Vec2 v1, Vec2 v2)
		{
			return new Vec2(MathF.Max(v1.x, v2.x), MathF.Max(v1.y, v2.y));
		}

		public static Vec2 Max(Vec2 v1, float f)
		{
			return new Vec2(MathF.Max(v1.x, f), MathF.Max(v1.y, f));
		}

		public static Vec2 Min(Vec2 v1, Vec2 v2)
		{
			return new Vec2(MathF.Min(v1.x, v2.x), MathF.Min(v1.y, v2.y));
		}

		public static Vec2 Min(Vec2 v1, float f)
		{
			return new Vec2(MathF.Min(v1.x, f), MathF.Min(v1.y, f));
		}

		public override string ToString()
		{
			return string.Concat(new object[] { "(Vec2) X: ", this.x, " Y: ", this.y });
		}

		public float DistanceSquared(Vec2 v)
		{
			return (v.x - this.x) * (v.x - this.x) + (v.y - this.y) * (v.y - this.y);
		}

		public float Distance(Vec2 v)
		{
			return MathF.Sqrt((v.x - this.x) * (v.x - this.x) + (v.y - this.y) * (v.y - this.y));
		}

		public static float DistanceToLine(Vec2 line1, Vec2 line2, Vec2 point)
		{
			float num = line2.x - line1.x;
			float num2 = line2.y - line1.y;
			return MathF.Abs(num * (line1.y - point.y) - (line1.x - point.x) * num2) / MathF.Sqrt(num * num + num2 * num2);
		}

		public static float DistanceToLineSegmentSquared(Vec2 line1, Vec2 line2, Vec2 point)
		{
			return point.DistanceSquared(MBMath.GetClosestPointInLineSegmentToPoint(point, line1, line2));
		}

		public float DistanceToLineSegment(Vec2 v, Vec2 w, out Vec2 closestPointOnLineSegment)
		{
			return MathF.Sqrt(this.DistanceSquaredToLineSegment(v, w, out closestPointOnLineSegment));
		}

		public float DistanceSquaredToLineSegment(Vec2 v, Vec2 w, out Vec2 closestPointOnLineSegment)
		{
			Vec2 vec = this;
			float num = v.DistanceSquared(w);
			if (num == 0f)
			{
				closestPointOnLineSegment = v;
			}
			else
			{
				float num2 = Vec2.DotProduct(vec - v, w - v) / num;
				if (num2 < 0f)
				{
					closestPointOnLineSegment = v;
				}
				else if (num2 > 1f)
				{
					closestPointOnLineSegment = w;
				}
				else
				{
					Vec2 vec2 = v + (w - v) * num2;
					closestPointOnLineSegment = vec2;
				}
			}
			return vec.DistanceSquared(closestPointOnLineSegment);
		}

		public static Vec2 Lerp(Vec2 v1, Vec2 v2, float alpha)
		{
			return v1 * (1f - alpha) + v2 * alpha;
		}

		public static Vec2 Slerp(Vec2 start, Vec2 end, float percent)
		{
			float num = Vec2.DotProduct(start, end);
			num = MBMath.ClampFloat(num, -1f, 1f);
			float num2 = MathF.Acos(num) * percent;
			Vec2 vec = end - start * num;
			vec.Normalize();
			return start * MathF.Cos(num2) + vec * MathF.Sin(num2);
		}

		public float AngleBetween(Vec2 vector2)
		{
			float num = this.x * vector2.y - vector2.x * this.y;
			float num2 = this.x * vector2.x + this.y * vector2.y;
			return MathF.Atan2(num, num2);
		}

		public bool IsValid
		{
			get
			{
				return !float.IsNaN(this.x) && !float.IsNaN(this.y) && !float.IsInfinity(this.x) && !float.IsInfinity(this.y);
			}
		}

		public static int SideOfLine(Vec2 point, Vec2 line1, Vec2 line2)
		{
			return MathF.Sign((line2.x - line1.x) * (point.y - line1.y) - (point.x - line1.x) * (line2.y - line1.y));
		}

		public float x;

		public float y;

		public static readonly Vec2 Side = new Vec2(1f, 0f);

		public static readonly Vec2 Forward = new Vec2(0f, 1f);

		public static readonly Vec2 One = new Vec2(1f, 1f);

		public static readonly Vec2 Zero = new Vec2(0f, 0f);

		public static readonly Vec2 Invalid = new Vec2(float.NaN, float.NaN);

		public struct StackArray6Vec2
		{
			public Vec2 this[int index]
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
					default:
						Debug.FailedAssert("Index out of range.", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\Base\\TaleWorlds.Library\\Vec2.cs", "Item", 36);
						return Vec2.Zero;
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
					default:
						Debug.FailedAssert("Index out of range.", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\Base\\TaleWorlds.Library\\Vec2.cs", "Item", 52);
						return;
					}
				}
			}

			private Vec2 _element0;

			private Vec2 _element1;

			private Vec2 _element2;

			private Vec2 _element3;

			private Vec2 _element4;

			private Vec2 _element5;

			public const int Length = 6;
		}
	}
}
