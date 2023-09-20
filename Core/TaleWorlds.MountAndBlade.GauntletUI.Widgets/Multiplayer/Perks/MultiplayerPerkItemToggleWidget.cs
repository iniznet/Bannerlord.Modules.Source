using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Perks
{
	public class MultiplayerPerkItemToggleWidget : ToggleButtonWidget
	{
		public MultiplayerPerkItemToggleWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnMouseReleased()
		{
			base.OnMouseReleased();
			MultiplayerPerkContainerPanelWidget containerPanel = this.ContainerPanel;
			if (containerPanel == null)
			{
				return;
			}
			containerPanel.PerkSelected(this._isSelectable ? this : null);
		}

		private void UpdateIcon()
		{
			if (string.IsNullOrEmpty(this.IconType) || this._iconWidget == null)
			{
				return;
			}
			foreach (Style style in this.IconWidget.Brush.Styles)
			{
				foreach (StyleLayer styleLayer in style.Layers)
				{
					styleLayer.Sprite = base.Context.SpriteData.GetSprite("General\\Perks\\" + this.IconType);
				}
			}
		}

		[DataSourceProperty]
		public string IconType
		{
			get
			{
				return this._iconType;
			}
			set
			{
				if (value != this._iconType)
				{
					this._iconType = value;
					base.OnPropertyChanged<string>(value, "IconType");
					this.UpdateIcon();
				}
			}
		}

		[DataSourceProperty]
		public BrushWidget IconWidget
		{
			get
			{
				return this._iconWidget;
			}
			set
			{
				if (value != this._iconWidget)
				{
					this._iconWidget = value;
					base.OnPropertyChanged<BrushWidget>(value, "IconWidget");
					this.UpdateIcon();
				}
			}
		}

		[DataSourceProperty]
		public bool IsSelectable
		{
			get
			{
				return this._isSelectable;
			}
			set
			{
				if (value != this._isSelectable)
				{
					this._isSelectable = value;
					base.OnPropertyChanged(value, "IsSelectable");
				}
			}
		}

		[DataSourceProperty]
		public MultiplayerPerkContainerPanelWidget ContainerPanel
		{
			get
			{
				return this._containerPanel;
			}
			set
			{
				if (value != this._containerPanel)
				{
					this._containerPanel = value;
					base.OnPropertyChanged<MultiplayerPerkContainerPanelWidget>(value, "ContainerPanel");
				}
			}
		}

		private string _iconType;

		private BrushWidget _iconWidget;

		private bool _isSelectable;

		private MultiplayerPerkContainerPanelWidget _containerPanel;
	}
}
