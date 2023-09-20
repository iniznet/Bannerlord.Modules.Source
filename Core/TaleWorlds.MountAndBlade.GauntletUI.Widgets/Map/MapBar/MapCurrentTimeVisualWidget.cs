using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Map.MapBar
{
	// Token: 0x0200010C RID: 268
	public class MapCurrentTimeVisualWidget : Widget
	{
		// Token: 0x06000DB7 RID: 3511 RVA: 0x00026774 File Offset: 0x00024974
		public MapCurrentTimeVisualWidget(UIContext context)
			: base(context)
		{
			base.AddState("Disabled");
		}

		// Token: 0x06000DB8 RID: 3512 RVA: 0x00026788 File Offset: 0x00024988
		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			if (base.IsDisabled)
			{
				this.SetState("Disabled");
				return;
			}
			this.SetState("Default");
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			switch (this.CurrentTimeState)
			{
			case 0:
			case 6:
				flag3 = true;
				break;
			case 1:
			case 3:
				flag = true;
				break;
			case 2:
			case 4:
			case 5:
				flag2 = true;
				break;
			}
			this.PlayButton.IsSelected = flag;
			this.FastForwardButton.IsSelected = flag2;
			this.PauseButton.IsSelected = flag3;
		}

		// Token: 0x170004E2 RID: 1250
		// (get) Token: 0x06000DB9 RID: 3513 RVA: 0x0002681A File Offset: 0x00024A1A
		// (set) Token: 0x06000DBA RID: 3514 RVA: 0x00026822 File Offset: 0x00024A22
		[Editor(false)]
		public int CurrentTimeState
		{
			get
			{
				return this._currenTimeState;
			}
			set
			{
				if (this._currenTimeState != value)
				{
					this._currenTimeState = value;
					base.OnPropertyChanged(value, "CurrentTimeState");
				}
			}
		}

		// Token: 0x170004E3 RID: 1251
		// (get) Token: 0x06000DBB RID: 3515 RVA: 0x00026840 File Offset: 0x00024A40
		// (set) Token: 0x06000DBC RID: 3516 RVA: 0x00026848 File Offset: 0x00024A48
		[Editor(false)]
		public ButtonWidget FastForwardButton
		{
			get
			{
				return this._fastForwardButton;
			}
			set
			{
				if (this._fastForwardButton != value)
				{
					this._fastForwardButton = value;
					base.OnPropertyChanged<ButtonWidget>(value, "FastForwardButton");
				}
			}
		}

		// Token: 0x170004E4 RID: 1252
		// (get) Token: 0x06000DBD RID: 3517 RVA: 0x00026866 File Offset: 0x00024A66
		// (set) Token: 0x06000DBE RID: 3518 RVA: 0x0002686E File Offset: 0x00024A6E
		[Editor(false)]
		public ButtonWidget PlayButton
		{
			get
			{
				return this._playButton;
			}
			set
			{
				if (this._playButton != value)
				{
					this._playButton = value;
					base.OnPropertyChanged<ButtonWidget>(value, "PlayButton");
				}
			}
		}

		// Token: 0x170004E5 RID: 1253
		// (get) Token: 0x06000DBF RID: 3519 RVA: 0x0002688C File Offset: 0x00024A8C
		// (set) Token: 0x06000DC0 RID: 3520 RVA: 0x00026894 File Offset: 0x00024A94
		[Editor(false)]
		public ButtonWidget PauseButton
		{
			get
			{
				return this._pauseButton;
			}
			set
			{
				if (this._pauseButton != value)
				{
					this._pauseButton = value;
					base.OnPropertyChanged<ButtonWidget>(value, "PauseButton");
				}
			}
		}

		// Token: 0x04000653 RID: 1619
		private int _currenTimeState;

		// Token: 0x04000654 RID: 1620
		private ButtonWidget _fastForwardButton;

		// Token: 0x04000655 RID: 1621
		private ButtonWidget _playButton;

		// Token: 0x04000656 RID: 1622
		private ButtonWidget _pauseButton;
	}
}
