using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Perks
{
	// Token: 0x0200008C RID: 140
	public class MultiplayerPerkItemToggleWidget : ToggleButtonWidget
	{
		// Token: 0x06000764 RID: 1892 RVA: 0x00015DA4 File Offset: 0x00013FA4
		public MultiplayerPerkItemToggleWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000765 RID: 1893 RVA: 0x00015DAD File Offset: 0x00013FAD
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

		// Token: 0x06000766 RID: 1894 RVA: 0x00015DD4 File Offset: 0x00013FD4
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

		// Token: 0x1700029B RID: 667
		// (get) Token: 0x06000767 RID: 1895 RVA: 0x00015EA0 File Offset: 0x000140A0
		// (set) Token: 0x06000768 RID: 1896 RVA: 0x00015EA8 File Offset: 0x000140A8
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

		// Token: 0x1700029C RID: 668
		// (get) Token: 0x06000769 RID: 1897 RVA: 0x00015ED1 File Offset: 0x000140D1
		// (set) Token: 0x0600076A RID: 1898 RVA: 0x00015ED9 File Offset: 0x000140D9
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

		// Token: 0x1700029D RID: 669
		// (get) Token: 0x0600076B RID: 1899 RVA: 0x00015EFD File Offset: 0x000140FD
		// (set) Token: 0x0600076C RID: 1900 RVA: 0x00015F05 File Offset: 0x00014105
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

		// Token: 0x1700029E RID: 670
		// (get) Token: 0x0600076D RID: 1901 RVA: 0x00015F23 File Offset: 0x00014123
		// (set) Token: 0x0600076E RID: 1902 RVA: 0x00015F2B File Offset: 0x0001412B
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

		// Token: 0x04000355 RID: 853
		private string _iconType;

		// Token: 0x04000356 RID: 854
		private BrushWidget _iconWidget;

		// Token: 0x04000357 RID: 855
		private bool _isSelectable;

		// Token: 0x04000358 RID: 856
		private MultiplayerPerkContainerPanelWidget _containerPanel;
	}
}
