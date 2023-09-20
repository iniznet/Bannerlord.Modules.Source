using System;
using System.Linq;
using TaleWorlds.CampaignSystem.Settlements;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x0200020C RID: 524
	public class PlayerIsNobleTag : ConversationTag
	{
		// Token: 0x17000790 RID: 1936
		// (get) Token: 0x06001E2A RID: 7722 RVA: 0x00087119 File Offset: 0x00085319
		public override string StringId
		{
			get
			{
				return "PlayerIsNobleTag";
			}
		}

		// Token: 0x06001E2B RID: 7723 RVA: 0x00087120 File Offset: 0x00085320
		public override bool IsApplicableTo(CharacterObject character)
		{
			return Settlement.All.Any((Settlement x) => x.OwnerClan == Hero.MainHero.Clan);
		}

		// Token: 0x0400098E RID: 2446
		public const string Id = "PlayerIsNobleTag";
	}
}
