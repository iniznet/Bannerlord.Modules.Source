using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	public class InitialMenuAnimControllerWidget : Widget
	{
		public bool IsAnimEnabled { get; set; }

		public InitialMenuAnimControllerWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this.IsAnimEnabled)
			{
				if (!this._isInitialized)
				{
					Widget optionsList = this.OptionsList;
					bool flag;
					if (optionsList == null)
					{
						flag = false;
					}
					else
					{
						List<Widget> children = optionsList.Children;
						int? num = ((children != null) ? new int?(children.Count) : null);
						int num2 = 0;
						flag = (num.GetValueOrDefault() > num2) & (num != null);
					}
					if (flag)
					{
						this.OptionsList.Children.ForEach(delegate(Widget x)
						{
							x.SetGlobalAlphaRecursively(0f);
						});
						this._totalOptionCount = this.OptionsList.Children.Count;
						this._isInitialized = true;
					}
				}
				if (this._isInitialized && !this._isFinalized && this.OptionsList != null)
				{
					this._timer += dt;
					if (this._timer >= this.InitialWaitTime + (float)this._currentOptionIndex * this.WaitTimeBetweenOptions)
					{
						Widget child = this.OptionsList.GetChild(this._currentOptionIndex);
						if (child != null)
						{
							child.SetState("Activated");
						}
						this._currentOptionIndex++;
					}
					for (int i = 0; i < this._currentOptionIndex; i++)
					{
						float num3 = this.InitialWaitTime + this.WaitTimeBetweenOptions * (float)i;
						float num4 = num3 + this.OptionFadeInTime;
						if (this._timer < num4)
						{
							float num5 = MathF.Clamp((this._timer - num3) / (num4 - num3), 0f, 1f);
							Widget child2 = this.OptionsList.GetChild(i);
							if (child2 != null)
							{
								child2.SetGlobalAlphaRecursively(num5);
							}
						}
					}
					this._isFinalized = this._timer > this.InitialWaitTime + this.WaitTimeBetweenOptions * (float)(this._totalOptionCount - 1) + this.OptionFadeInTime;
				}
			}
		}

		[Editor(false)]
		public Widget OptionsList
		{
			get
			{
				return this._optionsList;
			}
			set
			{
				if (this._optionsList != value)
				{
					this._optionsList = value;
					base.OnPropertyChanged<Widget>(value, "OptionsList");
				}
			}
		}

		[Editor(false)]
		public float InitialWaitTime
		{
			get
			{
				return this._initialWaitTime;
			}
			set
			{
				if (this._initialWaitTime != value)
				{
					this._initialWaitTime = value;
					base.OnPropertyChanged(value, "InitialWaitTime");
				}
			}
		}

		[Editor(false)]
		public float WaitTimeBetweenOptions
		{
			get
			{
				return this._waitTimeBetweenOptions;
			}
			set
			{
				if (this._waitTimeBetweenOptions != value)
				{
					this._waitTimeBetweenOptions = value;
					base.OnPropertyChanged(value, "WaitTimeBetweenOptions");
				}
			}
		}

		[Editor(false)]
		public float OptionFadeInTime
		{
			get
			{
				return this._optionFadeInTime;
			}
			set
			{
				if (this._optionFadeInTime != value)
				{
					this._optionFadeInTime = value;
					base.OnPropertyChanged(value, "OptionFadeInTime");
				}
			}
		}

		private bool _isInitialized;

		private bool _isFinalized;

		private int _currentOptionIndex;

		private int _totalOptionCount;

		private float _timer;

		private Widget _optionsList;

		private float _initialWaitTime;

		private float _waitTimeBetweenOptions;

		private float _optionFadeInTime;
	}
}
