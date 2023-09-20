using System;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.GauntletUI.ExtraWidgets
{
	public class AnimatedNumberTextWidget : TextWidget
	{
		public AnimatedNumberTextWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnUpdate(float dt)
		{
			if (!this._isAnimationActive)
			{
				return;
			}
			this._timePassed += dt;
			if (this._timePassed >= this.AnimationDelay)
			{
				float num = this._timePassed - this.AnimationDelay;
				this._currentNumber = (int)(num / this.AnimationDuration * (float)this.ReferenceNumber);
				this._currentNumber = MathF.Min(this._currentNumber, this.Number);
				if (this._currentNumber == this.Number)
				{
					this._isAnimationActive = false;
				}
				base.IntText = this._currentNumber;
			}
		}

		public void StartAnimation()
		{
			if (this.AnimationDuration <= 0f || this.ReferenceNumber <= 0)
			{
				return;
			}
			this._isAnimationActive = true;
			this._currentNumber = 0;
			base.IntText = 0;
			this._timePassed = 0f;
		}

		public void Reset()
		{
			this._isAnimationActive = false;
			this._currentNumber = 0;
			base.IntText = 0;
		}

		private void NumberChanged()
		{
			if (this.AutoStart)
			{
				this.StartAnimation();
			}
		}

		[Editor(false)]
		public float AnimationDelay
		{
			get
			{
				return this._animationDelay;
			}
			set
			{
				if (this._animationDelay != value)
				{
					this._animationDelay = value;
					base.OnPropertyChanged(value, "AnimationDelay");
				}
			}
		}

		[Editor(false)]
		public float AnimationDuration
		{
			get
			{
				return this._animationDuration;
			}
			set
			{
				if (this._animationDuration != value)
				{
					this._animationDuration = value;
					base.OnPropertyChanged(value, "AnimationDuration");
				}
			}
		}

		[Editor(false)]
		public int ReferenceNumber
		{
			get
			{
				return this._referenceNumber;
			}
			set
			{
				if (this._referenceNumber != value)
				{
					this._referenceNumber = value;
					base.OnPropertyChanged(value, "ReferenceNumber");
				}
			}
		}

		[Editor(false)]
		public int Number
		{
			get
			{
				return this._number;
			}
			set
			{
				if (this._number != value)
				{
					this._number = value;
					base.OnPropertyChanged(value, "Number");
					this.NumberChanged();
				}
			}
		}

		[Editor(false)]
		public bool AutoStart
		{
			get
			{
				return this._autoStart;
			}
			set
			{
				if (this._autoStart != value)
				{
					this._autoStart = value;
					base.OnPropertyChanged(value, "AutoStart");
				}
			}
		}

		private int _currentNumber;

		private bool _isAnimationActive;

		private float _timePassed;

		private float _animationDelay;

		private float _animationDuration;

		private int _referenceNumber;

		private int _number;

		private bool _autoStart;
	}
}
