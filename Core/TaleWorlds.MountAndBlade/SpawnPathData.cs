using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000200 RID: 512
	public struct SpawnPathData
	{
		// Token: 0x170005AC RID: 1452
		// (get) Token: 0x06001C77 RID: 7287 RVA: 0x000653ED File Offset: 0x000635ED
		public bool IsValid
		{
			get
			{
				return this.Path != null && this.Path.NumberOfPoints > 1;
			}
		}

		// Token: 0x06001C78 RID: 7288 RVA: 0x0006540D File Offset: 0x0006360D
		public SpawnPathData(Path path = null, SpawnPathOrientation orientation = SpawnPathOrientation.PathCenter, float centerRatio = 0f, bool isInverted = false)
		{
			this.Path = path;
			this.Orientation = orientation;
			this.CenterRatio = MathF.Clamp(centerRatio, 0f, 1f);
			this.IsInverted = isInverted;
		}

		// Token: 0x06001C79 RID: 7289 RVA: 0x0006543B File Offset: 0x0006363B
		public SpawnPathData Invert()
		{
			return new SpawnPathData(this.Path, this.Orientation, MathF.Max(1f - this.CenterRatio, 0f), !this.IsInverted);
		}

		// Token: 0x06001C7A RID: 7290 RVA: 0x00065470 File Offset: 0x00063670
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

		// Token: 0x06001C7B RID: 7291 RVA: 0x00065528 File Offset: 0x00063728
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

		// Token: 0x0400093F RID: 2367
		public static readonly SpawnPathData Invalid = new SpawnPathData(null, SpawnPathOrientation.PathCenter, 0f, false);

		// Token: 0x04000940 RID: 2368
		public readonly Path Path;

		// Token: 0x04000941 RID: 2369
		public readonly bool IsInverted;

		// Token: 0x04000942 RID: 2370
		public readonly float CenterRatio;

		// Token: 0x04000943 RID: 2371
		public readonly SpawnPathOrientation Orientation;
	}
}
