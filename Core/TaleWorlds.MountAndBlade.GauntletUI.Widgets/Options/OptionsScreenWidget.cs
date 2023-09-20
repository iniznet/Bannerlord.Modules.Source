using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Options
{
	// Token: 0x0200006C RID: 108
	public class OptionsScreenWidget : Widget
	{
		// Token: 0x1700020A RID: 522
		// (get) Token: 0x060005D5 RID: 1493 RVA: 0x0001165C File Offset: 0x0000F85C
		// (set) Token: 0x060005D6 RID: 1494 RVA: 0x00011664 File Offset: 0x0000F864
		public Widget VideoMemoryUsageWidget { get; set; }

		// Token: 0x1700020B RID: 523
		// (get) Token: 0x060005D7 RID: 1495 RVA: 0x0001166D File Offset: 0x0000F86D
		// (set) Token: 0x060005D8 RID: 1496 RVA: 0x00011675 File Offset: 0x0000F875
		public RichTextWidget CurrentOptionDescriptionWidget { get; set; }

		// Token: 0x1700020C RID: 524
		// (get) Token: 0x060005D9 RID: 1497 RVA: 0x0001167E File Offset: 0x0000F87E
		// (set) Token: 0x060005DA RID: 1498 RVA: 0x00011686 File Offset: 0x0000F886
		public RichTextWidget CurrentOptionNameWidget { get; set; }

		// Token: 0x1700020D RID: 525
		// (get) Token: 0x060005DB RID: 1499 RVA: 0x0001168F File Offset: 0x0000F88F
		// (set) Token: 0x060005DC RID: 1500 RVA: 0x00011697 File Offset: 0x0000F897
		public Widget CurrentOptionImageWidget { get; set; }

		// Token: 0x1700020E RID: 526
		// (get) Token: 0x060005DD RID: 1501 RVA: 0x000116A0 File Offset: 0x0000F8A0
		// (set) Token: 0x060005DE RID: 1502 RVA: 0x000116A8 File Offset: 0x0000F8A8
		public TabToggleWidget PerformanceTabToggle { get; set; }

		// Token: 0x060005DF RID: 1503 RVA: 0x000116B1 File Offset: 0x0000F8B1
		public OptionsScreenWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060005E0 RID: 1504 RVA: 0x000116BA File Offset: 0x0000F8BA
		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			if (!this._initialized)
			{
				this.PerformanceTabToggle.TabControlWidget.OnActiveTabChange += this.OnActiveTabChange;
				this.VideoMemoryUsageWidget.IsVisible = false;
				this._initialized = true;
			}
		}

		// Token: 0x060005E1 RID: 1505 RVA: 0x000116FA File Offset: 0x0000F8FA
		private void OnActiveTabChange()
		{
			this.VideoMemoryUsageWidget.IsVisible = this.PerformanceTabToggle.TabControlWidget.ActiveTab.Id == "PerformanceOptionsPage";
		}

		// Token: 0x060005E2 RID: 1506 RVA: 0x00011726 File Offset: 0x0000F926
		protected override void OnDisconnectedFromRoot()
		{
			base.OnDisconnectedFromRoot();
			TabToggleWidget performanceTabToggle = this.PerformanceTabToggle;
			if (((performanceTabToggle != null) ? performanceTabToggle.TabControlWidget : null) != null)
			{
				this.PerformanceTabToggle.TabControlWidget.OnActiveTabChange += this.OnActiveTabChange;
			}
		}

		// Token: 0x060005E3 RID: 1507 RVA: 0x00011760 File Offset: 0x0000F960
		public void SetCurrentOption(Widget currentOptionWidget, Sprite newgraphicsSprite)
		{
			if (this._currentOptionWidget != currentOptionWidget)
			{
				this._currentOptionWidget = currentOptionWidget;
				string text = "";
				string text2 = "";
				if (this._currentOptionWidget != null)
				{
					OptionsItemWidget optionsItemWidget;
					OptionsKeyItemListPanel optionsKeyItemListPanel;
					if ((optionsItemWidget = this._currentOptionWidget as OptionsItemWidget) != null)
					{
						text = optionsItemWidget.OptionDescription;
						text2 = optionsItemWidget.OptionTitle;
					}
					else if ((optionsKeyItemListPanel = this._currentOptionWidget as OptionsKeyItemListPanel) != null)
					{
						text = optionsKeyItemListPanel.OptionDescription;
						text2 = optionsKeyItemListPanel.OptionTitle;
					}
				}
				if (this.CurrentOptionDescriptionWidget != null)
				{
					this.CurrentOptionDescriptionWidget.Text = text;
				}
				if (this.CurrentOptionDescriptionWidget != null)
				{
					this.CurrentOptionNameWidget.Text = text2;
				}
			}
			if (this.CurrentOptionImageWidget != null && this.CurrentOptionImageWidget.Sprite != newgraphicsSprite)
			{
				this.CurrentOptionImageWidget.Sprite = newgraphicsSprite;
				if (newgraphicsSprite != null)
				{
					float num = this.CurrentOptionImageWidget.SuggestedWidth / (float)newgraphicsSprite.Width;
					this.CurrentOptionImageWidget.SuggestedHeight = (float)newgraphicsSprite.Height * num;
				}
			}
		}

		// Token: 0x04000283 RID: 643
		private Widget _currentOptionWidget;

		// Token: 0x04000289 RID: 649
		private bool _initialized;
	}
}
