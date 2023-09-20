using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Scoreboard
{
	// Token: 0x02000085 RID: 133
	public class MultiplayerScoreboardEndOfBattlePanelWidget : Widget
	{
		// Token: 0x06000722 RID: 1826 RVA: 0x00015483 File Offset: 0x00013683
		public MultiplayerScoreboardEndOfBattlePanelWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000723 RID: 1827 RVA: 0x00015498 File Offset: 0x00013698
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

		// Token: 0x06000724 RID: 1828 RVA: 0x0001552A File Offset: 0x0001372A
		public void StartAnimation()
		{
			this._isStarted = true;
			this._isFinished = false;
			this._isPreStateFinished = false;
			this._timePassed = 0f;
			base.AddState("PreOpened");
			base.AddState("Opened");
		}

		// Token: 0x06000725 RID: 1829 RVA: 0x00015562 File Offset: 0x00013762
		private void Reset()
		{
			this._isStarted = false;
			this._isPreStateFinished = false;
			this._isFinished = false;
			this.SetState("Default");
		}

		// Token: 0x06000726 RID: 1830 RVA: 0x00015584 File Offset: 0x00013784
		private void AvailableUpdated()
		{
			if (this.IsAvailable)
			{
				this.StartAnimation();
				return;
			}
			this.Reset();
		}

		// Token: 0x17000285 RID: 645
		// (get) Token: 0x06000727 RID: 1831 RVA: 0x0001559B File Offset: 0x0001379B
		// (set) Token: 0x06000728 RID: 1832 RVA: 0x000155A3 File Offset: 0x000137A3
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

		// Token: 0x17000286 RID: 646
		// (get) Token: 0x06000729 RID: 1833 RVA: 0x000155C7 File Offset: 0x000137C7
		// (set) Token: 0x0600072A RID: 1834 RVA: 0x000155CF File Offset: 0x000137CF
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

		// Token: 0x17000287 RID: 647
		// (get) Token: 0x0600072B RID: 1835 RVA: 0x000155ED File Offset: 0x000137ED
		// (set) Token: 0x0600072C RID: 1836 RVA: 0x000155F5 File Offset: 0x000137F5
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

		// Token: 0x04000333 RID: 819
		private bool _isStarted;

		// Token: 0x04000334 RID: 820
		private bool _isPreStateFinished;

		// Token: 0x04000335 RID: 821
		private bool _isFinished;

		// Token: 0x04000336 RID: 822
		private float _timePassed;

		// Token: 0x04000337 RID: 823
		private string _openedSoundEvent = "panels/scoreboard_flags";

		// Token: 0x04000338 RID: 824
		private bool _isAvailable;

		// Token: 0x04000339 RID: 825
		private float _firstDelay;

		// Token: 0x0400033A RID: 826
		private float _secondDelay;
	}
}
