using System;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade.View.Scripts
{
	// Token: 0x0200003A RID: 58
	public class PopupSceneSwitchCameraSequence : PopupSceneSequence
	{
		// Token: 0x060002A8 RID: 680 RVA: 0x00017DE0 File Offset: 0x00015FE0
		protected override void OnInit()
		{
			this._switchEntity = base.GameEntity.Scene.GetFirstEntityWithName(this.EntityName);
		}

		// Token: 0x060002A9 RID: 681 RVA: 0x00017E00 File Offset: 0x00016000
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

		// Token: 0x060002AA RID: 682 RVA: 0x00017E50 File Offset: 0x00016050
		public override void OnPositiveState()
		{
		}

		// Token: 0x060002AB RID: 683 RVA: 0x00017E52 File Offset: 0x00016052
		public override void OnNegativeState()
		{
		}

		// Token: 0x040001C8 RID: 456
		public string EntityName = "";

		// Token: 0x040001C9 RID: 457
		private GameEntity _switchEntity;
	}
}
