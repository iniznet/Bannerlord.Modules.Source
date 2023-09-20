using System;

namespace TaleWorlds.Library
{
	// Token: 0x0200005E RID: 94
	[Serializable]
	public struct MatrixFrame
	{
		// Token: 0x060002B9 RID: 697 RVA: 0x00008322 File Offset: 0x00006522
		public MatrixFrame(Mat3 rot, Vec3 o)
		{
			this.rotation = rot;
			this.origin = o;
		}

		// Token: 0x060002BA RID: 698 RVA: 0x00008334 File Offset: 0x00006534
		public MatrixFrame(float _11, float _12, float _13, float _21, float _22, float _23, float _31, float _32, float _33, float _41, float _42, float _43)
		{
			this.rotation = new Mat3(_11, _12, _13, _21, _22, _23, _31, _32, _33);
			this.origin = new Vec3(_41, _42, _43, -1f);
		}

		// Token: 0x060002BB RID: 699 RVA: 0x00008374 File Offset: 0x00006574
		public MatrixFrame(float _11, float _12, float _13, float _14, float _21, float _22, float _23, float _24, float _31, float _32, float _33, float _34, float _41, float _42, float _43, float _44)
		{
			this.rotation = default(Mat3);
			this.rotation.s = new Vec3(_11, _12, _13, _14);
			this.rotation.f = new Vec3(_21, _22, _23, _24);
			this.rotation.u = new Vec3(_31, _32, _33, _34);
			this.origin = new Vec3(_41, _42, _43, _44);
		}

		// Token: 0x060002BC RID: 700 RVA: 0x000083E8 File Offset: 0x000065E8
		public Vec3 TransformToParent(Vec3 v)
		{
			return new Vec3(this.rotation.s.x * v.x + this.rotation.f.x * v.y + this.rotation.u.x * v.z + this.origin.x, this.rotation.s.y * v.x + this.rotation.f.y * v.y + this.rotation.u.y * v.z + this.origin.y, this.rotation.s.z * v.x + this.rotation.f.z * v.y + this.rotation.u.z * v.z + this.origin.z, -1f);
		}

		// Token: 0x060002BD RID: 701 RVA: 0x000084F8 File Offset: 0x000066F8
		public Vec3 TransformToLocal(Vec3 v)
		{
			Vec3 vec = v - this.origin;
			return new Vec3(this.rotation.s.x * vec.x + this.rotation.s.y * vec.y + this.rotation.s.z * vec.z, this.rotation.f.x * vec.x + this.rotation.f.y * vec.y + this.rotation.f.z * vec.z, this.rotation.u.x * vec.x + this.rotation.u.y * vec.y + this.rotation.u.z * vec.z, -1f);
		}

		// Token: 0x060002BE RID: 702 RVA: 0x000085F1 File Offset: 0x000067F1
		public bool NearlyEquals(MatrixFrame rhs, float epsilon = 1E-05f)
		{
			return this.rotation.NearlyEquals(rhs.rotation, epsilon) && this.origin.NearlyEquals(rhs.origin, epsilon);
		}

		// Token: 0x060002BF RID: 703 RVA: 0x0000861C File Offset: 0x0000681C
		public Vec3 TransformToLocalNonOrthogonal(Vec3 v)
		{
			MatrixFrame matrixFrame = new MatrixFrame(this.rotation.s.x, this.rotation.s.y, this.rotation.s.z, 0f, this.rotation.f.x, this.rotation.f.y, this.rotation.f.z, 0f, this.rotation.u.x, this.rotation.u.y, this.rotation.u.z, 0f, this.origin.x, this.origin.y, this.origin.z, 1f);
			return matrixFrame.Inverse().TransformToParent(v);
		}

		// Token: 0x060002C0 RID: 704 RVA: 0x00008708 File Offset: 0x00006908
		public MatrixFrame TransformToLocalNonOrthogonal(ref MatrixFrame frame)
		{
			MatrixFrame matrixFrame = new MatrixFrame(this.rotation.s.x, this.rotation.s.y, this.rotation.s.z, 0f, this.rotation.f.x, this.rotation.f.y, this.rotation.f.z, 0f, this.rotation.u.x, this.rotation.u.y, this.rotation.u.z, 0f, this.origin.x, this.origin.y, this.origin.z, 1f);
			return matrixFrame.Inverse().TransformToParent(frame);
		}

		// Token: 0x060002C1 RID: 705 RVA: 0x000087F8 File Offset: 0x000069F8
		public static MatrixFrame Lerp(MatrixFrame m1, MatrixFrame m2, float alpha)
		{
			MatrixFrame matrixFrame;
			matrixFrame.rotation = Mat3.Lerp(m1.rotation, m2.rotation, alpha);
			matrixFrame.origin = Vec3.Lerp(m1.origin, m2.origin, alpha);
			return matrixFrame;
		}

		// Token: 0x060002C2 RID: 706 RVA: 0x00008838 File Offset: 0x00006A38
		public static MatrixFrame Slerp(MatrixFrame m1, MatrixFrame m2, float alpha)
		{
			MatrixFrame matrixFrame;
			matrixFrame.origin = Vec3.Lerp(m1.origin, m2.origin, alpha);
			matrixFrame.rotation = Quaternion.Slerp(Quaternion.QuaternionFromMat3(m1.rotation), Quaternion.QuaternionFromMat3(m2.rotation), alpha).ToMat3;
			return matrixFrame;
		}

		// Token: 0x060002C3 RID: 707 RVA: 0x0000888A File Offset: 0x00006A8A
		public MatrixFrame TransformToParent(MatrixFrame m)
		{
			return new MatrixFrame(this.rotation.TransformToParent(m.rotation), this.TransformToParent(m.origin));
		}

		// Token: 0x060002C4 RID: 708 RVA: 0x000088AE File Offset: 0x00006AAE
		public MatrixFrame TransformToLocal(MatrixFrame m)
		{
			return new MatrixFrame(this.rotation.TransformToLocal(m.rotation), this.TransformToLocal(m.origin));
		}

		// Token: 0x060002C5 RID: 709 RVA: 0x000088D4 File Offset: 0x00006AD4
		public Vec3 TransformToParentWithW(Vec3 _s)
		{
			return new Vec3(this.rotation.s.x * _s.x + this.rotation.f.x * _s.y + this.rotation.u.x * _s.z + this.origin.x * _s.w, this.rotation.s.y * _s.x + this.rotation.f.y * _s.y + this.rotation.u.y * _s.z + this.origin.y * _s.w, this.rotation.s.z * _s.x + this.rotation.f.z * _s.y + this.rotation.u.z * _s.z + this.origin.z * _s.w, this.rotation.s.w * _s.x + this.rotation.f.w * _s.y + this.rotation.u.w * _s.z + this.origin.w * _s.w);
		}

		// Token: 0x060002C6 RID: 710 RVA: 0x00008A4E File Offset: 0x00006C4E
		public MatrixFrame GetUnitRotFrame(float removedScale)
		{
			return new MatrixFrame(this.rotation.GetUnitRotation(removedScale), this.origin);
		}

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x060002C7 RID: 711 RVA: 0x00008A67 File Offset: 0x00006C67
		public static MatrixFrame Identity
		{
			get
			{
				return new MatrixFrame(Mat3.Identity, new Vec3(0f, 0f, 0f, 1f));
			}
		}

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x060002C8 RID: 712 RVA: 0x00008A8C File Offset: 0x00006C8C
		public static MatrixFrame Zero
		{
			get
			{
				return new MatrixFrame(new Mat3(Vec3.Zero, Vec3.Zero, Vec3.Zero), new Vec3(0f, 0f, 0f, 1f));
			}
		}

		// Token: 0x060002C9 RID: 713 RVA: 0x00008AC0 File Offset: 0x00006CC0
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

		// Token: 0x060002CA RID: 714 RVA: 0x000090B4 File Offset: 0x000072B4
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

		// Token: 0x060002CB RID: 715 RVA: 0x00009100 File Offset: 0x00007300
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

		// Token: 0x060002CC RID: 716 RVA: 0x000092F4 File Offset: 0x000074F4
		public static MatrixFrame operator *(MatrixFrame m1, MatrixFrame m2)
		{
			return m1.TransformToParent(m2);
		}

		// Token: 0x060002CD RID: 717 RVA: 0x000092FE File Offset: 0x000074FE
		public static bool operator ==(MatrixFrame m1, MatrixFrame m2)
		{
			return m1.origin == m2.origin && m1.rotation == m2.rotation;
		}

		// Token: 0x060002CE RID: 718 RVA: 0x00009326 File Offset: 0x00007526
		public static bool operator !=(MatrixFrame m1, MatrixFrame m2)
		{
			return m1.origin != m2.origin || m1.rotation != m2.rotation;
		}

		// Token: 0x060002CF RID: 719 RVA: 0x00009350 File Offset: 0x00007550
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

		// Token: 0x060002D0 RID: 720 RVA: 0x000093F1 File Offset: 0x000075F1
		public override bool Equals(object obj)
		{
			return this == (MatrixFrame)obj;
		}

		// Token: 0x060002D1 RID: 721 RVA: 0x00009404 File Offset: 0x00007604
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		// Token: 0x060002D2 RID: 722 RVA: 0x00009416 File Offset: 0x00007616
		public MatrixFrame Strafe(float a)
		{
			this.origin += this.rotation.s * a;
			return this;
		}

		// Token: 0x060002D3 RID: 723 RVA: 0x00009440 File Offset: 0x00007640
		public MatrixFrame Advance(float a)
		{
			this.origin += this.rotation.f * a;
			return this;
		}

		// Token: 0x060002D4 RID: 724 RVA: 0x0000946A File Offset: 0x0000766A
		public MatrixFrame Elevate(float a)
		{
			this.origin += this.rotation.u * a;
			return this;
		}

		// Token: 0x060002D5 RID: 725 RVA: 0x00009494 File Offset: 0x00007694
		public void Scale(Vec3 scalingVector)
		{
			MatrixFrame identity = MatrixFrame.Identity;
			identity.rotation.s.x = scalingVector.x;
			identity.rotation.f.y = scalingVector.y;
			identity.rotation.u.z = scalingVector.z;
			this.origin = this.TransformToParent(identity.origin);
			this.rotation = this.rotation.TransformToParent(identity.rotation);
		}

		// Token: 0x060002D6 RID: 726 RVA: 0x00009515 File Offset: 0x00007715
		public Vec3 GetScale()
		{
			return new Vec3(this.rotation.s.Length, this.rotation.f.Length, this.rotation.u.Length, -1f);
		}

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x060002D7 RID: 727 RVA: 0x00009551 File Offset: 0x00007751
		public bool IsIdentity
		{
			get
			{
				return !this.origin.IsNonZero && this.rotation.IsIdentity();
			}
		}

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x060002D8 RID: 728 RVA: 0x0000956D File Offset: 0x0000776D
		public bool IsZero
		{
			get
			{
				return !this.origin.IsNonZero && this.rotation.IsZero();
			}
		}

		// Token: 0x17000044 RID: 68
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

		// Token: 0x060002DB RID: 731 RVA: 0x00009680 File Offset: 0x00007880
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

		// Token: 0x060002DC RID: 732 RVA: 0x00009758 File Offset: 0x00007958
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

		// Token: 0x060002DD RID: 733 RVA: 0x00009850 File Offset: 0x00007A50
		public void Fill()
		{
			this.rotation.s.w = 0f;
			this.rotation.f.w = 0f;
			this.rotation.u.w = 0f;
			this.origin.w = 1f;
		}

		// Token: 0x060002DE RID: 734 RVA: 0x000098AC File Offset: 0x00007AAC
		private void AssertFilled()
		{
		}

		// Token: 0x040000FA RID: 250
		public Mat3 rotation;

		// Token: 0x040000FB RID: 251
		public Vec3 origin;
	}
}
