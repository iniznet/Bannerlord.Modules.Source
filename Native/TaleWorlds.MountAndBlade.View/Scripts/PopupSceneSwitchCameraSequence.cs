using System;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade.View.Scripts
{
	public class PopupSceneSwitchCameraSequence : PopupSceneSequence
	{
		protected override void OnInit()
		{
			this._switchEntity = base.GameEntity.Scene.GetFirstEntityWithName(this.EntityName);
		}

		public override void OnInitialState()
		{
			if (this._switchEntity != null)
			{
				GameEntity gameEntity = base.GameEntity.Scene.FindEntityWithTag("customcamera");
				if (gameEntity != null)
				{
					gameEntity.RemoveTag("customcamera");
				}
				this._switchEntity.AddTag("customcamera");
			}
		}

		public override void OnPositiveState()
		{
		}

		public override void OnNegativeState()
		{
		}

		public string EntityName = "";

		private GameEntity _switchEntity;
	}
}
