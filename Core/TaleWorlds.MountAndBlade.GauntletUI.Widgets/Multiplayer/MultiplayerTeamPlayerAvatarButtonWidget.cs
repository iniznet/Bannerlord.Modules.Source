using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer
{
	public class MultiplayerTeamPlayerAvatarButtonWidget : ButtonWidget
	{
		public MultiplayerTeamPlayerAvatarButtonWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (!this._isInitialized && this.AvatarImage != null)
			{
				this._originalAvatarImageAlpha = this.AvatarImage.ReadOnlyBrush.GlobalAlphaFactor;
				this.UpdateGlobalAlpha();
				this._isInitialized = true;
			}
		}

		private void UpdateGlobalAlpha()
		{
			if (this._isInitialized)
			{
				float num = (this.IsDead ? this.DeathAlphaFactor : 1f);
				float num2 = num * this._originalAvatarImageAlpha;
				this.SetGlobalAlphaRecursively(num);
				this.AvatarImage.Brush.GlobalAlphaFactor = num2;
			}
		}

		[DataSourceProperty]
		public bool IsDead
		{
			get
			{
				return this._isDead;
			}
			set
			{
				if (this._isDead != value)
				{
					this._isDead = value;
					base.OnPropertyChanged(value, "IsDead");
					this.UpdateGlobalAlpha();
				}
			}
		}

		[DataSourceProperty]
		public float DeathAlphaFactor
		{
			get
			{
				return this._deathAlphaFactor;
			}
			set
			{
				if (this._deathAlphaFactor != value)
				{
					this._deathAlphaFactor = value;
					base.OnPropertyChanged(value, "DeathAlphaFactor");
				}
			}
		}

		[DataSourceProperty]
		public ImageIdentifierWidget AvatarImage
		{
			get
			{
				return this._avatarImage;
			}
			set
			{
				if (this._avatarImage != value)
				{
					this._avatarImage = value;
					base.OnPropertyChanged<ImageIdentifierWidget>(value, "AvatarImage");
				}
			}
		}

		private bool _isInitialized;

		private float _originalAvatarImageAlpha = 1f;

		private bool _isDead;

		private float _deathAlphaFactor;

		private ImageIdentifierWidget _avatarImage;
	}
}
