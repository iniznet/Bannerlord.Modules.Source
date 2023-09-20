using System;
using TaleWorlds.Engine;

namespace SandBox.BoardGames.Objects
{
	// Token: 0x020000C0 RID: 192
	public class Tile : ScriptComponentBehavior
	{
		// Token: 0x06000B84 RID: 2948 RVA: 0x0005C570 File Offset: 0x0005A770
		protected override void OnInit()
		{
			base.OnInit();
			base.GameEntity.RemoveMultiMesh(base.GameEntity.GetMetaMesh(0));
		}

		// Token: 0x06000B85 RID: 2949 RVA: 0x0005C590 File Offset: 0x0005A790
		public void SetVisibility(bool visible)
		{
			base.GameEntity.SetVisibilityExcludeParents(visible);
		}

		// Token: 0x06000B86 RID: 2950 RVA: 0x0005C59E File Offset: 0x0005A79E
		protected override bool MovesEntity()
		{
			return false;
		}

		// Token: 0x04000415 RID: 1045
		public MetaMesh TileMesh;
	}
}
