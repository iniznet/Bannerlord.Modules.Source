using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Scoreboard
{
	// Token: 0x0200004A RID: 74
	public class ScoreboardBattleRewardsWidget : Widget
	{
		// Token: 0x060003E7 RID: 999 RVA: 0x0000CBFC File Offset: 0x0000ADFC
		public ScoreboardBattleRewardsWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060003E8 RID: 1000 RVA: 0x0000CC1B File Offset: 0x0000AE1B
		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			if (this._isAnimationActive)
			{
				this.UpdateAnimation(dt);
			}
		}

		// Token: 0x060003E9 RID: 1001 RVA: 0x0000CC34 File Offset: 0x0000AE34
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

		// Token: 0x060003EA RID: 1002 RVA: 0x0000CCA0 File Offset: 0x0000AEA0
		public void Reset()
		{
			for (int i = 0; i < this.ItemContainer.ChildCount; i++)
			{
				this.ItemContainer.GetChild(i).IsVisible = false;
			}
		}

		// Token: 0x060003EB RID: 1003 RVA: 0x0000CCD8 File Offset: 0x0000AED8
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

		// Token: 0x1700015E RID: 350
		// (get) Token: 0x060003EC RID: 1004 RVA: 0x0000CD85 File Offset: 0x0000AF85
		// (set) Token: 0x060003ED RID: 1005 RVA: 0x0000CD8D File Offset: 0x0000AF8D
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

		// Token: 0x1700015F RID: 351
		// (get) Token: 0x060003EE RID: 1006 RVA: 0x0000CDAB File Offset: 0x0000AFAB
		// (set) Token: 0x060003EF RID: 1007 RVA: 0x0000CDB3 File Offset: 0x0000AFB3
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

		// Token: 0x17000160 RID: 352
		// (get) Token: 0x060003F0 RID: 1008 RVA: 0x0000CDD1 File Offset: 0x0000AFD1
		// (set) Token: 0x060003F1 RID: 1009 RVA: 0x0000CDD9 File Offset: 0x0000AFD9
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

		// Token: 0x040001B1 RID: 433
		private bool _isAnimationActive;

		// Token: 0x040001B2 RID: 434
		private float _animationTimePassed;

		// Token: 0x040001B3 RID: 435
		private int _animationLastItemIndex;

		// Token: 0x040001B4 RID: 436
		private float _animationDelay = 1f;

		// Token: 0x040001B5 RID: 437
		private float _animationInterval = 0.25f;

		// Token: 0x040001B6 RID: 438
		private Widget _itemContainer;
	}
}
