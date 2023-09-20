using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Information
{
	// Token: 0x02000129 RID: 297
	public class MultiSelectionElementsWidget : Widget
	{
		// Token: 0x06000F7D RID: 3965 RVA: 0x0002B400 File Offset: 0x00029600
		public MultiSelectionElementsWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000F7E RID: 3966 RVA: 0x0002B41C File Offset: 0x0002961C
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this._updateRequired)
			{
				this.UpdateElementsList();
				this._updateRequired = false;
			}
			if (base.IsRecursivelyVisible())
			{
				if (this.MaxSelectableOptionCount <= 0)
				{
					this.DoneButtonWidget.IsEnabled = true;
				}
				else
				{
					int numOfChildrenSelected = this.GetNumOfChildrenSelected();
					this.DoneButtonWidget.IsEnabled = (numOfChildrenSelected <= this.MaxSelectableOptionCount && numOfChildrenSelected > 0) || this._elementsList.Count == 0;
				}
				if (this._latestClickedWidget != null && this.MaxSelectableOptionCount == 1)
				{
					this._elementsList.ForEach(delegate(ButtonWidget e)
					{
						if (e != this._latestClickedWidget)
						{
							e.IsSelected = false;
							return;
						}
						e.IsSelected = true;
					});
				}
			}
		}

		// Token: 0x06000F7F RID: 3967 RVA: 0x0002B4BC File Offset: 0x000296BC
		protected override void OnChildAdded(Widget child)
		{
			base.OnChildAdded(child);
			if (child is ListPanel)
			{
				this._elementContainer = child as ListPanel;
				this._elementContainer.ItemAddEventHandlers.Add(new Action<Widget, Widget>(this.OnElementAdded));
			}
		}

		// Token: 0x06000F80 RID: 3968 RVA: 0x0002B4F5 File Offset: 0x000296F5
		private void OnElementAdded(Widget parentWidget, Widget addedWidget)
		{
			this._updateRequired = true;
		}

		// Token: 0x06000F81 RID: 3969 RVA: 0x0002B500 File Offset: 0x00029700
		private int GetNumOfChildrenSelected()
		{
			int num = 0;
			for (int i = 0; i < this._elementsList.Count; i++)
			{
				if (this._elementsList[i].IsSelected)
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x06000F82 RID: 3970 RVA: 0x0002B540 File Offset: 0x00029740
		private void UpdateElementsList()
		{
			this._elementsList.Clear();
			this._latestClickedWidget = null;
			for (int i = 0; i < this._elementContainer.ChildCount; i++)
			{
				ButtonWidget buttonWidget = this._elementContainer.GetChild(i).GetChild(0) as ButtonWidget;
				buttonWidget.ClickEventHandlers.Add(new Action<Widget>(this.OnElementClick));
				this._elementsList.Add(buttonWidget);
			}
		}

		// Token: 0x06000F83 RID: 3971 RVA: 0x0002B5B0 File Offset: 0x000297B0
		private void OnElementClick(Widget widget)
		{
			ButtonWidget buttonWidget;
			if ((buttonWidget = widget as ButtonWidget) != null)
			{
				this._latestClickedWidget = buttonWidget;
			}
		}

		// Token: 0x17000571 RID: 1393
		// (get) Token: 0x06000F84 RID: 3972 RVA: 0x0002B5CE File Offset: 0x000297CE
		// (set) Token: 0x06000F85 RID: 3973 RVA: 0x0002B5D6 File Offset: 0x000297D6
		[Editor(false)]
		public int MaxSelectableOptionCount
		{
			get
			{
				return this._maxSelectableOptionCount;
			}
			set
			{
				if (this._maxSelectableOptionCount != value)
				{
					this._maxSelectableOptionCount = value;
					base.OnPropertyChanged(value, "MaxSelectableOptionCount");
				}
			}
		}

		// Token: 0x17000572 RID: 1394
		// (get) Token: 0x06000F86 RID: 3974 RVA: 0x0002B5F4 File Offset: 0x000297F4
		// (set) Token: 0x06000F87 RID: 3975 RVA: 0x0002B5FC File Offset: 0x000297FC
		[Editor(false)]
		public ButtonWidget DoneButtonWidget
		{
			get
			{
				return this._doneButtonWidget;
			}
			set
			{
				if (this._doneButtonWidget != value)
				{
					this._doneButtonWidget = value;
					base.OnPropertyChanged<ButtonWidget>(value, "DoneButtonWidget");
				}
			}
		}

		// Token: 0x04000714 RID: 1812
		private bool _updateRequired;

		// Token: 0x04000715 RID: 1813
		private List<ButtonWidget> _elementsList = new List<ButtonWidget>();

		// Token: 0x04000716 RID: 1814
		private ButtonWidget _latestClickedWidget;

		// Token: 0x04000717 RID: 1815
		private ButtonWidget _doneButtonWidget;

		// Token: 0x04000718 RID: 1816
		private ListPanel _elementContainer;

		// Token: 0x04000719 RID: 1817
		private int _maxSelectableOptionCount = -100;
	}
}
