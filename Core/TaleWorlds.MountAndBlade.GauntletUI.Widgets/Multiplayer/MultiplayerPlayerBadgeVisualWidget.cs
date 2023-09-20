using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer
{
	public class MultiplayerPlayerBadgeVisualWidget : Widget
	{
		public MultiplayerPlayerBadgeVisualWidget(UIContext context)
			: base(context)
		{
		}

		private void UpdateVisual(string badgeId)
		{
			base.Sprite = base.Context.SpriteData.GetSprite("MPPlayerBadges\\" + badgeId);
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this._hasForcedSize)
			{
				base.ScaledSuggestedWidth = this._forcedWidth * base._inverseScaleToUse;
				base.ScaledSuggestedHeight = this._forcedHeight * base._inverseScaleToUse;
			}
		}

		public void SetForcedSize(float width, float height)
		{
			this._forcedWidth = width;
			this._forcedHeight = height;
			this._hasForcedSize = true;
		}

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

		private float _forcedWidth;

		private float _forcedHeight;

		private bool _hasForcedSize;

		private string _badgeId;
	}
}
