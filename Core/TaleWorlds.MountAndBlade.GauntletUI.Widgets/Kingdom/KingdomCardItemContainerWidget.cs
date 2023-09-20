using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Kingdom
{
	// Token: 0x02000112 RID: 274
	public class KingdomCardItemContainerWidget : Widget
	{
		// Token: 0x06000DF9 RID: 3577 RVA: 0x0002703B File Offset: 0x0002523B
		public KingdomCardItemContainerWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000DFA RID: 3578 RVA: 0x0002705A File Offset: 0x0002525A
		protected override void OnChildRemoved(Widget child)
		{
			base.OnChildRemoved(child);
			child.EventFire -= this.ChildrenWidgetEventFired;
		}

		// Token: 0x06000DFB RID: 3579 RVA: 0x00027075 File Offset: 0x00025275
		protected override void OnChildAdded(Widget child)
		{
			base.OnChildAdded(child);
			child.EventFire += this.ChildrenWidgetEventFired;
		}

		// Token: 0x06000DFC RID: 3580 RVA: 0x00027090 File Offset: 0x00025290
		private void ChildrenWidgetEventFired(Widget widget, string eventName, object[] args)
		{
			if (eventName == "HoverBegin")
			{
				this._isMouseOverChildren = true;
				widget.RenderLate = true;
				return;
			}
			if (eventName == "HoverEnd")
			{
				this._isMouseOverChildren = false;
				widget.RenderLate = false;
			}
		}

		// Token: 0x06000DFD RID: 3581 RVA: 0x000270CC File Offset: 0x000252CC
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			float num = 0f;
			float num2 = 0f;
			if (base.ChildCount > 0)
			{
				num = base.GetChild(0).Size.X * (float)base.ChildCount;
				num2 = this._defaultXOffset * base._inverseScaleToUse * (float)(base.ChildCount - 1) + base.GetChild(0).Size.X;
				base.IsEnabled = true;
			}
			else
			{
				base.IsEnabled = false;
			}
			for (int i = 0; i < base.ChildCount; i++)
			{
				Widget child = base.GetChild(i);
				if (this._isMouseOverChildren || this._isMouseOverSelf)
				{
					if (base.ChildCount > 1)
					{
						if (num < base.Size.X)
						{
							float num3 = base.Size.X / 2f - num / 2f;
							this._targetXOffset = (float)i * child.Size.X + num3;
						}
						else
						{
							this._targetXOffset = (float)i / ((float)base.ChildCount - 1f) * (base.Size.X - child.Size.X);
						}
					}
					else if (base.ChildCount == 1)
					{
						this._targetXOffset = base.Size.X / 2f - child.Size.X / 2f;
					}
				}
				else if (base.ChildCount > 1)
				{
					float num4 = this._defaultXOffset;
					while (num2 > base.Size.X && num4 > 5f)
					{
						num4 -= 0.5f;
						num2 = num4 * (float)(base.ChildCount - 1) + child.Size.X;
					}
					this._targetXOffset = base.Size.X / 2f - num2 / 2f + num4 * (float)i;
				}
				else if (base.ChildCount == 1)
				{
					this._targetXOffset = base.Size.X / 2f - child.Size.X / 2f;
				}
				child.PositionXOffset = Mathf.Lerp(child.PositionXOffset, this._targetXOffset * base._inverseScaleToUse, dt * this._lerpFactor);
			}
		}

		// Token: 0x06000DFE RID: 3582 RVA: 0x00027302 File Offset: 0x00025502
		protected override void OnHoverBegin()
		{
			base.OnHoverBegin();
			this._isMouseOverSelf = true;
			base.RenderLate = true;
		}

		// Token: 0x06000DFF RID: 3583 RVA: 0x00027318 File Offset: 0x00025518
		protected override void OnHoverEnd()
		{
			base.OnHoverEnd();
			this._isMouseOverSelf = false;
			base.RenderLate = false;
		}

		// Token: 0x0400066E RID: 1646
		private float _targetXOffset;

		// Token: 0x0400066F RID: 1647
		private bool _isMouseOverChildren;

		// Token: 0x04000670 RID: 1648
		private bool _isMouseOverSelf;

		// Token: 0x04000671 RID: 1649
		private float _lerpFactor = 15f;

		// Token: 0x04000672 RID: 1650
		private float _defaultXOffset = 20f;
	}
}
