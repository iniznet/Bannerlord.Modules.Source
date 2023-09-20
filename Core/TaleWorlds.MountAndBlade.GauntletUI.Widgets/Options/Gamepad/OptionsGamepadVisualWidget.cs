using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Options.Gamepad
{
	// Token: 0x02000070 RID: 112
	public class OptionsGamepadVisualWidget : Widget
	{
		// Token: 0x1700021F RID: 543
		// (get) Token: 0x06000610 RID: 1552 RVA: 0x00011DEB File Offset: 0x0000FFEB
		// (set) Token: 0x06000611 RID: 1553 RVA: 0x00011DF3 File Offset: 0x0000FFF3
		public Widget ParentAreaWidget { get; set; }

		// Token: 0x17000220 RID: 544
		// (get) Token: 0x06000612 RID: 1554 RVA: 0x00011DFC File Offset: 0x0000FFFC
		private float _verticalMarginBetweenKeys
		{
			get
			{
				return 20f;
			}
		}

		// Token: 0x06000613 RID: 1555 RVA: 0x00011E03 File Offset: 0x00010003
		public OptionsGamepadVisualWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000614 RID: 1556 RVA: 0x00011E24 File Offset: 0x00010024
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (!this._initalized)
			{
				using (List<Widget>.Enumerator enumerator = base.ParentWidget.Children.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						OptionsGamepadKeyLocationWidget optionsGamepadKeyLocationWidget;
						if ((optionsGamepadKeyLocationWidget = enumerator.Current as OptionsGamepadKeyLocationWidget) != null)
						{
							this._allKeyLocations.Add(optionsGamepadKeyLocationWidget);
						}
					}
				}
				this._initalized = true;
			}
			if (this._isKeysDirty)
			{
				this._allKeyLocations.ForEach(delegate(OptionsGamepadKeyLocationWidget k)
				{
					k.SetKeyProperties(string.Empty, this.ParentAreaWidget);
				});
				foreach (Widget widget in base.Children)
				{
					OptionsGamepadOptionItemListPanel optionItem;
					if ((optionItem = widget as OptionsGamepadOptionItemListPanel) != null)
					{
						OptionsGamepadKeyLocationWidget optionsGamepadKeyLocationWidget2 = this._allKeyLocations.Find((OptionsGamepadKeyLocationWidget l) => l.KeyID == optionItem.KeyId);
						if (optionsGamepadKeyLocationWidget2 != null)
						{
							optionItem.SetKeyProperties(optionsGamepadKeyLocationWidget2, this.ParentAreaWidget);
						}
						else
						{
							optionItem.IsVisible = false;
						}
					}
				}
				this._isKeysDirty = false;
			}
		}

		// Token: 0x06000615 RID: 1557 RVA: 0x00011F5C File Offset: 0x0001015C
		private void OnActionTextChanged()
		{
			this._isKeysDirty = true;
		}

		// Token: 0x06000616 RID: 1558 RVA: 0x00011F68 File Offset: 0x00010168
		protected override void OnChildAdded(Widget child)
		{
			base.OnChildAdded(child);
			this._isKeysDirty = true;
			OptionsGamepadOptionItemListPanel optionsGamepadOptionItemListPanel;
			if ((optionsGamepadOptionItemListPanel = child as OptionsGamepadOptionItemListPanel) != null && !this._allChildKeyItems.Contains(optionsGamepadOptionItemListPanel))
			{
				this._allChildKeyItems.Add(optionsGamepadOptionItemListPanel);
				optionsGamepadOptionItemListPanel.OnActionTextChanged += this.OnActionTextChanged;
			}
		}

		// Token: 0x06000617 RID: 1559 RVA: 0x00011FBC File Offset: 0x000101BC
		protected override void OnChildRemoved(Widget child)
		{
			base.OnChildRemoved(child);
			this._isKeysDirty = true;
			OptionsGamepadOptionItemListPanel optionsGamepadOptionItemListPanel;
			if ((optionsGamepadOptionItemListPanel = child as OptionsGamepadOptionItemListPanel) != null && this._allChildKeyItems.Contains(optionsGamepadOptionItemListPanel))
			{
				this._allChildKeyItems.Remove(optionsGamepadOptionItemListPanel);
				optionsGamepadOptionItemListPanel.OnActionTextChanged -= this.OnActionTextChanged;
			}
		}

		// Token: 0x040002A1 RID: 673
		private List<OptionsGamepadKeyLocationWidget> _allKeyLocations = new List<OptionsGamepadKeyLocationWidget>();

		// Token: 0x040002A2 RID: 674
		private List<OptionsGamepadOptionItemListPanel> _allChildKeyItems = new List<OptionsGamepadOptionItemListPanel>();

		// Token: 0x040002A4 RID: 676
		private bool _initalized;

		// Token: 0x040002A5 RID: 677
		private bool _isKeysDirty;
	}
}
