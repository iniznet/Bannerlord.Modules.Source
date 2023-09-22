using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace SandBox.View.Map
{
	public class MapTracksVisual : CampaignEntityVisualComponent
	{
		public MapTracksVisual()
		{
			this._trackEntityPool = new List<GameEntity>();
			this._parallelUpdateTrackColorsPredicate = new TWParallel.ParallelForAuxPredicate(this.ParallelUpdateTrackColors);
			this._parallelMakeTrackPoolElementsInvisiblePredicate = new TWParallel.ParallelForAuxPredicate(this.ParallelMakeTrackPoolElementsInvisible);
			this._parallelUpdateTrackPoolPositionsPredicate = new TWParallel.ParallelForAuxPredicate(this.ParallelUpdateTrackPoolPositions);
			this._parallelUpdateVisibleTracksPredicate = new TWParallel.ParallelForAuxPredicate(this.ParallelUpdateVisibleTracks);
		}

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

		protected override void OnInitialize()
		{
			base.OnInitialize();
			CampaignEvents.TrackDetectedEvent.AddNonSerializedListener(this, new Action<Track>(this.OnTrackDetected));
			CampaignEvents.TrackLostEvent.AddNonSerializedListener(this, new Action<Track>(this.OnTrackLost));
			this.InitializeObjectPoolWithDefaultCount();
		}

		private void OnTrackDetected(Track track)
		{
			this._tracksDirty = true;
		}

		private void OnTrackLost(Track track)
		{
			this._tracksDirty = true;
		}

		private void ParallelUpdateTrackColors(int startInclusive, int endExclusive)
		{
			for (int i = startInclusive; i < endExclusive; i++)
			{
				(this._trackEntityPool[i].GetComponentAtIndex(0, 7) as Decal).SetFactor1(Campaign.Current.Models.MapTrackModel.GetTrackColor(MapScreen.Instance.MapTracksCampaignBehavior.DetectedTracks[i]));
			}
		}

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

		private void ParallelUpdateTrackPoolPositions(int startInclusive, int endExclusive)
		{
			for (int i = startInclusive; i < endExclusive; i++)
			{
				Track track = MapScreen.Instance.MapTracksCampaignBehavior.DetectedTracks[i];
				MatrixFrame matrixFrame = this.CalculateTrackFrame(track);
				this._trackEntityPool[i].SetFrame(ref matrixFrame);
			}
		}

		private void ParallelUpdateVisibleTracks(int startInclusive, int endExclusive)
		{
			for (int i = startInclusive; i < endExclusive; i++)
			{
				this._trackEntityPool[i].SetVisibilityExcludeParents(true);
			}
		}

		private void ParallelMakeTrackPoolElementsInvisible(int startInclusive, int endExclusive)
		{
			for (int i = startInclusive; i < endExclusive; i++)
			{
				this._trackEntityPool[i].SetVisibilityExcludeParents(false);
			}
		}

		private void InitializeObjectPoolWithDefaultCount()
		{
			this.CreateNewTrackPoolElements(5);
			foreach (GameEntity gameEntity in this._trackEntityPool)
			{
				gameEntity.SetVisibilityExcludeParents(false);
			}
		}

		private void CreateNewTrackPoolElements(int delta)
		{
			for (int i = 0; i < delta; i++)
			{
				GameEntity gameEntity = GameEntity.Instantiate(this.MapScene, "map_track_arrow", MatrixFrame.Identity);
				gameEntity.SetVisibilityExcludeParents(true);
				this._trackEntityPool.Add(gameEntity);
			}
		}

		public override void OnVisualTick(MapScreen screen, float realDt, float dt)
		{
			if (this._tracksDirty)
			{
				this.UpdateTrackMesh();
				this._tracksDirty = false;
			}
			TWParallel.For(0, MapScreen.Instance.MapTracksCampaignBehavior.DetectedTracks.Count, this._parallelUpdateTrackColorsPredicate, 16);
		}

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

		private MatrixFrame CalculateTrackFrame(Track track)
		{
			Vec3 vec = track.Position.ToVec3(0f);
			float scale = track.Scale;
			MatrixFrame identity = MatrixFrame.Identity;
			identity.origin = vec;
			float num;
			Vec3 vec2;
			Campaign.Current.MapSceneWrapper.GetTerrainHeightAndNormal(identity.origin.AsVec2, ref num, ref vec2);
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

		private const string TrackPrefabName = "map_track_arrow";

		private const int DefaultObjectPoolCount = 5;

		private readonly List<GameEntity> _trackEntityPool;

		private SphereData _trackSphere;

		private Scene _mapScene;

		private bool _tracksDirty = true;

		private readonly TWParallel.ParallelForAuxPredicate _parallelUpdateTrackColorsPredicate;

		private readonly TWParallel.ParallelForAuxPredicate _parallelMakeTrackPoolElementsInvisiblePredicate;

		private readonly TWParallel.ParallelForAuxPredicate _parallelUpdateTrackPoolPositionsPredicate;

		private readonly TWParallel.ParallelForAuxPredicate _parallelUpdateVisibleTracksPredicate;
	}
}
