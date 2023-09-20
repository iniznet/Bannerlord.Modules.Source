using System;
using Helpers;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	// Token: 0x020000FF RID: 255
	public class DefaultCompanionHiringPriceCalculationModel : CompanionHiringPriceCalculationModel
	{
		// Token: 0x06001515 RID: 5397 RVA: 0x00060BD0 File Offset: 0x0005EDD0
		public override int GetCompanionHiringPrice(Hero companion)
		{
			ExplainedNumber explainedNumber = new ExplainedNumber(0f, false, null);
			Settlement currentSettlement = companion.CurrentSettlement;
			Town town = ((currentSettlement != null) ? currentSettlement.Town : null);
			if (town == null)
			{
				town = SettlementHelper.FindNearestTown(null, null).Town;
			}
			float num = 0f;
			for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumEquipmentSetSlots; equipmentIndex++)
			{
				EquipmentElement equipmentElement = companion.CharacterObject.Equipment[equipmentIndex];
				if (equipmentElement.Item != null)
				{
					num += (float)town.GetItemPrice(equipmentElement, null, false);
				}
			}
			for (EquipmentIndex equipmentIndex2 = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex2 < EquipmentIndex.NumEquipmentSetSlots; equipmentIndex2++)
			{
				EquipmentElement equipmentElement2 = companion.CharacterObject.FirstCivilianEquipment[equipmentIndex2];
				if (equipmentElement2.Item != null)
				{
					num += (float)town.GetItemPrice(equipmentElement2, null, false);
				}
			}
			explainedNumber.Add(num / 2f, null, null);
			explainedNumber.Add((float)(companion.CharacterObject.Level * 10), null, null);
			if (Hero.MainHero.IsPartyLeader && Hero.MainHero.GetPerkValue(DefaultPerks.Steward.PaidInPromise))
			{
				explainedNumber.AddFactor(DefaultPerks.Steward.PaidInPromise.PrimaryBonus, null);
			}
			if (Hero.MainHero.PartyBelongedTo != null)
			{
				PerkHelper.AddPerkBonusForParty(DefaultPerks.Trade.GreatInvestor, Hero.MainHero.PartyBelongedTo, false, ref explainedNumber);
			}
			return (int)explainedNumber.ResultNumber;
		}
	}
}
