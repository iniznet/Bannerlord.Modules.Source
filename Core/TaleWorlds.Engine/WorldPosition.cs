using System;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	[EngineStruct("rglWorld_position::Plain_world_position", false)]
	public struct WorldPosition
	{
		internal WorldPosition(UIntPtr scenePointer, Vec3 position)
		{
			this = new WorldPosition(scenePointer, UIntPtr.Zero, position, false);
		}

		internal WorldPosition(UIntPtr scenePointer, UIntPtr navMesh, Vec3 position, bool hasValidZ)
		{
			this._scene = scenePointer;
			this._navMesh = navMesh;
			this._nearestNavMesh = this._navMesh;
			this._position = position;
			this.Normal = Vec3.Zero;
			if (hasValidZ)
			{
				this._lastValidZPosition = this._position.AsVec2;
				this.State = ZValidityState.Valid;
				return;
			}
			this._lastValidZPosition = Vec2.Invalid;
			this.State = ZValidityState.Invalid;
		}

		public WorldPosition(Scene scene, Vec3 position)
		{
			this = new WorldPosition((scene != null) ? scene.Pointer : UIntPtr.Zero, UIntPtr.Zero, position, false);
		}

		public WorldPosition(Scene scene, UIntPtr navMesh, Vec3 position, bool hasValidZ)
		{
			this = new WorldPosition((scene != null) ? scene.Pointer : UIntPtr.Zero, navMesh, position, hasValidZ);
		}

		public void SetVec3(UIntPtr navMesh, Vec3 position, bool hasValidZ)
		{
			this._navMesh = navMesh;
			this._nearestNavMesh = this._navMesh;
			this._position = position;
			this.Normal = Vec3.Zero;
			if (hasValidZ)
			{
				this._lastValidZPosition = this._position.AsVec2;
				this.State = ZValidityState.Valid;
				return;
			}
			this._lastValidZPosition = Vec2.Invalid;
			this.State = ZValidityState.Invalid;
		}

		public Vec2 AsVec2
		{
			get
			{
				return this._position.AsVec2;
			}
		}

		public float X
		{
			get
			{
				return this._position.x;
			}
		}

		public float Y
		{
			get
			{
				return this._position.y;
			}
		}

		private void ValidateZ(ZValidityState minimumValidityState)
		{
			if (this.State < minimumValidityState)
			{
				EngineApplicationInterface.IScene.WorldPositionValidateZ(ref this, (int)minimumValidityState);
			}
		}

		private void ValidateZMT(ZValidityState minimumValidityState)
		{
			if (this.State < minimumValidityState)
			{
				using (new TWSharedMutexReadLock(Scene.PhysicsAndRayCastLock))
				{
					EngineApplicationInterface.IScene.WorldPositionValidateZ(ref this, (int)minimumValidityState);
				}
			}
		}

		public UIntPtr GetNavMesh()
		{
			this.ValidateZ(ZValidityState.ValidAccordingToNavMesh);
			return this._navMesh;
		}

		public UIntPtr GetNearestNavMesh()
		{
			EngineApplicationInterface.IScene.WorldPositionComputeNearestNavMesh(ref this);
			return this._nearestNavMesh;
		}

		public float GetNavMeshZ()
		{
			this.ValidateZ(ZValidityState.ValidAccordingToNavMesh);
			if (this.State >= ZValidityState.ValidAccordingToNavMesh)
			{
				return this._position.z;
			}
			return float.NaN;
		}

		public float GetNavMeshZMT()
		{
			this.ValidateZMT(ZValidityState.ValidAccordingToNavMesh);
			if (this.State >= ZValidityState.ValidAccordingToNavMesh)
			{
				return this._position.z;
			}
			return float.NaN;
		}

		public float GetGroundZ()
		{
			this.ValidateZ(ZValidityState.Valid);
			if (this.State >= ZValidityState.Valid)
			{
				return this._position.z;
			}
			return float.NaN;
		}

		public float GetGroundZMT()
		{
			this.ValidateZMT(ZValidityState.Valid);
			if (this.State >= ZValidityState.Valid)
			{
				return this._position.z;
			}
			return float.NaN;
		}

		public Vec3 GetNavMeshVec3()
		{
			return new Vec3(this._position.AsVec2, this.GetNavMeshZ(), -1f);
		}

		public Vec3 GetNavMeshVec3MT()
		{
			return new Vec3(this._position.AsVec2, this.GetNavMeshZMT(), -1f);
		}

		public Vec3 GetGroundVec3()
		{
			return new Vec3(this._position.AsVec2, this.GetGroundZ(), -1f);
		}

		public Vec3 GetGroundVec3MT()
		{
			return new Vec3(this._position.AsVec2, this.GetGroundZMT(), -1f);
		}

		public void SetVec2(Vec2 value)
		{
			if (this._position.AsVec2 != value)
			{
				if (this.State != ZValidityState.Invalid)
				{
					this.State = ZValidityState.Invalid;
				}
				else if (!this._lastValidZPosition.IsValid)
				{
					this.ValidateZ(ZValidityState.ValidAccordingToNavMesh);
					this.State = ZValidityState.Invalid;
				}
				this._position.x = value.x;
				this._position.y = value.y;
			}
		}

		public bool IsValid
		{
			get
			{
				return this.AsVec2.IsValid && this._scene != UIntPtr.Zero;
			}
		}

		public float DistanceSquaredWithLimit(in Vec3 targetPoint, float limitSquared)
		{
			Vec2 asVec = this._position.AsVec2;
			Vec3 vec = targetPoint;
			float num = asVec.DistanceSquared(vec.AsVec2);
			if (num <= limitSquared)
			{
				return this.GetGroundVec3().DistanceSquared(targetPoint);
			}
			return num;
		}

		private readonly UIntPtr _scene;

		private UIntPtr _navMesh;

		private UIntPtr _nearestNavMesh;

		private Vec3 _position;

		public Vec3 Normal;

		private Vec2 _lastValidZPosition;

		[CustomEngineStructMemberData("z_validity_state")]
		public ZValidityState State;

		public static readonly WorldPosition Invalid = new WorldPosition(UIntPtr.Zero, UIntPtr.Zero, Vec3.Invalid, false);

		public enum WorldPositionEnforcedCache
		{
			None,
			NavMeshVec3,
			GroundVec3
		}
	}
}
