using System;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x02000097 RID: 151
	[EngineStruct("rglWorld_position::Plain_world_position")]
	public struct WorldPosition
	{
		// Token: 0x06000B70 RID: 2928 RVA: 0x0000C9F4 File Offset: 0x0000ABF4
		internal WorldPosition(UIntPtr scenePointer, Vec3 position)
		{
			this = new WorldPosition(scenePointer, UIntPtr.Zero, position, false);
		}

		// Token: 0x06000B71 RID: 2929 RVA: 0x0000CA04 File Offset: 0x0000AC04
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

		// Token: 0x06000B72 RID: 2930 RVA: 0x0000CA6C File Offset: 0x0000AC6C
		public WorldPosition(Scene scene, Vec3 position)
		{
			this = new WorldPosition((scene != null) ? scene.Pointer : UIntPtr.Zero, UIntPtr.Zero, position, false);
		}

		// Token: 0x06000B73 RID: 2931 RVA: 0x0000CA91 File Offset: 0x0000AC91
		public WorldPosition(Scene scene, UIntPtr navMesh, Vec3 position, bool hasValidZ)
		{
			this = new WorldPosition((scene != null) ? scene.Pointer : UIntPtr.Zero, navMesh, position, hasValidZ);
		}

		// Token: 0x06000B74 RID: 2932 RVA: 0x0000CAB4 File Offset: 0x0000ACB4
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

		// Token: 0x1700008E RID: 142
		// (get) Token: 0x06000B75 RID: 2933 RVA: 0x0000CB14 File Offset: 0x0000AD14
		public Vec2 AsVec2
		{
			get
			{
				return this._position.AsVec2;
			}
		}

		// Token: 0x1700008F RID: 143
		// (get) Token: 0x06000B76 RID: 2934 RVA: 0x0000CB21 File Offset: 0x0000AD21
		public float X
		{
			get
			{
				return this._position.x;
			}
		}

		// Token: 0x17000090 RID: 144
		// (get) Token: 0x06000B77 RID: 2935 RVA: 0x0000CB2E File Offset: 0x0000AD2E
		public float Y
		{
			get
			{
				return this._position.y;
			}
		}

		// Token: 0x06000B78 RID: 2936 RVA: 0x0000CB3B File Offset: 0x0000AD3B
		private void ValidateZ(ZValidityState minimumValidityState)
		{
			if (this.State < minimumValidityState)
			{
				EngineApplicationInterface.IScene.WorldPositionValidateZ(ref this, (int)minimumValidityState);
			}
		}

		// Token: 0x06000B79 RID: 2937 RVA: 0x0000CB54 File Offset: 0x0000AD54
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

		// Token: 0x06000B7A RID: 2938 RVA: 0x0000CBA4 File Offset: 0x0000ADA4
		public UIntPtr GetNavMesh()
		{
			this.ValidateZ(ZValidityState.ValidAccordingToNavMesh);
			return this._navMesh;
		}

		// Token: 0x06000B7B RID: 2939 RVA: 0x0000CBB3 File Offset: 0x0000ADB3
		public UIntPtr GetNearestNavMesh()
		{
			EngineApplicationInterface.IScene.WorldPositionComputeNearestNavMesh(ref this);
			return this._nearestNavMesh;
		}

		// Token: 0x06000B7C RID: 2940 RVA: 0x0000CBC6 File Offset: 0x0000ADC6
		public float GetNavMeshZ()
		{
			this.ValidateZ(ZValidityState.ValidAccordingToNavMesh);
			if (this.State >= ZValidityState.ValidAccordingToNavMesh)
			{
				return this._position.z;
			}
			return float.NaN;
		}

		// Token: 0x06000B7D RID: 2941 RVA: 0x0000CBE9 File Offset: 0x0000ADE9
		public float GetNavMeshZMT()
		{
			this.ValidateZMT(ZValidityState.ValidAccordingToNavMesh);
			if (this.State >= ZValidityState.ValidAccordingToNavMesh)
			{
				return this._position.z;
			}
			return float.NaN;
		}

		// Token: 0x06000B7E RID: 2942 RVA: 0x0000CC0C File Offset: 0x0000AE0C
		public float GetGroundZ()
		{
			this.ValidateZ(ZValidityState.Valid);
			if (this.State >= ZValidityState.Valid)
			{
				return this._position.z;
			}
			return float.NaN;
		}

		// Token: 0x06000B7F RID: 2943 RVA: 0x0000CC2F File Offset: 0x0000AE2F
		public float GetGroundZMT()
		{
			this.ValidateZMT(ZValidityState.Valid);
			if (this.State >= ZValidityState.Valid)
			{
				return this._position.z;
			}
			return float.NaN;
		}

		// Token: 0x06000B80 RID: 2944 RVA: 0x0000CC52 File Offset: 0x0000AE52
		public Vec3 GetNavMeshVec3()
		{
			return new Vec3(this._position.AsVec2, this.GetNavMeshZ(), -1f);
		}

		// Token: 0x06000B81 RID: 2945 RVA: 0x0000CC6F File Offset: 0x0000AE6F
		public Vec3 GetNavMeshVec3MT()
		{
			return new Vec3(this._position.AsVec2, this.GetNavMeshZMT(), -1f);
		}

		// Token: 0x06000B82 RID: 2946 RVA: 0x0000CC8C File Offset: 0x0000AE8C
		public Vec3 GetGroundVec3()
		{
			return new Vec3(this._position.AsVec2, this.GetGroundZ(), -1f);
		}

		// Token: 0x06000B83 RID: 2947 RVA: 0x0000CCA9 File Offset: 0x0000AEA9
		public Vec3 GetGroundVec3MT()
		{
			return new Vec3(this._position.AsVec2, this.GetGroundZMT(), -1f);
		}

		// Token: 0x06000B84 RID: 2948 RVA: 0x0000CCC8 File Offset: 0x0000AEC8
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

		// Token: 0x17000091 RID: 145
		// (get) Token: 0x06000B85 RID: 2949 RVA: 0x0000CD38 File Offset: 0x0000AF38
		public bool IsValid
		{
			get
			{
				return this.AsVec2.IsValid && this._scene != UIntPtr.Zero;
			}
		}

		// Token: 0x06000B86 RID: 2950 RVA: 0x0000CD68 File Offset: 0x0000AF68
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

		// Token: 0x040001EA RID: 490
		private readonly UIntPtr _scene;

		// Token: 0x040001EB RID: 491
		private UIntPtr _navMesh;

		// Token: 0x040001EC RID: 492
		private UIntPtr _nearestNavMesh;

		// Token: 0x040001ED RID: 493
		private Vec3 _position;

		// Token: 0x040001EE RID: 494
		public Vec3 Normal;

		// Token: 0x040001EF RID: 495
		private Vec2 _lastValidZPosition;

		// Token: 0x040001F0 RID: 496
		public ZValidityState State;

		// Token: 0x040001F1 RID: 497
		public static readonly WorldPosition Invalid = new WorldPosition(UIntPtr.Zero, UIntPtr.Zero, Vec3.Invalid, false);

		// Token: 0x020000CC RID: 204
		public enum WorldPositionEnforcedCache
		{
			// Token: 0x0400045D RID: 1117
			None,
			// Token: 0x0400045E RID: 1118
			NavMeshVec3,
			// Token: 0x0400045F RID: 1119
			GroundVec3
		}
	}
}
