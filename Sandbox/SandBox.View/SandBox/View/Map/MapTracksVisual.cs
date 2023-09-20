using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace SandBox.View.Map
{
	// Token: 0x02000055 RID: 85
	public class MapTracksVisual : CampaignEntityVisualComponent
	{
		// Token: 0x06000367 RID: 871 RVA: 0x0001CF10 File Offset: 0x0001B110
		public MapTracksVisual()
		{
			this._trackEntityPool = new List<GameEntity>();
			this._parallelUpdateTrackColorsPredicate = new TWParallel.ParallelForAuxPredicate(this.ParallelUpdateTrackColors);
			this._parallelMakeTrackPoolElementsInvisiblePredicate = new TWParallel.ParallelForAuxPredicate(this.ParallelMakeTrackPoolElementsInvisible);
			this._parallelUpdateTrackPoolPositionsPredicate = new TWParallel.ParallelForAuxPredicate(this.ParallelUpdateTrackPoolPositions);
			this._parallelUpdateVisibleTracksPredicate = new TWParallel.ParallelForAuxPredicate(this.ParallelUpdateVisibleTracks);
		}

		// Token: 0x17000062 RID: 98
		// (get) Token: 0x06000368 RID: 872 RVA: 0x0001CF80 File Offset: 0x0001B180
		public Scene MapScene
		{
			get
			{
				if (this._mapScene == null && Campaign.Current != null && Campaign.Current.MapSceneWrapper != null)
				{
					this._mapScene = ((MapScene)Campaign.Current.MapSceneWrapper).Scene;
				}
				return this._mapScene;
			}
		}

		// Token: 0x06000369 RID: 873 RVA: 0x0001CFCE File Offset: 0x0001B1CE
		protected override void OnInitialize()
		{
			base.OnInitialize();
			CampaignEvents.TrackDetectedEvent.AddNonSerializedListener(this, new Action<Track>(this.OnTrackDetected));
			CampaignEvents.TrackLostEvent.AddNonSerializedListener(this, new Action<Track>(this.OnTrackLost));
			this.InitializeObjectPoolWithDefaultCount();
		}

		// Token: 0x0600036A RID: 874 RVA: 0x0001D00A File Offset: 0x0001B20A
		private void OnTrackDetected(Track track)
		{
			this._tracksDirty = true;
		}

		// Token: 0x0600036B RID: 875 RVA: 0x0001D013 File Offset: 0x0001B213
		private void OnTrackLost(Track track)
		{
			this._tracksDirty = true;
		}

		// Token: 0x0600036C RID: 876 RVA: 0x0001D01C File Offset: 0x0001B21C
		public override void OnLoadSavedGame()
		{
			this.InitializeObjectPoolWithDefaultCount();
			this.UpdateTrackMesh();
		}

		// Token: 0x0600036D RID: 877 RVA: 0x0001D02C File Offset: 0x0001B22C
		private void ParallelUpdateTrackColors(int startInclusive, int endExclusive)
		{
			for (int i = startInclusive; i < endExclusive; i++)
			{
				(this._trackEntityPool[i].GetComponentAtIndex(0, 7) as Decal).SetFactor1(Campaign.Current.Models.MapTrackModel.GetTrackColor(MapScreen.Instance.MapTracksCampaignBehavior.DetectedTracks[i]));
			}
		}

		// Token: 0x0600036E RID: 878 RVA: 0x0001D08C File Offset: 0x0001B28C
		private void UpdateTrackMesh()
		{
			int num = this._trackEntityPool.Count - MapScreen.Instance.MapTracksCampaignBehavior.DetectedTracks.Count;
			if (num > 0)
			{
				int count = MapScreen.Instance.MapTracksCampaignBehavior.DetectedTracks.Count;
				TWParallel.For(count, count + num, this._parallelMakeTrackPoolElementsInvisiblePredicate, 16);
			}
			else
			{
				this.CreateNewTrackPoolElements(-num);
			}
			TWParallel.For(0, MapScreen.Instance.MapTracksCampaignBehavior.DetectedTracks.Count, this._parallelUpdateVisibleTracksPredicate, 16);
			TWParallel.For(0, MapScreen.Instance.MapTracksCampaignBehavior.DetectedTracks.Count, this._parallelUpdateTrackPoolPositionsPredicate, 16);
		}

		// Token: 0x0600036F RID: 879 RVA: 0x0001D130 File Offset: 0x0001B330
		private void ParallelUpdateTrackPoolPositions(int startInclusive, int endExclusive)
		{
			for (int i = startInclusive; i < endExclusive; i++)
			{
				Track track = MapScreen.Instance.MapTracksCampaignBehavior.DetectedTracks[i];
				MatrixFrame matrixFrame = this.CalculateTrackFrame(track);
				this._trackEntityPool[i].SetFrame(ref matrixFrame);
			}
		}

		// Token: 0x06000370 RID: 880 RVA: 0x0001D17C File Offset: 0x0001B37C
		private void ParallelUpdateVisibleTracks(int startInclusive, int endExclusive)
		{
			for (int i = startInclusive; i < endExclusive; i++)
			{
				this._trackEntityPool[i].SetVisibilityExcludeParents(true);
			}
		}

		// Token: 0x06000371 RID: 881 RVA: 0x0001D1A8 File Offset: 0x0001B3A8
		private void ParallelMakeTrackPoolElementsInvisible(int startInclusive, int endExclusive)
		{
			for (int i = startInclusive; i < endExclusive; i++)
			{
				this._trackEntityPool[i].SetVisibilityExcludeParents(false);
			}
		}

		// Token: 0x06000372 RID: 882 RVA: 0x0001D1D4 File Offset: 0x0001B3D4
		private void InitializeObjectPoolWithDefaultCount()
		{
			this.CreateNewTrackPoolElements(5);
			foreach (GameEntity gameEntity in this._trackEntityPool)
			{
				gameEntity.SetVisibilityExcludeParents(false);
			}
		}

		// Token: 0x06000373 RID: 883 RVA: 0x0001D22C File Offset: 0x0001B42C
		private void CreateNewTrackPoolElements(int delta)
		{
			for (int i = 0; i < delta; i++)
			{
				GameEntity gameEntity = GameEntity.Instantiate(this.MapScene, "map_track_arrow", MatrixFrame.Identity);
				gameEntity.SetVisibilityExcludeParents(true);
				this._trackEntityPool.Add(gameEntity);
			}
		}

		// Token: 0x06000374 RID: 884 RVA: 0x0001D26E File Offset: 0x0001B46E
		public override void OnVisualTick(MapScreen screen, float realDt, float dt)
		{
			if (this._tracksDirty)
			{
				this.UpdateTrackMesh();
				this._tracksDirty = false;
			}
			TWParallel.For(0, MapScreen.Instance.MapTracksCampaignBehavior.DetectedTracks.Count, this._parallelUpdateTrackColorsPredicate, 16);
		}

		// Token: 0x06000375 RID: 885 RVA: 0x0001D2A8 File Offset: 0x0001B4A8
		public bool RaySphereIntersection(Ray ray, SphereData sphere, ref Vec3 intersectionPoint)
		{
			Vec3 origin = sphere.Origin;
			float radius = sphere.Radius;
			Vec3 vec = origin - ray.Origin;
			float num = Vec3.DotProduct(ray.Direction, vec);
			if (num > 0f)
			{
				Vec3 vec2 = ray.Origin + ray.Direction * num - origin;
				float num2 = radius * radius - vec2.LengthSquared;
				if (num2 >= 0f)
				{
					float num3 = MathF.Sqrt(num2);
					float num4 = num - num3;
					if (num4 >= 0f && num4 <= ray.MaxDistance)
					{
						intersectionPoint = ray.Origin + ray.Direction * num4;
						return true;
					}
					if (num4 < 0f)
					{
						intersectionPoint = ray.Origin;
						return true;
					}
				}
			}
			else if ((ray.Origin - origin).LengthSquared < radius * radius)
			{
				intersectionPoint = ray.Origin;
				return true;
			}
			return false;
		}

		// Token: 0x06000376 RID: 886 RVA: 0x0001D3AC File Offset: 0x0001B5AC
		public Track GetTrackOnMouse(Ray mouseRay, Vec3 mouseIntersectionPoint)
		{
			Track track = null;
			for (int i = 0; i < MapScreen.Instance.MapTracksCampaignBehavior.DetectedTracks.Count; i++)
			{
				Track track2 = MapScreen.Instance.MapTracksCampaignBehavior.DetectedTracks[i];
				float trackScale = Campaign.Current.Models.MapTrackModel.GetTrackScale(track2);
				MatrixFrame matrixFrame = this.CalculateTrackFrame(track2);
				float lengthSquared = (matrixFrame.origin - mouseIntersectionPoint).LengthSquared;
				if (lengthSquared < 0.1f)
				{
					float num = MathF.Sqrt(lengthSquared);
					this._trackSphere.Origin = matrixFrame.origin;
					this._trackSphere.Radius = 0.05f + num * 0.01f + trackScale;
					Vec3 vec = default(Vec3);
					if (this.RaySphereIntersection(mouseRay, this._trackSphere, ref vec))
					{
						track = track2;
					}
				}
			}
			return track;
		}

		// Token: 0x06000377 RID: 887 RVA: 0x0001D488 File Offset: 0x0001B688
		private MatrixFrame CalculateTrackFrame(Track track)
		{
			Vec3 vec = track.Position.ToVec3(0f);
			float scale = track.Scale;
			MatrixFrame identity = MatrixFrame.Identity;
			identity.origin = vec;
			float num;
			Vec3 vec2;
			Campaign.Current.MapSceneWrapper.GetTerrainHeightandNormal(identity.origin.AsVec2, ref num, ref vec2);
			identity.origin.z = num + 0.01f;
			identity.rotation.u = vec2;
			Vec2 asVec = identity.rotation.f.AsVec2;
			asVec.RotateCCW(track.Direction);
			identity.rotation.f = new Vec3(asVec.x, asVec.y, identity.rotation.f.z, -1f);
			identity.rotation.s = Vec3.CrossProduct(identity.rotation.f, identity.rotation.u);
			identity.rotation.s.Normalize();
			identity.rotation.f = Vec3.CrossProduct(identity.rotation.u, identity.rotation.s);
			identity.rotation.f.Normalize();
			float num2 = scale;
			identity.rotation.s = identity.rotation.s * num2;
			identity.rotation.f = identity.rotation.f * num2;
			identity.rotation.u = identity.rotation.u * num2;
			return identity;
		}

		// Token: 0x040001C3 RID: 451
		private const string TrackPrefabName = "map_track_arrow";

		// Token: 0x040001C4 RID: 452
		private const int DefaultObjectPoolCount = 5;

		// Token: 0x040001C5 RID: 453
		private readonly List<GameEntity> _trackEntityPool;

		// Token: 0x040001C6 RID: 454
		private SphereData _trackSphere;

		// Token: 0x040001C7 RID: 455
		private Scene _mapScene;

		// Token: 0x040001C8 RID: 456
		private bool _tracksDirty = true;

		// Token: 0x040001C9 RID: 457
		private readonly TWParallel.ParallelForAuxPredicate _parallelUpdateTrackColorsPredicate;

		// Token: 0x040001CA RID: 458
		private readonly TWParallel.ParallelForAuxPredicate _parallelMakeTrackPoolElementsInvisiblePredicate;

		// Token: 0x040001CB RID: 459
		private readonly TWParallel.ParallelForAuxPredicate _parallelUpdateTrackPoolPositionsPredicate;

		// Token: 0x040001CC RID: 460
		private readonly TWParallel.ParallelForAuxPredicate _parallelUpdateVisibleTracksPredicate;
	}
}
