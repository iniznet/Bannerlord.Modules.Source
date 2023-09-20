using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public struct SpawnPathData
	{
		public bool IsValid
		{
			get
			{
				return this.Path != null && this.Path.NumberOfPoints > 1;
			}
		}

		public SpawnPathData(Path path = null, SpawnPathOrientation orientation = SpawnPathOrientation.PathCenter, float centerRatio = 0f, bool isInverted = false)
		{
			this.Path = path;
			this.Orientation = orientation;
			this.CenterRatio = MathF.Clamp(centerRatio, 0f, 1f);
			this.IsInverted = isInverted;
		}

		public SpawnPathData Invert()
		{
			return new SpawnPathData(this.Path, this.Orientation, MathF.Max(1f - this.CenterRatio, 0f), !this.IsInverted);
		}

		public MatrixFrame GetSpawnFrame(float offset = 0f)
		{
			MatrixFrame matrixFrame = MatrixFrame.Identity;
			if (this.IsValid)
			{
				float num = MathF.Clamp(this.CenterRatio + offset, 0f, 1f);
				num = (this.IsInverted ? (1f - num) : num);
				float num2 = this.Path.GetTotalLength() * num;
				bool isInverted = this.IsInverted;
				matrixFrame = this.Path.GetNearestFrameWithValidAlphaForDistance(num2, isInverted, 0.5f);
				matrixFrame.rotation.f = (this.IsInverted ? (-matrixFrame.rotation.f) : matrixFrame.rotation.f);
				matrixFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
			}
			return matrixFrame;
		}

		public void GetOrientedSpawnPathPosition(out Vec2 spawnPathPosition, out Vec2 spawnPathDirection, float pathOffset = 0f)
		{
			if (!this.IsValid)
			{
				spawnPathPosition = Vec2.Invalid;
				spawnPathDirection = Vec2.Invalid;
				return;
			}
			spawnPathPosition = this.GetSpawnFrame(pathOffset).origin.AsVec2;
			if (this.Orientation == SpawnPathOrientation.PathCenter)
			{
				Vec2 asVec = this.Invert().GetSpawnFrame(pathOffset).origin.AsVec2;
				spawnPathDirection = (asVec - spawnPathPosition).Normalized();
				return;
			}
			float num = ((pathOffset >= 0f) ? 1f : 0f) * MathF.Max(0.01f, Math.Abs(pathOffset));
			Vec2 asVec2 = this.GetSpawnFrame(num).origin.AsVec2;
			spawnPathDirection = (asVec2 - spawnPathPosition).Normalized();
		}

		public static readonly SpawnPathData Invalid = new SpawnPathData(null, SpawnPathOrientation.PathCenter, 0f, false);

		public readonly Path Path;

		public readonly bool IsInverted;

		public readonly float CenterRatio;

		public readonly SpawnPathOrientation Orientation;
	}
}
