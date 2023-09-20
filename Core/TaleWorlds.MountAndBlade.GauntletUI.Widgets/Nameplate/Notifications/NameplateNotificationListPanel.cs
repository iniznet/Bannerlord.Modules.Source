using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Nameplate.Notifications
{
	public class NameplateNotificationListPanel : ListPanel
	{
		public NameplateNotificationListPanel(UIContext context)
			: base(context)
		{
		}

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

		private static readonly Color NegativeRelationColor = Color.ConvertStringToColor("#D6543BFF");

		private static readonly Color NeutralRelationColor = Color.ConvertStringToColor("#ECB05BFF");

		private static readonly Color PositiveRelationColor = Color.ConvertStringToColor("#98CA3AFF");

		private float _totalDt;

		private bool _isFirstFrame = true;

		private Widget _relationVisualWidget;

		private float _stayAmount = 2f;

		private float _fadeTime = 1f;

		private int _relationType = -2;
	}
}
