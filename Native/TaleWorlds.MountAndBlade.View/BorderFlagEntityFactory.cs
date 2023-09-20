using System;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade.View
{
	// Token: 0x02000009 RID: 9
	public class BorderFlagEntityFactory : IEntityFactory
	{
		// Token: 0x06000042 RID: 66 RVA: 0x000038F6 File Offset: 0x00001AF6
		public BorderFlagEntityFactory(string prefabName)
		{
			this._prefabName = prefabName;
		}

		// Token: 0x06000043 RID: 67 RVA: 0x00003908 File Offset: 0x00001B08
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

		// Token: 0x0400000C RID: 12
		private readonly string _prefabName;

		// Token: 0x0400000D RID: 13
		private GameEntity _cachedFlagEntity;
	}
}
