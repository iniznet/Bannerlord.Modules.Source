using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.KillFeed
{
	// Token: 0x020000AE RID: 174
	public class MultiplayerGeneralKillFeedWidget : Widget
	{
		// Token: 0x17000323 RID: 803
		// (get) Token: 0x060008ED RID: 2285 RVA: 0x00019805 File Offset: 0x00017A05
		// (set) Token: 0x060008EE RID: 2286 RVA: 0x0001980D File Offset: 0x00017A0D
		public float VerticalPaddingAmount { get; set; } = 3f;

		// Token: 0x060008EF RID: 2287 RVA: 0x00019816 File Offset: 0x00017A16
		public MultiplayerGeneralKillFeedWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060008F0 RID: 2288 RVA: 0x00019834 File Offset: 0x00017A34
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this._normalWidgetHeight <= 0f && base.ChildCount > 1)
			{
				this._normalWidgetHeight = base.GetChild(0).ScaledSuggestedHeight * base._inverseScaleToUse;
			}
			for (int i = 0; i < base.ChildCount; i++)
			{
				Widget child = base.GetChild(i);
				child.PositionYOffset = Mathf.Lerp(child.PositionYOffset, this.GetVerticalPositionOfChildByIndex(i, base.ChildCount), 0.35f);
			}
		}

		// Token: 0x060008F1 RID: 2289 RVA: 0x000198B1 File Offset: 0x00017AB1
		protected override void OnChildAdded(Widget child)
		{
			base.OnChildAdded(child);
			child.PositionYOffset = this.GetVerticalPositionOfChildByIndex(child.GetSiblingIndex(), base.ChildCount);
			this.UpdateSpeedModifiers();
		}

		// Token: 0x060008F2 RID: 2290 RVA: 0x000198D8 File Offset: 0x00017AD8
		private float GetVerticalPositionOfChildByIndex(int indexOfChild, int numOfTotalChild)
		{
			int num = numOfTotalChild - 1 - indexOfChild;
			return (this._normalWidgetHeight + this.VerticalPaddingAmount) * (float)num;
		}

		// Token: 0x060008F3 RID: 2291 RVA: 0x000198FC File Offset: 0x00017AFC
		private void UpdateSpeedModifiers()
		{
			if (base.ChildCount > this._speedUpWidgetLimit)
			{
				float num = (float)(base.ChildCount - this._speedUpWidgetLimit) / 20f + 1f;
				for (int i = 0; i < base.ChildCount - this._speedUpWidgetLimit; i++)
				{
					(base.GetChild(i) as MultiplayerGeneralKillFeedItemWidget).SetSpeedModifier(num);
				}
			}
		}

		// Token: 0x04000413 RID: 1043
		private float _normalWidgetHeight;

		// Token: 0x04000414 RID: 1044
		private int _speedUpWidgetLimit = 10;
	}
}
