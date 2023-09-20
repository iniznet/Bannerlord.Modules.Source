using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	// Token: 0x02000030 RID: 48
	public class RadioContainerWidget : Widget
	{
		// Token: 0x060002AD RID: 685 RVA: 0x00008E4B File Offset: 0x0000704B
		public RadioContainerWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060002AE RID: 686 RVA: 0x00008E54 File Offset: 0x00007054
		private void ContainerOnPropertyChanged(PropertyOwnerObject owner, string propertyName, int value)
		{
			if (propertyName == "IntValue")
			{
				this.SelectedIndex = this.Container.IntValue;
			}
		}

		// Token: 0x060002AF RID: 687 RVA: 0x00008E74 File Offset: 0x00007074
		private void ContainerOnEventFire(Widget owner, string eventName, object[] arguments)
		{
			if (eventName == "ItemAdd" || eventName == "ItemRemove")
			{
				this.Container.IntValue = this.SelectedIndex;
			}
		}

		// Token: 0x060002B0 RID: 688 RVA: 0x00008EA4 File Offset: 0x000070A4
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

		// Token: 0x170000F0 RID: 240
		// (get) Token: 0x060002B1 RID: 689 RVA: 0x00008F1A File Offset: 0x0000711A
		// (set) Token: 0x060002B2 RID: 690 RVA: 0x00008F22 File Offset: 0x00007122
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

		// Token: 0x170000F1 RID: 241
		// (get) Token: 0x060002B3 RID: 691 RVA: 0x00008F59 File Offset: 0x00007159
		// (set) Token: 0x060002B4 RID: 692 RVA: 0x00008F61 File Offset: 0x00007161
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

		// Token: 0x04000115 RID: 277
		private int _selectedIndex;

		// Token: 0x04000116 RID: 278
		private Container _container;
	}
}
