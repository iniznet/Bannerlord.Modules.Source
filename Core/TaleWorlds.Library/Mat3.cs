using System;

namespace TaleWorlds.Library
{
	// Token: 0x0200005D RID: 93
	[Serializable]
	public struct Mat3
	{
		// Token: 0x0600028E RID: 654 RVA: 0x0000749D File Offset: 0x0000569D
		public Mat3(Vec3 s, Vec3 f, Vec3 u)
		{
			this.s = s;
			this.f = f;
			this.u = u;
		}

		// Token: 0x0600028F RID: 655 RVA: 0x000074B4 File Offset: 0x000056B4
		public Mat3(float sx, float sy, float sz, float fx, float fy, float fz, float ux, float uy, float uz)
		{
			this.s = new Vec3(sx, sy, sz, -1f);
			this.f = new Vec3(fx, fy, fz, -1f);
			this.u = new Vec3(ux, uy, uz, -1f);
		}

		// Token: 0x1700003E RID: 62
		public Vec3 this[int i]
		{
			get
			{
				switch (i)
				{
				case 0:
					return this.s;
				case 1:
					return this.f;
				case 2:
					return this.u;
				default:
					throw new IndexOutOfRangeException("Vec3 out of bounds.");
				}
			}
			set
			{
				switch (i)
				{
				case 0:
					this.s = value;
					return;
				case 1:
					this.f = value;
					return;
				case 2:
					this.u = value;
					return;
				default:
					throw new IndexOutOfRangeException("Vec3 out of bounds.");
				}
			}
		}

		// Token: 0x06000292 RID: 658 RVA: 0x00007570 File Offset: 0x00005770
		public void RotateAboutSide(float a)
		{
			float num;
			float num2;
			MathF.SinCos(a, out num, out num2);
			Vec3 vec = this.f * num2 + this.u * num;
			Vec3 vec2 = this.u * num2 - this.f * num;
			this.u = vec2;
			this.f = vec;
		}

		// Token: 0x06000293 RID: 659 RVA: 0x000075D4 File Offset: 0x000057D4
		public void RotateAboutForward(float a)
		{
			float num;
			float num2;
			MathF.SinCos(a, out num, out num2);
			Vec3 vec = this.s * num2 - this.u * num;
			Vec3 vec2 = this.u * num2 + this.s * num;
			this.s = vec;
			this.u = vec2;
		}

		// Token: 0x06000294 RID: 660 RVA: 0x00007638 File Offset: 0x00005838
		public void RotateAboutUp(float a)
		{
			float num;
			float num2;
			MathF.SinCos(a, out num, out num2);
			Vec3 vec = this.s * num2 + this.f * num;
			Vec3 vec2 = this.f * num2 - this.s * num;
			this.s = vec;
			this.f = vec2;
		}

		// Token: 0x06000295 RID: 661 RVA: 0x00007699 File Offset: 0x00005899
		public void RotateAboutAnArbitraryVector(Vec3 v, float a)
		{
			this.s = this.s.RotateAboutAnArbitraryVector(v, a);
			this.f = this.f.RotateAboutAnArbitraryVector(v, a);
			this.u = this.u.RotateAboutAnArbitraryVector(v, a);
		}

		// Token: 0x06000296 RID: 662 RVA: 0x000076D4 File Offset: 0x000058D4
		public bool IsOrthonormal()
		{
			bool flag = this.s.IsUnit && this.f.IsUnit && this.u.IsUnit;
			float num = Vec3.DotProduct(this.s, this.f);
			if (num > 0.01f || num < -0.01f)
			{
				flag = false;
			}
			else
			{
				Vec3 vec = Vec3.CrossProduct(this.s, this.f);
				if (!this.u.NearlyEquals(vec, 0.01f))
				{
					flag = false;
				}
			}
			return flag;
		}

		// Token: 0x06000297 RID: 663 RVA: 0x00007758 File Offset: 0x00005958
		public bool IsLeftHanded()
		{
			return Vec3.DotProduct(Vec3.CrossProduct(this.s, this.f), this.u) < 0f;
		}

		// Token: 0x06000298 RID: 664 RVA: 0x0000777D File Offset: 0x0000597D
		public bool NearlyEquals(Mat3 rhs, float epsilon = 1E-05f)
		{
			return this.s.NearlyEquals(rhs.s, epsilon) && this.f.NearlyEquals(rhs.f, epsilon) && this.u.NearlyEquals(rhs.u, epsilon);
		}

		// Token: 0x06000299 RID: 665 RVA: 0x000077BC File Offset: 0x000059BC
		public Vec3 TransformToParent(Vec3 v)
		{
			return new Vec3(this.s.x * v.x + this.f.x * v.y + this.u.x * v.z, this.s.y * v.x + this.f.y * v.y + this.u.y * v.z, this.s.z * v.x + this.f.z * v.y + this.u.z * v.z, -1f);
		}

		// Token: 0x0600029A RID: 666 RVA: 0x0000787C File Offset: 0x00005A7C
		public Vec3 TransformToParent(ref Vec3 v)
		{
			return new Vec3(this.s.x * v.x + this.f.x * v.y + this.u.x * v.z, this.s.y * v.x + this.f.y * v.y + this.u.y * v.z, this.s.z * v.x + this.f.z * v.y + this.u.z * v.z, -1f);
		}

		// Token: 0x0600029B RID: 667 RVA: 0x0000793C File Offset: 0x00005B3C
		public Vec3 TransformToLocal(Vec3 v)
		{
			return new Vec3(this.s.x * v.x + this.s.y * v.y + this.s.z * v.z, this.f.x * v.x + this.f.y * v.y + this.f.z * v.z, this.u.x * v.x + this.u.y * v.y + this.u.z * v.z, -1f);
		}

		// Token: 0x0600029C RID: 668 RVA: 0x000079FB File Offset: 0x00005BFB
		public Mat3 TransformToParent(Mat3 m)
		{
			return new Mat3(this.TransformToParent(m.s), this.TransformToParent(m.f), this.TransformToParent(m.u));
		}

		// Token: 0x0600029D RID: 669 RVA: 0x00007A28 File Offset: 0x00005C28
		public Mat3 TransformToLocal(Mat3 m)
		{
			Mat3 mat;
			mat.s = this.TransformToLocal(m.s);
			mat.f = this.TransformToLocal(m.f);
			mat.u = this.TransformToLocal(m.u);
			return mat;
		}

		// Token: 0x0600029E RID: 670 RVA: 0x00007A70 File Offset: 0x00005C70
		public void Orthonormalize()
		{
			this.f.Normalize();
			this.s = Vec3.CrossProduct(this.f, this.u);
			this.s.Normalize();
			this.u = Vec3.CrossProduct(this.s, this.f);
		}

		// Token: 0x0600029F RID: 671 RVA: 0x00007AC3 File Offset: 0x00005CC3
		public void OrthonormalizeAccordingToForwardAndKeepUpAsZAxis()
		{
			this.f.z = 0f;
			this.f.Normalize();
			this.u = Vec3.Up;
			this.s = Vec3.CrossProduct(this.f, this.u);
		}

		// Token: 0x060002A0 RID: 672 RVA: 0x00007B04 File Offset: 0x00005D04
		public Mat3 GetUnitRotation(float removedScale)
		{
			float num = 1f / removedScale;
			return new Mat3(this.s * num, this.f * num, this.u * num);
		}

		// Token: 0x060002A1 RID: 673 RVA: 0x00007B44 File Offset: 0x00005D44
		public Vec3 MakeUnit()
		{
			return new Vec3
			{
				x = this.s.Normalize(),
				y = this.f.Normalize(),
				z = this.u.Normalize()
			};
		}

		// Token: 0x060002A2 RID: 674 RVA: 0x00007B90 File Offset: 0x00005D90
		public bool IsUnit()
		{
			return this.s.IsUnit && this.f.IsUnit && this.u.IsUnit;
		}

		// Token: 0x060002A3 RID: 675 RVA: 0x00007BB9 File Offset: 0x00005DB9
		public void ApplyScaleLocal(float scaleAmount)
		{
			this.s *= scaleAmount;
			this.f *= scaleAmount;
			this.u *= scaleAmount;
		}

		// Token: 0x060002A4 RID: 676 RVA: 0x00007BF4 File Offset: 0x00005DF4
		public void ApplyScaleLocal(Vec3 scaleAmountXYZ)
		{
			this.s *= scaleAmountXYZ.x;
			this.f *= scaleAmountXYZ.y;
			this.u *= scaleAmountXYZ.z;
		}

		// Token: 0x060002A5 RID: 677 RVA: 0x00007C46 File Offset: 0x00005E46
		public bool HasScale()
		{
			return !this.s.IsUnit || !this.f.IsUnit || !this.u.IsUnit;
		}

		// Token: 0x060002A6 RID: 678 RVA: 0x00007C72 File Offset: 0x00005E72
		public Vec3 GetScaleVector()
		{
			return new Vec3(this.s.Length, this.f.Length, this.u.Length, -1f);
		}

		// Token: 0x060002A7 RID: 679 RVA: 0x00007C9F File Offset: 0x00005E9F
		public Vec3 GetScaleVectorSquared()
		{
			return new Vec3(this.s.LengthSquared, this.f.LengthSquared, this.u.LengthSquared, -1f);
		}

		// Token: 0x060002A8 RID: 680 RVA: 0x00007CCC File Offset: 0x00005ECC
		public void ToQuaternion(out Quaternion quat)
		{
			quat = Quaternion.QuaternionFromMat3(this);
		}

		// Token: 0x060002A9 RID: 681 RVA: 0x00007CDF File Offset: 0x00005EDF
		public Quaternion ToQuaternion()
		{
			return Quaternion.QuaternionFromMat3(this);
		}

		// Token: 0x060002AA RID: 682 RVA: 0x00007CEC File Offset: 0x00005EEC
		public static Mat3 Lerp(Mat3 m1, Mat3 m2, float alpha)
		{
			Mat3 identity = Mat3.Identity;
			identity.f = Vec3.Lerp(m1.f, m2.f, alpha);
			identity.u = Vec3.Lerp(m1.u, m2.u, alpha);
			identity.Orthonormalize();
			return identity;
		}

		// Token: 0x060002AB RID: 683 RVA: 0x00007D3C File Offset: 0x00005F3C
		public static Mat3 CreateMat3WithForward(in Vec3 direction)
		{
			Mat3 identity = Mat3.Identity;
			identity.f = direction;
			identity.f.Normalize();
			if (MathF.Abs(identity.f.z) < 0.99f)
			{
				identity.u = new Vec3(0f, 0f, 1f, -1f);
			}
			else
			{
				identity.u = new Vec3(0f, 1f, 0f, -1f);
			}
			identity.s = Vec3.CrossProduct(identity.f, identity.u);
			identity.s.Normalize();
			identity.u = Vec3.CrossProduct(identity.s, identity.f);
			identity.u.Normalize();
			return identity;
		}

		// Token: 0x060002AC RID: 684 RVA: 0x00007E10 File Offset: 0x00006010
		public Vec3 GetEulerAngles()
		{
			Mat3 mat = this;
			mat.Orthonormalize();
			return new Vec3(MathF.Asin(mat.f.z), MathF.Atan2(-mat.s.z, mat.u.z), MathF.Atan2(-mat.f.x, mat.f.y), -1f);
		}

		// Token: 0x060002AD RID: 685 RVA: 0x00007E80 File Offset: 0x00006080
		public Mat3 Transpose()
		{
			return new Mat3(this.s.x, this.f.x, this.u.x, this.s.y, this.f.y, this.u.y, this.s.z, this.f.z, this.u.z);
		}

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x060002AE RID: 686 RVA: 0x00007EF8 File Offset: 0x000060F8
		public static Mat3 Identity
		{
			get
			{
				return new Mat3(new Vec3(1f, 0f, 0f, -1f), new Vec3(0f, 1f, 0f, -1f), new Vec3(0f, 0f, 1f, -1f));
			}
		}

		// Token: 0x060002AF RID: 687 RVA: 0x00007F55 File Offset: 0x00006155
		public static Mat3 operator *(Mat3 v, float a)
		{
			return new Mat3(v.s * a, v.f * a, v.u * a);
		}

		// Token: 0x060002B0 RID: 688 RVA: 0x00007F80 File Offset: 0x00006180
		public static bool operator ==(Mat3 m1, Mat3 m2)
		{
			return m1.f == m2.f && m1.u == m2.u;
		}

		// Token: 0x060002B1 RID: 689 RVA: 0x00007FA8 File Offset: 0x000061A8
		public static bool operator !=(Mat3 m1, Mat3 m2)
		{
			return m1.f != m2.f || m1.u != m2.u;
		}

		// Token: 0x060002B2 RID: 690 RVA: 0x00007FD0 File Offset: 0x000061D0
		public override string ToString()
		{
			string text = "Mat3: ";
			text = string.Concat(new object[]
			{
				text,
				"s: ",
				this.s.x,
				", ",
				this.s.y,
				", ",
				this.s.z,
				";"
			});
			text = string.Concat(new object[]
			{
				text,
				"f: ",
				this.f.x,
				", ",
				this.f.y,
				", ",
				this.f.z,
				";"
			});
			text = string.Concat(new object[]
			{
				text,
				"u: ",
				this.u.x,
				", ",
				this.u.y,
				", ",
				this.u.z,
				";"
			});
			return text + "\n";
		}

		// Token: 0x060002B3 RID: 691 RVA: 0x00008129 File Offset: 0x00006329
		public override bool Equals(object obj)
		{
			return this == (Mat3)obj;
		}

		// Token: 0x060002B4 RID: 692 RVA: 0x0000813C File Offset: 0x0000633C
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		// Token: 0x060002B5 RID: 693 RVA: 0x00008150 File Offset: 0x00006350
		public bool IsIdentity()
		{
			return this.s.x == 1f && this.s.y == 0f && this.s.z == 0f && this.f.x == 0f && this.f.y == 1f && this.f.z == 0f && this.u.x == 0f && this.u.y == 0f && this.u.z == 1f;
		}

		// Token: 0x060002B6 RID: 694 RVA: 0x00008204 File Offset: 0x00006404
		public bool IsZero()
		{
			return this.s.x == 0f && this.s.y == 0f && this.s.z == 0f && this.f.x == 0f && this.f.y == 0f && this.f.z == 0f && this.u.x == 0f && this.u.y == 0f && this.u.z == 0f;
		}

		// Token: 0x060002B7 RID: 695 RVA: 0x000082B8 File Offset: 0x000064B8
		public bool IsUniformScaled()
		{
			Vec3 scaleVectorSquared = this.GetScaleVectorSquared();
			return MBMath.ApproximatelyEquals(scaleVectorSquared.x, scaleVectorSquared.y, 0.01f) && MBMath.ApproximatelyEquals(scaleVectorSquared.x, scaleVectorSquared.z, 0.01f);
		}

		// Token: 0x060002B8 RID: 696 RVA: 0x000082FC File Offset: 0x000064FC
		public void ApplyEulerAngles(Vec3 eulerAngles)
		{
			this.RotateAboutUp(eulerAngles.z);
			this.RotateAboutSide(eulerAngles.x);
			this.RotateAboutForward(eulerAngles.y);
		}

		// Token: 0x040000F7 RID: 247
		public Vec3 s;

		// Token: 0x040000F8 RID: 248
		public Vec3 f;

		// Token: 0x040000F9 RID: 249
		public Vec3 u;
	}
}
