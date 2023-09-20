using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission
{
	// Token: 0x020000CD RID: 205
	public class TakenDamageItemBrushWidget : BrushWidget
	{
		// Token: 0x170003AB RID: 939
		// (get) Token: 0x06000A6C RID: 2668 RVA: 0x0001D6E6 File Offset: 0x0001B8E6
		// (set) Token: 0x06000A6D RID: 2669 RVA: 0x0001D6EE File Offset: 0x0001B8EE
		public float VerticalWidth { get; set; }

		// Token: 0x170003AC RID: 940
		// (get) Token: 0x06000A6E RID: 2670 RVA: 0x0001D6F7 File Offset: 0x0001B8F7
		// (set) Token: 0x06000A6F RID: 2671 RVA: 0x0001D6FF File Offset: 0x0001B8FF
		public float VerticalHeight { get; set; }

		// Token: 0x170003AD RID: 941
		// (get) Token: 0x06000A70 RID: 2672 RVA: 0x0001D708 File Offset: 0x0001B908
		// (set) Token: 0x06000A71 RID: 2673 RVA: 0x0001D710 File Offset: 0x0001B910
		public float HorizontalWidth { get; set; }

		// Token: 0x170003AE RID: 942
		// (get) Token: 0x06000A72 RID: 2674 RVA: 0x0001D719 File Offset: 0x0001B919
		// (set) Token: 0x06000A73 RID: 2675 RVA: 0x0001D721 File Offset: 0x0001B921
		public float HorizontalHeight { get; set; }

		// Token: 0x170003AF RID: 943
		// (get) Token: 0x06000A74 RID: 2676 RVA: 0x0001D72A File Offset: 0x0001B92A
		// (set) Token: 0x06000A75 RID: 2677 RVA: 0x0001D732 File Offset: 0x0001B932
		public float RangedOnScreenStayTime { get; set; } = 0.3f;

		// Token: 0x170003B0 RID: 944
		// (get) Token: 0x06000A76 RID: 2678 RVA: 0x0001D73B File Offset: 0x0001B93B
		// (set) Token: 0x06000A77 RID: 2679 RVA: 0x0001D743 File Offset: 0x0001B943
		public float MeleeOnScreenStayTime { get; set; } = 1f;

		// Token: 0x06000A78 RID: 2680 RVA: 0x0001D74C File Offset: 0x0001B94C
		public TakenDamageItemBrushWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000A79 RID: 2681 RVA: 0x0001D76C File Offset: 0x0001B96C
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

		// Token: 0x06000A7A RID: 2682 RVA: 0x0001D7E0 File Offset: 0x0001B9E0
		private void UpdateAlpha(float dt)
		{
			if (base.AlphaFactor < 0.01f)
			{
				base.EventFired("OnRemove", Array.Empty<object>());
			}
			float num = (this.IsRanged ? this.RangedOnScreenStayTime : this.MeleeOnScreenStayTime);
			this.SetGlobalAlphaRecursively(MathF.Lerp(base.AlphaFactor, 0f, dt / num, 1E-05f));
		}

		// Token: 0x06000A7B RID: 2683 RVA: 0x0001D83F File Offset: 0x0001BA3F
		protected override void OnRender(TwoDimensionContext twoDimensionContext, TwoDimensionDrawContext drawContext)
		{
			if (base.AlphaFactor > 0f)
			{
				base.OnRender(twoDimensionContext, drawContext);
			}
		}

		// Token: 0x170003B1 RID: 945
		// (get) Token: 0x06000A7C RID: 2684 RVA: 0x0001D856 File Offset: 0x0001BA56
		// (set) Token: 0x06000A7D RID: 2685 RVA: 0x0001D85E File Offset: 0x0001BA5E
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

		// Token: 0x170003B2 RID: 946
		// (get) Token: 0x06000A7E RID: 2686 RVA: 0x0001D87C File Offset: 0x0001BA7C
		// (set) Token: 0x06000A7F RID: 2687 RVA: 0x0001D884 File Offset: 0x0001BA84
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

		// Token: 0x170003B3 RID: 947
		// (get) Token: 0x06000A80 RID: 2688 RVA: 0x0001D8A2 File Offset: 0x0001BAA2
		// (set) Token: 0x06000A81 RID: 2689 RVA: 0x0001D8AA File Offset: 0x0001BAAA
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

		// Token: 0x170003B4 RID: 948
		// (get) Token: 0x06000A82 RID: 2690 RVA: 0x0001D8C8 File Offset: 0x0001BAC8
		// (set) Token: 0x06000A83 RID: 2691 RVA: 0x0001D8D0 File Offset: 0x0001BAD0
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

		// Token: 0x040004C1 RID: 1217
		private bool _initialized;

		// Token: 0x040004C8 RID: 1224
		private int _damageAmount;

		// Token: 0x040004C9 RID: 1225
		private Vec2 _screenPosOfAffectorAgent;

		// Token: 0x040004CA RID: 1226
		private bool _isBehind;

		// Token: 0x040004CB RID: 1227
		private bool _isRanged;
	}
}
