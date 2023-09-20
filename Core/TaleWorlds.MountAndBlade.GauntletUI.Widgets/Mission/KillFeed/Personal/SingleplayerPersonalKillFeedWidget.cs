using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission.KillFeed.Personal
{
	// Token: 0x020000DF RID: 223
	public class SingleplayerPersonalKillFeedWidget : Widget
	{
		// Token: 0x06000B9A RID: 2970 RVA: 0x000204AA File Offset: 0x0001E6AA
		public SingleplayerPersonalKillFeedWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000B9B RID: 2971 RVA: 0x000204C8 File Offset: 0x0001E6C8
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this._normalWidgetHeight == -1f && base.ChildCount > 1)
			{
				this._normalWidgetHeight = base.GetChild(0).ScaledSuggestedHeight * base._inverseScaleToUse;
			}
			for (int i = 0; i < base.ChildCount; i++)
			{
				Widget child = base.GetChild(i);
				child.PositionYOffset = Mathf.Lerp(child.PositionYOffset, this.GetVerticalPositionOfChildByIndex(i, base.ChildCount), 0.2f);
			}
		}

		// Token: 0x06000B9C RID: 2972 RVA: 0x00020545 File Offset: 0x0001E745
		protected override void OnChildAdded(Widget child)
		{
			base.OnChildAdded(child);
			child.PositionYOffset = this.GetVerticalPositionOfChildByIndex(child.GetSiblingIndex(), base.ChildCount);
			this.UpdateSpeedModifiers();
		}

		// Token: 0x06000B9D RID: 2973 RVA: 0x0002056C File Offset: 0x0001E76C
		private float GetVerticalPositionOfChildByIndex(int indexOfChild, int numOfTotalChild)
		{
			return -(this._normalWidgetHeight * (float)indexOfChild);
		}

		// Token: 0x06000B9E RID: 2974 RVA: 0x00020578 File Offset: 0x0001E778
		private void UpdateSpeedModifiers()
		{
			if (base.ChildCount > this._speedUpWidgetLimit)
			{
				float num = (float)(base.ChildCount - this._speedUpWidgetLimit) / 3f + 1f;
				for (int i = 0; i < base.ChildCount - this._speedUpWidgetLimit; i++)
				{
					(base.GetChild(i) as SingleplayerPersonalKillFeedItemWidget).SetSpeedModifier(num);
				}
			}
		}

		// Token: 0x0400054E RID: 1358
		private float _normalWidgetHeight = -1f;

		// Token: 0x0400054F RID: 1359
		private int _speedUpWidgetLimit = 3;
	}
}
