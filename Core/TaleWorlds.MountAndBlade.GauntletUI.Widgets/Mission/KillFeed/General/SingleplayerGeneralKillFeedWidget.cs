using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission.KillFeed.General
{
	// Token: 0x020000E1 RID: 225
	public class SingleplayerGeneralKillFeedWidget : Widget
	{
		// Token: 0x17000434 RID: 1076
		// (get) Token: 0x06000BBA RID: 3002 RVA: 0x00020A01 File Offset: 0x0001EC01
		// (set) Token: 0x06000BBB RID: 3003 RVA: 0x00020A09 File Offset: 0x0001EC09
		public float VerticalPaddingAmount { get; set; } = 3f;

		// Token: 0x06000BBC RID: 3004 RVA: 0x00020A12 File Offset: 0x0001EC12
		public SingleplayerGeneralKillFeedWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000BBD RID: 3005 RVA: 0x00020A30 File Offset: 0x0001EC30
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

		// Token: 0x06000BBE RID: 3006 RVA: 0x00020AAD File Offset: 0x0001ECAD
		protected override void OnChildAdded(Widget child)
		{
			base.OnChildAdded(child);
			child.PositionYOffset = this.GetVerticalPositionOfChildByIndex(child.GetSiblingIndex(), base.ChildCount);
			this.UpdateSpeedModifiers();
		}

		// Token: 0x06000BBF RID: 3007 RVA: 0x00020AD4 File Offset: 0x0001ECD4
		private float GetVerticalPositionOfChildByIndex(int indexOfChild, int numOfTotalChild)
		{
			return (this._normalWidgetHeight + this.VerticalPaddingAmount) * (float)(numOfTotalChild - 1 - indexOfChild);
		}

		// Token: 0x06000BC0 RID: 3008 RVA: 0x00020AEC File Offset: 0x0001ECEC
		private void UpdateSpeedModifiers()
		{
			if (base.ChildCount > this._speedUpWidgetLimit)
			{
				float num = (float)(base.ChildCount - this._speedUpWidgetLimit) / 20f + 1f;
				for (int i = 0; i < base.ChildCount - this._speedUpWidgetLimit; i++)
				{
					(base.GetChild(i) as SingleplayerGeneralKillFeedItemWidget).SetSpeedModifier(num);
				}
			}
		}

		// Token: 0x04000565 RID: 1381
		private float _normalWidgetHeight;

		// Token: 0x04000566 RID: 1382
		private int _speedUpWidgetLimit = 10;
	}
}
