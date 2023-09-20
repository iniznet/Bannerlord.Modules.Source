using System;
using System.Collections;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.Party
{
	public class PartyScreenData : IEnumerable<ValueTuple<TroopRosterElement, bool>>, IEnumerable
	{
		public PartyBase RightParty { get; private set; }

		public PartyBase LeftParty { get; private set; }

		public Hero RightPartyLeaderHero { get; private set; }

		public Hero LeftPartyLeaderHero { get; private set; }

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public PartyScreenData()
		{
			this.PartyGoldChangeAmount = 0;
			this.PartyInfluenceChangeAmount = new ValueTuple<int, int, int>(0, 0, 0);
			this.PartyMoraleChangeAmount = 0;
			this.PartyHorseChangeAmount = 0;
			this.RightRecruitableData = new Dictionary<CharacterObject, int>();
			this.UpgradedTroopsHistory = new List<Tuple<CharacterObject, CharacterObject, int>>();
			this.TransferredPrisonersHistory = new List<Tuple<CharacterObject, int>>();
			this.RecruitedPrisonersHistory = new List<Tuple<CharacterObject, int>>();
			this.UsedUpgradeHorsesHistory = new List<Tuple<EquipmentElement, int>>();
		}

		public void InitializeCopyFrom(PartyBase rightParty, PartyBase leftParty)
		{
			if (rightParty != null)
			{
				this.RightParty = rightParty;
				this.RightPartyLeaderHero = rightParty.LeaderHero;
			}
			if (leftParty != null)
			{
				this.LeftParty = leftParty;
				this.LeftPartyLeaderHero = leftParty.LeaderHero;
			}
			this.RightMemberRoster = TroopRoster.CreateDummyTroopRoster();
			this.LeftMemberRoster = TroopRoster.CreateDummyTroopRoster();
			this.RightPrisonerRoster = TroopRoster.CreateDummyTroopRoster();
			this.LeftPrisonerRoster = TroopRoster.CreateDummyTroopRoster();
			this.RightItemRoster = new ItemRoster();
		}

		public void CopyFromPartyAndRoster(TroopRoster rightPartyMemberRoster, TroopRoster rightPartyPrisonerRoster, TroopRoster leftPartyMemberRoster, TroopRoster leftPartyPrisonerRoster, PartyBase rightParty)
		{
			PrisonerRecruitmentCalculationModel prisonerRecruitmentCalculationModel = Campaign.Current.Models.PrisonerRecruitmentCalculationModel;
			for (int i = 0; i < rightPartyMemberRoster.Count; i++)
			{
				TroopRosterElement elementCopyAtIndex = rightPartyMemberRoster.GetElementCopyAtIndex(i);
				this.RightMemberRoster.AddToCounts(elementCopyAtIndex.Character, elementCopyAtIndex.Number, false, elementCopyAtIndex.WoundedNumber, elementCopyAtIndex.Xp, true, -1);
			}
			for (int j = 0; j < leftPartyMemberRoster.Count; j++)
			{
				TroopRosterElement elementCopyAtIndex2 = leftPartyMemberRoster.GetElementCopyAtIndex(j);
				this.LeftMemberRoster.AddToCounts(elementCopyAtIndex2.Character, elementCopyAtIndex2.Number, false, elementCopyAtIndex2.WoundedNumber, elementCopyAtIndex2.Xp, true, -1);
			}
			this.RightRecruitableData.Clear();
			for (int k = 0; k < rightPartyPrisonerRoster.Count; k++)
			{
				TroopRosterElement elementCopyAtIndex3 = rightPartyPrisonerRoster.GetElementCopyAtIndex(k);
				this.RightPrisonerRoster.AddToCounts(elementCopyAtIndex3.Character, elementCopyAtIndex3.Number, false, elementCopyAtIndex3.WoundedNumber, elementCopyAtIndex3.Xp, true, -1);
				if (rightParty != null)
				{
					MobileParty mobileParty = rightParty.MobileParty;
					bool? flag = ((mobileParty != null) ? new bool?(mobileParty.IsMainParty) : null);
					bool flag2 = true;
					if ((flag.GetValueOrDefault() == flag2) & (flag != null))
					{
						int num = prisonerRecruitmentCalculationModel.CalculateRecruitableNumber(PartyBase.MainParty, elementCopyAtIndex3.Character);
						if (!this.RightRecruitableData.ContainsKey(elementCopyAtIndex3.Character))
						{
							this.RightRecruitableData.Add(elementCopyAtIndex3.Character, num);
						}
					}
				}
			}
			for (int l = 0; l < leftPartyPrisonerRoster.Count; l++)
			{
				TroopRosterElement elementCopyAtIndex4 = leftPartyPrisonerRoster.GetElementCopyAtIndex(l);
				this.LeftPrisonerRoster.AddToCounts(elementCopyAtIndex4.Character, elementCopyAtIndex4.Number, false, elementCopyAtIndex4.WoundedNumber, elementCopyAtIndex4.Xp, true, -1);
			}
			if (rightParty != null)
			{
				for (int m = 0; m < rightParty.ItemRoster.Count; m++)
				{
					ItemRosterElement elementCopyAtIndex5 = rightParty.ItemRoster.GetElementCopyAtIndex(m);
					this.RightItemRoster.AddToCounts(elementCopyAtIndex5.EquipmentElement, elementCopyAtIndex5.Amount);
				}
			}
		}

		public void CopyFromScreenData(PartyScreenData data)
		{
			this.RightMemberRoster.Clear();
			for (int i = 0; i < data.RightMemberRoster.Count; i++)
			{
				TroopRosterElement elementCopyAtIndex = data.RightMemberRoster.GetElementCopyAtIndex(i);
				this.RightMemberRoster.AddToCounts(elementCopyAtIndex.Character, elementCopyAtIndex.Number, false, elementCopyAtIndex.WoundedNumber, elementCopyAtIndex.Xp, true, -1);
			}
			this.RightPrisonerRoster.Clear();
			for (int j = 0; j < data.RightPrisonerRoster.Count; j++)
			{
				TroopRosterElement elementCopyAtIndex2 = data.RightPrisonerRoster.GetElementCopyAtIndex(j);
				this.RightPrisonerRoster.AddToCounts(elementCopyAtIndex2.Character, elementCopyAtIndex2.Number, false, elementCopyAtIndex2.WoundedNumber, elementCopyAtIndex2.Xp, true, -1);
			}
			this.RightItemRoster.Clear();
			if (data.RightItemRoster != null)
			{
				for (int k = 0; k < data.RightItemRoster.Count; k++)
				{
					ItemRosterElement elementCopyAtIndex3 = data.RightItemRoster.GetElementCopyAtIndex(k);
					this.RightItemRoster.AddToCounts(elementCopyAtIndex3.EquipmentElement, elementCopyAtIndex3.Amount);
				}
			}
			this.LeftMemberRoster.Clear();
			for (int l = 0; l < data.LeftMemberRoster.Count; l++)
			{
				TroopRosterElement elementCopyAtIndex4 = data.LeftMemberRoster.GetElementCopyAtIndex(l);
				this.LeftMemberRoster.AddToCounts(elementCopyAtIndex4.Character, elementCopyAtIndex4.Number, false, elementCopyAtIndex4.WoundedNumber, elementCopyAtIndex4.Xp, true, -1);
			}
			this.LeftPrisonerRoster.Clear();
			for (int m = 0; m < data.LeftPrisonerRoster.Count; m++)
			{
				TroopRosterElement elementCopyAtIndex5 = data.LeftPrisonerRoster.GetElementCopyAtIndex(m);
				this.LeftPrisonerRoster.AddToCounts(elementCopyAtIndex5.Character, elementCopyAtIndex5.Number, false, elementCopyAtIndex5.WoundedNumber, elementCopyAtIndex5.Xp, true, -1);
			}
			this.PartyGoldChangeAmount = data.PartyGoldChangeAmount;
			this.PartyInfluenceChangeAmount = data.PartyInfluenceChangeAmount;
			this.PartyMoraleChangeAmount = data.PartyMoraleChangeAmount;
			this.PartyHorseChangeAmount = data.PartyHorseChangeAmount;
			this.RightRecruitableData = new Dictionary<CharacterObject, int>(data.RightRecruitableData);
			this.UpgradedTroopsHistory = new List<Tuple<CharacterObject, CharacterObject, int>>(data.UpgradedTroopsHistory);
			this.TransferredPrisonersHistory = new List<Tuple<CharacterObject, int>>(data.TransferredPrisonersHistory);
			this.RecruitedPrisonersHistory = new List<Tuple<CharacterObject, int>>(data.RecruitedPrisonersHistory);
			this.UsedUpgradeHorsesHistory = new List<Tuple<EquipmentElement, int>>(data.UsedUpgradeHorsesHistory);
		}

		public void BindRostersFrom(TroopRoster rightPartyMemberRoster, TroopRoster rightPartyPrisonerRoster, TroopRoster leftPartyMemberRoster, TroopRoster leftPartyPrisonerRoster, PartyBase rightParty, PartyBase leftParty)
		{
			this.RightParty = rightParty;
			this.LeftParty = leftParty;
			if (rightParty != null)
			{
				this.RightItemRoster = rightParty.ItemRoster;
				this.RightPartyLeaderHero = rightParty.LeaderHero;
			}
			if (leftParty != null)
			{
				this.LeftPartyLeaderHero = leftParty.LeaderHero;
			}
			this.RightMemberRoster = rightPartyMemberRoster;
			this.LeftMemberRoster = leftPartyMemberRoster;
			this.RightPrisonerRoster = rightPartyPrisonerRoster;
			this.LeftPrisonerRoster = leftPartyPrisonerRoster;
			if (rightParty != null)
			{
				MobileParty mobileParty = rightParty.MobileParty;
				bool? flag = ((mobileParty != null) ? new bool?(mobileParty.IsMainParty) : null);
				bool flag2 = true;
				if ((flag.GetValueOrDefault() == flag2) & (flag != null))
				{
					this.RightRecruitableData = new Dictionary<CharacterObject, int>();
					PrisonerRecruitmentCalculationModel prisonerRecruitmentCalculationModel = Campaign.Current.Models.PrisonerRecruitmentCalculationModel;
					foreach (TroopRosterElement troopRosterElement in rightParty.PrisonRoster.GetTroopRoster())
					{
						int num = prisonerRecruitmentCalculationModel.CalculateRecruitableNumber(PartyBase.MainParty, troopRosterElement.Character);
						if (!this.RightRecruitableData.ContainsKey(troopRosterElement.Character))
						{
							this.RightRecruitableData.Add(troopRosterElement.Character, num);
						}
					}
				}
			}
		}

		public void ResetUsing(PartyScreenData partyScreenData)
		{
			this.RightMemberRoster.Clear();
			this.RightMemberRoster.RemoveZeroCounts();
			for (int i = 0; i < partyScreenData.RightMemberRoster.Count; i++)
			{
				TroopRosterElement elementCopyAtIndex = partyScreenData.RightMemberRoster.GetElementCopyAtIndex(i);
				this.RightMemberRoster.AddToCounts(elementCopyAtIndex.Character, elementCopyAtIndex.Number, false, elementCopyAtIndex.WoundedNumber, elementCopyAtIndex.Xp, true, -1);
			}
			PartyBase rightParty = this.RightParty;
			if (((rightParty != null) ? rightParty.MobileParty : null) != null)
			{
				this.RightParty.MobileParty.ChangePartyLeader(partyScreenData.RightPartyLeaderHero);
			}
			this.LeftMemberRoster.Clear();
			this.LeftMemberRoster.RemoveZeroCounts();
			for (int j = 0; j < partyScreenData.LeftMemberRoster.Count; j++)
			{
				TroopRosterElement elementCopyAtIndex2 = partyScreenData.LeftMemberRoster.GetElementCopyAtIndex(j);
				this.LeftMemberRoster.AddToCounts(elementCopyAtIndex2.Character, elementCopyAtIndex2.Number, false, elementCopyAtIndex2.WoundedNumber, elementCopyAtIndex2.Xp, true, -1);
			}
			PartyBase leftParty = this.LeftParty;
			if (((leftParty != null) ? leftParty.MobileParty : null) != null)
			{
				this.LeftParty.MobileParty.ChangePartyLeader(partyScreenData.LeftPartyLeaderHero);
			}
			this.RightPrisonerRoster.Clear();
			this.LeftPrisonerRoster.Clear();
			this.RightPrisonerRoster.RemoveZeroCounts();
			for (int k = 0; k < partyScreenData.RightPrisonerRoster.Count; k++)
			{
				TroopRosterElement elementCopyAtIndex3 = partyScreenData.RightPrisonerRoster.GetElementCopyAtIndex(k);
				this.RightPrisonerRoster.AddToCounts(elementCopyAtIndex3.Character, elementCopyAtIndex3.Number, false, elementCopyAtIndex3.WoundedNumber, elementCopyAtIndex3.Xp, true, -1);
			}
			this.LeftPrisonerRoster.RemoveZeroCounts();
			for (int l = 0; l < partyScreenData.LeftPrisonerRoster.Count; l++)
			{
				TroopRosterElement elementCopyAtIndex4 = partyScreenData.LeftPrisonerRoster.GetElementCopyAtIndex(l);
				this.LeftPrisonerRoster.AddToCounts(elementCopyAtIndex4.Character, elementCopyAtIndex4.Number, false, elementCopyAtIndex4.WoundedNumber, elementCopyAtIndex4.Xp, true, -1);
			}
			if (this.RightItemRoster != null)
			{
				this.RightItemRoster.Clear();
				for (int m = 0; m < partyScreenData.RightItemRoster.Count; m++)
				{
					ItemRosterElement elementCopyAtIndex5 = partyScreenData.RightItemRoster.GetElementCopyAtIndex(m);
					this.RightItemRoster.AddToCounts(elementCopyAtIndex5.EquipmentElement, elementCopyAtIndex5.Amount);
				}
			}
			this.PartyGoldChangeAmount = partyScreenData.PartyGoldChangeAmount;
			this.PartyInfluenceChangeAmount = partyScreenData.PartyInfluenceChangeAmount;
			this.PartyMoraleChangeAmount = partyScreenData.PartyMoraleChangeAmount;
			this.PartyHorseChangeAmount = partyScreenData.PartyHorseChangeAmount;
			this.RightRecruitableData = new Dictionary<CharacterObject, int>(partyScreenData.RightRecruitableData);
			this.UpgradedTroopsHistory = new List<Tuple<CharacterObject, CharacterObject, int>>(partyScreenData.UpgradedTroopsHistory);
			this.TransferredPrisonersHistory = new List<Tuple<CharacterObject, int>>(partyScreenData.TransferredPrisonersHistory);
			this.RecruitedPrisonersHistory = new List<Tuple<CharacterObject, int>>(partyScreenData.RecruitedPrisonersHistory);
			this.UsedUpgradeHorsesHistory = new List<Tuple<EquipmentElement, int>>(partyScreenData.UsedUpgradeHorsesHistory);
		}

		public List<TroopTradeDifference> GetTroopTradeDifferencesFromTo(PartyScreenData toPartyScreenData)
		{
			List<TroopTradeDifference> list = new List<TroopTradeDifference>();
			string text = "Current settlement: ";
			Settlement currentSettlement = Settlement.CurrentSettlement;
			Debug.Print(text + ((currentSettlement != null) ? currentSettlement.StringId : null), 0, Debug.DebugColor.White, 17592186044416UL);
			string text2 = "Left party id: ";
			PartyBase leftParty = toPartyScreenData.LeftParty;
			string text3;
			if (leftParty == null)
			{
				text3 = null;
			}
			else
			{
				MobileParty mobileParty = leftParty.MobileParty;
				text3 = ((mobileParty != null) ? mobileParty.StringId : null);
			}
			Debug.Print(text2 + text3, 0, Debug.DebugColor.White, 17592186044416UL);
			string text4 = "Right party id: ";
			PartyBase rightParty = toPartyScreenData.RightParty;
			string text5;
			if (rightParty == null)
			{
				text5 = null;
			}
			else
			{
				MobileParty mobileParty2 = rightParty.MobileParty;
				text5 = ((mobileParty2 != null) ? mobileParty2.StringId : null);
			}
			Debug.Print(text4 + text5, 0, Debug.DebugColor.White, 17592186044416UL);
			foreach (ValueTuple<TroopRosterElement, bool> valueTuple in this)
			{
				TroopRosterElement troopRosterElement = valueTuple.Item1;
				int number = troopRosterElement.Number;
				int num = 0;
				foreach (ValueTuple<TroopRosterElement, bool> valueTuple2 in toPartyScreenData)
				{
					if (valueTuple2.Item1.Character == valueTuple.Item1.Character && valueTuple2.Item2 == valueTuple.Item2)
					{
						int num2 = num;
						troopRosterElement = valueTuple2.Item1;
						num = num2 + troopRosterElement.Number;
					}
				}
				if (number != num)
				{
					TroopTradeDifference troopTradeDifference = new TroopTradeDifference
					{
						Troop = valueTuple.Item1.Character,
						ToCount = num,
						FromCount = number,
						IsPrisoner = valueTuple.Item2
					};
					list.Add(troopTradeDifference);
				}
				Debug.Print(string.Concat(new object[]
				{
					"currently owned: ",
					number,
					", previously owned: ",
					num,
					" name: ",
					valueTuple.Item1.Character.StringId
				}), 0, Debug.DebugColor.White, 17592186044416UL);
			}
			return list;
		}

		private IEnumerator<ValueTuple<TroopRosterElement, bool>> EnumerateElements()
		{
			int num;
			for (int i = 0; i < this.RightMemberRoster.Count; i = num + 1)
			{
				TroopRosterElement elementCopyAtIndex = this.RightMemberRoster.GetElementCopyAtIndex(i);
				yield return new ValueTuple<TroopRosterElement, bool>(elementCopyAtIndex, false);
				num = i;
			}
			for (int i = 0; i < this.RightPrisonerRoster.Count; i = num + 1)
			{
				TroopRosterElement elementCopyAtIndex2 = this.RightPrisonerRoster.GetElementCopyAtIndex(i);
				yield return new ValueTuple<TroopRosterElement, bool>(elementCopyAtIndex2, true);
				num = i;
			}
			yield break;
		}

		public IEnumerator<ValueTuple<TroopRosterElement, bool>> GetEnumerator()
		{
			return this.EnumerateElements();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.EnumerateElements();
		}

		public override bool Equals(object obj)
		{
			return this == obj;
		}

		public static bool operator ==(PartyScreenData a, PartyScreenData b)
		{
			if (a == b)
			{
				return true;
			}
			if (a == null || b == null)
			{
				return false;
			}
			if (a.PartyGoldChangeAmount != b.PartyGoldChangeAmount || a.PartyInfluenceChangeAmount.Item1 != b.PartyInfluenceChangeAmount.Item1 || a.PartyInfluenceChangeAmount.Item2 != b.PartyInfluenceChangeAmount.Item2 || a.PartyInfluenceChangeAmount.Item3 != b.PartyInfluenceChangeAmount.Item3 || a.PartyMoraleChangeAmount != b.PartyMoraleChangeAmount || a.PartyHorseChangeAmount != b.PartyHorseChangeAmount)
			{
				return false;
			}
			if (a.RightMemberRoster.Count != b.RightMemberRoster.Count || a.RightPrisonerRoster.Count != b.RightPrisonerRoster.Count || a.RightRecruitableData.Count != b.RightRecruitableData.Count || a.UpgradedTroopsHistory.Count != b.UpgradedTroopsHistory.Count || a.TransferredPrisonersHistory.Count != b.TransferredPrisonersHistory.Count || a.RecruitedPrisonersHistory.Count != b.RecruitedPrisonersHistory.Count || a.UsedUpgradeHorsesHistory.Count != b.UsedUpgradeHorsesHistory.Count)
			{
				return false;
			}
			if (a.RightMemberRoster != b.RightMemberRoster)
			{
				return false;
			}
			if (a.RightPrisonerRoster != b.RightPrisonerRoster)
			{
				return false;
			}
			foreach (CharacterObject characterObject in a.RightRecruitableData.Keys)
			{
				if (!b.RightRecruitableData.ContainsKey(characterObject) || a.RightRecruitableData[characterObject] != b.RightRecruitableData[characterObject])
				{
					return false;
				}
			}
			for (int i = 0; i < a.UpgradedTroopsHistory.Count; i++)
			{
				if (a.UpgradedTroopsHistory[i].Item1 != b.UpgradedTroopsHistory[i].Item1 || a.UpgradedTroopsHistory[i].Item2 != b.UpgradedTroopsHistory[i].Item2 || a.UpgradedTroopsHistory[i].Item3 != b.UpgradedTroopsHistory[i].Item3)
				{
					return false;
				}
			}
			for (int j = 0; j < a.TransferredPrisonersHistory.Count; j++)
			{
				if (a.TransferredPrisonersHistory[j].Item1 != b.TransferredPrisonersHistory[j].Item1 || a.TransferredPrisonersHistory[j].Item2 != b.TransferredPrisonersHistory[j].Item2)
				{
					return false;
				}
			}
			for (int k = 0; k < a.RecruitedPrisonersHistory.Count; k++)
			{
				if (a.RecruitedPrisonersHistory[k].Item1 != b.RecruitedPrisonersHistory[k].Item1 || a.RecruitedPrisonersHistory[k].Item2 != b.RecruitedPrisonersHistory[k].Item2)
				{
					return false;
				}
			}
			for (int l = 0; l < a.UsedUpgradeHorsesHistory.Count; l++)
			{
				if (a.UsedUpgradeHorsesHistory[l].Item1.Item != b.UsedUpgradeHorsesHistory[l].Item1.Item || a.UsedUpgradeHorsesHistory[l].Item2 != b.UsedUpgradeHorsesHistory[l].Item2)
				{
					return false;
				}
			}
			return true;
		}

		public static bool operator !=(PartyScreenData first, PartyScreenData second)
		{
			return !(first == second);
		}

		public TroopRoster RightMemberRoster;

		public TroopRoster LeftMemberRoster;

		public TroopRoster RightPrisonerRoster;

		public TroopRoster LeftPrisonerRoster;

		public ItemRoster RightItemRoster;

		public Dictionary<CharacterObject, int> RightRecruitableData;

		public int PartyGoldChangeAmount;

		public ValueTuple<int, int, int> PartyInfluenceChangeAmount;

		public int PartyMoraleChangeAmount;

		public int PartyHorseChangeAmount;

		public List<Tuple<CharacterObject, CharacterObject, int>> UpgradedTroopsHistory;

		public List<Tuple<CharacterObject, int>> TransferredPrisonersHistory;

		public List<Tuple<CharacterObject, int>> RecruitedPrisonersHistory;

		public List<Tuple<EquipmentElement, int>> UsedUpgradeHorsesHistory;
	}
}
