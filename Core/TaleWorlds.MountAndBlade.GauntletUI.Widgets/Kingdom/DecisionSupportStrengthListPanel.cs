using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Kingdom
{
	// Token: 0x02000111 RID: 273
	public class DecisionSupportStrengthListPanel : ListPanel
	{
		// Token: 0x170004EE RID: 1262
		// (get) Token: 0x06000DE0 RID: 3552 RVA: 0x00026E0A File Offset: 0x0002500A
		// (set) Token: 0x06000DE1 RID: 3553 RVA: 0x00026E12 File Offset: 0x00025012
		public bool IsAbstain { get; set; }

		// Token: 0x170004EF RID: 1263
		// (get) Token: 0x06000DE2 RID: 3554 RVA: 0x00026E1B File Offset: 0x0002501B
		// (set) Token: 0x06000DE3 RID: 3555 RVA: 0x00026E23 File Offset: 0x00025023
		public bool IsPlayerSupporter { get; set; }

		// Token: 0x170004F0 RID: 1264
		// (get) Token: 0x06000DE4 RID: 3556 RVA: 0x00026E2C File Offset: 0x0002502C
		// (set) Token: 0x06000DE5 RID: 3557 RVA: 0x00026E34 File Offset: 0x00025034
		public bool IsOptionSelected { get; set; }

		// Token: 0x170004F1 RID: 1265
		// (get) Token: 0x06000DE6 RID: 3558 RVA: 0x00026E3D File Offset: 0x0002503D
		// (set) Token: 0x06000DE7 RID: 3559 RVA: 0x00026E45 File Offset: 0x00025045
		public bool IsKingsOutcome { get; set; }

		// Token: 0x06000DE8 RID: 3560 RVA: 0x00026E4E File Offset: 0x0002504E
		public DecisionSupportStrengthListPanel(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000DE9 RID: 3561 RVA: 0x00026E58 File Offset: 0x00025058
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			switch (this.CurrentIndex)
			{
			case 2:
				this.StrengthButton0.IsSelected = true;
				this.StrengthButton1.IsSelected = false;
				this.StrengthButton2.IsSelected = false;
				break;
			case 3:
				this.StrengthButton0.IsSelected = false;
				this.StrengthButton1.IsSelected = true;
				this.StrengthButton2.IsSelected = false;
				break;
			case 4:
				this.StrengthButton0.IsSelected = false;
				this.StrengthButton1.IsSelected = false;
				this.StrengthButton2.IsSelected = true;
				break;
			}
			base.GamepadNavigationIndex = (this.IsOptionSelected ? (-1) : 0);
		}

		// Token: 0x06000DEA RID: 3562 RVA: 0x00026F0B File Offset: 0x0002510B
		private void SetButtonsEnabled(bool isEnabled)
		{
			this.StrengthButton0.IsEnabled = isEnabled;
			this.StrengthButton1.IsEnabled = isEnabled;
			this.StrengthButton2.IsEnabled = isEnabled;
		}

		// Token: 0x170004F2 RID: 1266
		// (get) Token: 0x06000DEB RID: 3563 RVA: 0x00026F31 File Offset: 0x00025131
		// (set) Token: 0x06000DEC RID: 3564 RVA: 0x00026F39 File Offset: 0x00025139
		[Editor(false)]
		public int CurrentIndex
		{
			get
			{
				return this._currentIndex;
			}
			set
			{
				if (this._currentIndex != value)
				{
					this._currentIndex = value;
					base.OnPropertyChanged(value, "CurrentIndex");
				}
			}
		}

		// Token: 0x170004F3 RID: 1267
		// (get) Token: 0x06000DED RID: 3565 RVA: 0x00026F57 File Offset: 0x00025157
		// (set) Token: 0x06000DEE RID: 3566 RVA: 0x00026F5F File Offset: 0x0002515F
		[Editor(false)]
		public ButtonWidget StrengthButton0
		{
			get
			{
				return this._strengthButton0;
			}
			set
			{
				if (this._strengthButton0 != value)
				{
					this._strengthButton0 = value;
					base.OnPropertyChanged<ButtonWidget>(value, "StrengthButton0");
				}
			}
		}

		// Token: 0x170004F4 RID: 1268
		// (get) Token: 0x06000DEF RID: 3567 RVA: 0x00026F7D File Offset: 0x0002517D
		// (set) Token: 0x06000DF0 RID: 3568 RVA: 0x00026F85 File Offset: 0x00025185
		[Editor(false)]
		public ButtonWidget StrengthButton1
		{
			get
			{
				return this._strengthButton1;
			}
			set
			{
				if (this._strengthButton1 != value)
				{
					this._strengthButton1 = value;
					base.OnPropertyChanged<ButtonWidget>(value, "StrengthButton1");
				}
			}
		}

		// Token: 0x170004F5 RID: 1269
		// (get) Token: 0x06000DF1 RID: 3569 RVA: 0x00026FA3 File Offset: 0x000251A3
		// (set) Token: 0x06000DF2 RID: 3570 RVA: 0x00026FAB File Offset: 0x000251AB
		[Editor(false)]
		public ButtonWidget StrengthButton2
		{
			get
			{
				return this._strengthButton2;
			}
			set
			{
				if (this._strengthButton2 != value)
				{
					this._strengthButton2 = value;
					base.OnPropertyChanged<ButtonWidget>(value, "StrengthButton2");
				}
			}
		}

		// Token: 0x170004F6 RID: 1270
		// (get) Token: 0x06000DF3 RID: 3571 RVA: 0x00026FC9 File Offset: 0x000251C9
		// (set) Token: 0x06000DF4 RID: 3572 RVA: 0x00026FD1 File Offset: 0x000251D1
		[Editor(false)]
		public RichTextWidget StrengthButton0Text
		{
			get
			{
				return this._strengthButton0Text;
			}
			set
			{
				if (this._strengthButton0Text != value)
				{
					this._strengthButton0Text = value;
					base.OnPropertyChanged<RichTextWidget>(value, "StrengthButton0Text");
				}
			}
		}

		// Token: 0x170004F7 RID: 1271
		// (get) Token: 0x06000DF5 RID: 3573 RVA: 0x00026FEF File Offset: 0x000251EF
		// (set) Token: 0x06000DF6 RID: 3574 RVA: 0x00026FF7 File Offset: 0x000251F7
		[Editor(false)]
		public RichTextWidget StrengthButton1Text
		{
			get
			{
				return this._strengthButton1Text;
			}
			set
			{
				if (this._strengthButton1Text != value)
				{
					this._strengthButton1Text = value;
					base.OnPropertyChanged<RichTextWidget>(value, "StrengthButton1Text");
				}
			}
		}

		// Token: 0x170004F8 RID: 1272
		// (get) Token: 0x06000DF7 RID: 3575 RVA: 0x00027015 File Offset: 0x00025215
		// (set) Token: 0x06000DF8 RID: 3576 RVA: 0x0002701D File Offset: 0x0002521D
		[Editor(false)]
		public RichTextWidget StrengthButton2Text
		{
			get
			{
				return this._strengthButton2Text;
			}
			set
			{
				if (this._strengthButton2Text != value)
				{
					this._strengthButton2Text = value;
					base.OnPropertyChanged<RichTextWidget>(value, "StrengthButton2Text");
				}
			}
		}

		// Token: 0x04000667 RID: 1639
		private ButtonWidget _strengthButton0;

		// Token: 0x04000668 RID: 1640
		private RichTextWidget _strengthButton0Text;

		// Token: 0x04000669 RID: 1641
		private ButtonWidget _strengthButton1;

		// Token: 0x0400066A RID: 1642
		private RichTextWidget _strengthButton1Text;

		// Token: 0x0400066B RID: 1643
		private ButtonWidget _strengthButton2;

		// Token: 0x0400066C RID: 1644
		private RichTextWidget _strengthButton2Text;

		// Token: 0x0400066D RID: 1645
		private int _currentIndex;
	}
}
