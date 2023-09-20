using System;
using System.Linq;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	// Token: 0x02000017 RID: 23
	public class DropdownButtonWidget : ButtonWidget
	{
		// Token: 0x1700005C RID: 92
		// (get) Token: 0x06000114 RID: 276 RVA: 0x00004D9F File Offset: 0x00002F9F
		// (set) Token: 0x06000115 RID: 277 RVA: 0x00004DA8 File Offset: 0x00002FA8
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

		// Token: 0x06000116 RID: 278 RVA: 0x00004E82 File Offset: 0x00003082
		public DropdownButtonWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000117 RID: 279 RVA: 0x00004E8B File Offset: 0x0000308B
		private void OnListItemSelected(Widget list)
		{
			this.HideList();
		}

		// Token: 0x06000118 RID: 280 RVA: 0x00004E94 File Offset: 0x00003094
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

		// Token: 0x06000119 RID: 281 RVA: 0x00004F9C File Offset: 0x0000319C
		private void DisplayList()
		{
			this.DisplayedList.ParentWidget = base.EventManager.Root;
			this.DisplayedList.IsVisible = true;
			this.DisplayedList.HorizontalAlignment = HorizontalAlignment.Left;
			this.DisplayedList.VerticalAlignment = VerticalAlignment.Top;
			this._isDisplayingList = true;
			base.DoNotUseCustomScaleAndChildren = false;
		}

		// Token: 0x0600011A RID: 282 RVA: 0x00004FF4 File Offset: 0x000031F4
		private void HideList()
		{
			this.DisplayedList.ParentWidget = this;
			this.DisplayedList.IsVisible = false;
			this.DisplayedList.PositionXOffset = 0f;
			this.DisplayedList.PositionYOffset = 0f;
			this._isDisplayingList = false;
			base.DoNotUseCustomScaleAndChildren = true;
		}

		// Token: 0x0600011B RID: 283 RVA: 0x00005047 File Offset: 0x00003247
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

		// Token: 0x04000086 RID: 134
		private Widget _displayedList;

		// Token: 0x04000087 RID: 135
		private bool _isDisplayingList;
	}
}
