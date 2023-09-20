using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Nameplate.Notifications
{
	// Token: 0x02000076 RID: 118
	public class NameplateNotificationListPanel : ListPanel
	{
		// Token: 0x060006A9 RID: 1705 RVA: 0x00013F01 File Offset: 0x00012101
		public NameplateNotificationListPanel(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060006AA RID: 1706 RVA: 0x00013F30 File Offset: 0x00012130
		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			if (this._isFirstFrame)
			{
				switch (this.RelationType)
				{
				case -1:
					this.RelationVisualWidget.Color = NameplateNotificationListPanel.NegativeRelationColor;
					break;
				case 0:
					this.RelationVisualWidget.Color = NameplateNotificationListPanel.NeutralRelationColor;
					break;
				case 1:
					this.RelationVisualWidget.Color = NameplateNotificationListPanel.PositiveRelationColor;
					break;
				}
				this._isFirstFrame = false;
			}
			this._totalDt += dt;
			if (base.AlphaFactor <= 0f || this._totalDt > this._stayAmount + this._fadeTime)
			{
				base.EventFired("OnRemove", Array.Empty<object>());
				return;
			}
			if (this._totalDt > this._stayAmount)
			{
				float num = 1f - (this._totalDt - this._stayAmount) / this._fadeTime;
				this.SetGlobalAlphaRecursively(num);
			}
		}

		// Token: 0x1700025D RID: 605
		// (get) Token: 0x060006AB RID: 1707 RVA: 0x00014015 File Offset: 0x00012215
		// (set) Token: 0x060006AC RID: 1708 RVA: 0x0001401D File Offset: 0x0001221D
		public Widget RelationVisualWidget
		{
			get
			{
				return this._relationVisualWidget;
			}
			set
			{
				if (this._relationVisualWidget != value)
				{
					this._relationVisualWidget = value;
					base.OnPropertyChanged<Widget>(value, "RelationVisualWidget");
				}
			}
		}

		// Token: 0x1700025E RID: 606
		// (get) Token: 0x060006AD RID: 1709 RVA: 0x0001403B File Offset: 0x0001223B
		// (set) Token: 0x060006AE RID: 1710 RVA: 0x00014043 File Offset: 0x00012243
		public int RelationType
		{
			get
			{
				return this._relationType;
			}
			set
			{
				if (this._relationType != value)
				{
					this._relationType = value;
					base.OnPropertyChanged(value, "RelationType");
				}
			}
		}

		// Token: 0x1700025F RID: 607
		// (get) Token: 0x060006AF RID: 1711 RVA: 0x00014061 File Offset: 0x00012261
		// (set) Token: 0x060006B0 RID: 1712 RVA: 0x00014069 File Offset: 0x00012269
		public float StayAmount
		{
			get
			{
				return this._stayAmount;
			}
			set
			{
				if (this._stayAmount != value)
				{
					this._stayAmount = value;
					base.OnPropertyChanged(value, "StayAmount");
				}
			}
		}

		// Token: 0x17000260 RID: 608
		// (get) Token: 0x060006B1 RID: 1713 RVA: 0x00014087 File Offset: 0x00012287
		// (set) Token: 0x060006B2 RID: 1714 RVA: 0x0001408F File Offset: 0x0001228F
		public float FadeTime
		{
			get
			{
				return this._fadeTime;
			}
			set
			{
				if (this._fadeTime != value)
				{
					this._fadeTime = value;
					base.OnPropertyChanged(value, "FadeTime");
				}
			}
		}

		// Token: 0x040002EC RID: 748
		private static readonly Color NegativeRelationColor = Color.ConvertStringToColor("#D6543BFF");

		// Token: 0x040002ED RID: 749
		private static readonly Color NeutralRelationColor = Color.ConvertStringToColor("#ECB05BFF");

		// Token: 0x040002EE RID: 750
		private static readonly Color PositiveRelationColor = Color.ConvertStringToColor("#98CA3AFF");

		// Token: 0x040002EF RID: 751
		private float _totalDt;

		// Token: 0x040002F0 RID: 752
		private bool _isFirstFrame = true;

		// Token: 0x040002F1 RID: 753
		private Widget _relationVisualWidget;

		// Token: 0x040002F2 RID: 754
		private float _stayAmount = 2f;

		// Token: 0x040002F3 RID: 755
		private float _fadeTime = 1f;

		// Token: 0x040002F4 RID: 756
		private int _relationType = -2;
	}
}
