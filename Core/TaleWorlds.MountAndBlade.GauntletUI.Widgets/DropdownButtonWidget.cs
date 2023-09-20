using System;
using System.Linq;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	public class DropdownButtonWidget : ButtonWidget
	{
		public Widget DisplayedList
		{
			get
			{
				return this._displayedList;
			}
			set
			{
				if (value != this._displayedList)
				{
					if (this._displayedList != null)
					{
						ListPanel listPanel = this._displayedList.AllChildrenAndThis.FirstOrDefault((Widget x) => x is ListPanel) as ListPanel;
						if (listPanel != null)
						{
							listPanel.SelectEventHandlers.Remove(new Action<Widget>(this.OnListItemSelected));
						}
					}
					this._displayedList = value;
					this._displayedList.IsVisible = false;
					this._isDisplayingList = false;
					ListPanel listPanel2 = this._displayedList.AllChildrenAndThis.FirstOrDefault((Widget x) => x is ListPanel) as ListPanel;
					if (listPanel2 != null)
					{
						listPanel2.SelectEventHandlers.Add(new Action<Widget>(this.OnListItemSelected));
					}
				}
			}
		}

		public DropdownButtonWidget(UIContext context)
			: base(context)
		{
		}

		private void OnListItemSelected(Widget list)
		{
			this.HideList();
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this._isDisplayingList)
			{
				this.DisplayedList.ScaledPositionXOffset = Mathf.Clamp(base.GlobalPosition.X, 0f, base.EventManager.Root.Size.X * base._inverseScaleToUse - this.DisplayedList.Size.X);
				this.DisplayedList.ScaledPositionYOffset = Mathf.Clamp(base.GlobalPosition.Y + base.Size.Y, 0f, base.EventManager.Root.Size.Y * base._inverseScaleToUse - this.DisplayedList.Size.Y);
				if (base.EventManager.LatestMouseUpWidget == null)
				{
					this.HideList();
					return;
				}
				if (base.EventManager.LatestMouseUpWidget != this && !this.DisplayedList.CheckIsMyChildRecursive(base.EventManager.LatestMouseUpWidget))
				{
					this.HideList();
				}
			}
		}

		private void DisplayList()
		{
			this.DisplayedList.ParentWidget = base.EventManager.Root;
			this.DisplayedList.IsVisible = true;
			this.DisplayedList.HorizontalAlignment = HorizontalAlignment.Left;
			this.DisplayedList.VerticalAlignment = VerticalAlignment.Top;
			this._isDisplayingList = true;
			base.DoNotUseCustomScaleAndChildren = false;
		}

		private void HideList()
		{
			this.DisplayedList.ParentWidget = this;
			this.DisplayedList.IsVisible = false;
			this.DisplayedList.PositionXOffset = 0f;
			this.DisplayedList.PositionYOffset = 0f;
			this._isDisplayingList = false;
			base.DoNotUseCustomScaleAndChildren = true;
		}

		protected override void OnClick()
		{
			base.OnClick();
			if (this.DisplayedList != null)
			{
				if (!this._isDisplayingList)
				{
					this.DisplayList();
					return;
				}
				this.HideList();
			}
		}

		private Widget _displayedList;

		private bool _isDisplayingList;
	}
}
