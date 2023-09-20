using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.HUD
{
	public class ServerStatusItemBrushWidget : BrushWidget
	{
		public ServerStatusItemBrushWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (!this._initialized)
			{
				this.RegisterBrushStatesOfWidget();
				this._initialized = true;
				this.OnStatusChange(this.Status);
			}
			if (Math.Abs(base.ReadOnlyBrush.GlobalAlphaFactor - this._currentAlphaTarget) > 0.001f)
			{
				this.SetGlobalAlphaRecursively(MathF.Lerp(base.ReadOnlyBrush.GlobalAlphaFactor, this._currentAlphaTarget, dt * 5f, 1E-05f));
			}
		}

		private void OnStatusChange(int value)
		{
			this.SetState(value.ToString());
			if (value == 0)
			{
				this._currentAlphaTarget = 0f;
				return;
			}
			if (value - 1 > 1)
			{
				return;
			}
			this._currentAlphaTarget = 1f;
		}

		public int Status
		{
			get
			{
				return this._status;
			}
			set
			{
				if (value != this._status)
				{
					this._status = value;
					base.OnPropertyChanged(value, "Status");
					this.OnStatusChange(value);
				}
			}
		}

		private float _currentAlphaTarget;

		private bool _initialized;

		private int _status = -1;
	}
}
