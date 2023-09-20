using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.KillFeed
{
	// Token: 0x020000B0 RID: 176
	public class MultiplayerPersonalKillFeedWidget : Widget
	{
		// Token: 0x17000331 RID: 817
		// (get) Token: 0x06000919 RID: 2329 RVA: 0x0001A0D2 File Offset: 0x000182D2
		private int _speedUpWidgetLimit
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x0600091A RID: 2330 RVA: 0x0001A0D5 File Offset: 0x000182D5
		public MultiplayerPersonalKillFeedWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x0600091B RID: 2331 RVA: 0x0001A0E0 File Offset: 0x000182E0
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			for (int i = 0; i < base.ChildCount; i++)
			{
				Widget child = base.GetChild(i);
				child.PositionYOffset = Mathf.Lerp(child.PositionYOffset, this.GetVerticalPositionOfChildByIndex(i), 0.35f);
			}
		}

		// Token: 0x0600091C RID: 2332 RVA: 0x0001A128 File Offset: 0x00018328
		protected override void OnChildAdded(Widget child)
		{
			base.OnChildAdded(child);
			this.UpdateSpeedModifiers();
			this.UpdateMaxTargetAlphas();
		}

		// Token: 0x0600091D RID: 2333 RVA: 0x0001A140 File Offset: 0x00018340
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

		// Token: 0x0600091E RID: 2334 RVA: 0x0001A1EC File Offset: 0x000183EC
		private float GetVerticalPositionOfChildByIndex(int indexOfChild)
		{
			float num = 0f;
			for (int i = base.ChildCount - 1; i > indexOfChild; i--)
			{
				num += base.GetChild(i).Size.Y * base._inverseScaleToUse;
			}
			return num;
		}

		// Token: 0x0600091F RID: 2335 RVA: 0x0001A230 File Offset: 0x00018430
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
