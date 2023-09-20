using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Kingdom
{
	// Token: 0x02000115 RID: 277
	public class KingdomDecisionOptionWidget : Widget
	{
		// Token: 0x170004FB RID: 1275
		// (get) Token: 0x06000E08 RID: 3592 RVA: 0x00027425 File Offset: 0x00025625
		// (set) Token: 0x06000E09 RID: 3593 RVA: 0x0002742D File Offset: 0x0002562D
		public Widget SealVisualWidget { get; set; }

		// Token: 0x170004FC RID: 1276
		// (get) Token: 0x06000E0A RID: 3594 RVA: 0x00027436 File Offset: 0x00025636
		// (set) Token: 0x06000E0B RID: 3595 RVA: 0x0002743E File Offset: 0x0002563E
		public DecisionSupportStrengthListPanel StrengthWidget { get; set; }

		// Token: 0x170004FD RID: 1277
		// (get) Token: 0x06000E0C RID: 3596 RVA: 0x00027447 File Offset: 0x00025647
		// (set) Token: 0x06000E0D RID: 3597 RVA: 0x0002744F File Offset: 0x0002564F
		public bool IsPlayerSupporter { get; set; }

		// Token: 0x170004FE RID: 1278
		// (get) Token: 0x06000E0E RID: 3598 RVA: 0x00027458 File Offset: 0x00025658
		// (set) Token: 0x06000E0F RID: 3599 RVA: 0x00027460 File Offset: 0x00025660
		public bool IsAbstain { get; set; }

		// Token: 0x170004FF RID: 1279
		// (get) Token: 0x06000E10 RID: 3600 RVA: 0x00027469 File Offset: 0x00025669
		// (set) Token: 0x06000E11 RID: 3601 RVA: 0x00027471 File Offset: 0x00025671
		public float SealStartWidth { get; set; } = 232f;

		// Token: 0x17000500 RID: 1280
		// (get) Token: 0x06000E12 RID: 3602 RVA: 0x0002747A File Offset: 0x0002567A
		// (set) Token: 0x06000E13 RID: 3603 RVA: 0x00027482 File Offset: 0x00025682
		public float SealStartHeight { get; set; } = 232f;

		// Token: 0x17000501 RID: 1281
		// (get) Token: 0x06000E14 RID: 3604 RVA: 0x0002748B File Offset: 0x0002568B
		// (set) Token: 0x06000E15 RID: 3605 RVA: 0x00027493 File Offset: 0x00025693
		public float SealEndWidth { get; set; } = 140f;

		// Token: 0x17000502 RID: 1282
		// (get) Token: 0x06000E16 RID: 3606 RVA: 0x0002749C File Offset: 0x0002569C
		// (set) Token: 0x06000E17 RID: 3607 RVA: 0x000274A4 File Offset: 0x000256A4
		public float SealEndHeight { get; set; } = 140f;

		// Token: 0x17000503 RID: 1283
		// (get) Token: 0x06000E18 RID: 3608 RVA: 0x000274AD File Offset: 0x000256AD
		// (set) Token: 0x06000E19 RID: 3609 RVA: 0x000274B5 File Offset: 0x000256B5
		public float SealAnimLength { get; set; } = 0.2f;

		// Token: 0x06000E1A RID: 3610 RVA: 0x000274C0 File Offset: 0x000256C0
		public KingdomDecisionOptionWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000E1B RID: 3611 RVA: 0x00027518 File Offset: 0x00025718
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			this.StrengthWidget.IsVisible = !this.IsAbstain && this.IsPlayerSupporter && this.IsOptionSelected && !this.IsKingsOption && !this._isKingsDecisionDone;
			if (this._animStartTime != -1f && base.EventManager.Time - this._animStartTime < this.SealAnimLength)
			{
				this.SealVisualWidget.IsVisible = true;
				float num = (base.EventManager.Time - this._animStartTime) / this.SealAnimLength;
				this.SealVisualWidget.SuggestedWidth = Mathf.Lerp(this.SealStartWidth, this.SealEndWidth, num);
				this.SealVisualWidget.SuggestedHeight = Mathf.Lerp(this.SealStartHeight, this.SealEndHeight, num);
				this.SealVisualWidget.SetGlobalAlphaRecursively(Mathf.Lerp(0f, 1f, num));
			}
		}

		// Token: 0x06000E1C RID: 3612 RVA: 0x00027608 File Offset: 0x00025808
		internal void OnKingsDecisionDone()
		{
			this._isKingsDecisionDone = true;
		}

		// Token: 0x06000E1D RID: 3613 RVA: 0x00027611 File Offset: 0x00025811
		internal void OnFinalDone()
		{
			this._isKingsDecisionDone = false;
			this._animStartTime = -1f;
		}

		// Token: 0x06000E1E RID: 3614 RVA: 0x00027625 File Offset: 0x00025825
		private void OnSelectionChange(bool value)
		{
			if (!this.IsPlayerSupporter)
			{
				this.SealVisualWidget.IsVisible = value;
				this.SealVisualWidget.SetGlobalAlphaRecursively(0.2f);
				return;
			}
			this.SealVisualWidget.IsVisible = false;
		}

		// Token: 0x06000E1F RID: 3615 RVA: 0x00027658 File Offset: 0x00025858
		private void HandleKingsOption()
		{
			this._animStartTime = base.EventManager.Time;
		}

		// Token: 0x17000504 RID: 1284
		// (get) Token: 0x06000E20 RID: 3616 RVA: 0x0002766B File Offset: 0x0002586B
		// (set) Token: 0x06000E21 RID: 3617 RVA: 0x00027673 File Offset: 0x00025873
		[Editor(false)]
		public bool IsOptionSelected
		{
			get
			{
				return this._isOptionSelected;
			}
			set
			{
				if (this._isOptionSelected != value)
				{
					this._isOptionSelected = value;
					base.OnPropertyChanged(value, "IsOptionSelected");
					this.OnSelectionChange(value);
					base.GamepadNavigationIndex = (value ? (-1) : 0);
				}
			}
		}

		// Token: 0x17000505 RID: 1285
		// (get) Token: 0x06000E22 RID: 3618 RVA: 0x000276A5 File Offset: 0x000258A5
		// (set) Token: 0x06000E23 RID: 3619 RVA: 0x000276AD File Offset: 0x000258AD
		[Editor(false)]
		public bool IsKingsOption
		{
			get
			{
				return this._isKingsOption;
			}
			set
			{
				if (this._isKingsOption != value)
				{
					this._isKingsOption = value;
					base.OnPropertyChanged(value, "IsKingsOption");
					this.HandleKingsOption();
				}
			}
		}

		// Token: 0x0400067E RID: 1662
		private float _animStartTime = -1f;

		// Token: 0x0400067F RID: 1663
		private bool _isKingsDecisionDone;

		// Token: 0x04000680 RID: 1664
		private bool _isOptionSelected;

		// Token: 0x04000681 RID: 1665
		public bool _isKingsOption;
	}
}
