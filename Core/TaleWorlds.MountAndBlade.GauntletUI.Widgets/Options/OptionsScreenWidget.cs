using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Options
{
	public class OptionsScreenWidget : Widget
	{
		public Widget VideoMemoryUsageWidget { get; set; }

		public RichTextWidget CurrentOptionDescriptionWidget { get; set; }

		public RichTextWidget CurrentOptionNameWidget { get; set; }

		public Widget CurrentOptionImageWidget { get; set; }

		public TabToggleWidget PerformanceTabToggle { get; set; }

		public OptionsScreenWidget(UIContext context)
			: base(context)
		{
		}

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

		private void OnActiveTabChange()
		{
			this.VideoMemoryUsageWidget.IsVisible = this.PerformanceTabToggle.TabControlWidget.ActiveTab.Id == "PerformanceOptionsPage";
		}

		protected override void OnDisconnectedFromRoot()
		{
			base.OnDisconnectedFromRoot();
			TabToggleWidget performanceTabToggle = this.PerformanceTabToggle;
			if (((performanceTabToggle != null) ? performanceTabToggle.TabControlWidget : null) != null)
			{
				this.PerformanceTabToggle.TabControlWidget.OnActiveTabChange += this.OnActiveTabChange;
			}
		}

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

		private Widget _currentOptionWidget;

		private bool _initialized;
	}
}
