using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Map.Siege
{
	public class MapSiegeMachineButtonWidget : ButtonWidget
	{
		public MapSiegeMachineButtonWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this.ColoredImageWidget != null && !this._machineSpritesUpdated)
			{
				this.SetStylesSprite(this.ColoredImageWidget, "SPGeneral\\Siege\\" + this.MachineID);
				this._machineSpritesUpdated = true;
			}
		}

		private void SetStylesSprite(Widget widget, string spriteName)
		{
			widget.Sprite = base.Context.SpriteData.GetSprite(spriteName);
		}

		[Editor(false)]
		public Widget ColoredImageWidget
		{
			get
			{
				return this._coloredImageWidget;
			}
			set
			{
				if (value != this._coloredImageWidget)
				{
					this._coloredImageWidget = value;
					base.OnPropertyChanged<Widget>(value, "ColoredImageWidget");
				}
			}
		}

		[Editor(false)]
		public bool IsDeploymentTarget
		{
			get
			{
				return this._isDeploymentTarget;
			}
			set
			{
				if (value != this._isDeploymentTarget)
				{
					this._isDeploymentTarget = value;
					base.OnPropertyChanged(value, "IsDeploymentTarget");
				}
			}
		}

		[Editor(false)]
		public string MachineID
		{
			get
			{
				return this._machineID;
			}
			set
			{
				if (value != this._machineID)
				{
					this._machineID = value;
					base.OnPropertyChanged<string>(value, "MachineID");
					this._machineSpritesUpdated = false;
				}
			}
		}

		private Vec2 _orgClipSize = new Vec2(-1f, -1f);

		private bool _machineSpritesUpdated;

		private Widget _coloredImageWidget;

		private bool _isDeploymentTarget;

		private string _machineID;
	}
}
