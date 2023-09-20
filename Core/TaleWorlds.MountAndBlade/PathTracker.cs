using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class PathTracker
	{
		public float TotalDistanceTraveled { get; set; }

		public bool HasChanged
		{
			get
			{
				return this._path != null && this._version < this._path.GetVersion();
			}
		}

		public bool IsValid
		{
			get
			{
				return this._path != null;
			}
		}

		public bool HasReachedEnd
		{
			get
			{
				return this.TotalDistanceTraveled >= this._path.TotalDistance;
			}
		}

		public float PathTraveledPercentage
		{
			get
			{
				return this.TotalDistanceTraveled / this._path.TotalDistance;
			}
		}

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

		public void UpdateVersion()
		{
			this._version = this._path.GetVersion();
		}

		public bool PathExists()
		{
			return this._path != null;
		}

		public void Advance(float deltaDistance)
		{
			this.TotalDistanceTraveled += deltaDistance;
			this.TotalDistanceTraveled = MathF.Min(this.TotalDistanceTraveled, this._path.TotalDistance);
		}

		public float GetPathLength()
		{
			return this._path.TotalDistance;
		}

		public void CurrentFrameAndColor(out MatrixFrame frame, out Vec3 color)
		{
			this._path.GetFrameAndColorForDistance(this.TotalDistanceTraveled, out frame, out color);
			frame.rotation.RotateAboutUp(3.1415927f);
			frame.rotation.ApplyScaleLocal(this._initialScale);
		}

		public void Reset()
		{
			this.TotalDistanceTraveled = 0f;
		}

		private readonly Path _path;

		private readonly Vec3 _initialScale;

		private int _version = -1;
	}
}
