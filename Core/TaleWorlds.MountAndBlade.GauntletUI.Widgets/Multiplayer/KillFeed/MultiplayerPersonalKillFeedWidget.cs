using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.KillFeed
{
	public class MultiplayerPersonalKillFeedWidget : Widget
	{
		private int _speedUpWidgetLimit
		{
			get
			{
				return 2;
			}
		}

		public MultiplayerPersonalKillFeedWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			for (int i = 0; i < base.ChildCount; i++)
			{
				Widget child = base.GetChild(i);
				child.PositionYOffset = Mathf.Lerp(child.PositionYOffset, this.GetVerticalPositionOfChildByIndex(i), 0.35f);
			}
		}

		protected override void OnChildAdded(Widget child)
		{
			base.OnChildAdded(child);
			this.UpdateSpeedModifiers();
			this.UpdateMaxTargetAlphas();
		}

		private void UpdateMaxTargetAlphas()
		{
			for (int i = base.ChildCount - 1; i >= 0; i--)
			{
				MultiplayerPersonalKillFeedItemWidget multiplayerPersonalKillFeedItemWidget = base.GetChild(i) as MultiplayerPersonalKillFeedItemWidget;
				if (i <= base.ChildCount - 1 && i >= base.ChildCount - 4)
				{
					multiplayerPersonalKillFeedItemWidget.SetMaxAlphaValue(1f);
				}
				else if (i == base.ChildCount - 5)
				{
					multiplayerPersonalKillFeedItemWidget.SetMaxAlphaValue(0.7f);
				}
				else if (i == base.ChildCount - 6)
				{
					multiplayerPersonalKillFeedItemWidget.SetMaxAlphaValue(0.4f);
				}
				else if (i == base.ChildCount - 7)
				{
					multiplayerPersonalKillFeedItemWidget.SetMaxAlphaValue(0.15f);
				}
				else
				{
					multiplayerPersonalKillFeedItemWidget.SetMaxAlphaValue(0f);
				}
			}
		}

		private float GetVerticalPositionOfChildByIndex(int indexOfChild)
		{
			float num = 0f;
			for (int i = base.ChildCount - 1; i > indexOfChild; i--)
			{
				num += base.GetChild(i).Size.Y * base._inverseScaleToUse;
			}
			return num;
		}

		private void UpdateSpeedModifiers()
		{
			if (base.ChildCount > this._speedUpWidgetLimit)
			{
				float num = (float)(base.ChildCount - this._speedUpWidgetLimit) / 2f + 1f;
				for (int i = 0; i < base.ChildCount - this._speedUpWidgetLimit; i++)
				{
					MultiplayerPersonalKillFeedItemWidget multiplayerPersonalKillFeedItemWidget = base.GetChild(i) as MultiplayerPersonalKillFeedItemWidget;
					if (multiplayerPersonalKillFeedItemWidget != null)
					{
						multiplayerPersonalKillFeedItemWidget.SetSpeedModifier(num);
					}
				}
			}
		}
	}
}
