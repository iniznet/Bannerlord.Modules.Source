using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission
{
	// Token: 0x020000C3 RID: 195
	public class AgentLockVisualBrushWidget : BrushWidget
	{
		// Token: 0x060009DC RID: 2524 RVA: 0x0001C27C File Offset: 0x0001A47C
		public AgentLockVisualBrushWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060009DD RID: 2525 RVA: 0x0001C28C File Offset: 0x0001A48C
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			base.ScaledPositionXOffset = this.Position.X - base.Size.X / 2f;
			base.ScaledPositionYOffset = this.Position.Y - base.Size.Y / 2f;
		}

		// Token: 0x060009DE RID: 2526 RVA: 0x0001C2EC File Offset: 0x0001A4EC
		private void UpdateVisualState(int lockState)
		{
			if (lockState == 0)
			{
				this.SetState("Possible");
				return;
			}
			if (lockState != 1)
			{
				return;
			}
			this.SetState("Active");
		}

		// Token: 0x17000373 RID: 883
		// (get) Token: 0x060009DF RID: 2527 RVA: 0x0001C30D File Offset: 0x0001A50D
		// (set) Token: 0x060009E0 RID: 2528 RVA: 0x0001C315 File Offset: 0x0001A515
		[Editor(false)]
		public Vec2 Position
		{
			get
			{
				return this._position;
			}
			set
			{
				if (this._position != value)
				{
					this._position = value;
					base.OnPropertyChanged(value, "Position");
				}
			}
		}

		// Token: 0x17000374 RID: 884
		// (get) Token: 0x060009E1 RID: 2529 RVA: 0x0001C338 File Offset: 0x0001A538
		// (set) Token: 0x060009E2 RID: 2530 RVA: 0x0001C340 File Offset: 0x0001A540
		[Editor(false)]
		public int LockState
		{
			get
			{
				return this._lockState;
			}
			set
			{
				if (this._lockState != value)
				{
					this._lockState = value;
					base.OnPropertyChanged(value, "LockState");
					this.UpdateVisualState(value);
				}
			}
		}

		// Token: 0x04000482 RID: 1154
		private Vec2 _position;

		// Token: 0x04000483 RID: 1155
		private int _lockState = -1;
	}
}
