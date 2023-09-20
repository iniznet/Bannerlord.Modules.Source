using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000341 RID: 833
	public class PathTracker
	{
		// Token: 0x170007F6 RID: 2038
		// (get) Token: 0x06002C7A RID: 11386 RVA: 0x000ACBB0 File Offset: 0x000AADB0
		// (set) Token: 0x06002C7B RID: 11387 RVA: 0x000ACBB8 File Offset: 0x000AADB8
		public float TotalDistanceTraveled { get; set; }

		// Token: 0x170007F7 RID: 2039
		// (get) Token: 0x06002C7C RID: 11388 RVA: 0x000ACBC1 File Offset: 0x000AADC1
		public bool HasChanged
		{
			get
			{
				return this._path != null && this._version < this._path.GetVersion();
			}
		}

		// Token: 0x170007F8 RID: 2040
		// (get) Token: 0x06002C7D RID: 11389 RVA: 0x000ACBE6 File Offset: 0x000AADE6
		public bool IsValid
		{
			get
			{
				return this._path != null;
			}
		}

		// Token: 0x170007F9 RID: 2041
		// (get) Token: 0x06002C7E RID: 11390 RVA: 0x000ACBF4 File Offset: 0x000AADF4
		public bool HasReachedEnd
		{
			get
			{
				return this.TotalDistanceTraveled >= this._path.TotalDistance;
			}
		}

		// Token: 0x170007FA RID: 2042
		// (get) Token: 0x06002C7F RID: 11391 RVA: 0x000ACC0C File Offset: 0x000AAE0C
		public float PathTraveledPercentage
		{
			get
			{
				return this.TotalDistanceTraveled / this._path.TotalDistance;
			}
		}

		// Token: 0x170007FB RID: 2043
		// (get) Token: 0x06002C80 RID: 11392 RVA: 0x000ACC20 File Offset: 0x000AAE20
		public MatrixFrame CurrentFrame
		{
			get
			{
				MatrixFrame frameForDistance = this._path.GetFrameForDistance(this.TotalDistanceTraveled);
				frameForDistance.rotation.RotateAboutUp(3.1415927f);
				frameForDistance.rotation.ApplyScaleLocal(this._initialScale);
				return frameForDistance;
			}
		}

		// Token: 0x06002C81 RID: 11393 RVA: 0x000ACC63 File Offset: 0x000AAE63
		public PathTracker(Path path, Vec3 initialScaleOfEntity)
		{
			this._path = path;
			this._initialScale = initialScaleOfEntity;
			if (path != null)
			{
				this.UpdateVersion();
			}
			this.Reset();
		}

		// Token: 0x06002C82 RID: 11394 RVA: 0x000ACC95 File Offset: 0x000AAE95
		public void UpdateVersion()
		{
			this._version = this._path.GetVersion();
		}

		// Token: 0x06002C83 RID: 11395 RVA: 0x000ACCA8 File Offset: 0x000AAEA8
		public bool PathExists()
		{
			return this._path != null;
		}

		// Token: 0x06002C84 RID: 11396 RVA: 0x000ACCB6 File Offset: 0x000AAEB6
		public void Advance(float deltaDistance)
		{
			this.TotalDistanceTraveled += deltaDistance;
			this.TotalDistanceTraveled = MathF.Min(this.TotalDistanceTraveled, this._path.TotalDistance);
		}

		// Token: 0x06002C85 RID: 11397 RVA: 0x000ACCE2 File Offset: 0x000AAEE2
		public float GetPathLength()
		{
			return this._path.TotalDistance;
		}

		// Token: 0x06002C86 RID: 11398 RVA: 0x000ACCEF File Offset: 0x000AAEEF
		public void CurrentFrameAndColor(out MatrixFrame frame, out Vec3 color)
		{
			this._path.GetFrameAndColorForDistance(this.TotalDistanceTraveled, out frame, out color);
			frame.rotation.RotateAboutUp(3.1415927f);
			frame.rotation.ApplyScaleLocal(this._initialScale);
		}

		// Token: 0x06002C87 RID: 11399 RVA: 0x000ACD25 File Offset: 0x000AAF25
		public void Reset()
		{
			this.TotalDistanceTraveled = 0f;
		}

		// Token: 0x040010FA RID: 4346
		private readonly Path _path;

		// Token: 0x040010FB RID: 4347
		private readonly Vec3 _initialScale;

		// Token: 0x040010FC RID: 4348
		private int _version = -1;
	}
}
