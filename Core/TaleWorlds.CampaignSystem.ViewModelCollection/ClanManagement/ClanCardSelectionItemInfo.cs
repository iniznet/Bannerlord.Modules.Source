using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement
{
	public readonly struct ClanCardSelectionItemInfo
	{
		public ClanCardSelectionItemInfo(object identifier, TextObject title, ImageIdentifier image, CardSelectionItemSpriteType spriteType, string spriteName, string spriteLabel, IEnumerable<ClanCardSelectionItemPropertyInfo> properties, bool isDisabled, TextObject disabledReason, TextObject actionResult)
		{
			this.Identifier = identifier;
			this.Title = title;
			this.Image = image;
			this.SpriteType = spriteType;
			this.SpriteName = spriteName;
			this.SpriteLabel = spriteLabel;
			this.Properties = properties;
			this.IsSpecialActionItem = false;
			this.SpecialActionText = null;
			this.IsDisabled = isDisabled;
			this.DisabledReason = disabledReason;
			this.ActionResult = actionResult;
		}

		public ClanCardSelectionItemInfo(TextObject specialActionText, bool isDisabled, TextObject disabledReason, TextObject actionResult)
		{
			this.Identifier = null;
			this.Title = null;
			this.Image = null;
			this.SpriteType = CardSelectionItemSpriteType.None;
			this.SpriteName = null;
			this.SpriteLabel = null;
			this.Properties = null;
			this.IsSpecialActionItem = true;
			this.SpecialActionText = specialActionText;
			this.IsDisabled = isDisabled;
			this.DisabledReason = disabledReason;
			this.ActionResult = actionResult;
		}

		public readonly object Identifier;

		public readonly TextObject Title;

		public readonly ImageIdentifier Image;

		public readonly CardSelectionItemSpriteType SpriteType;

		public readonly string SpriteName;

		public readonly string SpriteLabel;

		public readonly IEnumerable<ClanCardSelectionItemPropertyInfo> Properties;

		public readonly bool IsSpecialActionItem;

		public readonly TextObject SpecialActionText;

		public readonly bool IsDisabled;

		public readonly TextObject DisabledReason;

		public readonly TextObject ActionResult;
	}
}
