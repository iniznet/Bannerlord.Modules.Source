using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.BarterSystem.Barterables
{
	public class SafePassageBarterable : Barterable
	{
		public override string StringID
		{
			get
			{
				return "safe_passage_barterable";
			}
		}

		public override TextObject Name
		{
			get
			{
				TextObject textObject;
				if (this._otherHero != null)
				{
					StringHelpers.SetCharacterProperties("HERO", this._otherHero.CharacterObject, null, false);
					textObject = new TextObject("{=BJbbahYe}Let {HERO.NAME} Go", null);
				}
				else
				{
					textObject = new TextObject("{=QKNWsJRb}Let {PARTY} Go", null);
					textObject.SetTextVariable("PARTY", this._otherParty.Name);
				}
				return textObject;
			}
		}

		public SafePassageBarterable(Hero originalOwner, Hero otherHero, PartyBase ownerParty, PartyBase otherParty)
			: base(originalOwner, ownerParty)
		{
			this._otherHero = otherHero;
			this._otherParty = otherParty;
		}

		public override int GetUnitValueForFaction(IFaction faction)
		{
			float num = MathF.Clamp(PlayerEncounter.Current.GetPlayerStrengthRatioInEncounter(), 0f, 1f);
			int num2 = (int)MathF.Clamp((float)(Hero.MainHero.Gold + PartyBase.MainParty.ItemRoster.Sum((ItemRosterElement t) => t.EquipmentElement.Item.Value * t.Amount)), 0f, 2.1474836E+09f);
			float num3 = ((num < 1f) ? (0.05f + (1f - num) * 0.2f) : 0.1f);
			float num4 = ((faction.Leader == null) ? 1f : MathF.Clamp((50f + (float)faction.Leader.GetRelation(this._otherHero)) / 50f, 0.05f, 1.1f));
			if (!PlayerEncounter.EncounteredParty.IsMobile || !PlayerEncounter.EncounteredParty.MobileParty.IsBandit)
			{
				num2 += 3000 + (int)(Hero.MainHero.Clan.Renown * 50f);
				num3 *= 1.5f;
			}
			if (MobileParty.MainParty.MapEvent != null || MobileParty.MainParty.SiegeEvent != null)
			{
				num3 *= 1.2f;
			}
			int num5 = (int)((float)num2 * num3 + 1000f);
			MobileParty mobileParty = PlayerEncounter.EncounteredParty.MobileParty;
			if (mobileParty != null && mobileParty.IsBandit)
			{
				num5 /= 8;
				if (Hero.MainHero.GetPerkValue(DefaultPerks.Roguery.SweetTalker))
				{
					num5 += MathF.Round((float)num5 * DefaultPerks.Roguery.SweetTalker.PrimaryBonus);
				}
			}
			else
			{
				num5 /= 2;
				num5 += (int)(0.3f * num3 * Campaign.Current.Models.ValuationModel.GetMilitaryValueOfParty(this._otherParty.MobileParty));
				num5 += (int)(0.3f * num3 * Campaign.Current.Models.ValuationModel.GetValueOfHero(this._otherHero));
			}
			if (Hero.MainHero.GetPerkValue(DefaultPerks.Trade.MarketDealer))
			{
				num5 += MathF.Round((float)num5 * DefaultPerks.Trade.MarketDealer.PrimaryBonus);
			}
			Hero originalOwner = base.OriginalOwner;
			if (faction != ((originalOwner != null) ? originalOwner.Clan : null))
			{
				Hero originalOwner2 = base.OriginalOwner;
				if (faction != ((originalOwner2 != null) ? originalOwner2.MapFaction : null) && faction != base.OriginalParty.MapFaction)
				{
					Hero otherHero = this._otherHero;
					if (faction != ((otherHero != null) ? otherHero.Clan : null))
					{
						Hero otherHero2 = this._otherHero;
						if (faction != ((otherHero2 != null) ? otherHero2.MapFaction : null) && faction != this._otherParty.MapFaction)
						{
							return num5;
						}
					}
					return (int)(0.9f * (float)num5);
				}
			}
			return -(int)((float)num5 / (num4 * num4));
		}

		public override bool IsCompatible(Barterable barterable)
		{
			return true;
		}

		public override ImageIdentifier GetVisualIdentifier()
		{
			return null;
		}

		public override void Apply()
		{
			if (PlayerEncounter.Current != null)
			{
				List<MobileParty> list = new List<MobileParty>();
				List<MobileParty> list2 = new List<MobileParty> { base.OriginalParty.MobileParty };
				PlayerEncounter.Current.FindAllNpcPartiesWhoWillJoinEvent(ref list, ref list2);
				PartyBase originalParty = base.OriginalParty;
				if (((originalParty != null) ? originalParty.SiegeEvent : null) != null && base.OriginalParty.SiegeEvent.BesiegerCamp.HasInvolvedPartyForEventType(base.OriginalParty, MapEvent.BattleTypes.Siege) && this._otherParty != null && base.OriginalParty.SiegeEvent.BesiegedSettlement.HasInvolvedPartyForEventType(this._otherParty, MapEvent.BattleTypes.Siege))
				{
					if (base.OriginalParty.SiegeEvent.BesiegedSettlement.MapFaction == Hero.MainHero.MapFaction)
					{
						GainKingdomInfluenceAction.ApplyForSiegeSafePassageBarter(MobileParty.MainParty, -10f);
					}
					Campaign.Current.GameMenuManager.SetNextMenu("menu_siege_safe_passage_accepted");
					PlayerSiege.ClosePlayerSiege();
					using (List<MobileParty>.Enumerator enumerator = list2.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							MobileParty mobileParty = enumerator.Current;
							mobileParty.Ai.SetDoNotAttackMainParty(32);
						}
						return;
					}
				}
				Settlement settlement = (from t in Settlement.All.Where((Settlement t) => base.OriginalParty.MobileParty.IsBandit == t.IsHideout && !base.OriginalParty.MobileParty.MapFaction.IsAtWarWith(t.MapFaction)).ToList<Settlement>()
					orderby t.GatePosition.DistanceSquared(base.OriginalParty.Position2D)
					select t).First<Settlement>();
				foreach (MobileParty mobileParty2 in list2)
				{
					mobileParty2.Ai.SetDoNotAttackMainParty(32);
					mobileParty2.Ai.SetMoveModeHold();
					mobileParty2.IgnoreForHours(32f);
					mobileParty2.Ai.SetInitiative(0f, 0.8f, 8f);
					if (settlement != null)
					{
						mobileParty2.Ai.SetMovePatrolAroundSettlement(settlement);
					}
				}
				PlayerEncounter.LeaveEncounter = true;
				if (MobileParty.MainParty.SiegeEvent != null && MobileParty.MainParty.SiegeEvent.BesiegerCamp.HasInvolvedPartyForEventType(PartyBase.MainParty, MapEvent.BattleTypes.Siege))
				{
					MobileParty.MainParty.BesiegerCamp = null;
				}
				PartyBase originalParty2 = base.OriginalParty;
				bool flag;
				if (originalParty2 == null)
				{
					flag = null != null;
				}
				else
				{
					MobileParty mobileParty3 = originalParty2.MobileParty;
					flag = ((mobileParty3 != null) ? mobileParty3.Ai.AiBehaviorPartyBase : null) != null;
				}
				if (flag && base.OriginalParty != PartyBase.MainParty)
				{
					base.OriginalParty.MobileParty.Ai.SetMoveModeHold();
					if (base.OriginalParty.MobileParty.Army != null && MobileParty.MainParty.Army != base.OriginalParty.MobileParty.Army)
					{
						base.OriginalParty.MobileParty.Army.LeaderParty.Ai.SetMoveModeHold();
						return;
					}
				}
			}
			else
			{
				Debug.FailedAssert("Can not find player encounter for safe passage barterable", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\BarterSystem\\Barterables\\SafePassageBarterable.cs", "Apply", 189);
			}
		}

		internal static void AutoGeneratedStaticCollectObjectsSafePassageBarterable(object o, List<object> collectedObjects)
		{
			((SafePassageBarterable)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		private readonly Hero _otherHero;

		private readonly PartyBase _otherParty;
	}
}
