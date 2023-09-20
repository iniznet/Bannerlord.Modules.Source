using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer
{
	// Token: 0x02000081 RID: 129
	public class MultiplayerTeamPlayerAvatarButtonWidget : ButtonWidget
	{
		// Token: 0x060006FD RID: 1789 RVA: 0x00014B1F File Offset: 0x00012D1F
		public MultiplayerTeamPlayerAvatarButtonWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060006FE RID: 1790 RVA: 0x00014B33 File Offset: 0x00012D33
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

		// Token: 0x060006FF RID: 1791 RVA: 0x00014B70 File Offset: 0x00012D70
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

		// Token: 0x17000279 RID: 633
		// (get) Token: 0x06000700 RID: 1792 RVA: 0x00014BBC File Offset: 0x00012DBC
		// (set) Token: 0x06000701 RID: 1793 RVA: 0x00014BC4 File Offset: 0x00012DC4
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

		// Token: 0x1700027A RID: 634
		// (get) Token: 0x06000702 RID: 1794 RVA: 0x00014BE8 File Offset: 0x00012DE8
		// (set) Token: 0x06000703 RID: 1795 RVA: 0x00014BF0 File Offset: 0x00012DF0
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

		// Token: 0x1700027B RID: 635
		// (get) Token: 0x06000704 RID: 1796 RVA: 0x00014C0E File Offset: 0x00012E0E
		// (set) Token: 0x06000705 RID: 1797 RVA: 0x00014C16 File Offset: 0x00012E16
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

		// Token: 0x0400031A RID: 794
		private bool _isInitialized;

		// Token: 0x0400031B RID: 795
		private float _originalAvatarImageAlpha = 1f;

		// Token: 0x0400031C RID: 796
		private bool _isDead;

		// Token: 0x0400031D RID: 797
		private float _deathAlphaFactor;

		// Token: 0x0400031E RID: 798
		private ImageIdentifierWidget _avatarImage;
	}
}
