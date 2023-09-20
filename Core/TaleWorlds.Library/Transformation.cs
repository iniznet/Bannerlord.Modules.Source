using System;

namespace TaleWorlds.Library
{
	[Serializable]
	public struct Transformation
	{
		public static Transformation Identity
		{
			get
			{
				return new Transformation(new Vec3(0f, 0f, 0f, 1f), Mat3.Identity, Vec3.One);
			}
		}

		public Transformation(Vec3 origin, Mat3 rotation, Vec3 scale)
		{
			this.Origin = origin;
			this.Rotation = rotation;
			this.Scale = scale;
		}

		public MatrixFrame AsMatrixFrame
		{
			get
			{
				MatrixFrame matrixFrame = default(MatrixFrame);
				matrixFrame.origin = this.Origin;
				matrixFrame.rotation = this.Rotation;
				matrixFrame.rotation.ApplyScaleLocal(this.Scale);
				matrixFrame.Fill();
				return matrixFrame;
			}
		}

		public static Transformation CreateFromMatrixFrame(MatrixFrame matrixFrame)
		{
			Mat3 rotation = matrixFrame.rotation;
			Vec3 scaleVector = matrixFrame.rotation.GetScaleVector();
			rotation.ApplyScaleLocal(new Vec3(1f / scaleVector.X, 1f / scaleVector.Y, 1f / scaleVector.Z, -1f));
			return new Transformation(matrixFrame.origin, rotation, scaleVector);
		}

		public bool HasNegativeScale()
		{
			return this.Scale.X * this.Scale.Y * this.Scale.Z < 0f;
		}

		public static Transformation CreateFromRotation(Mat3 rotation)
		{
			return new Transformation(Vec3.Zero, rotation, Vec3.One);
		}

		public Vec3 TransformToParent(Vec3 v)
		{
			return this.AsMatrixFrame.TransformToParent(v);
		}

		public Transformation TransformToParent(Transformation t)
		{
			return Transformation.CreateFromMatrixFrame(this.AsMatrixFrame.TransformToParent(t.AsMatrixFrame));
		}

		public Vec3 TransformToLocal(Vec3 v)
		{
			return this.AsMatrixFrame.TransformToLocal(v);
		}

		public Transformation TransformToLocal(Transformation t)
		{
			return Transformation.CreateFromMatrixFrame(this.AsMatrixFrame.TransformToLocal(t.AsMatrixFrame));
		}

		public void Rotate(float radian, Vec3 axis)
		{
			Transformation transformation = this;
			transformation.Scale = Vec3.One;
			MatrixFrame asMatrixFrame = transformation.AsMatrixFrame;
			asMatrixFrame.Rotate(radian, axis);
			this.Rotation = asMatrixFrame.rotation;
			this.Origin = asMatrixFrame.origin;
		}

		public static bool operator ==(Transformation t1, Transformation t2)
		{
			return t1.Origin == t2.Origin && t1.Rotation == t2.Rotation && t1.Scale == t2.Scale;
		}

		public void ApplyScale(Vec3 vec3)
		{
			this.Scale.x = this.Scale.x * vec3.x;
			this.Scale.y = this.Scale.y * vec3.y;
			this.Scale.z = this.Scale.z * vec3.z;
		}

		public static bool operator !=(Transformation t1, Transformation t2)
		{
			return t1.Origin != t2.Origin || t1.Rotation != t2.Rotation || t1.Scale != t2.Scale;
		}

		public override bool Equals(object obj)
		{
			return this == (Transformation)obj;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

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

		public Vec3 Origin;

		public Mat3 Rotation;

		public Vec3 Scale;
	}
}
