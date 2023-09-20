using System;
using System.Collections.Generic;
using Helpers;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.BarterSystem.Barterables;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.BarterSystem
{
	// Token: 0x0200040F RID: 1039
	public class BarterManager
	{
		// Token: 0x17000CFF RID: 3327
		// (get) Token: 0x06003D70 RID: 15728 RVA: 0x00126EAE File Offset: 0x001250AE
		public static BarterManager Instance
		{
			get
			{
				return Campaign.Current.BarterManager;
			}
		}

		// Token: 0x17000D00 RID: 3328
		// (get) Token: 0x06003D71 RID: 15729 RVA: 0x00126EBA File Offset: 0x001250BA
		// (set) Token: 0x06003D72 RID: 15730 RVA: 0x00126EC2 File Offset: 0x001250C2
		[SaveableProperty(1)]
		public bool LastBarterIsAccepted { get; internal set; }

		// Token: 0x06003D73 RID: 15731 RVA: 0x00126ECB File Offset: 0x001250CB
		public BarterManager()
		{
			this._barteredHeroes = new Dictionary<Hero, CampaignTime>();
		}

		// Token: 0x06003D74 RID: 15732 RVA: 0x00126EDE File Offset: 0x001250DE
		public void BeginPlayerBarter(BarterData args)
		{
			if (this.BarterBegin != null)
			{
				this.BarterBegin(args);
			}
			ICampaignMission campaignMission = CampaignMission.Current;
			if (campaignMission == null)
			{
				return;
			}
			campaignMission.SetMissionMode(MissionMode.Barter, false);
		}

		// Token: 0x06003D75 RID: 15733 RVA: 0x00126F08 File Offset: 0x00125108
		private void AddBaseBarterables(BarterData args, IEnumerable<Barterable> defaultBarterables)
		{
			if (defaultBarterables != null)
			{
				bool flag = false;
				foreach (Barterable barterable in defaultBarterables)
				{
					if (!flag)
					{
						args.AddBarterGroup(new DefaultsBarterGroup());
						flag = true;
					}
					barterable.SetIsOffered(true);
					args.AddBarterable<OtherBarterGroup>(barterable, true);
					barterable.SetIsOffered(true);
				}
			}
		}

		// Token: 0x06003D76 RID: 15734 RVA: 0x00126F74 File Offset: 0x00125174
		public void StartBarterOffer(Hero offerer, Hero other, PartyBase offererParty, PartyBase otherParty, Hero beneficiaryOfOtherHero = null, BarterManager.BarterContextInitializer InitContext = null, int persuasionCostReduction = 0, bool isAIBarter = false, IEnumerable<Barterable> defaultBarterables = null)
		{
			this.LastBarterIsAccepted = false;
			if (offerer == Hero.MainHero && other != null && InitContext == null)
			{
				if (!this.CanPlayerBarterWithHero(other))
				{
					Debug.FailedAssert("Barter with the hero is on cooldown.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\BarterSystem\\BarterManager.cs", "StartBarterOffer", 87);
					return;
				}
				this.ClearHeroCooldowns();
			}
			BarterData barterData = new BarterData(offerer, beneficiaryOfOtherHero ?? other, offererParty, otherParty, InitContext, persuasionCostReduction, false);
			this.AddBaseBarterables(barterData, defaultBarterables);
			CampaignEventDispatcher.Instance.OnBarterablesRequested(barterData);
			if (!isAIBarter)
			{
				Campaign.Current.BarterManager.BeginPlayerBarter(barterData);
			}
		}

		// Token: 0x06003D77 RID: 15735 RVA: 0x00126FFC File Offset: 0x001251FC
		public void ExecuteAiBarter(IFaction faction1, IFaction faction2, Hero faction1Hero, Hero faction2Hero, Barterable barterable)
		{
			this.ExecuteAiBarter(faction1, faction2, faction1Hero, faction2Hero, new Barterable[] { barterable });
		}

		// Token: 0x06003D78 RID: 15736 RVA: 0x00127020 File Offset: 0x00125220
		public void ExecuteAiBarter(IFaction faction1, IFaction faction2, Hero faction1Hero, Hero faction2Hero, IEnumerable<Barterable> baseBarterables)
		{
			BarterData barterData = new BarterData(faction1.Leader, faction2.Leader, null, null, null, 0, true);
			barterData.AddBarterGroup(new DefaultsBarterGroup());
			foreach (Barterable barterable in baseBarterables)
			{
				barterable.SetIsOffered(true);
				barterData.AddBarterable<DefaultsBarterGroup>(barterable, true);
			}
			CampaignEventDispatcher.Instance.OnBarterablesRequested(barterData);
			Campaign.Current.BarterManager.ExecuteAIBarter(barterData, faction1, faction2, faction1Hero, faction2Hero);
		}

		// Token: 0x06003D79 RID: 15737 RVA: 0x001270B4 File Offset: 0x001252B4
		public void ExecuteAIBarter(BarterData barterData, IFaction faction1, IFaction faction2, Hero faction1Hero, Hero faction2Hero)
		{
			this.MakeBalanced(barterData, faction1, faction2, faction2Hero, 1f);
			this.MakeBalanced(barterData, faction2, faction1, faction1Hero, 1f);
			float offerValueForFaction = this.GetOfferValueForFaction(barterData, faction1);
			float offerValueForFaction2 = this.GetOfferValueForFaction(barterData, faction2);
			if (offerValueForFaction >= 0f && offerValueForFaction2 >= 0f)
			{
				this.ApplyBarterOffer(barterData.OffererHero, barterData.OtherHero, barterData.GetOfferedBarterables());
			}
		}

		// Token: 0x06003D7A RID: 15738 RVA: 0x0012711C File Offset: 0x0012531C
		private void MakeBalanced(BarterData args, IFaction faction1, IFaction faction2, Hero faction2Hero, float fulfillRatio)
		{
			foreach (ValueTuple<Barterable, int> valueTuple in BarterHelper.GetAutoBalanceBarterablesAdd(args, faction1, faction2, faction2Hero, fulfillRatio))
			{
				Barterable item = valueTuple.Item1;
				int item2 = valueTuple.Item2;
				if (!item.IsOffered)
				{
					item.SetIsOffered(true);
					item.CurrentAmount = 0;
				}
				item.CurrentAmount += item2;
			}
		}

		// Token: 0x06003D7B RID: 15739 RVA: 0x00127198 File Offset: 0x00125398
		public void Close()
		{
			if (CampaignMission.Current != null)
			{
				CampaignMission.Current.SetMissionMode(MissionMode.Conversation, false);
			}
			if (this.Closed != null)
			{
				this.Closed();
			}
		}

		// Token: 0x06003D7C RID: 15740 RVA: 0x001271C0 File Offset: 0x001253C0
		public bool IsOfferAcceptable(BarterData args, Hero hero, PartyBase party)
		{
			return this.GetOfferValue(hero, party, args.OffererParty, args.GetOfferedBarterables()) > -0.01f;
		}

		// Token: 0x06003D7D RID: 15741 RVA: 0x001271E0 File Offset: 0x001253E0
		public float GetOfferValueForFaction(BarterData barterData, IFaction faction)
		{
			int num = 0;
			foreach (Barterable barterable in barterData.GetOfferedBarterables())
			{
				num += barterable.GetValueForFaction(faction);
			}
			return (float)num;
		}

		// Token: 0x06003D7E RID: 15742 RVA: 0x0012723C File Offset: 0x0012543C
		public float GetOfferValue(Hero selfHero, PartyBase selfParty, PartyBase offererParty, IEnumerable<Barterable> offeredBarters)
		{
			float num = 0f;
			IFaction faction;
			if (((selfHero != null) ? selfHero.Clan : null) != null)
			{
				IFaction clan = selfHero.Clan;
				faction = clan;
			}
			else
			{
				faction = selfParty.MapFaction;
			}
			IFaction faction2 = faction;
			foreach (Barterable barterable in offeredBarters)
			{
				num += (float)barterable.GetValueForFaction(faction2);
			}
			this._overpayAmount = ((num > 0f) ? num : 0f);
			return num;
		}

		// Token: 0x06003D7F RID: 15743 RVA: 0x001272C8 File Offset: 0x001254C8
		public void ApplyAndFinalizePlayerBarter(Hero offererHero, Hero otherHero, BarterData barterData)
		{
			this.LastBarterIsAccepted = true;
			this.ApplyBarterOffer(offererHero, otherHero, barterData.GetOfferedBarterables());
			if (otherHero != null)
			{
				this.HandleHeroCooldown(otherHero);
			}
		}

		// Token: 0x06003D80 RID: 15744 RVA: 0x001272E9 File Offset: 0x001254E9
		public void CancelAndFinalizePlayerBarter(Hero offererHero, Hero otherHero, BarterData barterData)
		{
			this.CancelBarter(offererHero, otherHero, barterData.GetOfferedBarterables());
		}

		// Token: 0x06003D81 RID: 15745 RVA: 0x001272FC File Offset: 0x001254FC
		private void ApplyBarterOffer(Hero offererHero, Hero otherHero, List<Barterable> barters)
		{
			foreach (Barterable barterable in barters)
			{
				barterable.Apply();
			}
			CampaignEventDispatcher.Instance.OnBarterAccepted(offererHero, otherHero, barters);
			if (offererHero == Hero.MainHero)
			{
				if (this._overpayAmount > 0f && otherHero != null)
				{
					this.ApplyOverpayBonus(otherHero);
				}
				this.Close();
				if (Campaign.Current.ConversationManager.IsConversationInProgress)
				{
					Campaign.Current.ConversationManager.ContinueConversation();
				}
				MBInformationManager.AddQuickInformation(GameTexts.FindText("str_offer_accepted", null), 0, null, "");
			}
		}

		// Token: 0x06003D82 RID: 15746 RVA: 0x001273B0 File Offset: 0x001255B0
		private void CancelBarter(Hero offererHero, Hero otherHero, List<Barterable> offeredBarters)
		{
			this.Close();
			MBInformationManager.AddQuickInformation(GameTexts.FindText("str_offer_rejected", null), 0, null, "");
			CampaignEventDispatcher.Instance.OnBarterCanceled(offererHero, otherHero, offeredBarters);
			Campaign.Current.ConversationManager.ContinueConversation();
		}

		// Token: 0x06003D83 RID: 15747 RVA: 0x001273EC File Offset: 0x001255EC
		private void ApplyOverpayBonus(Hero otherHero)
		{
			if (otherHero.MapFaction.IsAtWarWith(Hero.MainHero.MapFaction))
			{
				return;
			}
			int num = Campaign.Current.Models.BarterModel.CalculateOverpayRelationIncreaseCosts(otherHero, this._overpayAmount);
			if (num > 0)
			{
				ChangeRelationAction.ApplyRelationChangeBetweenHeroes(Hero.MainHero, otherHero, num, true);
			}
		}

		// Token: 0x06003D84 RID: 15748 RVA: 0x00127440 File Offset: 0x00125640
		public bool CanPlayerBarterWithHero(Hero hero)
		{
			CampaignTime campaignTime;
			return !this._barteredHeroes.TryGetValue(hero, out campaignTime) || campaignTime.IsPast;
		}

		// Token: 0x06003D85 RID: 15749 RVA: 0x00127468 File Offset: 0x00125668
		private void HandleHeroCooldown(Hero hero)
		{
			CampaignTime campaignTime = CampaignTime.Now + CampaignTime.Days((float)Campaign.Current.Models.BarterModel.BarterCooldownWithHeroInDays);
			if (!this._barteredHeroes.ContainsKey(hero))
			{
				this._barteredHeroes.Add(hero, campaignTime);
				return;
			}
			this._barteredHeroes[hero] = campaignTime;
		}

		// Token: 0x06003D86 RID: 15750 RVA: 0x001274C4 File Offset: 0x001256C4
		private void ClearHeroCooldowns()
		{
			foreach (KeyValuePair<Hero, CampaignTime> keyValuePair in new Dictionary<Hero, CampaignTime>(this._barteredHeroes))
			{
				if (keyValuePair.Value.IsPast)
				{
					this._barteredHeroes.Remove(keyValuePair.Key);
				}
			}
		}

		// Token: 0x06003D87 RID: 15751 RVA: 0x0012753C File Offset: 0x0012573C
		public bool InitializeMarriageBarterContext(Barterable barterable, BarterData args, object obj)
		{
			Hero hero = null;
			Hero hero2 = null;
			if (obj != null)
			{
				Tuple<Hero, Hero> tuple = obj as Tuple<Hero, Hero>;
				if (tuple != null)
				{
					hero = tuple.Item1;
					hero2 = tuple.Item2;
				}
			}
			MarriageBarterable marriageBarterable = barterable as MarriageBarterable;
			return marriageBarterable != null && hero != null && hero2 != null && marriageBarterable.ProposingHero == hero2 && marriageBarterable.HeroBeingProposedTo == hero;
		}

		// Token: 0x06003D88 RID: 15752 RVA: 0x0012758C File Offset: 0x0012578C
		public bool InitializeJoinFactionBarterContext(Barterable barterable, BarterData args, object obj)
		{
			return barterable.GetType() == typeof(JoinKingdomAsClanBarterable) && barterable.OriginalOwner == Hero.OneToOneConversationHero;
		}

		// Token: 0x06003D89 RID: 15753 RVA: 0x001275B4 File Offset: 0x001257B4
		public bool InitializeMakePeaceBarterContext(Barterable barterable, BarterData args, object obj)
		{
			return barterable.GetType() == typeof(PeaceBarterable) && barterable.OriginalOwner == args.OtherHero;
		}

		// Token: 0x06003D8A RID: 15754 RVA: 0x001275DD File Offset: 0x001257DD
		public bool InitializeSafePassageBarterContext(Barterable barterable, BarterData args, object obj)
		{
			if (barterable.GetType() == typeof(SafePassageBarterable))
			{
				PartyBase originalParty = barterable.OriginalParty;
				MobileParty conversationParty = MobileParty.ConversationParty;
				return originalParty == ((conversationParty != null) ? conversationParty.Party : null);
			}
			return false;
		}

		// Token: 0x06003D8B RID: 15755 RVA: 0x00127611 File Offset: 0x00125811
		internal static void AutoGeneratedStaticCollectObjectsBarterManager(object o, List<object> collectedObjects)
		{
			((BarterManager)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		// Token: 0x06003D8C RID: 15756 RVA: 0x0012761F File Offset: 0x0012581F
		protected virtual void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			collectedObjects.Add(this._barteredHeroes);
		}

		// Token: 0x06003D8D RID: 15757 RVA: 0x0012762D File Offset: 0x0012582D
		internal static object AutoGeneratedGetMemberValueLastBarterIsAccepted(object o)
		{
			return ((BarterManager)o).LastBarterIsAccepted;
		}

		// Token: 0x06003D8E RID: 15758 RVA: 0x0012763F File Offset: 0x0012583F
		internal static object AutoGeneratedGetMemberValue_barteredHeroes(object o)
		{
			return ((BarterManager)o)._barteredHeroes;
		}

		// Token: 0x0400127F RID: 4735
		public BarterManager.BarterCloseEventDelegate Closed;

		// Token: 0x04001280 RID: 4736
		public BarterManager.BarterBeginEventDelegate BarterBegin;

		// Token: 0x04001281 RID: 4737
		[SaveableField(2)]
		private readonly Dictionary<Hero, CampaignTime> _barteredHeroes;

		// Token: 0x04001282 RID: 4738
		private float _overpayAmount;

		// Token: 0x0200074D RID: 1869
		// (Invoke) Token: 0x0600568E RID: 22158
		public delegate bool BarterContextInitializer(Barterable barterable, BarterData args, object obj = null);

		// Token: 0x0200074E RID: 1870
		// (Invoke) Token: 0x06005692 RID: 22162
		public delegate void BarterCloseEventDelegate();

		// Token: 0x0200074F RID: 1871
		// (Invoke) Token: 0x06005696 RID: 22166
		public delegate void BarterBeginEventDelegate(BarterData args);
	}
}
