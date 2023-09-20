using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Kingdom
{
	public class KingdomCardItemContainerWidget : Widget
	{
		public KingdomCardItemContainerWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnChildRemoved(Widget child)
		{
			base.OnChildRemoved(child);
			child.EventFire -= this.ChildrenWidgetEventFired;
		}

		protected override void OnChildAdded(Widget child)
		{
			base.OnChildAdded(child);
			child.EventFire += this.ChildrenWidgetEventFired;
		}

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

		protected override void OnHoverBegin()
		{
			base.OnHoverBegin();
			this._isMouseOverSelf = true;
			base.RenderLate = true;
		}

		protected override void OnHoverEnd()
		{
			base.OnHoverEnd();
			this._isMouseOverSelf = false;
			base.RenderLate = false;
		}

		private float _targetXOffset;

		private bool _isMouseOverChildren;

		private bool _isMouseOverSelf;

		private float _lerpFactor = 15f;

		private float _defaultXOffset = 20f;
	}
}
