using System;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x02000095 RID: 149
	public struct WorldFrame
	{
		// Token: 0x06000B6B RID: 2923 RVA: 0x0000C991 File Offset: 0x0000AB91
		public WorldFrame(Mat3 rotation, WorldPosition origin)
		{
			this.Rotation = rotation;
			this.Origin = origin;
		}

		// Token: 0x1700008D RID: 141
		// (get) Token: 0x06000B6C RID: 2924 RVA: 0x0000C9A1 File Offset: 0x0000ABA1
		public bool IsValid
		{
			get
			{
				return this.Origin.IsValid;
			}
		}

		// Token: 0x06000B6D RID: 2925 RVA: 0x0000C9AE File Offset: 0x0000ABAE
		public MatrixFrame ToGroundMatrixFrame()
		{
			return new MatrixFrame(this.Rotation, this.Origin.GetGroundVec3());
		}

		// Token: 0x06000B6E RID: 2926 RVA: 0x0000C9C6 File Offset: 0x0000ABC6
		public MatrixFrame ToNavMeshMatrixFrame()
		{
			return new MatrixFrame(this.Rotation, this.Origin.GetNavMeshVec3());
		}

		// Token: 0x040001E2 RID: 482
		public Mat3 Rotation;

		// Token: 0x040001E3 RID: 483
		public WorldPosition Origin;

		// Token: 0x040001E4 RID: 484
		public static readonly WorldFrame Invalid = new WorldFrame(Mat3.Identity, WorldPosition.Invalid);
	}
}
