using System;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000356 RID: 854
	public class PathLastNodeFixer : UsableMissionObjectComponent
	{
		// Token: 0x06002DE8 RID: 11752 RVA: 0x000B586C File Offset: 0x000B3A6C
		protected internal override void OnEditorTick(float dt)
		{
			base.OnEditorTick(dt);
			Path pathWithName = this._scene.GetPathWithName(this.PathHolder.PathEntity);
			this.Update(pathWithName);
		}

		// Token: 0x06002DE9 RID: 11753 RVA: 0x000B589E File Offset: 0x000B3A9E
		protected internal override void OnAdded(Scene scene)
		{
			base.OnAdded(scene);
			this._scene = scene;
			this.Update();
		}

		// Token: 0x06002DEA RID: 11754 RVA: 0x000B58B4 File Offset: 0x000B3AB4
		public void Update()
		{
			Path pathWithName = this._scene.GetPathWithName(this.PathHolder.PathEntity);
			this.Update(pathWithName);
		}

		// Token: 0x06002DEB RID: 11755 RVA: 0x000B58DF File Offset: 0x000B3ADF
		private void Update(Path path)
		{
			path != null;
		}

		// Token: 0x0400123D RID: 4669
		public IPathHolder PathHolder;

		// Token: 0x0400123E RID: 4670
		private Scene _scene;
	}
}
