using System;

namespace TaleWorlds.Library
{
	[Serializable]
	public struct MatrixFrame
	{
		public MatrixFrame(Mat3 rot, Vec3 o)
		{
			this.rotation = rot;
			this.origin = o;
		}

		public MatrixFrame(float _11, float _12, float _13, float _21, float _22, float _23, float _31, float _32, float _33, float _41, float _42, float _43)
		{
			this.rotation = new Mat3(_11, _12, _13, _21, _22, _23, _31, _32, _33);
			this.origin = new Vec3(_41, _42, _43, -1f);
		}

		public MatrixFrame(float _11, float _12, float _13, float _14, float _21, float _22, float _23, float _24, float _31, float _32, float _33, float _34, float _41, float _42, float _43, float _44)
		{
			this.rotation = default(Mat3);
			this.rotation.s = new Vec3(_11, _12, _13, _14);
			this.rotation.f = new Vec3(_21, _22, _23, _24);
			this.rotation.u = new Vec3(_31, _32, _33, _34);
			this.origin = new Vec3(_41, _42, _43, _44);
		}

		public Vec3 TransformToParent(Vec3 v)
		{
			return new Vec3(this.rotation.s.x * v.x + this.rotation.f.x * v.y + this.rotation.u.x * v.z + this.origin.x, this.rotation.s.y * v.x + this.rotation.f.y * v.y + this.rotation.u.y * v.z + this.origin.y, this.rotation.s.z * v.x + this.rotation.f.z * v.y + this.rotation.u.z * v.z + this.origin.z, -1f);
		}

		public Vec3 TransformToLocal(Vec3 v)
		{
			Vec3 vec = v - this.origin;
			return new Vec3(this.rotation.s.x * vec.x + this.rotation.s.y * vec.y + this.rotation.s.z * vec.z, this.rotation.f.x * vec.x + this.rotation.f.y * vec.y + this.rotation.f.z * vec.z, this.rotation.u.x * vec.x + this.rotation.u.y * vec.y + this.rotation.u.z * vec.z, -1f);
		}

		public Vec3 TransformToLocalNonUnit(Vec3 v)
		{
			Vec3 vec = v - this.origin;
			return new Vec3(this.rotation.s.x * vec.x + this.rotation.s.y * vec.y + this.rotation.s.z * vec.z, this.rotation.f.x * vec.x + this.rotation.f.y * vec.y + this.rotation.f.z * vec.z, this.rotation.u.x * vec.x + this.rotation.u.y * vec.y + this.rotation.u.z * vec.z, -1f);
		}

		public bool NearlyEquals(MatrixFrame rhs, float epsilon = 1E-05f)
		{
			return this.rotation.NearlyEquals(rhs.rotation, epsilon) && this.origin.NearlyEquals(rhs.origin, epsilon);
		}

		public Vec3 TransformToLocalNonOrthogonal(Vec3 v)
		{
			MatrixFrame matrixFrame = new MatrixFrame(this.rotation.s.x, this.rotation.s.y, this.rotation.s.z, 0f, this.rotation.f.x, this.rotation.f.y, this.rotation.f.z, 0f, this.rotation.u.x, this.rotation.u.y, this.rotation.u.z, 0f, this.origin.x, this.origin.y, this.origin.z, 1f);
			return matrixFrame.Inverse().TransformToParent(v);
		}

		public MatrixFrame TransformToLocalNonOrthogonal(ref MatrixFrame frame)
		{
			MatrixFrame matrixFrame = new MatrixFrame(this.rotation.s.x, this.rotation.s.y, this.rotation.s.z, 0f, this.rotation.f.x, this.rotation.f.y, this.rotation.f.z, 0f, this.rotation.u.x, this.rotation.u.y, this.rotation.u.z, 0f, this.origin.x, this.origin.y, this.origin.z, 1f);
			return matrixFrame.Inverse().TransformToParent(frame);
		}

		public static MatrixFrame Lerp(MatrixFrame m1, MatrixFrame m2, float alpha)
		{
			MatrixFrame matrixFrame;
			matrixFrame.rotation = Mat3.Lerp(m1.rotation, m2.rotation, alpha);
			matrixFrame.origin = Vec3.Lerp(m1.origin, m2.origin, alpha);
			return matrixFrame;
		}

		public static MatrixFrame Slerp(MatrixFrame m1, MatrixFrame m2, float alpha)
		{
			MatrixFrame matrixFrame;
			matrixFrame.origin = Vec3.Lerp(m1.origin, m2.origin, alpha);
			matrixFrame.rotation = Quaternion.Slerp(Quaternion.QuaternionFromMat3(m1.rotation), Quaternion.QuaternionFromMat3(m2.rotation), alpha).ToMat3;
			return matrixFrame;
		}

		public MatrixFrame TransformToParent(MatrixFrame m)
		{
			return new MatrixFrame(this.rotation.TransformToParent(m.rotation), this.TransformToParent(m.origin));
		}

		public MatrixFrame TransformToLocal(MatrixFrame m)
		{
			return new MatrixFrame(this.rotation.TransformToLocal(m.rotation), this.TransformToLocal(m.origin));
		}

		public Vec3 TransformToParentWithW(Vec3 _s)
		{
			return new Vec3(this.rotation.s.x * _s.x + this.rotation.f.x * _s.y + this.rotation.u.x * _s.z + this.origin.x * _s.w, this.rotation.s.y * _s.x + this.rotation.f.y * _s.y + this.rotation.u.y * _s.z + this.origin.y * _s.w, this.rotation.s.z * _s.x + this.rotation.f.z * _s.y + this.rotation.u.z * _s.z + this.origin.z * _s.w, this.rotation.s.w * _s.x + this.rotation.f.w * _s.y + this.rotation.u.w * _s.z + this.origin.w * _s.w);
		}

		public MatrixFrame GetUnitRotFrame(float removedScale)
		{
			return new MatrixFrame(this.rotation.GetUnitRotation(removedScale), this.origin);
		}

		public static MatrixFrame Identity
		{
			get
			{
				return new MatrixFrame(Mat3.Identity, new Vec3(0f, 0f, 0f, 1f));
			}
		}

		public static MatrixFrame Zero
		{
			get
			{
				return new MatrixFrame(new Mat3(Vec3.Zero, Vec3.Zero, Vec3.Zero), new Vec3(0f, 0f, 0f, 1f));
			}
		}

		public MatrixFrame Inverse()
		{
			this.AssertFilled();
			MatrixFrame matrixFrame = default(MatrixFrame);
			float num = this[2, 2] * this[3, 3] - this[2, 3] * this[3, 2];
			float num2 = this[1, 2] * this[3, 3] - this[1, 3] * this[3, 2];
			float num3 = this[1, 2] * this[2, 3] - this[1, 3] * this[2, 2];
			float num4 = this[0, 2] * this[3, 3] - this[0, 3] * this[3, 2];
			float num5 = this[0, 2] * this[2, 3] - this[0, 3] * this[2, 2];
			float num6 = this[0, 2] * this[1, 3] - this[0, 3] * this[1, 2];
			float num7 = this[2, 1] * this[3, 3] - this[2, 3] * this[3, 1];
			float num8 = this[1, 1] * this[3, 3] - this[1, 3] * this[3, 1];
			float num9 = this[1, 1] * this[2, 3] - this[1, 3] * this[2, 1];
			float num10 = this[0, 1] * this[3, 3] - this[0, 3] * this[3, 1];
			float num11 = this[0, 1] * this[2, 3] - this[0, 3] * this[2, 1];
			float num12 = this[1, 1] * this[3, 3] - this[1, 3] * this[3, 1];
			float num13 = this[0, 1] * this[1, 3] - this[0, 3] * this[1, 1];
			float num14 = this[2, 1] * this[3, 2] - this[2, 2] * this[3, 1];
			float num15 = this[1, 1] * this[3, 2] - this[1, 2] * this[3, 1];
			float num16 = this[1, 1] * this[2, 2] - this[1, 2] * this[2, 1];
			float num17 = this[0, 1] * this[3, 2] - this[0, 2] * this[3, 1];
			float num18 = this[0, 1] * this[2, 2] - this[0, 2] * this[2, 1];
			float num19 = this[0, 1] * this[1, 2] - this[0, 2] * this[1, 1];
			matrixFrame[0, 0] = this[1, 1] * num - this[2, 1] * num2 + this[3, 1] * num3;
			matrixFrame[0, 1] = -this[0, 1] * num + this[2, 1] * num4 - this[3, 1] * num5;
			matrixFrame[0, 2] = this[0, 1] * num2 - this[1, 1] * num4 + this[3, 1] * num6;
			matrixFrame[0, 3] = -this[0, 1] * num3 + this[1, 1] * num5 - this[2, 1] * num6;
			matrixFrame[1, 0] = -this[1, 0] * num + this[2, 0] * num2 - this[3, 0] * num3;
			matrixFrame[1, 1] = this[0, 0] * num - this[2, 0] * num4 + this[3, 0] * num5;
			matrixFrame[1, 2] = -this[0, 0] * num2 + this[1, 0] * num4 - this[3, 0] * num6;
			matrixFrame[1, 3] = this[0, 0] * num3 - this[1, 0] * num5 + this[2, 0] * num6;
			matrixFrame[2, 0] = this[1, 0] * num7 - this[2, 0] * num8 + this[3, 0] * num9;
			matrixFrame[2, 1] = -this[0, 0] * num7 + this[2, 0] * num10 - this[3, 0] * num11;
			matrixFrame[2, 2] = this[0, 0] * num12 - this[1, 0] * num10 + this[3, 0] * num13;
			matrixFrame[2, 3] = -this[0, 0] * num9 + this[1, 0] * num11 - this[2, 0] * num13;
			matrixFrame[3, 0] = -this[1, 0] * num14 + this[2, 0] * num15 - this[3, 0] * num16;
			matrixFrame[3, 1] = this[0, 0] * num14 - this[2, 0] * num17 + this[3, 0] * num18;
			matrixFrame[3, 2] = -this[0, 0] * num15 + this[1, 0] * num17 - this[3, 0] * num19;
			matrixFrame[3, 3] = this[0, 0] * num16 - this[1, 0] * num18 + this[2, 0] * num19;
			float num20 = this[0, 0] * matrixFrame[0, 0] + this[1, 0] * matrixFrame[0, 1] + this[2, 0] * matrixFrame[0, 2] + this[3, 0] * matrixFrame[0, 3];
			if (num20 != 1f)
			{
				MatrixFrame.DivideWith(ref matrixFrame, num20);
			}
			return matrixFrame;
		}

		private static void DivideWith(ref MatrixFrame matrix, float w)
		{
			float num = 1f / w;
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					ref MatrixFrame ptr = ref matrix;
					int num2 = i;
					int num3 = j;
					ptr[num2, num3] *= num;
				}
			}
		}

		public void Rotate(float radian, Vec3 axis)
		{
			float num;
			float num2;
			MathF.SinCos(radian, out num, out num2);
			MatrixFrame matrixFrame = default(MatrixFrame);
			matrixFrame[0, 0] = axis.x * axis.x * (1f - num2) + num2;
			matrixFrame[1, 0] = axis.x * axis.y * (1f - num2) - axis.z * num;
			matrixFrame[2, 0] = axis.x * axis.z * (1f - num2) + axis.y * num;
			matrixFrame[3, 0] = 0f;
			matrixFrame[0, 1] = axis.y * axis.x * (1f - num2) + axis.z * num;
			matrixFrame[1, 1] = axis.y * axis.y * (1f - num2) + num2;
			matrixFrame[2, 1] = axis.y * axis.z * (1f - num2) - axis.x * num;
			matrixFrame[3, 1] = 0f;
			matrixFrame[0, 2] = axis.x * axis.z * (1f - num2) - axis.y * num;
			matrixFrame[1, 2] = axis.y * axis.z * (1f - num2) + axis.x * num;
			matrixFrame[2, 2] = axis.z * axis.z * (1f - num2) + num2;
			matrixFrame[3, 2] = 0f;
			matrixFrame[0, 3] = 0f;
			matrixFrame[1, 3] = 0f;
			matrixFrame[2, 3] = 0f;
			matrixFrame[3, 3] = 1f;
			this.origin = this.TransformToParent(matrixFrame.origin);
			this.rotation = this.rotation.TransformToParent(matrixFrame.rotation);
		}

		public static MatrixFrame operator *(MatrixFrame m1, MatrixFrame m2)
		{
			return m1.TransformToParent(m2);
		}

		public static bool operator ==(MatrixFrame m1, MatrixFrame m2)
		{
			return m1.origin == m2.origin && m1.rotation == m2.rotation;
		}

		public static bool operator !=(MatrixFrame m1, MatrixFrame m2)
		{
			return m1.origin != m2.origin || m1.rotation != m2.rotation;
		}

		public override string ToString()
		{
			string text = "MatrixFrame:\n";
			text += "Rotation:\n";
			text += this.rotation.ToString();
			return string.Concat(new object[]
			{
				text,
				"Origin: ",
				this.origin.x,
				", ",
				this.origin.y,
				", ",
				this.origin.z,
				"\n"
			});
		}

		public override bool Equals(object obj)
		{
			return this == (MatrixFrame)obj;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public MatrixFrame Strafe(float a)
		{
			this.origin += this.rotation.s * a;
			return this;
		}

		public MatrixFrame Advance(float a)
		{
			this.origin += this.rotation.f * a;
			return this;
		}

		public MatrixFrame Elevate(float a)
		{
			this.origin += this.rotation.u * a;
			return this;
		}

		public void Scale(Vec3 scalingVector)
		{
			MatrixFrame identity = MatrixFrame.Identity;
			identity.rotation.s.x = scalingVector.x;
			identity.rotation.f.y = scalingVector.y;
			identity.rotation.u.z = scalingVector.z;
			this.origin = this.TransformToParent(identity.origin);
			this.rotation = this.rotation.TransformToParent(identity.rotation);
		}

		public Vec3 GetScale()
		{
			return new Vec3(this.rotation.s.Length, this.rotation.f.Length, this.rotation.u.Length, -1f);
		}

		public bool IsIdentity
		{
			get
			{
				return !this.origin.IsNonZero && this.rotation.IsIdentity();
			}
		}

		public bool IsZero
		{
			get
			{
				return !this.origin.IsNonZero && this.rotation.IsZero();
			}
		}

		public float this[int i, int j]
		{
			get
			{
				float num;
				switch (i)
				{
				case 0:
					num = this.rotation.s[j];
					break;
				case 1:
					num = this.rotation.f[j];
					break;
				case 2:
					num = this.rotation.u[j];
					break;
				case 3:
					num = this.origin[j];
					break;
				default:
					throw new IndexOutOfRangeException("MatrixFrame out of bounds.");
				}
				return num;
			}
			set
			{
				switch (i)
				{
				case 0:
					this.rotation.s[j] = value;
					return;
				case 1:
					this.rotation.f[j] = value;
					return;
				case 2:
					this.rotation.u[j] = value;
					return;
				case 3:
					this.origin[j] = value;
					return;
				default:
					throw new IndexOutOfRangeException("MatrixFrame out of bounds.");
				}
			}
		}

		public static MatrixFrame CreateLookAt(Vec3 position, Vec3 target, Vec3 upVector)
		{
			Vec3 vec = target - position;
			vec.Normalize();
			Vec3 vec2 = Vec3.CrossProduct(upVector, vec);
			vec2.Normalize();
			Vec3 vec3 = Vec3.CrossProduct(vec, vec2);
			float x = vec2.x;
			float x2 = vec3.x;
			float x3 = vec.x;
			float num = 0f;
			float y = vec2.y;
			float y2 = vec3.y;
			float y3 = vec.y;
			float num2 = 0f;
			float z = vec2.z;
			float z2 = vec3.z;
			float z3 = vec.z;
			float num3 = 0f;
			float num4 = -Vec3.DotProduct(vec2, position);
			float num5 = -Vec3.DotProduct(vec3, position);
			float num6 = -Vec3.DotProduct(vec, position);
			float num7 = 1f;
			return new MatrixFrame(x, x2, x3, num, y, y2, y3, num2, z, z2, z3, num3, num4, num5, num6, num7);
		}

		public static MatrixFrame CenterFrameOfTwoPoints(Vec3 p1, Vec3 p2, Vec3 upVector)
		{
			MatrixFrame matrixFrame;
			matrixFrame.origin = (p1 + p2) * 0.5f;
			matrixFrame.rotation.s = p2 - p1;
			matrixFrame.rotation.s.Normalize();
			if (MathF.Abs(Vec3.DotProduct(matrixFrame.rotation.s, upVector)) > 0.95f)
			{
				upVector = new Vec3(0f, 1f, 0f, -1f);
			}
			matrixFrame.rotation.u = upVector;
			matrixFrame.rotation.f = Vec3.CrossProduct(matrixFrame.rotation.u, matrixFrame.rotation.s);
			matrixFrame.rotation.f.Normalize();
			matrixFrame.rotation.u = Vec3.CrossProduct(matrixFrame.rotation.s, matrixFrame.rotation.f);
			matrixFrame.Fill();
			return matrixFrame;
		}

		public void Fill()
		{
			this.rotation.s.w = 0f;
			this.rotation.f.w = 0f;
			this.rotation.u.w = 0f;
			this.origin.w = 1f;
		}

		private void AssertFilled()
		{
		}

		public Mat3 rotation;

		public Vec3 origin;
	}
}
