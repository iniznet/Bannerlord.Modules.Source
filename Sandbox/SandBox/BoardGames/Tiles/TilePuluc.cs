using System;
using SandBox.BoardGames.Objects;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace SandBox.BoardGames.Tiles
{
	public class TilePuluc : Tile1D
	{
		public Vec3 PosLeft { get; private set; }

		public Vec3 PosLeftMid { get; private set; }

		public Vec3 PosRight { get; private set; }

		public Vec3 PosRightMid { get; private set; }

		public TilePuluc(GameEntity entity, BoardGameDecal decal, int x)
			: base(entity, decal, x)
		{
			this.UpdateTilePosition();
		}

		public void UpdateTilePosition()
		{
			MatrixFrame globalFrame = base.Entity.GetGlobalFrame();
			MetaMesh tileMesh = base.Entity.GetFirstScriptOfType<Tile>().TileMesh;
			Vec3 vec = tileMesh.GetBoundingBox().max - tileMesh.GetBoundingBox().min;
			Mat3 mat = globalFrame.rotation.TransformToParent(tileMesh.Frame.rotation);
			Vec3 vec2 = mat.TransformToParent(new Vec3(0f, vec.y / 6f, 0f, -1f));
			Vec3 vec3 = mat.TransformToParent(new Vec3(0f, vec.y / 3f, 0f, -1f));
			Vec3 globalPosition = base.Entity.GlobalPosition;
			this.PosLeft = globalPosition + vec3;
			this.PosLeftMid = globalPosition + vec2;
			this.PosRight = globalPosition - vec3;
			this.PosRightMid = globalPosition - vec2;
		}
	}
}
