using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Helpers
{
	// Token: 0x02000008 RID: 8
	public static class AlleyHelper
	{
		// Token: 0x06000023 RID: 35 RVA: 0x000037D1 File Offset: 0x000019D1
		public static void OpenScreenForManagingAlley(TroopRoster leftMemberRoster, PartyPresentationDoneButtonDelegate onDoneButtonClicked, TextObject leftText, PartyPresentationCancelButtonDelegate onCancelButtonClicked = null)
		{
			PartyScreenManager.OpenScreenForManagingAlley(leftMemberRoster, new IsTroopTransferableDelegate(AlleyHelper.IsTroopTransferable), new PartyPresentationDoneButtonConditionDelegate(AlleyHelper.DoneButtonCondition), onDoneButtonClicked, leftText, onCancelButtonClicked);
		}

		// Token: 0x06000024 RID: 36 RVA: 0x000037F4 File Offset: 0x000019F4
		private static Tuple<bool, TextObject> DoneButtonCondition(TroopRoster leftMemberRoster, TroopRoster leftPrisonRoster, TroopRoster rightMemberRoster, TroopRoster rightPrisonRoster, int lefLimitNum, int rightLimitNum)
		{
			if (leftMemberRoster.TotalRegulars > Campaign.Current.Models.AlleyModel.MaximumTroopCountInPlayerOwnedAlley)
			{
				TextObject textObject = new TextObject("{=5Y4rnaDX}You can not transfer more than {UPPER_LIMIT} troops", null);
				textObject.SetTextVariable("UPPER_LIMIT", Campaign.Current.Models.AlleyModel.MaximumTroopCountInPlayerOwnedAlley);
				return new Tuple<bool, TextObject>(false, textObject);
			}
			if (leftMemberRoster.TotalRegulars < Campaign.Current.Models.AlleyModel.MinimumTroopCountInPlayerOwnedAlley)
			{
				TextObject textObject2 = new TextObject("{=v5HPLGXs}You can not transfer less than {LOWER_LIMIT} troops", null);
				textObject2.SetTextVariable("LOWER_LIMIT", Campaign.Current.Models.AlleyModel.MinimumTroopCountInPlayerOwnedAlley);
				return new Tuple<bool, TextObject>(false, textObject2);
			}
			return new Tuple<bool, TextObject>(true, TextObject.Empty);
		}

		// Token: 0x06000025 RID: 37 RVA: 0x000038AC File Offset: 0x00001AAC
		private static bool IsTroopTransferable(CharacterObject character, PartyScreenLogic.TroopType type, PartyScreenLogic.PartyRosterSide side, PartyBase leftOwnerParty)
		{
			return !character.IsHero && type != PartyScreenLogic.TroopType.Prisoner;
		}

		// Token: 0x06000026 RID: 38 RVA: 0x000038C0 File Offset: 0x00001AC0
		public static void CreateMultiSelectionInquiryForSelectingClanMemberToAlley(Alley alley, Action<List<InquiryElement>> affirmativeAction, Action<List<InquiryElement>> negativeAction)
		{
			List<InquiryElement> list = new List<InquiryElement>();
			foreach (ValueTuple<Hero, DefaultAlleyModel.AlleyMemberAvailabilityDetail> valueTuple in Campaign.Current.Models.AlleyModel.GetClanMembersAndAvailabilityDetailsForLeadingAnAlley(alley))
			{
				Hero item = valueTuple.Item1;
				DefaultAlleyModel.AlleyMemberAvailabilityDetail item2 = valueTuple.Item2;
				TextObject disabledReasonTextForHero = Campaign.Current.Models.AlleyModel.GetDisabledReasonTextForHero(item, alley, item2);
				list.Add(new InquiryElement(item.CharacterObject, item.Name.ToString(), new ImageIdentifier(CharacterCode.CreateFrom(item.CharacterObject)), item2 == DefaultAlleyModel.AlleyMemberAvailabilityDetail.Available || item2 == DefaultAlleyModel.AlleyMemberAvailabilityDetail.AvailableWithDelay, disabledReasonTextForHero.ToString()));
			}
			MBInformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData("Select companion", "Select a companion to lead this alley.", list, true, 1, GameTexts.FindText("str_done", null).ToString(), new TextObject("{=3CpNUnVl}Cancel", null).ToString(), affirmativeAction, negativeAction, ""), true, false);
		}
	}
}
