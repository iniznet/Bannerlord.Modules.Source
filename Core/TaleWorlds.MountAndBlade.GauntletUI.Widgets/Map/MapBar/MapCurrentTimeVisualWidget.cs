using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Map.MapBar
{
	public class MapCurrentTimeVisualWidget : Widget
	{
		public MapCurrentTimeVisualWidget(UIContext context)
			: base(context)
		{
			base.AddState("Disabled");
		}

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

		private int _currenTimeState;

		private ButtonWidget _fastForwardButton;

		private ButtonWidget _playButton;

		private ButtonWidget _pauseButton;
	}
}
