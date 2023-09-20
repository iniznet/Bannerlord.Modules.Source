using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Options.Gamepad
{
	public class OptionsGamepadVisualWidget : Widget
	{
		public Widget ParentAreaWidget { get; set; }

		private float _verticalMarginBetweenKeys
		{
			get
			{
				return 20f;
			}
		}

		public OptionsGamepadVisualWidget(UIContext context)
			: base(context)
		{
		}

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

		private void OnActionTextChanged()
		{
			this._isKeysDirty = true;
		}

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

		private List<OptionsGamepadKeyLocationWidget> _allKeyLocations = new List<OptionsGamepadKeyLocationWidget>();

		private List<OptionsGamepadOptionItemListPanel> _allChildKeyItems = new List<OptionsGamepadOptionItemListPanel>();

		private bool _initalized;

		private bool _isKeysDirty;
	}
}
