using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	public class RadioContainerWidget : Widget
	{
		public RadioContainerWidget(UIContext context)
			: base(context)
		{
		}

		private void ContainerOnPropertyChanged(PropertyOwnerObject owner, string propertyName, int value)
		{
			if (propertyName == "IntValue")
			{
				this.SelectedIndex = this.Container.IntValue;
			}
		}

		private void ContainerOnEventFire(Widget owner, string eventName, object[] arguments)
		{
			if (eventName == "ItemAdd" || eventName == "ItemRemove")
			{
				this.Container.IntValue = this.SelectedIndex;
			}
		}

		private void ContainerUpdated(Container newContainer)
		{
			if (this.Container != null)
			{
				this.Container.intPropertyChanged -= this.ContainerOnPropertyChanged;
				this.Container.EventFire -= this.ContainerOnEventFire;
			}
			if (newContainer != null)
			{
				newContainer.intPropertyChanged += this.ContainerOnPropertyChanged;
				newContainer.EventFire += this.ContainerOnEventFire;
				newContainer.IntValue = this.SelectedIndex;
			}
		}

		[Editor(false)]
		public int SelectedIndex
		{
			get
			{
				return this._selectedIndex;
			}
			set
			{
				if (this._selectedIndex != value)
				{
					this._selectedIndex = value;
					base.OnPropertyChanged(value, "SelectedIndex");
					if (this.Container != null)
					{
						this.Container.IntValue = this._selectedIndex;
					}
				}
			}
		}

		[Editor(false)]
		public Container Container
		{
			get
			{
				return this._container;
			}
			set
			{
				if (this._container != value)
				{
					this.ContainerUpdated(value);
					this._container = value;
					base.OnPropertyChanged<Container>(value, "Container");
				}
			}
		}

		private int _selectedIndex;

		private Container _container;
	}
}
