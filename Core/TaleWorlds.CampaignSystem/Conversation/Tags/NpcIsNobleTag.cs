using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x02000212 RID: 530
	public class NpcIsNobleTag : ConversationTag
	{
		// Token: 0x17000796 RID: 1942
		// (get) Token: 0x06001E3C RID: 7740 RVA: 0x000871E4 File Offset: 0x000853E4
		public override string StringId
		{
			get
			{
				return "NpcIsNobleTag";
			}
		}

		// Token: 0x06001E3D RID: 7741 RVA: 0x000871EC File Offset: 0x000853EC
		public override bool IsApplicableTo(CharacterObject character)
		{
			Hero heroObject = character.HeroObject;
			if (heroObject == null)
			{
				return false;
			}
			Clan clan = heroObject.Clan;
			bool? flag = ((clan != null) ? new bool?(clan.IsNoble) : null);
			bool flag2 = true;
			return (flag.GetValueOrDefault() == flag2) & (flag != null);
		}

		// Token: 0x04000994 RID: 2452
		public const string Id = "NpcIsNobleTag";
	}
}
