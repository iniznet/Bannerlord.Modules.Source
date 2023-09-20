using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Scoreboard
{
	public class ScoreboardBattleRewardsWidget : Widget
	{
		public ScoreboardBattleRewardsWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			if (this._isAnimationActive)
			{
				this.UpdateAnimation(dt);
			}
		}

		public void StartAnimation()
		{
			this._isAnimationActive = true;
			this._animationTimePassed = 0f;
			this._animationLastItemIndex = -1;
			this.ItemContainer.SetState("Opened");
			for (int i = 0; i < this.ItemContainer.ChildCount; i++)
			{
				Widget child = this.ItemContainer.GetChild(i);
				child.IsVisible = false;
				child.AddState("Opened");
			}
		}

		public void Reset()
		{
			for (int i = 0; i < this.ItemContainer.ChildCount; i++)
			{
				this.ItemContainer.GetChild(i).IsVisible = false;
			}
		}

		private void UpdateAnimation(float dt)
		{
			if (this._animationTimePassed >= this.AnimationDelay + this.AnimationInterval * (float)this.ItemContainer.ChildCount)
			{
				return;
			}
			if (this._animationTimePassed >= this.AnimationDelay)
			{
				int num = MathF.Floor((this._animationTimePassed - this.AnimationDelay) / this.AnimationInterval);
				if (num != this._animationLastItemIndex && num < this.ItemContainer.ChildCount)
				{
					for (int i = this._animationLastItemIndex + 1; i <= num; i++)
					{
						Widget child = this.ItemContainer.GetChild(i);
						child.IsVisible = true;
						child.SetState("Opened");
					}
				}
			}
			this._animationTimePassed += dt;
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
		public float AnimationInterval
		{
			get
			{
				return this._animationInterval;
			}
			set
			{
				if (this._animationInterval != value)
				{
					this._animationInterval = value;
					base.OnPropertyChanged(value, "AnimationInterval");
				}
			}
		}

		[Editor(false)]
		public Widget ItemContainer
		{
			get
			{
				return this._itemContainer;
			}
			set
			{
				if (this._itemContainer != value)
				{
					this._itemContainer = value;
					base.OnPropertyChanged<Widget>(value, "ItemContainer");
				}
			}
		}

		private bool _isAnimationActive;

		private float _animationTimePassed;

		private int _animationLastItemIndex;

		private float _animationDelay = 1f;

		private float _animationInterval = 0.25f;

		private Widget _itemContainer;
	}
}
