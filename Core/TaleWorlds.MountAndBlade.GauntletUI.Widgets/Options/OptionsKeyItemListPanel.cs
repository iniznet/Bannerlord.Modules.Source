using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Options
{
	public class OptionsKeyItemListPanel : ListPanel
	{
		public OptionsKeyItemListPanel(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this._screenWidget == null)
			{
				this._screenWidget = base.EventManager.Root.GetChild(0).FindChild("Options") as OptionsScreenWidget;
			}
			if (!this._eventsRegistered)
			{
				this.RegisterHoverEvents();
				this._eventsRegistered = true;
			}
		}

		protected override void OnHoverBegin()
		{
			base.OnHoverBegin();
			this.SetCurrentOption(false, false, -1);
		}

		protected override void OnHoverEnd()
		{
			base.OnHoverEnd();
			this.ResetCurrentOption();
		}

		private void SetCurrentOption(bool fromHoverOverDropdown, bool fromBooleanSelection, int hoverDropdownItemIndex = -1)
		{
			OptionsScreenWidget screenWidget = this._screenWidget;
			if (screenWidget == null)
			{
				return;
			}
			screenWidget.SetCurrentOption(this, null);
		}

		private void ResetCurrentOption()
		{
			OptionsScreenWidget screenWidget = this._screenWidget;
			if (screenWidget == null)
			{
				return;
			}
			screenWidget.SetCurrentOption(null, null);
		}

		private void RegisterHoverEvents()
		{
			foreach (Widget widget in base.AllChildren)
			{
				widget.boolPropertyChanged += this.Child_PropertyChanged;
			}
		}

		private void Child_PropertyChanged(PropertyOwnerObject childWidget, string propertyName, bool propertyValue)
		{
			if (propertyName == "IsHovered")
			{
				if (propertyValue)
				{
					this.SetCurrentOption(false, false, -1);
					return;
				}
				this.ResetCurrentOption();
			}
		}

		public string OptionTitle
		{
			get
			{
				return this._optionTitle;
			}
			set
			{
				if (this._optionTitle != value)
				{
					this._optionTitle = value;
				}
			}
		}

		public string OptionDescription
		{
			get
			{
				return this._optionDescription;
			}
			set
			{
				if (this._optionDescription != value)
				{
					this._optionDescription = value;
				}
			}
		}

		private OptionsScreenWidget _screenWidget;

		private bool _eventsRegistered;

		private string _optionDescription;

		private string _optionTitle;
	}
}
