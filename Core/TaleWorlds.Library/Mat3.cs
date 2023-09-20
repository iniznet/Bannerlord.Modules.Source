using System;

namespace TaleWorlds.Library
{
	[Serializable]
	public struct Mat3
	{
		public Mat3(Vec3 s, Vec3 f, Vec3 u)
		{
			this.s = s;
			this.f = f;
			this.u = u;
		}

		public Mat3(float sx, float sy, float sz, float fx, float fy, float fz, float ux, float uy, float uz)
		{
			this.s = new Vec3(sx, sy, sz, -1f);
			this.f = new Vec3(fx, fy, fz, -1f);
			this.u = new Vec3(ux, uy, uz, -1f);
		}

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

		public void RotateAboutAnArbitraryVector(Vec3 v, float a)
		{
			this.s = this.s.RotateAboutAnArbitraryVector(v, a);
			this.f = this.f.RotateAboutAnArbitraryVector(v, a);
			this.u = this.u.RotateAboutAnArbitraryVector(v, a);
		}

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

		public bool IsLeftHanded()
		{
			return Vec3.DotProduct(Vec3.CrossProduct(this.s, this.f), this.u) < 0f;
		}

		public bool NearlyEquals(Mat3 rhs, float epsilon = 1E-05f)
		{
			return this.s.NearlyEquals(rhs.s, epsilon) && this.f.NearlyEquals(rhs.f, epsilon) && this.u.NearlyEquals(rhs.u, epsilon);
		}

		public Vec3 TransformToParent(Vec3 v)
		{
			return new Vec3(this.s.x * v.x + this.f.x * v.y + this.u.x * v.z, this.s.y * v.x + this.f.y * v.y + this.u.y * v.z, this.s.z * v.x + this.f.z * v.y + this.u.z * v.z, -1f);
		}

		public Vec3 TransformToParent(ref Vec3 v)
		{
			return new Vec3(this.s.x * v.x + this.f.x * v.y + this.u.x * v.z, this.s.y * v.x + this.f.y * v.y + this.u.y * v.z, this.s.z * v.x + this.f.z * v.y + this.u.z * v.z, -1f);
		}

		public Vec3 TransformToLocal(Vec3 v)
		{
			return new Vec3(this.s.x * v.x + this.s.y * v.y + this.s.z * v.z, this.f.x * v.x + this.f.y * v.y + this.f.z * v.z, this.u.x * v.x + this.u.y * v.y + this.u.z * v.z, -1f);
		}

		public Mat3 TransformToParent(Mat3 m)
		{
			return new Mat3(this.TransformToParent(m.s), this.TransformToParent(m.f), this.TransformToParent(m.u));
		}

		public Mat3 TransformToLocal(Mat3 m)
		{
			Mat3 mat;
			mat.s = this.TransformToLocal(m.s);
			mat.f = this.TransformToLocal(m.f);
			mat.u = this.TransformToLocal(m.u);
			return mat;
		}

		public void Orthonormalize()
		{
			this.f.Normalize();
			this.s = Vec3.CrossProduct(this.f, this.u);
			this.s.Normalize();
			this.u = Vec3.CrossProduct(this.s, this.f);
		}

		public void OrthonormalizeAccordingToForwardAndKeepUpAsZAxis()
		{
			this.f.z = 0f;
			this.f.Normalize();
			this.u = Vec3.Up;
			this.s = Vec3.CrossProduct(this.f, this.u);
		}

		public Mat3 GetUnitRotation(float removedScale)
		{
			float num = 1f / removedScale;
			return new Mat3(this.s * num, this.f * num, this.u * num);
		}

		public Vec3 MakeUnit()
		{
			return new Vec3
			{
				x = this.s.Normalize(),
				y = this.f.Normalize(),
				z = this.u.Normalize()
			};
		}

		public bool IsUnit()
		{
			return this.s.IsUnit && this.f.IsUnit && this.u.IsUnit;
		}

		public void ApplyScaleLocal(float scaleAmount)
		{
			this.s *= scaleAmount;
			this.f *= scaleAmount;
			this.u *= scaleAmount;
		}

		public void ApplyScaleLocal(Vec3 scaleAmountXYZ)
		{
			this.s *= scaleAmountXYZ.x;
			this.f *= scaleAmountXYZ.y;
			this.u *= scaleAmountXYZ.z;
		}

		public bool HasScale()
		{
			return !this.s.IsUnit || !this.f.IsUnit || !this.u.IsUnit;
		}

		public Vec3 GetScaleVector()
		{
			return new Vec3(this.s.Length, this.f.Length, this.u.Length, -1f);
		}

		public Vec3 GetScaleVectorSquared()
		{
			return new Vec3(this.s.LengthSquared, this.f.LengthSquared, this.u.LengthSquared, -1f);
		}

		public void ToQuaternion(out Quaternion quat)
		{
			quat = Quaternion.QuaternionFromMat3(this);
		}

		public Quaternion ToQuaternion()
		{
			return Quaternion.QuaternionFromMat3(this);
		}

		public static Mat3 Lerp(Mat3 m1, Mat3 m2, float alpha)
		{
			Mat3 identity = Mat3.Identity;
			identity.f = Vec3.Lerp(m1.f, m2.f, alpha);
			identity.u = Vec3.Lerp(m1.u, m2.u, alpha);
			identity.Orthonormalize();
			return identity;
		}

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

		public Vec3 GetEulerAngles()
		{
			Mat3 mat = this;
			mat.Orthonormalize();
			return new Vec3(MathF.Asin(mat.f.z), MathF.Atan2(-mat.s.z, mat.u.z), MathF.Atan2(-mat.f.x, mat.f.y), -1f);
		}

		public Mat3 Transpose()
		{
			return new Mat3(this.s.x, this.f.x, this.u.x, this.s.y, this.f.y, this.u.y, this.s.z, this.f.z, this.u.z);
		}

		public static Mat3 Identity
		{
			get
			{
				return new Mat3(new Vec3(1f, 0f, 0f, -1f), new Vec3(0f, 1f, 0f, -1f), new Vec3(0f, 0f, 1f, -1f));
			}
		}

		public static Mat3 operator *(Mat3 v, float a)
		{
			return new Mat3(v.s * a, v.f * a, v.u * a);
		}

		public static bool operator ==(Mat3 m1, Mat3 m2)
		{
			return m1.f == m2.f && m1.u == m2.u;
		}

		public static bool operator !=(Mat3 m1, Mat3 m2)
		{
			return m1.f != m2.f || m1.u != m2.u;
		}

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

		public override bool Equals(object obj)
		{
			return this == (Mat3)obj;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public bool IsIdentity()
		{
			return this.s.x == 1f && this.s.y == 0f && this.s.z == 0f && this.f.x == 0f && this.f.y == 1f && this.f.z == 0f && this.u.x == 0f && this.u.y == 0f && this.u.z == 1f;
		}

		public bool IsZero()
		{
			return this.s.x == 0f && this.s.y == 0f && this.s.z == 0f && this.f.x == 0f && this.f.y == 0f && this.f.z == 0f && this.u.x == 0f && this.u.y == 0f && this.u.z == 0f;
		}

		public bool IsUniformScaled()
		{
			Vec3 scaleVectorSquared = this.GetScaleVectorSquared();
			return MBMath.ApproximatelyEquals(scaleVectorSquared.x, scaleVectorSquared.y, 0.01f) && MBMath.ApproximatelyEquals(scaleVectorSquared.x, scaleVectorSquared.z, 0.01f);
		}

		public void ApplyEulerAngles(Vec3 eulerAngles)
		{
			this.RotateAboutUp(eulerAngles.z);
			this.RotateAboutSide(eulerAngles.x);
			this.RotateAboutForward(eulerAngles.y);
		}

		public Vec3 s;

		public Vec3 f;

		public Vec3 u;
	}
}
