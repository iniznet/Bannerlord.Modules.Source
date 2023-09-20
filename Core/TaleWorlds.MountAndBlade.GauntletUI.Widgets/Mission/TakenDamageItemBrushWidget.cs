using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission
{
	public class TakenDamageItemBrushWidget : BrushWidget
	{
		public float VerticalWidth { get; set; }

		public float VerticalHeight { get; set; }

		public float HorizontalWidth { get; set; }

		public float HorizontalHeight { get; set; }

		public float RangedOnScreenStayTime { get; set; } = 0.3f;

		public float MeleeOnScreenStayTime { get; set; } = 1f;

		public TakenDamageItemBrushWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (!this._initialized)
			{
				this.RegisterBrushStatesOfWidget();
				this._initialized = true;
				if (!this.IsRanged)
				{
					float num = (float)this.DamageAmount / 70f;
					num = MathF.Clamp(num, 0f, 1f);
					base.AlphaFactor = MathF.Lerp(0.3f, 1f, num, 1E-05f);
				}
			}
			this.UpdateAlpha(dt);
		}

		private void UpdateAlpha(float dt)
		{
			if (base.AlphaFactor < 0.01f)
			{
				base.EventFired("OnRemove", Array.Empty<object>());
			}
			float num = (this.IsRanged ? this.RangedOnScreenStayTime : this.MeleeOnScreenStayTime);
			this.SetGlobalAlphaRecursively(MathF.Lerp(base.AlphaFactor, 0f, dt / num, 1E-05f));
		}

		protected override void OnRender(TwoDimensionContext twoDimensionContext, TwoDimensionDrawContext drawContext)
		{
			if (base.AlphaFactor > 0f)
			{
				base.OnRender(twoDimensionContext, drawContext);
			}
		}

		[DataSourceProperty]
		public int DamageAmount
		{
			get
			{
				return this._damageAmount;
			}
			set
			{
				if (this._damageAmount != value)
				{
					this._damageAmount = value;
					base.OnPropertyChanged(value, "DamageAmount");
				}
			}
		}

		[DataSourceProperty]
		public bool IsBehind
		{
			get
			{
				return this._isBehind;
			}
			set
			{
				if (this._isBehind != value)
				{
					this._isBehind = value;
					base.OnPropertyChanged(value, "IsBehind");
				}
			}
		}

		[DataSourceProperty]
		public bool IsRanged
		{
			get
			{
				return this._isRanged;
			}
			set
			{
				if (this._isRanged != value)
				{
					this._isRanged = value;
					base.OnPropertyChanged(value, "IsRanged");
				}
			}
		}

		[DataSourceProperty]
		public Vec2 ScreenPosOfAffectorAgent
		{
			get
			{
				return this._screenPosOfAffectorAgent;
			}
			set
			{
				if (this._screenPosOfAffectorAgent != value)
				{
					this._screenPosOfAffectorAgent = value;
					base.OnPropertyChanged(value, "ScreenPosOfAffectorAgent");
				}
			}
		}

		private bool _initialized;

		private int _damageAmount;

		private Vec2 _screenPosOfAffectorAgent;

		private bool _isBehind;

		private bool _isRanged;
	}
}
