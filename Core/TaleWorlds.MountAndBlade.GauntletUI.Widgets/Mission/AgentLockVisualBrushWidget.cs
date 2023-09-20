using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission
{
	public class AgentLockVisualBrushWidget : BrushWidget
	{
		public AgentLockVisualBrushWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			base.ScaledPositionXOffset = this.Position.X - base.Size.X / 2f;
			base.ScaledPositionYOffset = this.Position.Y - base.Size.Y / 2f;
		}

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

		private Vec2 _position;

		private int _lockState = -1;
	}
}
