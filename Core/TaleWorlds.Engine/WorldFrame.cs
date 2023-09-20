using System;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	public struct WorldFrame
	{
		public WorldFrame(Mat3 rotation, WorldPosition origin)
		{
			this.Rotation = rotation;
			this.Origin = origin;
		}

		public bool IsValid
		{
			get
			{
				return this.Origin.IsValid;
			}
		}

		public MatrixFrame ToGroundMatrixFrame()
		{
			return new MatrixFrame(this.Rotation, this.Origin.GetGroundVec3());
		}

		public MatrixFrame ToNavMeshMatrixFrame()
		{
			return new MatrixFrame(this.Rotation, this.Origin.GetNavMeshVec3());
		}

		public Mat3 Rotation;

		public WorldPosition Origin;

		public static readonly WorldFrame Invalid = new WorldFrame(Mat3.Identity, WorldPosition.Invalid);
	}
}
