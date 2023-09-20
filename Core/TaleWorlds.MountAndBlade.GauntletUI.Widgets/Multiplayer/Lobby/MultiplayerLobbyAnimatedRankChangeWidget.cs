using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Lobby
{
	// Token: 0x02000090 RID: 144
	public class MultiplayerLobbyAnimatedRankChangeWidget : Widget
	{
		// Token: 0x06000797 RID: 1943 RVA: 0x00016530 File Offset: 0x00014730
		public MultiplayerLobbyAnimatedRankChangeWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000798 RID: 1944 RVA: 0x00016550 File Offset: 0x00014750
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (!this.IsAnimationRequested)
			{
				return;
			}
			if (this._preAnimationTimeElapsed < this._animationDelay)
			{
				this._preAnimationTimeElapsed += dt;
				return;
			}
			if (this._animationTimeElapsed >= this._animationDuration)
			{
				this.NewRankName.SetGlobalAlphaRecursively(1f);
				this.NewRankSprite.SetGlobalAlphaRecursively(1f);
				this.OldRankName.SetGlobalAlphaRecursively(0f);
				this.OldRankSprite.SetGlobalAlphaRecursively(0f);
				this.NewRankSprite.ScaledSuggestedWidth = base.ScaledSuggestedWidth / 2f;
				this.NewRankSprite.ScaledSuggestedHeight = base.ScaledSuggestedHeight / 2f;
				return;
			}
			float num = MathF.Lerp(0f, 1f, this._animationTimeElapsed / this._animationDuration, 1E-05f);
			this.OldRankSprite.SetGlobalAlphaRecursively(1f - num);
			this.OldRankName.SetGlobalAlphaRecursively(1f - num);
			this.NewRankSprite.SetGlobalAlphaRecursively(num);
			this.NewRankName.SetGlobalAlphaRecursively(num);
			this.NewRankSprite.ScaledSuggestedWidth = MathF.Lerp(base.ScaledSuggestedWidth, base.ScaledSuggestedWidth / 2f, this._animationTimeElapsed / this._animationDuration, 1E-05f);
			this.NewRankSprite.ScaledSuggestedHeight = MathF.Lerp(base.ScaledSuggestedHeight, base.ScaledSuggestedHeight / 2f, this._animationTimeElapsed / this._animationDuration, 1E-05f);
			this._animationTimeElapsed += dt;
		}

		// Token: 0x06000799 RID: 1945 RVA: 0x000166DC File Offset: 0x000148DC
		private void StartAnimation()
		{
			this.NewRankName.SetGlobalAlphaRecursively(0f);
			this.NewRankSprite.SetGlobalAlphaRecursively(0f);
			this.OldRankName.SetGlobalAlphaRecursively(1f);
			this.OldRankSprite.SetGlobalAlphaRecursively(1f);
			this.OldRankSprite.ScaledSuggestedWidth = base.ScaledSuggestedWidth / 2f;
			this.OldRankSprite.ScaledSuggestedHeight = base.ScaledSuggestedHeight / 2f;
			this._preAnimationTimeElapsed = 0f;
			this._animationTimeElapsed = 0f;
		}

		// Token: 0x170002AC RID: 684
		// (get) Token: 0x0600079A RID: 1946 RVA: 0x0001676D File Offset: 0x0001496D
		// (set) Token: 0x0600079B RID: 1947 RVA: 0x00016775 File Offset: 0x00014975
		[Editor(false)]
		public bool IsAnimationRequested
		{
			get
			{
				return this._isAnimationRequested;
			}
			set
			{
				if (value != this._isAnimationRequested)
				{
					this._isAnimationRequested = value;
					base.OnPropertyChanged(value, "IsAnimationRequested");
					this.StartAnimation();
				}
			}
		}

		// Token: 0x170002AD RID: 685
		// (get) Token: 0x0600079C RID: 1948 RVA: 0x00016799 File Offset: 0x00014999
		// (set) Token: 0x0600079D RID: 1949 RVA: 0x000167A1 File Offset: 0x000149A1
		[Editor(false)]
		public bool IsPromoted
		{
			get
			{
				return this._isPromoted;
			}
			set
			{
				if (value != this._isPromoted)
				{
					this._isPromoted = value;
					base.OnPropertyChanged(value, "IsPromoted");
				}
			}
		}

		// Token: 0x170002AE RID: 686
		// (get) Token: 0x0600079E RID: 1950 RVA: 0x000167BF File Offset: 0x000149BF
		// (set) Token: 0x0600079F RID: 1951 RVA: 0x000167C7 File Offset: 0x000149C7
		[Editor(false)]
		public TextWidget OldRankName
		{
			get
			{
				return this._oldRankName;
			}
			set
			{
				if (value != this._oldRankName)
				{
					this._oldRankName = value;
					base.OnPropertyChanged<TextWidget>(value, "OldRankName");
				}
			}
		}

		// Token: 0x170002AF RID: 687
		// (get) Token: 0x060007A0 RID: 1952 RVA: 0x000167E5 File Offset: 0x000149E5
		// (set) Token: 0x060007A1 RID: 1953 RVA: 0x000167ED File Offset: 0x000149ED
		[Editor(false)]
		public TextWidget NewRankName
		{
			get
			{
				return this._newRankName;
			}
			set
			{
				if (value != this._newRankName)
				{
					this._newRankName = value;
					base.OnPropertyChanged<TextWidget>(value, "NewRankName");
				}
			}
		}

		// Token: 0x170002B0 RID: 688
		// (get) Token: 0x060007A2 RID: 1954 RVA: 0x0001680B File Offset: 0x00014A0B
		// (set) Token: 0x060007A3 RID: 1955 RVA: 0x00016813 File Offset: 0x00014A13
		[Editor(false)]
		public MultiplayerLobbyRankItemButtonWidget OldRankSprite
		{
			get
			{
				return this._oldRankSprite;
			}
			set
			{
				if (value != this._oldRankSprite)
				{
					this._oldRankSprite = value;
					base.OnPropertyChanged<MultiplayerLobbyRankItemButtonWidget>(value, "OldRankSprite");
				}
			}
		}

		// Token: 0x170002B1 RID: 689
		// (get) Token: 0x060007A4 RID: 1956 RVA: 0x00016831 File Offset: 0x00014A31
		// (set) Token: 0x060007A5 RID: 1957 RVA: 0x00016839 File Offset: 0x00014A39
		[Editor(false)]
		public MultiplayerLobbyRankItemButtonWidget NewRankSprite
		{
			get
			{
				return this._newRankSprite;
			}
			set
			{
				if (value != this._newRankSprite)
				{
					this._newRankSprite = value;
					base.OnPropertyChanged<MultiplayerLobbyRankItemButtonWidget>(value, "NewRankSprite");
				}
			}
		}

		// Token: 0x0400036B RID: 875
		private float _animationTimeElapsed;

		// Token: 0x0400036C RID: 876
		private float _animationDuration = 0.25f;

		// Token: 0x0400036D RID: 877
		private float _preAnimationTimeElapsed;

		// Token: 0x0400036E RID: 878
		private float _animationDelay = 0.5f;

		// Token: 0x0400036F RID: 879
		private bool _isAnimationRequested;

		// Token: 0x04000370 RID: 880
		private bool _isPromoted;

		// Token: 0x04000371 RID: 881
		private TextWidget _oldRankName;

		// Token: 0x04000372 RID: 882
		private TextWidget _newRankName;

		// Token: 0x04000373 RID: 883
		private MultiplayerLobbyRankItemButtonWidget _oldRankSprite;

		// Token: 0x04000374 RID: 884
		private MultiplayerLobbyRankItemButtonWidget _newRankSprite;
	}
}
