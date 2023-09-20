using System;
using System.Numerics;
using System.Xml.Serialization;

namespace TaleWorlds.Library
{
	// Token: 0x02000099 RID: 153
	[Serializable]
	public struct Vec3
	{
		// Token: 0x17000097 RID: 151
		// (get) Token: 0x06000555 RID: 1365 RVA: 0x00010C8B File Offset: 0x0000EE8B
		public float X
		{
			get
			{
				return this.x;
			}
		}

		// Token: 0x17000098 RID: 152
		// (get) Token: 0x06000556 RID: 1366 RVA: 0x00010C93 File Offset: 0x0000EE93
		public float Y
		{
			get
			{
				return this.y;
			}
		}

		// Token: 0x17000099 RID: 153
		// (get) Token: 0x06000557 RID: 1367 RVA: 0x00010C9B File Offset: 0x0000EE9B
		public float Z
		{
			get
			{
				return this.z;
			}
		}

		// Token: 0x06000558 RID: 1368 RVA: 0x00010CA3 File Offset: 0x0000EEA3
		public Vec3(float x = 0f, float y = 0f, float z = 0f, float w = -1f)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}

		// Token: 0x06000559 RID: 1369 RVA: 0x00010CC2 File Offset: 0x0000EEC2
		public Vec3(Vec3 c, float w = -1f)
		{
			this.x = c.x;
			this.y = c.y;
			this.z = c.z;
			this.w = w;
		}

		// Token: 0x0600055A RID: 1370 RVA: 0x00010CEF File Offset: 0x0000EEEF
		public Vec3(Vec2 xy, float z = 0f, float w = -1f)
		{
			this.x = xy.x;
			this.y = xy.y;
			this.z = z;
			this.w = w;
		}

		// Token: 0x0600055B RID: 1371 RVA: 0x00010D17 File Offset: 0x0000EF17
		public Vec3(Vector3 vector3)
		{
			this = new Vec3(vector3.X, vector3.Y, vector3.Z, -1f);
		}

		// Token: 0x0600055C RID: 1372 RVA: 0x00010D36 File Offset: 0x0000EF36
		public static Vec3 Abs(Vec3 vec)
		{
			return new Vec3(MathF.Abs(vec.x), MathF.Abs(vec.y), MathF.Abs(vec.z), -1f);
		}

		// Token: 0x0600055D RID: 1373 RVA: 0x00010D63 File Offset: 0x0000EF63
		public static explicit operator Vector3(Vec3 vec3)
		{
			return new Vector3(vec3.x, vec3.y, vec3.z);
		}

		// Token: 0x1700009A RID: 154
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

		// Token: 0x06000560 RID: 1376 RVA: 0x00010E0B File Offset: 0x0000F00B
		public static float DotProduct(Vec3 v1, Vec3 v2)
		{
			return v1.x * v2.x + v1.y * v2.y + v1.z * v2.z;
		}

		// Token: 0x06000561 RID: 1377 RVA: 0x00010E36 File Offset: 0x0000F036
		public static Vec3 Lerp(Vec3 v1, Vec3 v2, float alpha)
		{
			return v1 * (1f - alpha) + v2 * alpha;
		}

		// Token: 0x06000562 RID: 1378 RVA: 0x00010E54 File Offset: 0x0000F054
		public static Vec3 Slerp(Vec3 start, Vec3 end, float percent)
		{
			float num = Vec3.DotProduct(start, end);
			num = MBMath.ClampFloat(num, -1f, 1f);
			float num2 = MathF.Acos(num) * percent;
			Vec3 vec = end - start * num;
			vec.Normalize();
			return start * MathF.Cos(num2) + vec * MathF.Sin(num2);
		}

		// Token: 0x06000563 RID: 1379 RVA: 0x00010EB6 File Offset: 0x0000F0B6
		public static Vec3 Vec3Max(Vec3 v1, Vec3 v2)
		{
			return new Vec3(MathF.Max(v1.x, v2.x), MathF.Max(v1.y, v2.y), MathF.Max(v1.z, v2.z), -1f);
		}

		// Token: 0x06000564 RID: 1380 RVA: 0x00010EF5 File Offset: 0x0000F0F5
		public static Vec3 Vec3Min(Vec3 v1, Vec3 v2)
		{
			return new Vec3(MathF.Min(v1.x, v2.x), MathF.Min(v1.y, v2.y), MathF.Min(v1.z, v2.z), -1f);
		}

		// Token: 0x06000565 RID: 1381 RVA: 0x00010F34 File Offset: 0x0000F134
		public static Vec3 CrossProduct(Vec3 va, Vec3 vb)
		{
			return new Vec3(va.y * vb.z - va.z * vb.y, va.z * vb.x - va.x * vb.z, va.x * vb.y - va.y * vb.x, -1f);
		}

		// Token: 0x06000566 RID: 1382 RVA: 0x00010F9C File Offset: 0x0000F19C
		public static Vec3 operator -(Vec3 v)
		{
			return new Vec3(-v.x, -v.y, -v.z, -1f);
		}

		// Token: 0x06000567 RID: 1383 RVA: 0x00010FBD File Offset: 0x0000F1BD
		public static Vec3 operator +(Vec3 v1, Vec3 v2)
		{
			return new Vec3(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z, -1f);
		}

		// Token: 0x06000568 RID: 1384 RVA: 0x00010FF0 File Offset: 0x0000F1F0
		public static Vec3 operator -(Vec3 v1, Vec3 v2)
		{
			return new Vec3(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z, -1f);
		}

		// Token: 0x06000569 RID: 1385 RVA: 0x00011023 File Offset: 0x0000F223
		public static Vec3 operator *(Vec3 v, float f)
		{
			return new Vec3(v.x * f, v.y * f, v.z * f, -1f);
		}

		// Token: 0x0600056A RID: 1386 RVA: 0x00011047 File Offset: 0x0000F247
		public static Vec3 operator *(float f, Vec3 v)
		{
			return new Vec3(v.x * f, v.y * f, v.z * f, -1f);
		}

		// Token: 0x0600056B RID: 1387 RVA: 0x0001106C File Offset: 0x0000F26C
		public static Vec3 operator *(Vec3 v, MatrixFrame frame)
		{
			return new Vec3(frame.rotation.s.x * v.x + frame.rotation.f.x * v.y + frame.rotation.u.x * v.z + frame.origin.x * v.w, frame.rotation.s.y * v.x + frame.rotation.f.y * v.y + frame.rotation.u.y * v.z + frame.origin.y * v.w, frame.rotation.s.z * v.x + frame.rotation.f.z * v.y + frame.rotation.u.z * v.z + frame.origin.z * v.w, frame.rotation.s.w * v.x + frame.rotation.f.w * v.y + frame.rotation.u.w * v.z + frame.origin.w * v.w);
		}

		// Token: 0x0600056C RID: 1388 RVA: 0x000111E6 File Offset: 0x0000F3E6
		public static Vec3 operator /(Vec3 v, float f)
		{
			f = 1f / f;
			return new Vec3(v.x * f, v.y * f, v.z * f, -1f);
		}

		// Token: 0x0600056D RID: 1389 RVA: 0x00011213 File Offset: 0x0000F413
		public static bool operator ==(Vec3 v1, Vec3 v2)
		{
			return v1.x == v2.x && v1.y == v2.y && v1.z == v2.z;
		}

		// Token: 0x0600056E RID: 1390 RVA: 0x00011241 File Offset: 0x0000F441
		public static bool operator !=(Vec3 v1, Vec3 v2)
		{
			return v1.x != v2.x || v1.y != v2.y || v1.z != v2.z;
		}

		// Token: 0x1700009B RID: 155
		// (get) Token: 0x0600056F RID: 1391 RVA: 0x00011272 File Offset: 0x0000F472
		public float Length
		{
			get
			{
				return MathF.Sqrt(this.x * this.x + this.y * this.y + this.z * this.z);
			}
		}

		// Token: 0x1700009C RID: 156
		// (get) Token: 0x06000570 RID: 1392 RVA: 0x000112A2 File Offset: 0x0000F4A2
		public float LengthSquared
		{
			get
			{
				return this.x * this.x + this.y * this.y + this.z * this.z;
			}
		}

		// Token: 0x06000571 RID: 1393 RVA: 0x000112D0 File Offset: 0x0000F4D0
		public override bool Equals(object obj)
		{
			return obj != null && !(base.GetType() != obj.GetType()) && (((Vec3)obj).x == this.x && ((Vec3)obj).y == this.y) && ((Vec3)obj).z == this.z;
		}

		// Token: 0x06000572 RID: 1394 RVA: 0x0001133A File Offset: 0x0000F53A
		public override int GetHashCode()
		{
			return (int)(1001f * this.x + 10039f * this.y + 117f * this.z);
		}

		// Token: 0x1700009D RID: 157
		// (get) Token: 0x06000573 RID: 1395 RVA: 0x00011364 File Offset: 0x0000F564
		public bool IsValid
		{
			get
			{
				return !float.IsNaN(this.x) && !float.IsNaN(this.y) && !float.IsNaN(this.z) && !float.IsInfinity(this.x) && !float.IsInfinity(this.y) && !float.IsInfinity(this.z);
			}
		}

		// Token: 0x1700009E RID: 158
		// (get) Token: 0x06000574 RID: 1396 RVA: 0x000113C4 File Offset: 0x0000F5C4
		public bool IsValidXYZW
		{
			get
			{
				return !float.IsNaN(this.x) && !float.IsNaN(this.y) && !float.IsNaN(this.z) && !float.IsNaN(this.w) && !float.IsInfinity(this.x) && !float.IsInfinity(this.y) && !float.IsInfinity(this.z) && !float.IsInfinity(this.w);
			}
		}

		// Token: 0x1700009F RID: 159
		// (get) Token: 0x06000575 RID: 1397 RVA: 0x0001143C File Offset: 0x0000F63C
		public bool IsUnit
		{
			get
			{
				float lengthSquared = this.LengthSquared;
				return lengthSquared > 0.98010004f && lengthSquared < 1.0201f;
			}
		}

		// Token: 0x170000A0 RID: 160
		// (get) Token: 0x06000576 RID: 1398 RVA: 0x00011462 File Offset: 0x0000F662
		public bool IsNonZero
		{
			get
			{
				return this.x != 0f || this.y != 0f || this.z != 0f;
			}
		}

		// Token: 0x06000577 RID: 1399 RVA: 0x00011490 File Offset: 0x0000F690
		public Vec3 NormalizedCopy()
		{
			Vec3 vec = this;
			vec.Normalize();
			return vec;
		}

		// Token: 0x06000578 RID: 1400 RVA: 0x000114B0 File Offset: 0x0000F6B0
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

		// Token: 0x06000579 RID: 1401 RVA: 0x00011524 File Offset: 0x0000F724
		public Vec3 ClampedCopy(float min, float max)
		{
			Vec3 vec = this;
			vec.x = MathF.Clamp(vec.x, min, max);
			vec.y = MathF.Clamp(vec.y, min, max);
			vec.z = MathF.Clamp(vec.z, min, max);
			return vec;
		}

		// Token: 0x0600057A RID: 1402 RVA: 0x00011578 File Offset: 0x0000F778
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

		// Token: 0x0600057B RID: 1403 RVA: 0x000115CC File Offset: 0x0000F7CC
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

		// Token: 0x0600057C RID: 1404 RVA: 0x0001166C File Offset: 0x0000F86C
		public bool NearlyEquals(Vec3 v, float epsilon = 1E-05f)
		{
			return MathF.Abs(this.x - v.x) < epsilon && MathF.Abs(this.y - v.y) < epsilon && MathF.Abs(this.z - v.z) < epsilon;
		}

		// Token: 0x0600057D RID: 1405 RVA: 0x000116BC File Offset: 0x0000F8BC
		public void RotateAboutX(float a)
		{
			float num;
			float num2;
			MathF.SinCos(a, out num, out num2);
			float num3 = this.y * num2 - this.z * num;
			this.z = this.z * num2 + this.y * num;
			this.y = num3;
		}

		// Token: 0x0600057E RID: 1406 RVA: 0x00011704 File Offset: 0x0000F904
		public void RotateAboutY(float a)
		{
			float num;
			float num2;
			MathF.SinCos(a, out num, out num2);
			float num3 = this.x * num2 + this.z * num;
			this.z = this.z * num2 - this.x * num;
			this.x = num3;
		}

		// Token: 0x0600057F RID: 1407 RVA: 0x0001174C File Offset: 0x0000F94C
		public void RotateAboutZ(float a)
		{
			float num;
			float num2;
			MathF.SinCos(a, out num, out num2);
			float num3 = this.x * num2 - this.y * num;
			this.y = this.y * num2 + this.x * num;
			this.x = num3;
		}

		// Token: 0x06000580 RID: 1408 RVA: 0x00011794 File Offset: 0x0000F994
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

		// Token: 0x06000581 RID: 1409 RVA: 0x000118BC File Offset: 0x0000FABC
		public Vec3 Reflect(Vec3 normal)
		{
			return this - normal * (2f * Vec3.DotProduct(this, normal));
		}

		// Token: 0x06000582 RID: 1410 RVA: 0x000118E1 File Offset: 0x0000FAE1
		public Vec3 ProjectOnUnitVector(Vec3 ov)
		{
			return ov * (this.x * ov.x + this.y * ov.y + this.z * ov.z);
		}

		// Token: 0x06000583 RID: 1411 RVA: 0x00011914 File Offset: 0x0000FB14
		public float DistanceSquared(Vec3 v)
		{
			return (v.x - this.x) * (v.x - this.x) + (v.y - this.y) * (v.y - this.y) + (v.z - this.z) * (v.z - this.z);
		}

		// Token: 0x06000584 RID: 1412 RVA: 0x00011974 File Offset: 0x0000FB74
		public float Distance(Vec3 v)
		{
			return MathF.Sqrt((v.x - this.x) * (v.x - this.x) + (v.y - this.y) * (v.y - this.y) + (v.z - this.z) * (v.z - this.z));
		}

		// Token: 0x06000585 RID: 1413 RVA: 0x000119D9 File Offset: 0x0000FBD9
		public static float AngleBetweenTwoVectors(Vec3 v1, Vec3 v2)
		{
			return MathF.Acos(MathF.Clamp(Vec3.DotProduct(v1, v2) / (v1.Length * v2.Length), -1f, 1f));
		}

		// Token: 0x170000A1 RID: 161
		// (get) Token: 0x06000586 RID: 1414 RVA: 0x00011A06 File Offset: 0x0000FC06
		// (set) Token: 0x06000587 RID: 1415 RVA: 0x00011A19 File Offset: 0x0000FC19
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

		// Token: 0x06000588 RID: 1416 RVA: 0x00011A34 File Offset: 0x0000FC34
		public override string ToString()
		{
			return string.Concat(new object[] { "(", this.x, ", ", this.y, ", ", this.z, ")" });
		}

		// Token: 0x170000A2 RID: 162
		// (get) Token: 0x06000589 RID: 1417 RVA: 0x00011A98 File Offset: 0x0000FC98
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

		// Token: 0x170000A3 RID: 163
		// (get) Token: 0x0600058A RID: 1418 RVA: 0x00011B12 File Offset: 0x0000FD12
		public float RotationZ
		{
			get
			{
				return MathF.Atan2(-this.x, this.y);
			}
		}

		// Token: 0x170000A4 RID: 164
		// (get) Token: 0x0600058B RID: 1419 RVA: 0x00011B26 File Offset: 0x0000FD26
		public float RotationX
		{
			get
			{
				return MathF.Atan2(this.z, MathF.Sqrt(this.x * this.x + this.y * this.y));
			}
		}

		// Token: 0x0600058C RID: 1420 RVA: 0x00011B54 File Offset: 0x0000FD54
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

		// Token: 0x04000191 RID: 401
		[XmlAttribute]
		public float x;

		// Token: 0x04000192 RID: 402
		[XmlAttribute]
		public float y;

		// Token: 0x04000193 RID: 403
		[XmlAttribute]
		public float z;

		// Token: 0x04000194 RID: 404
		[XmlAttribute]
		public float w;

		// Token: 0x04000195 RID: 405
		public static readonly Vec3 Side = new Vec3(1f, 0f, 0f, -1f);

		// Token: 0x04000196 RID: 406
		public static readonly Vec3 Forward = new Vec3(0f, 1f, 0f, -1f);

		// Token: 0x04000197 RID: 407
		public static readonly Vec3 Up = new Vec3(0f, 0f, 1f, -1f);

		// Token: 0x04000198 RID: 408
		public static readonly Vec3 One = new Vec3(1f, 1f, 1f, -1f);

		// Token: 0x04000199 RID: 409
		public static readonly Vec3 Zero = new Vec3(0f, 0f, 0f, -1f);

		// Token: 0x0400019A RID: 410
		public static readonly Vec3 Invalid = new Vec3(float.NaN, float.NaN, float.NaN, -1f);

		// Token: 0x020000DF RID: 223
		public struct StackArray8Vec3
		{
			// Token: 0x170000F7 RID: 247
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

			// Token: 0x040002B7 RID: 695
			private Vec3 _element0;

			// Token: 0x040002B8 RID: 696
			private Vec3 _element1;

			// Token: 0x040002B9 RID: 697
			private Vec3 _element2;

			// Token: 0x040002BA RID: 698
			private Vec3 _element3;

			// Token: 0x040002BB RID: 699
			private Vec3 _element4;

			// Token: 0x040002BC RID: 700
			private Vec3 _element5;

			// Token: 0x040002BD RID: 701
			private Vec3 _element6;

			// Token: 0x040002BE RID: 702
			private Vec3 _element7;

			// Token: 0x040002BF RID: 703
			public const int Length = 8;
		}
	}
}
