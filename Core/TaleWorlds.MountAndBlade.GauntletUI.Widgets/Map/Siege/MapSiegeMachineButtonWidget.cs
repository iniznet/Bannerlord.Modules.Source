using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Map.Siege
{
	// Token: 0x020000FC RID: 252
	public class MapSiegeMachineButtonWidget : ButtonWidget
	{
		// Token: 0x06000CFE RID: 3326 RVA: 0x000247C3 File Offset: 0x000229C3
		public MapSiegeMachineButtonWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000CFF RID: 3327 RVA: 0x000247E1 File Offset: 0x000229E1
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this.ColoredImageWidget != null && !this._machineSpritesUpdated)
			{
				this.SetStylesSprite(this.ColoredImageWidget, "SPGeneral\\Siege\\" + this.MachineID);
				this._machineSpritesUpdated = true;
			}
		}

		// Token: 0x06000D00 RID: 3328 RVA: 0x0002481D File Offset: 0x00022A1D
		private void SetStylesSprite(Widget widget, string spriteName)
		{
			widget.Sprite = base.Context.SpriteData.GetSprite(spriteName);
		}

		// Token: 0x170004A8 RID: 1192
		// (get) Token: 0x06000D01 RID: 3329 RVA: 0x00024836 File Offset: 0x00022A36
		// (set) Token: 0x06000D02 RID: 3330 RVA: 0x0002483E File Offset: 0x00022A3E
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

		// Token: 0x170004A9 RID: 1193
		// (get) Token: 0x06000D03 RID: 3331 RVA: 0x0002485C File Offset: 0x00022A5C
		// (set) Token: 0x06000D04 RID: 3332 RVA: 0x00024864 File Offset: 0x00022A64
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

		// Token: 0x170004AA RID: 1194
		// (get) Token: 0x06000D05 RID: 3333 RVA: 0x00024882 File Offset: 0x00022A82
		// (set) Token: 0x06000D06 RID: 3334 RVA: 0x0002488A File Offset: 0x00022A8A
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

		// Token: 0x04000606 RID: 1542
		private Vec2 _orgClipSize = new Vec2(-1f, -1f);

		// Token: 0x04000607 RID: 1543
		private bool _machineSpritesUpdated;

		// Token: 0x04000608 RID: 1544
		private Widget _coloredImageWidget;

		// Token: 0x04000609 RID: 1545
		private bool _isDeploymentTarget;

		// Token: 0x0400060A RID: 1546
		private string _machineID;
	}
}
