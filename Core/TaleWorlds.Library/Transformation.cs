using System;

namespace TaleWorlds.Library
{
	// Token: 0x0200008B RID: 139
	[Serializable]
	public struct Transformation
	{
		// Token: 0x17000086 RID: 134
		// (get) Token: 0x060004CB RID: 1227 RVA: 0x0000F860 File Offset: 0x0000DA60
		public static Transformation Identity
		{
			get
			{
				return new Transformation(new Vec3(0f, 0f, 0f, 1f), Mat3.Identity, Vec3.One);
			}
		}

		// Token: 0x060004CC RID: 1228 RVA: 0x0000F88A File Offset: 0x0000DA8A
		public Transformation(Vec3 origin, Mat3 rotation, Vec3 scale)
		{
			this.Origin = origin;
			this.Rotation = rotation;
			this.Scale = scale;
		}

		// Token: 0x17000087 RID: 135
		// (get) Token: 0x060004CD RID: 1229 RVA: 0x0000F8A4 File Offset: 0x0000DAA4
		public MatrixFrame AsMatrixFrame
		{
			get
			{
				MatrixFrame matrixFrame = default(MatrixFrame);
				matrixFrame.origin = this.Origin;
				matrixFrame.rotation = this.Rotation;
				matrixFrame.rotation.ApplyScaleLocal(this.Scale);
				return matrixFrame;
			}
		}

		// Token: 0x060004CE RID: 1230 RVA: 0x0000F8E8 File Offset: 0x0000DAE8
		public static Transformation CreateFromMatrixFrame(MatrixFrame matrixFrame)
		{
			Mat3 rotation = matrixFrame.rotation;
			Vec3 scaleVector = matrixFrame.rotation.GetScaleVector();
			rotation.ApplyScaleLocal(new Vec3(1f / scaleVector.X, 1f / scaleVector.Y, 1f / scaleVector.Z, -1f));
			return new Transformation(matrixFrame.origin, rotation, scaleVector);
		}

		// Token: 0x060004CF RID: 1231 RVA: 0x0000F94E File Offset: 0x0000DB4E
		public bool HasNegativeScale()
		{
			return this.Scale.X * this.Scale.Y * this.Scale.Z < 0f;
		}

		// Token: 0x060004D0 RID: 1232 RVA: 0x0000F97A File Offset: 0x0000DB7A
		public static Transformation CreateFromRotation(Mat3 rotation)
		{
			return new Transformation(Vec3.Zero, rotation, Vec3.One);
		}

		// Token: 0x060004D1 RID: 1233 RVA: 0x0000F98C File Offset: 0x0000DB8C
		public Vec3 TransformToParent(Vec3 v)
		{
			return this.AsMatrixFrame.TransformToParent(v);
		}

		// Token: 0x060004D2 RID: 1234 RVA: 0x0000F9A8 File Offset: 0x0000DBA8
		public Transformation TransformToParent(Transformation t)
		{
			return Transformation.CreateFromMatrixFrame(this.AsMatrixFrame.TransformToParent(t.AsMatrixFrame));
		}

		// Token: 0x060004D3 RID: 1235 RVA: 0x0000F9D0 File Offset: 0x0000DBD0
		public Vec3 TransformToLocal(Vec3 v)
		{
			return this.AsMatrixFrame.TransformToLocal(v);
		}

		// Token: 0x060004D4 RID: 1236 RVA: 0x0000F9EC File Offset: 0x0000DBEC
		public Transformation TransformToLocal(Transformation t)
		{
			return Transformation.CreateFromMatrixFrame(this.AsMatrixFrame.TransformToLocal(t.AsMatrixFrame));
		}

		// Token: 0x060004D5 RID: 1237 RVA: 0x0000FA14 File Offset: 0x0000DC14
		public void Rotate(float radian, Vec3 axis)
		{
			Transformation transformation = this;
			transformation.Scale = Vec3.One;
			MatrixFrame asMatrixFrame = transformation.AsMatrixFrame;
			asMatrixFrame.Rotate(radian, axis);
			this.Rotation = asMatrixFrame.rotation;
			this.Origin = asMatrixFrame.origin;
		}

		// Token: 0x060004D6 RID: 1238 RVA: 0x0000FA5D File Offset: 0x0000DC5D
		public static bool operator ==(Transformation t1, Transformation t2)
		{
			return t1.Origin == t2.Origin && t1.Rotation == t2.Rotation && t1.Scale == t2.Scale;
		}

		// Token: 0x060004D7 RID: 1239 RVA: 0x0000FA98 File Offset: 0x0000DC98
		public void ApplyScale(Vec3 vec3)
		{
			this.Scale.x = this.Scale.x * vec3.x;
			this.Scale.y = this.Scale.y * vec3.y;
			this.Scale.z = this.Scale.z * vec3.z;
		}

		// Token: 0x060004D8 RID: 1240 RVA: 0x0000FAE4 File Offset: 0x0000DCE4
		public static bool operator !=(Transformation t1, Transformation t2)
		{
			return t1.Origin != t2.Origin || t1.Rotation != t2.Rotation || t1.Scale != t2.Scale;
		}

		// Token: 0x060004D9 RID: 1241 RVA: 0x0000FB1F File Offset: 0x0000DD1F
		public override bool Equals(object obj)
		{
			return this == (Transformation)obj;
		}

		// Token: 0x060004DA RID: 1242 RVA: 0x0000FB32 File Offset: 0x0000DD32
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		// Token: 0x060004DB RID: 1243 RVA: 0x0000FB44 File Offset: 0x0000DD44
		public override string ToString()
		{
			string text = "Transformation:\n";
			text = string.Concat(new object[]
			{
				text,
				"Origin: ",
				this.Origin.x,
				", ",
				this.Origin.y,
				", ",
				this.Origin.z,
				"\n"
			});
			text += "Rotation:\n";
			text += this.Rotation.ToString();
			return string.Concat(new object[]
			{
				text,
				"Scale: ",
				this.Scale.x,
				", ",
				this.Scale.y,
				", ",
				this.Scale.z,
				"\n"
			});
		}

		// Token: 0x04000174 RID: 372
		public Vec3 Origin;

		// Token: 0x04000175 RID: 373
		public Mat3 Rotation;

		// Token: 0x04000176 RID: 374
		public Vec3 Scale;
	}
}
