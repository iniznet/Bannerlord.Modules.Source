using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Scoreboard
{
	public class MultiplayerScoreboardEndOfBattlePanelWidget : Widget
	{
		public MultiplayerScoreboardEndOfBattlePanelWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			if (this._isFinished || !this._isStarted)
			{
				return;
			}
			this._timePassed += dt;
			if (this._timePassed >= this.SecondDelay)
			{
				this._isFinished = true;
				this.SetState("Opened");
				base.Context.TwoDimensionContext.PlaySound(this._openedSoundEvent);
				return;
			}
			if (this._timePassed >= this.FirstDelay && !this._isPreStateFinished)
			{
				this._isPreStateFinished = true;
				this.SetState("PreOpened");
			}
		}

		public void StartAnimation()
		{
			this._isStarted = true;
			this._isFinished = false;
			this._isPreStateFinished = false;
			this._timePassed = 0f;
			base.AddState("PreOpened");
			base.AddState("Opened");
		}

		private void Reset()
		{
			this._isStarted = false;
			this._isPreStateFinished = false;
			this._isFinished = false;
			this.SetState("Default");
		}

		private void AvailableUpdated()
		{
			if (this.IsAvailable)
			{
				this.StartAnimation();
				return;
			}
			this.Reset();
		}

		[Editor(false)]
		public bool IsAvailable
		{
			get
			{
				return this._isAvailable;
			}
			set
			{
				if (value != this._isAvailable)
				{
					this._isAvailable = value;
					base.OnPropertyChanged(value, "IsAvailable");
					this.AvailableUpdated();
				}
			}
		}

		[Editor(false)]
		public float FirstDelay
		{
			get
			{
				return this._firstDelay;
			}
			set
			{
				if (value != this._firstDelay)
				{
					this._firstDelay = value;
					base.OnPropertyChanged(value, "FirstDelay");
				}
			}
		}

		[Editor(false)]
		public float SecondDelay
		{
			get
			{
				return this._secondDelay;
			}
			set
			{
				if (value != this._secondDelay)
				{
					this._secondDelay = value;
					base.OnPropertyChanged(value, "SecondDelay");
				}
			}
		}

		private bool _isStarted;

		private bool _isPreStateFinished;

		private bool _isFinished;

		private float _timePassed;

		private string _openedSoundEvent = "panels/scoreboard_flags";

		private bool _isAvailable;

		private float _firstDelay;

		private float _secondDelay;
	}
}
