using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.CharacterDeveloper
{
	// Token: 0x02000161 RID: 353
	public class PerkItemButtonWidget : ButtonWidget
	{
		// Token: 0x1700065F RID: 1631
		// (get) Token: 0x06001218 RID: 4632 RVA: 0x00032210 File Offset: 0x00030410
		// (set) Token: 0x06001219 RID: 4633 RVA: 0x00032218 File Offset: 0x00030418
		public Brush NotEarnedPerkBrush { get; set; }

		// Token: 0x17000660 RID: 1632
		// (get) Token: 0x0600121A RID: 4634 RVA: 0x00032221 File Offset: 0x00030421
		// (set) Token: 0x0600121B RID: 4635 RVA: 0x00032229 File Offset: 0x00030429
		public Brush EarnedNotSelectedPerkBrush { get; set; }

		// Token: 0x17000661 RID: 1633
		// (get) Token: 0x0600121C RID: 4636 RVA: 0x00032232 File Offset: 0x00030432
		// (set) Token: 0x0600121D RID: 4637 RVA: 0x0003223A File Offset: 0x0003043A
		public Brush InSelectionPerkBrush { get; set; }

		// Token: 0x17000662 RID: 1634
		// (get) Token: 0x0600121E RID: 4638 RVA: 0x00032243 File Offset: 0x00030443
		// (set) Token: 0x0600121F RID: 4639 RVA: 0x0003224B File Offset: 0x0003044B
		public Brush EarnedActivePerkBrush { get; set; }

		// Token: 0x17000663 RID: 1635
		// (get) Token: 0x06001220 RID: 4640 RVA: 0x00032254 File Offset: 0x00030454
		// (set) Token: 0x06001221 RID: 4641 RVA: 0x0003225C File Offset: 0x0003045C
		public Brush EarnedNotActivePerkBrush { get; set; }

		// Token: 0x17000664 RID: 1636
		// (get) Token: 0x06001222 RID: 4642 RVA: 0x00032265 File Offset: 0x00030465
		// (set) Token: 0x06001223 RID: 4643 RVA: 0x0003226D File Offset: 0x0003046D
		public Brush EarnedPreviousPerkNotSelectedPerkBrush { get; set; }

		// Token: 0x17000665 RID: 1637
		// (get) Token: 0x06001224 RID: 4644 RVA: 0x00032276 File Offset: 0x00030476
		// (set) Token: 0x06001225 RID: 4645 RVA: 0x0003227E File Offset: 0x0003047E
		public BrushWidget PerkVisualWidgetParent { get; set; }

		// Token: 0x06001226 RID: 4646 RVA: 0x00032287 File Offset: 0x00030487
		public PerkItemButtonWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06001227 RID: 4647 RVA: 0x00032298 File Offset: 0x00030498
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this.PerkVisualWidget != null && ((this.PerkVisualWidget.Sprite != null && base.Context.SpriteData.GetSprite(this.PerkVisualWidget.Sprite.Name) == null) || this.PerkVisualWidget.Sprite == null))
			{
				this.PerkVisualWidget.Sprite = base.Context.SpriteData.GetSprite("SPPerks\\locked_fallback");
			}
			if (this._animState == PerkItemButtonWidget.AnimState.Start)
			{
				this._tickCount++;
				if (this._tickCount > 20)
				{
					this._animState = PerkItemButtonWidget.AnimState.Starting;
					return;
				}
			}
			else if (this._animState == PerkItemButtonWidget.AnimState.Starting)
			{
				this.PerkVisualWidgetParent.BrushRenderer.RestartAnimation();
				this._animState = PerkItemButtonWidget.AnimState.Playing;
			}
		}

		// Token: 0x06001228 RID: 4648 RVA: 0x0003235C File Offset: 0x0003055C
		private void SetColorState(bool isActive)
		{
			if (this.PerkVisualWidget != null)
			{
				float num = (isActive ? 1f : 1f);
				float num2 = (isActive ? 1.3f : 0.75f);
				using (IEnumerator<Widget> enumerator = base.AllChildrenAndThis.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						BrushWidget brushWidget;
						if ((brushWidget = enumerator.Current as BrushWidget) != null)
						{
							foreach (Style style in brushWidget.Brush.Styles)
							{
								for (int i = 0; i < style.LayerCount; i++)
								{
									StyleLayer layer = style.GetLayer(i);
									layer.AlphaFactor = num;
									layer.ColorFactor = num2;
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06001229 RID: 4649 RVA: 0x00032444 File Offset: 0x00030644
		protected override void OnClick()
		{
			base.OnClick();
			if (this._isSelectable)
			{
				base.Context.TwoDimensionContext.PlaySound("popup");
			}
		}

		// Token: 0x0600122A RID: 4650 RVA: 0x0003246C File Offset: 0x0003066C
		private void UpdatePerkStateVisual(int perkState)
		{
			switch (perkState)
			{
			case 0:
				this.PerkVisualWidgetParent.Brush = this.NotEarnedPerkBrush;
				this._isSelectable = false;
				return;
			case 1:
				this.PerkVisualWidgetParent.Brush = this.EarnedNotSelectedPerkBrush;
				this._animState = PerkItemButtonWidget.AnimState.Start;
				this._isSelectable = true;
				return;
			case 2:
				this.PerkVisualWidgetParent.Brush = this.InSelectionPerkBrush;
				this._isSelectable = false;
				return;
			case 3:
				this.PerkVisualWidgetParent.Brush = this.EarnedActivePerkBrush;
				this._isSelectable = false;
				return;
			case 4:
				this.PerkVisualWidgetParent.Brush = this.EarnedNotActivePerkBrush;
				this._isSelectable = false;
				return;
			case 5:
				this.PerkVisualWidgetParent.Brush = this.EarnedPreviousPerkNotSelectedPerkBrush;
				this._isSelectable = false;
				return;
			default:
				Debug.FailedAssert("Perk visual state is not defined", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.GauntletUI.Widgets\\CharacterDeveloper\\PerkItemButtonWidget.cs", "UpdatePerkStateVisual", 134);
				return;
			}
		}

		// Token: 0x17000666 RID: 1638
		// (get) Token: 0x0600122B RID: 4651 RVA: 0x00032552 File Offset: 0x00030752
		// (set) Token: 0x0600122C RID: 4652 RVA: 0x0003255A File Offset: 0x0003075A
		public int Level
		{
			get
			{
				return this._level;
			}
			set
			{
				if (this._level != value)
				{
					this._level = value;
					base.OnPropertyChanged(value, "Level");
				}
			}
		}

		// Token: 0x17000667 RID: 1639
		// (get) Token: 0x0600122D RID: 4653 RVA: 0x00032578 File Offset: 0x00030778
		// (set) Token: 0x0600122E RID: 4654 RVA: 0x00032580 File Offset: 0x00030780
		public Widget PerkVisualWidget
		{
			get
			{
				return this._perkVisualWidget;
			}
			set
			{
				if (this._perkVisualWidget != value)
				{
					this._perkVisualWidget = value;
					base.OnPropertyChanged<Widget>(value, "PerkVisualWidget");
				}
			}
		}

		// Token: 0x17000668 RID: 1640
		// (get) Token: 0x0600122F RID: 4655 RVA: 0x0003259E File Offset: 0x0003079E
		// (set) Token: 0x06001230 RID: 4656 RVA: 0x000325A6 File Offset: 0x000307A6
		public int PerkState
		{
			get
			{
				return this._perkState;
			}
			set
			{
				if (this._perkState != value)
				{
					this._perkState = value;
					base.OnPropertyChanged(value, "PerkState");
					this.UpdatePerkStateVisual(this.PerkState);
				}
			}
		}

		// Token: 0x17000669 RID: 1641
		// (get) Token: 0x06001231 RID: 4657 RVA: 0x000325D0 File Offset: 0x000307D0
		// (set) Token: 0x06001232 RID: 4658 RVA: 0x000325D8 File Offset: 0x000307D8
		public int AlternativeType
		{
			get
			{
				return this._alternativeType;
			}
			set
			{
				if (this._alternativeType != value)
				{
					this._alternativeType = value;
					base.OnPropertyChanged(value, "AlternativeType");
				}
			}
		}

		// Token: 0x0400084F RID: 2127
		private PerkItemButtonWidget.AnimState _animState;

		// Token: 0x04000850 RID: 2128
		private int _tickCount;

		// Token: 0x04000851 RID: 2129
		private bool _isSelectable;

		// Token: 0x04000852 RID: 2130
		private int _level;

		// Token: 0x04000853 RID: 2131
		private int _alternativeType;

		// Token: 0x04000854 RID: 2132
		private int _perkState = -1;

		// Token: 0x04000855 RID: 2133
		private Widget _perkVisualWidget;

		// Token: 0x020001A2 RID: 418
		public enum AnimState
		{
			// Token: 0x0400093F RID: 2367
			Idle,
			// Token: 0x04000940 RID: 2368
			Start,
			// Token: 0x04000941 RID: 2369
			Starting,
			// Token: 0x04000942 RID: 2370
			Playing
		}
	}
}
