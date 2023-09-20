using System;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	public class GameEntityWithWorldPosition
	{
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

		public GameEntity GameEntity
		{
			get
			{
				WeakNativeObjectReference gameEntity = this._gameEntity;
				return ((gameEntity != null) ? gameEntity.GetNativeObject() : null) as GameEntity;
			}
		}

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

		public void InvalidateWorldPosition()
		{
			this._worldPosition.State = ZValidityState.Invalid;
		}

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

		private readonly WeakNativeObjectReference _gameEntity;

		private WorldPosition _worldPosition;

		private Mat3 _orthonormalRotation;
	}
}
