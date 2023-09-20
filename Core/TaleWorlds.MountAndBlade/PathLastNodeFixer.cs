using System;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
	public class PathLastNodeFixer : UsableMissionObjectComponent
	{
		protected internal override void OnEditorTick(float dt)
		{
			base.OnEditorTick(dt);
			Path pathWithName = this._scene.GetPathWithName(this.PathHolder.PathEntity);
			this.Update(pathWithName);
		}

		protected internal override void OnAdded(Scene scene)
		{
			base.OnAdded(scene);
			this._scene = scene;
			this.Update();
		}

		public void Update()
		{
			Path pathWithName = this._scene.GetPathWithName(this.PathHolder.PathEntity);
			this.Update(pathWithName);
		}

		private void Update(Path path)
		{
			path != null;
		}

		public IPathHolder PathHolder;

		private Scene _scene;
	}
}
