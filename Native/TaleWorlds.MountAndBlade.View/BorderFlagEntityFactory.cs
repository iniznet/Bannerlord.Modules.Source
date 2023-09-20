using System;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade.View
{
	public class BorderFlagEntityFactory : IEntityFactory
	{
		public BorderFlagEntityFactory(string prefabName)
		{
			this._prefabName = prefabName;
		}

		public GameEntity MakeEntity(params object[] paramObjects)
		{
			Scene scene = Mission.Current.Scene;
			if (this._cachedFlagEntity == null)
			{
				this._cachedFlagEntity = GameEntity.Instantiate(null, this._prefabName, false);
			}
			GameEntity gameEntity = GameEntity.CopyFrom(scene, this._cachedFlagEntity);
			gameEntity.SetMobility(1);
			Banner banner = (Banner)(paramObjects.FirstOrDefault((object o) => o is Banner) ?? Banner.CreateRandomBanner());
			Mesh firstMesh = gameEntity.GetFirstMesh();
			Material material = firstMesh.GetMaterial();
			Material tableauMaterial = material.CreateCopy();
			Action<Texture> action = delegate(Texture tex)
			{
				tableauMaterial.SetTexture(1, tex);
			};
			banner.GetTableauTextureSmall(action);
			firstMesh.SetMaterial(tableauMaterial);
			return gameEntity;
		}

		private readonly string _prefabName;

		private GameEntity _cachedFlagEntity;
	}
}
