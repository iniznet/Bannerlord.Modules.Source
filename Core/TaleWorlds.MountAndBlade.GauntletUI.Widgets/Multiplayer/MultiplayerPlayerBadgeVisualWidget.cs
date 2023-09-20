using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer
{
	// Token: 0x0200007F RID: 127
	public class MultiplayerPlayerBadgeVisualWidget : Widget
	{
		// Token: 0x060006F1 RID: 1777 RVA: 0x000149E6 File Offset: 0x00012BE6
		public MultiplayerPlayerBadgeVisualWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060006F2 RID: 1778 RVA: 0x000149EF File Offset: 0x00012BEF
		private void UpdateVisual(string badgeId)
		{
			base.Sprite = base.Context.SpriteData.GetSprite("MPPlayerBadges\\" + badgeId);
		}

		// Token: 0x060006F3 RID: 1779 RVA: 0x00014A12 File Offset: 0x00012C12
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this._hasForcedSize)
			{
				base.ScaledSuggestedWidth = this._forcedWidth * base._inverseScaleToUse;
				base.ScaledSuggestedHeight = this._forcedHeight * base._inverseScaleToUse;
			}
		}

		// Token: 0x060006F4 RID: 1780 RVA: 0x00014A49 File Offset: 0x00012C49
		public void SetForcedSize(float width, float height)
		{
			this._forcedWidth = width;
			this._forcedHeight = height;
			this._hasForcedSize = true;
		}

		// Token: 0x17000276 RID: 630
		// (get) Token: 0x060006F5 RID: 1781 RVA: 0x00014A60 File Offset: 0x00012C60
		// (set) Token: 0x060006F6 RID: 1782 RVA: 0x00014A68 File Offset: 0x00012C68
		public string BadgeId
		{
			get
			{
				return this._badgeId;
			}
			set
			{
				if (value != this._badgeId)
				{
					this._badgeId = value;
					base.OnPropertyChanged<string>(value, "BadgeId");
					this.UpdateVisual(value);
				}
			}
		}

		// Token: 0x04000312 RID: 786
		private float _forcedWidth;

		// Token: 0x04000313 RID: 787
		private float _forcedHeight;

		// Token: 0x04000314 RID: 788
		private bool _hasForcedSize;

		// Token: 0x04000315 RID: 789
		private string _badgeId;
	}
}
