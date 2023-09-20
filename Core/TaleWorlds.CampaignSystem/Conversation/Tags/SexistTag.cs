using System;
using System.Linq;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x02000219 RID: 537
	public class SexistTag : ConversationTag
	{
		// Token: 0x1700079D RID: 1949
		// (get) Token: 0x06001E51 RID: 7761 RVA: 0x0008741D File Offset: 0x0008561D
		public override string StringId
		{
			get
			{
				return "SexistTag";
			}
		}

		// Token: 0x06001E52 RID: 7762 RVA: 0x00087424 File Offset: 0x00085624
		public override bool IsApplicableTo(CharacterObject character)
		{
			bool flag = character.HeroObject.Clan.Heroes.Any((Hero x) => x.IsFemale && x.IsCommander);
			int num = character.GetTraitLevel(DefaultTraits.Calculating) + character.GetTraitLevel(DefaultTraits.Mercy);
			int num2 = character.GetTraitLevel(DefaultTraits.Valor) + character.GetTraitLevel(DefaultTraits.Generosity);
			return num < 0 && num2 <= 0 && !flag;
		}

		// Token: 0x0400099B RID: 2459
		public const string Id = "SexistTag";
	}
}
