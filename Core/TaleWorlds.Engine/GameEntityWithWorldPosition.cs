using System;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x0200004A RID: 74
	public class GameEntityWithWorldPosition
	{
		// Token: 0x0600069D RID: 1693 RVA: 0x00004B3C File Offset: 0x00002D3C
		public GameEntityWithWorldPosition(GameEntity gameEntity)
		{
			this._gameEntity = new WeakNativeObjectReference(gameEntity);
			Scene scene = gameEntity.Scene;
			float groundHeightAtPosition = scene.GetGroundHeightAtPosition(gameEntity.GlobalPosition, BodyFlags.CommonCollisionExcludeFlags);
			this._worldPosition = new WorldPosition(scene, UIntPtr.Zero, new Vec3(gameEntity.GlobalPosition.AsVec2, groundHeightAtPosition, -1f), false);
			this._worldPosition.GetGroundVec3();
			this._orthonormalRotation = gameEntity.GetGlobalFrame().rotation;
			this._orthonormalRotation.Orthonormalize();
		}

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x0600069E RID: 1694 RVA: 0x00004BC7 File Offset: 0x00002DC7
		public GameEntity GameEntity
		{
			get
			{
				WeakNativeObjectReference gameEntity = this._gameEntity;
				return ((gameEntity != null) ? gameEntity.GetNativeObject() : null) as GameEntity;
			}
		}

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x0600069F RID: 1695 RVA: 0x00004BE0 File Offset: 0x00002DE0
		public WorldPosition WorldPosition
		{
			get
			{
				Vec3 origin = this.GameEntity.GetGlobalFrame().origin;
				if (!this._worldPosition.AsVec2.NearlyEquals(origin.AsVec2, 1E-05f))
				{
					this._worldPosition.SetVec3(UIntPtr.Zero, origin, false);
				}
				return this._worldPosition;
			}
		}

		// Token: 0x060006A0 RID: 1696 RVA: 0x00004C37 File Offset: 0x00002E37
		public void InvalidateWorldPosition()
		{
			this._worldPosition.State = ZValidityState.Invalid;
		}

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x060006A1 RID: 1697 RVA: 0x00004C48 File Offset: 0x00002E48
		public WorldFrame WorldFrame
		{
			get
			{
				Mat3 rotation = this.GameEntity.GetGlobalFrame().rotation;
				if (!rotation.NearlyEquals(this._orthonormalRotation, 1E-05f))
				{
					this._orthonormalRotation = rotation;
					this._orthonormalRotation.Orthonormalize();
				}
				return new WorldFrame(this._orthonormalRotation, this.WorldPosition);
			}
		}

		// Token: 0x0400009A RID: 154
		private readonly WeakNativeObjectReference _gameEntity;

		// Token: 0x0400009B RID: 155
		private WorldPosition _worldPosition;

		// Token: 0x0400009C RID: 156
		private Mat3 _orthonormalRotation;
	}
}
