using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	// Token: 0x02000039 RID: 57
	public class TabControlWidget : Widget
	{
		// Token: 0x06000318 RID: 792 RVA: 0x00009FE0 File Offset: 0x000081E0
		public TabControlWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000319 RID: 793 RVA: 0x00009FEC File Offset: 0x000081EC
		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			if (!this.FirstButton.ClickEventHandlers.Contains(new Action<Widget>(this.OnFirstButtonClick)))
			{
				this.FirstButton.ClickEventHandlers.Add(new Action<Widget>(this.OnFirstButtonClick));
			}
			if (!this.SecondButton.ClickEventHandlers.Contains(new Action<Widget>(this.OnSecondButtonClick)))
			{
				this.SecondButton.ClickEventHandlers.Add(new Action<Widget>(this.OnSecondButtonClick));
			}
			this.FirstButton.IsSelected = this.FirstItem.IsVisible;
			this.SecondButton.IsSelected = this.SecondItem.IsVisible;
		}

		// Token: 0x0600031A RID: 794 RVA: 0x0000A0A0 File Offset: 0x000082A0
		public void OnFirstButtonClick(Widget widget)
		{
			if (!this._firstItem.IsVisible && this._secondItem.IsVisible)
			{
				this._secondItem.IsVisible = false;
				this._firstItem.IsVisible = true;
			}
		}

		// Token: 0x0600031B RID: 795 RVA: 0x0000A0D4 File Offset: 0x000082D4
		public void OnSecondButtonClick(Widget widget)
		{
			if (this._firstItem.IsVisible && !this._secondItem.IsVisible)
			{
				this._secondItem.IsVisible = true;
				this._firstItem.IsVisible = false;
			}
		}

		// Token: 0x17000116 RID: 278
		// (get) Token: 0x0600031C RID: 796 RVA: 0x0000A108 File Offset: 0x00008308
		// (set) Token: 0x0600031D RID: 797 RVA: 0x0000A110 File Offset: 0x00008310
		[Editor(false)]
		public ButtonWidget FirstButton
		{
			get
			{
				return this._firstButton;
			}
			set
			{
				if (this._firstButton != value)
				{
					this._firstButton = value;
					base.OnPropertyChanged<ButtonWidget>(value, "FirstButton");
				}
			}
		}

		// Token: 0x17000117 RID: 279
		// (get) Token: 0x0600031E RID: 798 RVA: 0x0000A12E File Offset: 0x0000832E
		// (set) Token: 0x0600031F RID: 799 RVA: 0x0000A136 File Offset: 0x00008336
		[Editor(false)]
		public ButtonWidget SecondButton
		{
			get
			{
				return this._secondButton;
			}
			set
			{
				if (this._secondButton != value)
				{
					this._secondButton = value;
					base.OnPropertyChanged<ButtonWidget>(value, "SecondButton");
				}
			}
		}

		// Token: 0x17000118 RID: 280
		// (get) Token: 0x06000320 RID: 800 RVA: 0x0000A154 File Offset: 0x00008354
		// (set) Token: 0x06000321 RID: 801 RVA: 0x0000A15C File Offset: 0x0000835C
		[Editor(false)]
		public Widget SecondItem
		{
			get
			{
				return this._secondItem;
			}
			set
			{
				if (this._secondItem != value)
				{
					this._secondItem = value;
					base.OnPropertyChanged<Widget>(value, "SecondItem");
				}
			}
		}

		// Token: 0x17000119 RID: 281
		// (get) Token: 0x06000322 RID: 802 RVA: 0x0000A17A File Offset: 0x0000837A
		// (set) Token: 0x06000323 RID: 803 RVA: 0x0000A182 File Offset: 0x00008382
		[Editor(false)]
		public Widget FirstItem
		{
			get
			{
				return this._firstItem;
			}
			set
			{
				if (this._firstItem != value)
				{
					this._firstItem = value;
					base.OnPropertyChanged<Widget>(value, "FirstItem");
				}
			}
		}

		// Token: 0x04000148 RID: 328
		private ButtonWidget _firstButton;

		// Token: 0x04000149 RID: 329
		private ButtonWidget _secondButton;

		// Token: 0x0400014A RID: 330
		private Widget _firstItem;

		// Token: 0x0400014B RID: 331
		private Widget _secondItem;
	}
}
