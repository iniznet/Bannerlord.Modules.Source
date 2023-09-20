using System;
using TaleWorlds.CampaignSystem.BarterSystem;
using TaleWorlds.CampaignSystem.BarterSystem.Barterables;
using TaleWorlds.CampaignSystem.Party;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors.BarterBehaviors
{
	// Token: 0x020003FD RID: 1021
	public class TransferPrisonerBarterBehavior : CampaignBehaviorBase
	{
		// Token: 0x06003D19 RID: 15641 RVA: 0x00122B00 File Offset: 0x00120D00
		public override void RegisterEvents()
		{
			CampaignEvents.BarterablesRequested.AddNonSerializedListener(this, new Action<BarterData>(this.CheckForBarters));
		}

		// Token: 0x06003D1A RID: 15642 RVA: 0x00122B19 File Offset: 0x00120D19
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x06003D1B RID: 15643 RVA: 0x00122B1C File Offset: 0x00120D1C
		public void CheckForBarters(BarterData args)
		{
			PartyBase offererParty = args.OffererParty;
			PartyBase otherParty = args.OtherParty;
			if (offererParty != null && otherParty != null)
			{
				foreach (CharacterObject characterObject in offererParty.PrisonerHeroes)
				{
					if (characterObject.IsHero && FactionManager.IsAtWarAgainstFaction(characterObject.HeroObject.MapFaction, otherParty.MapFaction))
					{
						Barterable barterable = new TransferPrisonerBarterable(characterObject.HeroObject, args.OffererHero, args.OffererParty, args.OtherHero, otherParty);
						args.AddBarterable<PrisonerBarterGroup>(barterable, false);
					}
				}
				foreach (CharacterObject characterObject2 in otherParty.PrisonerHeroes)
				{
					if (characterObject2.IsHero && FactionManager.IsAtWarAgainstFaction(characterObject2.HeroObject.MapFaction, offererParty.MapFaction))
					{
						Barterable barterable2 = new TransferPrisonerBarterable(characterObject2.HeroObject, args.OtherHero, args.OtherParty, args.OffererHero, offererParty);
						args.AddBarterable<PrisonerBarterGroup>(barterable2, false);
					}
				}
			}
		}
	}
}
