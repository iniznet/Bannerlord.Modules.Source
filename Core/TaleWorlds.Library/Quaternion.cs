using System;

namespace TaleWorlds.Library
{
	// Token: 0x0200007A RID: 122
	[Serializable]
	public struct Quaternion
	{
		// Token: 0x0600041F RID: 1055 RVA: 0x0000D1A9 File Offset: 0x0000B3A9
		public Quaternion(float x, float y, float z, float w)
		{
			this.X = x;
			this.Y = y;
			this.Z = z;
			this.W = w;
		}

		// Token: 0x06000420 RID: 1056 RVA: 0x0000D1C8 File Offset: 0x0000B3C8
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		// Token: 0x06000421 RID: 1057 RVA: 0x0000D1DA File Offset: 0x0000B3DA
		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		// Token: 0x06000422 RID: 1058 RVA: 0x0000D1F0 File Offset: 0x0000B3F0
		public static bool operator ==(Quaternion a, Quaternion b)
		{
			return a == b || (a != null && b != null && (a.X == b.X && a.Y == b.Y && a.Z == b.Z) && a.W == b.W);
		}

		// Token: 0x06000423 RID: 1059 RVA: 0x0000D259 File Offset: 0x0000B459
		public static bool operator !=(Quaternion a, Quaternion b)
		{
			return !(a == b);
		}

		// Token: 0x06000424 RID: 1060 RVA: 0x0000D265 File Offset: 0x0000B465
		public static Quaternion operator +(Quaternion a, Quaternion b)
		{
			return new Quaternion(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);
		}

		// Token: 0x06000425 RID: 1061 RVA: 0x0000D2A0 File Offset: 0x0000B4A0
		public static Quaternion operator -(Quaternion a, Quaternion b)
		{
			return new Quaternion(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W);
		}

		// Token: 0x06000426 RID: 1062 RVA: 0x0000D2DB File Offset: 0x0000B4DB
		public static Quaternion operator *(Quaternion a, float b)
		{
			return new Quaternion(a.X * b, a.Y * b, a.Z * b, a.W * b);
		}

		// Token: 0x06000427 RID: 1063 RVA: 0x0000D302 File Offset: 0x0000B502
		public static Quaternion operator *(float s, Quaternion v)
		{
			return v * s;
		}

		// Token: 0x1700006D RID: 109
		public float this[int i]
		{
			get
			{
				float num;
				switch (i)
				{
				case 0:
					num = this.W;
					break;
				case 1:
					num = this.X;
					break;
				case 2:
					num = this.Y;
					break;
				case 3:
					num = this.Z;
					break;
				default:
					throw new IndexOutOfRangeException("Quaternion out of bounds.");
				}
				return num;
			}
			set
			{
				switch (i)
				{
				case 0:
					this.W = value;
					return;
				case 1:
					this.X = value;
					return;
				case 2:
					this.Y = value;
					return;
				case 3:
					this.Z = value;
					return;
				default:
					throw new IndexOutOfRangeException("Quaternion out of bounds.");
				}
			}
		}

		// Token: 0x0600042A RID: 1066 RVA: 0x0000D3B8 File Offset: 0x0000B5B8
		public float Normalize()
		{
			float num = MathF.Sqrt(this.X * this.X + this.Y * this.Y + this.Z * this.Z + this.W * this.W);
			if (num <= 1E-07f)
			{
				this.X = 0f;
				this.Y = 0f;
				this.Z = 0f;
				this.W = 1f;
			}
			else
			{
				float num2 = 1f / num;
				this.X *= num2;
				this.Y *= num2;
				this.Z *= num2;
				this.W *= num2;
			}
			return num;
		}

		// Token: 0x0600042B RID: 1067 RVA: 0x0000D47C File Offset: 0x0000B67C
		public float SafeNormalize()
		{
			double num = Math.Sqrt((double)this.X * (double)this.X + (double)this.Y * (double)this.Y + (double)this.Z * (double)this.Z + (double)this.W * (double)this.W);
			if (num <= 1E-07)
			{
				this.X = 0f;
				this.Y = 0f;
				this.Z = 0f;
				this.W = 1f;
			}
			else
			{
				this.X = (float)((double)this.X / num);
				this.Y = (float)((double)this.Y / num);
				this.Z = (float)((double)this.Z / num);
				this.W = (float)((double)this.W / num);
			}
			return (float)num;
		}

		// Token: 0x0600042C RID: 1068 RVA: 0x0000D54C File Offset: 0x0000B74C
		public float NormalizeWeighted()
		{
			float num = this.X * this.X + this.Y * this.Y + this.Z * this.Z;
			if (num <= 1E-09f)
			{
				this.X = 1f;
				this.Y = 0f;
				this.Z = 0f;
				this.W = 0f;
			}
			else
			{
				this.W = MathF.Sqrt(1f - num);
			}
			return num;
		}

		// Token: 0x0600042D RID: 1069 RVA: 0x0000D5CC File Offset: 0x0000B7CC
		public void SetToRotationX(float angle)
		{
			float num;
			float num2;
			MathF.SinCos(angle * 0.5f, out num, out num2);
			this.X = num;
			this.Y = 0f;
			this.Z = 0f;
			this.W = num2;
		}

		// Token: 0x0600042E RID: 1070 RVA: 0x0000D610 File Offset: 0x0000B810
		public void SetToRotationY(float angle)
		{
			float num;
			float num2;
			MathF.SinCos(angle * 0.5f, out num, out num2);
			this.X = 0f;
			this.Y = num;
			this.Z = 0f;
			this.W = num2;
		}

		// Token: 0x0600042F RID: 1071 RVA: 0x0000D654 File Offset: 0x0000B854
		public void SetToRotationZ(float angle)
		{
			float num;
			float num2;
			MathF.SinCos(angle * 0.5f, out num, out num2);
			this.X = 0f;
			this.Y = 0f;
			this.Z = num;
			this.W = num2;
		}

		// Token: 0x06000430 RID: 1072 RVA: 0x0000D695 File Offset: 0x0000B895
		public void Flip()
		{
			this.X = -this.X;
			this.Y = -this.Y;
			this.Z = -this.Z;
			this.W = -this.W;
		}

		// Token: 0x1700006E RID: 110
		// (get) Token: 0x06000431 RID: 1073 RVA: 0x0000D6CB File Offset: 0x0000B8CB
		public bool IsIdentity
		{
			get
			{
				return this.X == 0f && this.Y == 0f && this.Z == 0f && this.W == 1f;
			}
		}

		// Token: 0x1700006F RID: 111
		// (get) Token: 0x06000432 RID: 1074 RVA: 0x0000D704 File Offset: 0x0000B904
		public bool IsUnit
		{
			get
			{
				return MBMath.ApproximatelyEquals(this.X * this.X + this.Y * this.Y + this.Z * this.Z + this.W * this.W, 1f, 0.2f);
			}
		}

		// Token: 0x17000070 RID: 112
		// (get) Token: 0x06000433 RID: 1075 RVA: 0x0000D757 File Offset: 0x0000B957
		public static Quaternion Identity
		{
			get
			{
				return new Quaternion(0f, 0f, 0f, 1f);
			}
		}

		// Token: 0x06000434 RID: 1076 RVA: 0x0000D774 File Offset: 0x0000B974
		public Quaternion TransformToParent(Quaternion q)
		{
			return new Quaternion
			{
				X = this.Y * q.Z - this.Z * q.Y + this.W * q.X + this.X * q.W,
				Y = this.Z * q.X - this.X * q.Z + this.W * q.Y + this.Y * q.W,
				Z = this.X * q.Y - this.Y * q.X + this.W * q.Z + this.Z * q.W,
				W = this.W * q.W - (this.X * q.X + this.Y * q.Y + this.Z * q.Z)
			};
		}

		// Token: 0x06000435 RID: 1077 RVA: 0x0000D884 File Offset: 0x0000BA84
		public Quaternion TransformToLocal(Quaternion q)
		{
			return new Quaternion
			{
				X = this.Z * q.Y - this.Y * q.Z + this.W * q.X - this.X * q.W,
				Y = this.X * q.Z - this.Z * q.X + this.W * q.Y - this.Y * q.W,
				Z = this.Y * q.X - this.X * q.Y + this.W * q.Z - this.Z * q.W,
				W = this.W * q.W + (this.X * q.X + this.Y * q.Y + this.Z * q.Z)
			};
		}

		// Token: 0x06000436 RID: 1078 RVA: 0x0000D994 File Offset: 0x0000BB94
		public Quaternion TransformToLocalWithoutNormalize(Quaternion q)
		{
			return new Quaternion
			{
				X = this.Z * q.Y - this.Y * q.Z + this.W * q.X - this.X * q.W,
				Y = this.X * q.Z - this.Z * q.X + this.W * q.Y - this.Y * q.W,
				Z = this.Y * q.X - this.X * q.Y + this.W * q.Z - this.Z * q.W,
				W = this.W * q.W + (this.X * q.X + this.Y * q.Y + this.Z * q.Z)
			};
		}

		// Token: 0x06000437 RID: 1079 RVA: 0x0000DAA4 File Offset: 0x0000BCA4
		public static Quaternion Slerp(Quaternion from, Quaternion to, float t)
		{
			float num = from.Dotp4(to);
			float num2;
			if (num < 0f)
			{
				num = -num;
				num2 = -1f;
			}
			else
			{
				num2 = 1f;
			}
			float num6;
			float num7;
			if (0.9 >= (double)num)
			{
				float num3 = MathF.Acos(num);
				float num4 = 1f / MathF.Sin(num3);
				float num5 = t * num3;
				num6 = MathF.Sin(num3 - num5) * num4;
				num7 = MathF.Sin(num5) * num4;
			}
			else
			{
				num6 = 1f - t;
				num7 = t;
			}
			num7 *= num2;
			return new Quaternion
			{
				X = num6 * from.X + num7 * to.X,
				Y = num6 * from.Y + num7 * to.Y,
				Z = num6 * from.Z + num7 * to.Z,
				W = num6 * from.W + num7 * to.W
			};
		}

		// Token: 0x06000438 RID: 1080 RVA: 0x0000DB98 File Offset: 0x0000BD98
		public static Quaternion Lerp(Quaternion from, Quaternion to, float t)
		{
			float num = from.Dotp4(to);
			float num2 = 1f - t;
			float num3;
			if (num < 0f)
			{
				num = -num;
				num3 = -t;
			}
			else
			{
				num3 = t;
			}
			return new Quaternion
			{
				X = num2 * from.X + num3 * to.X,
				Y = num2 * from.Y + num3 * to.Y,
				Z = num2 * from.Z + num3 * to.Z,
				W = num2 * from.W + num3 * to.W
			};
		}

		// Token: 0x06000439 RID: 1081 RVA: 0x0000DC34 File Offset: 0x0000BE34
		public static Mat3 Mat3FromQuaternion(Quaternion quat)
		{
			Mat3 mat = default(Mat3);
			float num = quat.X + quat.X;
			float num2 = quat.Y + quat.Y;
			float num3 = quat.Z + quat.Z;
			float num4 = quat.X * num;
			float num5 = quat.X * num2;
			float num6 = quat.X * num3;
			float num7 = quat.Y * num2;
			float num8 = quat.Y * num3;
			float num9 = quat.Z * num3;
			float num10 = quat.W * num;
			float num11 = quat.W * num2;
			float num12 = quat.W * num3;
			mat.s.x = 1f - (num7 + num9);
			mat.s.y = num5 + num12;
			mat.s.z = num6 - num11;
			mat.f.x = num5 - num12;
			mat.f.y = 1f - (num4 + num9);
			mat.f.z = num8 + num10;
			mat.u.x = num6 + num11;
			mat.u.y = num8 - num10;
			mat.u.z = 1f - (num4 + num7);
			return mat;
		}

		// Token: 0x0600043A RID: 1082 RVA: 0x0000DD7C File Offset: 0x0000BF7C
		public static Quaternion QuaternionFromMat3(Mat3 m)
		{
			Quaternion quaternion = default(Quaternion);
			float num;
			if (m[2][2] < 0f)
			{
				if (m[0][0] > m[1][1])
				{
					num = 1f + m[0][0] - m[1][1] - m[2][2];
					quaternion.W = m[1][2] - m[2][1];
					quaternion.X = num;
					quaternion.Y = m[0][1] + m[1][0];
					quaternion.Z = m[2][0] + m[0][2];
				}
				else
				{
					num = 1f - m[0][0] + m[1][1] - m[2][2];
					quaternion.W = m[2][0] - m[0][2];
					quaternion.X = m[0][1] + m[1][0];
					quaternion.Y = num;
					quaternion.Z = m[1][2] + m[2][1];
				}
			}
			else if (m[0][0] < -m[1][1])
			{
				num = 1f - m[0][0] - m[1][1] + m[2][2];
				quaternion.W = m[0][1] - m[1][0];
				quaternion.X = m[2][0] + m[0][2];
				quaternion.Y = m[1][2] + m[2][1];
				quaternion.Z = num;
			}
			else
			{
				num = 1f + m[0][0] + m[1][1] + m[2][2];
				quaternion.W = num;
				quaternion.X = m[1][2] - m[2][1];
				quaternion.Y = m[2][0] - m[0][2];
				quaternion.Z = m[0][1] - m[1][0];
			}
			float num2 = 0.5f / MathF.Sqrt(num);
			quaternion.W *= num2;
			quaternion.X *= num2;
			quaternion.Y *= num2;
			quaternion.Z *= num2;
			return quaternion;
		}

		// Token: 0x0600043B RID: 1083 RVA: 0x0000E150 File Offset: 0x0000C350
		public static void AxisAngleFromQuaternion(out Vec3 axis, out float angle, Quaternion quat)
		{
			axis = default(Vec3);
			float w = quat.W;
			if (w > 0.9999999f)
			{
				axis.x = 1f;
				axis.y = 0f;
				axis.z = 0f;
				angle = 0f;
				return;
			}
			float num = MathF.Sqrt(1f - w * w);
			if (num < 0.0001f)
			{
				num = 1f;
			}
			axis.x = quat.X / num;
			axis.y = quat.Y / num;
			axis.z = quat.Z / num;
			angle = MathF.Acos(w) * 2f;
		}

		// Token: 0x0600043C RID: 1084 RVA: 0x0000E1F4 File Offset: 0x0000C3F4
		public static Quaternion QuaternionFromAxisAngle(Vec3 axis, float angle)
		{
			Quaternion quaternion = default(Quaternion);
			float num;
			float num2;
			MathF.SinCos(angle * 0.5f, out num, out num2);
			quaternion.X = axis.x * num;
			quaternion.Y = axis.y * num;
			quaternion.Z = axis.z * num;
			quaternion.W = num2;
			return quaternion;
		}

		// Token: 0x0600043D RID: 1085 RVA: 0x0000E250 File Offset: 0x0000C450
		public static Vec3 EulerAngleFromQuaternion(Quaternion quat)
		{
			float w = quat.W;
			float x = quat.X;
			float y = quat.Y;
			float z = quat.Z;
			float num = w * w;
			float num2 = x * x;
			float num3 = y * y;
			float num4 = z * z;
			return new Vec3
			{
				z = MathF.Atan2(2f * (x * y + z * w), num2 - num3 - num4 + num),
				x = MathF.Atan2(2f * (y * z + x * w), -num2 - num3 + num4 + num),
				y = MathF.Asin(-2f * (x * z - y * w))
			};
		}

		// Token: 0x0600043E RID: 1086 RVA: 0x0000E2FC File Offset: 0x0000C4FC
		public static Quaternion FindShortestArcAsQuaternion(Vec3 v0, Vec3 v1)
		{
			Vec3 vec = Vec3.CrossProduct(v0, v1);
			float num = Vec3.DotProduct(v0, v1);
			if ((double)num < -0.9999900000002526)
			{
				Vec3 vec2 = default(Vec3);
				if (MathF.Abs(v0.z) < 0.8f)
				{
					vec2 = Vec3.CrossProduct(v0, new Vec3(0f, 0f, 1f, -1f));
				}
				else
				{
					vec2 = Vec3.CrossProduct(v0, new Vec3(1f, 0f, 0f, -1f));
				}
				vec2.Normalize();
				return new Quaternion(vec2.x, vec2.y, vec2.z, 0f);
			}
			float num2 = MathF.Sqrt((1f + num) * 2f);
			float num3 = 1f / num2;
			return new Quaternion(vec.x * num3, vec.y * num3, vec.z * num3, num2 * 0.5f);
		}

		// Token: 0x0600043F RID: 1087 RVA: 0x0000E3EE File Offset: 0x0000C5EE
		public float Dotp4(Quaternion q2)
		{
			return this.X * q2.X + this.Y * q2.Y + this.Z * q2.Z + this.W * q2.W;
		}

		// Token: 0x17000071 RID: 113
		// (get) Token: 0x06000440 RID: 1088 RVA: 0x0000E427 File Offset: 0x0000C627
		public Mat3 ToMat3
		{
			get
			{
				return Quaternion.Mat3FromQuaternion(this);
			}
		}

		// Token: 0x06000441 RID: 1089 RVA: 0x0000E434 File Offset: 0x0000C634
		public bool InverseDirection(Quaternion q2)
		{
			return this.Dotp4(q2) < 0f;
		}

		// Token: 0x0400013C RID: 316
		public float W;

		// Token: 0x0400013D RID: 317
		public float X;

		// Token: 0x0400013E RID: 318
		public float Y;

		// Token: 0x0400013F RID: 319
		public float Z;
	}
}
