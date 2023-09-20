using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Lobby
{
	public class MultiplayerLobbyAnimatedRankChangeWidget : Widget
	{
		public MultiplayerLobbyAnimatedRankChangeWidget(UIContext context)
			: base(context)
		{
		}

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

		private float _animationTimeElapsed;

		private float _animationDuration = 0.25f;

		private float _preAnimationTimeElapsed;

		private float _animationDelay = 0.5f;

		private bool _isAnimationRequested;

		private bool _isPromoted;

		private TextWidget _oldRankName;

		private TextWidget _newRankName;

		private MultiplayerLobbyRankItemButtonWidget _oldRankSprite;

		private MultiplayerLobbyRankItemButtonWidget _newRankSprite;
	}
}
