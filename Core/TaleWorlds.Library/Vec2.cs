using System;
using System.Numerics;

namespace TaleWorlds.Library
{
	// Token: 0x02000097 RID: 151
	[Serializable]
	public struct Vec2
	{
		// Token: 0x1700008E RID: 142
		// (get) Token: 0x06000512 RID: 1298 RVA: 0x000101B2 File Offset: 0x0000E3B2
		public float X
		{
			get
			{
				return this.x;
			}
		}

		// Token: 0x1700008F RID: 143
		// (get) Token: 0x06000513 RID: 1299 RVA: 0x000101BA File Offset: 0x0000E3BA
		public float Y
		{
			get
			{
				return this.y;
			}
		}

		// Token: 0x06000514 RID: 1300 RVA: 0x000101C2 File Offset: 0x0000E3C2
		public Vec2(float a, float b)
		{
			this.x = a;
			this.y = b;
		}

		// Token: 0x06000515 RID: 1301 RVA: 0x000101D2 File Offset: 0x0000E3D2
		public Vec2(Vec2 v)
		{
			this.x = v.x;
			this.y = v.y;
		}

		// Token: 0x06000516 RID: 1302 RVA: 0x000101EC File Offset: 0x0000E3EC
		public Vec2(Vector2 v)
		{
			this.x = v.X;
			this.y = v.Y;
		}

		// Token: 0x06000517 RID: 1303 RVA: 0x00010206 File Offset: 0x0000E406
		public Vec3 ToVec3(float z = 0f)
		{
			return new Vec3(this.x, this.y, z, -1f);
		}

		// Token: 0x06000518 RID: 1304 RVA: 0x0001021F File Offset: 0x0000E41F
		public static explicit operator Vector2(Vec2 vec2)
		{
			return new Vector2(vec2.x, vec2.y);
		}

		// Token: 0x06000519 RID: 1305 RVA: 0x00010232 File Offset: 0x0000E432
		public static implicit operator Vec2(Vector2 vec2)
		{
			return new Vec2(vec2.X, vec2.Y);
		}

		// Token: 0x17000090 RID: 144
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

		// Token: 0x0600051C RID: 1308 RVA: 0x00010284 File Offset: 0x0000E484
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

		// Token: 0x0600051D RID: 1309 RVA: 0x000102D8 File Offset: 0x0000E4D8
		public Vec2 Normalized()
		{
			Vec2 vec = this;
			vec.Normalize();
			return vec;
		}

		// Token: 0x0600051E RID: 1310 RVA: 0x000102F8 File Offset: 0x0000E4F8
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

		// Token: 0x0600051F RID: 1311 RVA: 0x00010330 File Offset: 0x0000E530
		public static float CCW(Vec2 va, Vec2 vb)
		{
			return va.x * vb.y - va.y * vb.x;
		}

		// Token: 0x17000091 RID: 145
		// (get) Token: 0x06000520 RID: 1312 RVA: 0x0001034D File Offset: 0x0000E54D
		public float Length
		{
			get
			{
				return MathF.Sqrt(this.x * this.x + this.y * this.y);
			}
		}

		// Token: 0x17000092 RID: 146
		// (get) Token: 0x06000521 RID: 1313 RVA: 0x0001036F File Offset: 0x0000E56F
		public float LengthSquared
		{
			get
			{
				return this.x * this.x + this.y * this.y;
			}
		}

		// Token: 0x06000522 RID: 1314 RVA: 0x0001038C File Offset: 0x0000E58C
		public override bool Equals(object obj)
		{
			return obj != null && !(base.GetType() != obj.GetType()) && ((Vec2)obj).x == this.x && ((Vec2)obj).y == this.y;
		}

		// Token: 0x06000523 RID: 1315 RVA: 0x000103E3 File Offset: 0x0000E5E3
		public override int GetHashCode()
		{
			return (int)(1001f * this.x + 10039f * this.y);
		}

		// Token: 0x06000524 RID: 1316 RVA: 0x000103FF File Offset: 0x0000E5FF
		public static bool operator ==(Vec2 v1, Vec2 v2)
		{
			return v1.x == v2.x && v1.y == v2.y;
		}

		// Token: 0x06000525 RID: 1317 RVA: 0x0001041F File Offset: 0x0000E61F
		public static bool operator !=(Vec2 v1, Vec2 v2)
		{
			return v1.x != v2.x || v1.y != v2.y;
		}

		// Token: 0x06000526 RID: 1318 RVA: 0x00010442 File Offset: 0x0000E642
		public static Vec2 operator -(Vec2 v)
		{
			return new Vec2(-v.x, -v.y);
		}

		// Token: 0x06000527 RID: 1319 RVA: 0x00010457 File Offset: 0x0000E657
		public static Vec2 operator +(Vec2 v1, Vec2 v2)
		{
			return new Vec2(v1.x + v2.x, v1.y + v2.y);
		}

		// Token: 0x06000528 RID: 1320 RVA: 0x00010478 File Offset: 0x0000E678
		public static Vec2 operator -(Vec2 v1, Vec2 v2)
		{
			return new Vec2(v1.x - v2.x, v1.y - v2.y);
		}

		// Token: 0x06000529 RID: 1321 RVA: 0x00010499 File Offset: 0x0000E699
		public static Vec2 operator *(Vec2 v, float f)
		{
			return new Vec2(v.x * f, v.y * f);
		}

		// Token: 0x0600052A RID: 1322 RVA: 0x000104B0 File Offset: 0x0000E6B0
		public static Vec2 operator *(float f, Vec2 v)
		{
			return new Vec2(v.x * f, v.y * f);
		}

		// Token: 0x0600052B RID: 1323 RVA: 0x000104C7 File Offset: 0x0000E6C7
		public static Vec2 operator /(float f, Vec2 v)
		{
			return new Vec2(v.x / f, v.y / f);
		}

		// Token: 0x0600052C RID: 1324 RVA: 0x000104DE File Offset: 0x0000E6DE
		public static Vec2 operator /(Vec2 v, float f)
		{
			return new Vec2(v.x / f, v.y / f);
		}

		// Token: 0x0600052D RID: 1325 RVA: 0x000104F8 File Offset: 0x0000E6F8
		public bool IsUnit()
		{
			float length = this.Length;
			return (double)length > 0.95 && (double)length < 1.05;
		}

		// Token: 0x0600052E RID: 1326 RVA: 0x00010528 File Offset: 0x0000E728
		public bool IsNonZero()
		{
			float num = 1E-05f;
			return this.x > num || this.x < -num || this.y > num || this.y < -num;
		}

		// Token: 0x0600052F RID: 1327 RVA: 0x00010563 File Offset: 0x0000E763
		public bool NearlyEquals(Vec2 v, float epsilon = 1E-05f)
		{
			return MathF.Abs(this.x - v.x) < epsilon && MathF.Abs(this.y - v.y) < epsilon;
		}

		// Token: 0x06000530 RID: 1328 RVA: 0x00010594 File Offset: 0x0000E794
		public void RotateCCW(float angleInRadians)
		{
			float num;
			float num2;
			MathF.SinCos(angleInRadians, out num, out num2);
			float num3 = this.x * num2 - this.y * num;
			this.y = this.y * num2 + this.x * num;
			this.x = num3;
		}

		// Token: 0x06000531 RID: 1329 RVA: 0x000105DB File Offset: 0x0000E7DB
		public float DotProduct(Vec2 v)
		{
			return v.x * this.x + v.y * this.y;
		}

		// Token: 0x06000532 RID: 1330 RVA: 0x000105F8 File Offset: 0x0000E7F8
		public static float DotProduct(Vec2 va, Vec2 vb)
		{
			return va.x * vb.x + va.y * vb.y;
		}

		// Token: 0x17000093 RID: 147
		// (get) Token: 0x06000533 RID: 1331 RVA: 0x00010615 File Offset: 0x0000E815
		public float RotationInRadians
		{
			get
			{
				return MathF.Atan2(-this.x, this.y);
			}
		}

		// Token: 0x06000534 RID: 1332 RVA: 0x00010629 File Offset: 0x0000E829
		public static Vec2 FromRotation(float rotation)
		{
			return new Vec2(-MathF.Sin(rotation), MathF.Cos(rotation));
		}

		// Token: 0x06000535 RID: 1333 RVA: 0x0001063D File Offset: 0x0000E83D
		public Vec2 TransformToLocalUnitF(Vec2 a)
		{
			return new Vec2(this.y * a.x - this.x * a.y, this.x * a.x + this.y * a.y);
		}

		// Token: 0x06000536 RID: 1334 RVA: 0x0001067A File Offset: 0x0000E87A
		public Vec2 TransformToParentUnitF(Vec2 a)
		{
			return new Vec2(this.y * a.x + this.x * a.y, -this.x * a.x + this.y * a.y);
		}

		// Token: 0x06000537 RID: 1335 RVA: 0x000106B8 File Offset: 0x0000E8B8
		public Vec2 TransformToLocalUnitFLeftHanded(Vec2 a)
		{
			return new Vec2(-this.y * a.x + this.x * a.y, this.x * a.x + this.y * a.y);
		}

		// Token: 0x06000538 RID: 1336 RVA: 0x000106F6 File Offset: 0x0000E8F6
		public Vec2 TransformToParentUnitFLeftHanded(Vec2 a)
		{
			return new Vec2(-this.y * a.x + this.x * a.y, this.x * a.x + this.y * a.y);
		}

		// Token: 0x06000539 RID: 1337 RVA: 0x00010734 File Offset: 0x0000E934
		public Vec2 RightVec()
		{
			return new Vec2(this.y, -this.x);
		}

		// Token: 0x0600053A RID: 1338 RVA: 0x00010748 File Offset: 0x0000E948
		public Vec2 LeftVec()
		{
			return new Vec2(-this.y, this.x);
		}

		// Token: 0x0600053B RID: 1339 RVA: 0x0001075C File Offset: 0x0000E95C
		public static Vec2 Max(Vec2 v1, Vec2 v2)
		{
			return new Vec2(MathF.Max(v1.x, v2.x), MathF.Max(v1.y, v2.y));
		}

		// Token: 0x0600053C RID: 1340 RVA: 0x00010785 File Offset: 0x0000E985
		public static Vec2 Max(Vec2 v1, float f)
		{
			return new Vec2(MathF.Max(v1.x, f), MathF.Max(v1.y, f));
		}

		// Token: 0x0600053D RID: 1341 RVA: 0x000107A4 File Offset: 0x0000E9A4
		public static Vec2 Min(Vec2 v1, Vec2 v2)
		{
			return new Vec2(MathF.Min(v1.x, v2.x), MathF.Min(v1.y, v2.y));
		}

		// Token: 0x0600053E RID: 1342 RVA: 0x000107CD File Offset: 0x0000E9CD
		public static Vec2 Min(Vec2 v1, float f)
		{
			return new Vec2(MathF.Min(v1.x, f), MathF.Min(v1.y, f));
		}

		// Token: 0x0600053F RID: 1343 RVA: 0x000107EC File Offset: 0x0000E9EC
		public override string ToString()
		{
			return string.Concat(new object[] { "(Vec2) X: ", this.x, " Y: ", this.y });
		}

		// Token: 0x06000540 RID: 1344 RVA: 0x00010825 File Offset: 0x0000EA25
		public float DistanceSquared(Vec2 v)
		{
			return (v.x - this.x) * (v.x - this.x) + (v.y - this.y) * (v.y - this.y);
		}

		// Token: 0x06000541 RID: 1345 RVA: 0x0001085E File Offset: 0x0000EA5E
		public float Distance(Vec2 v)
		{
			return MathF.Sqrt((v.x - this.x) * (v.x - this.x) + (v.y - this.y) * (v.y - this.y));
		}

		// Token: 0x06000542 RID: 1346 RVA: 0x0001089C File Offset: 0x0000EA9C
		public static float DistanceToLine(Vec2 line1, Vec2 line2, Vec2 point)
		{
			float num = line2.x - line1.x;
			float num2 = line2.y - line1.y;
			return MathF.Abs(num * (line1.y - point.y) - (line1.x - point.x) * num2) / MathF.Sqrt(num * num + num2 * num2);
		}

		// Token: 0x06000543 RID: 1347 RVA: 0x000108F6 File Offset: 0x0000EAF6
		public static float DistanceToLineSegmentSquared(Vec2 line1, Vec2 line2, Vec2 point)
		{
			return point.DistanceSquared(MBMath.GetClosestPointInLineSegmentToPoint(point, line1, line2));
		}

		// Token: 0x06000544 RID: 1348 RVA: 0x00010907 File Offset: 0x0000EB07
		public float DistanceToLineSegment(Vec2 v, Vec2 w, out Vec2 closestPointOnLineSegment)
		{
			return MathF.Sqrt(this.DistanceSquaredToLineSegment(v, w, out closestPointOnLineSegment));
		}

		// Token: 0x06000545 RID: 1349 RVA: 0x00010918 File Offset: 0x0000EB18
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

		// Token: 0x06000546 RID: 1350 RVA: 0x000109A6 File Offset: 0x0000EBA6
		public static Vec2 Lerp(Vec2 v1, Vec2 v2, float alpha)
		{
			return v1 * (1f - alpha) + v2 * alpha;
		}

		// Token: 0x06000547 RID: 1351 RVA: 0x000109C4 File Offset: 0x0000EBC4
		public static Vec2 Slerp(Vec2 start, Vec2 end, float percent)
		{
			float num = Vec2.DotProduct(start, end);
			num = MBMath.ClampFloat(num, -1f, 1f);
			float num2 = MathF.Acos(num) * percent;
			Vec2 vec = end - start * num;
			vec.Normalize();
			return start * MathF.Cos(num2) + vec * MathF.Sin(num2);
		}

		// Token: 0x06000548 RID: 1352 RVA: 0x00010A28 File Offset: 0x0000EC28
		public float AngleBetween(Vec2 vector2)
		{
			float num = this.x * vector2.y - vector2.x * this.y;
			float num2 = this.x * vector2.x + this.y * vector2.y;
			return MathF.Atan2(num, num2);
		}

		// Token: 0x17000094 RID: 148
		// (get) Token: 0x06000549 RID: 1353 RVA: 0x00010A72 File Offset: 0x0000EC72
		public bool IsValid
		{
			get
			{
				return !float.IsNaN(this.x) && !float.IsNaN(this.y) && !float.IsInfinity(this.x) && !float.IsInfinity(this.y);
			}
		}

		// Token: 0x0600054A RID: 1354 RVA: 0x00010AAB File Offset: 0x0000ECAB
		public static int SideOfLine(Vec2 point, Vec2 line1, Vec2 line2)
		{
			return MathF.Sign((line2.x - line1.x) * (point.y - line1.y) - (point.x - line1.x) * (line2.y - line1.y));
		}

		// Token: 0x04000184 RID: 388
		public float x;

		// Token: 0x04000185 RID: 389
		public float y;

		// Token: 0x04000186 RID: 390
		public static readonly Vec2 Side = new Vec2(1f, 0f);

		// Token: 0x04000187 RID: 391
		public static readonly Vec2 Forward = new Vec2(0f, 1f);

		// Token: 0x04000188 RID: 392
		public static readonly Vec2 One = new Vec2(1f, 1f);

		// Token: 0x04000189 RID: 393
		public static readonly Vec2 Zero = new Vec2(0f, 0f);

		// Token: 0x0400018A RID: 394
		public static readonly Vec2 Invalid = new Vec2(float.NaN, float.NaN);

		// Token: 0x020000DE RID: 222
		public struct StackArray6Vec2
		{
			// Token: 0x170000F6 RID: 246
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

			// Token: 0x040002B0 RID: 688
			private Vec2 _element0;

			// Token: 0x040002B1 RID: 689
			private Vec2 _element1;

			// Token: 0x040002B2 RID: 690
			private Vec2 _element2;

			// Token: 0x040002B3 RID: 691
			private Vec2 _element3;

			// Token: 0x040002B4 RID: 692
			private Vec2 _element4;

			// Token: 0x040002B5 RID: 693
			private Vec2 _element5;

			// Token: 0x040002B6 RID: 694
			public const int Length = 6;
		}
	}
}
