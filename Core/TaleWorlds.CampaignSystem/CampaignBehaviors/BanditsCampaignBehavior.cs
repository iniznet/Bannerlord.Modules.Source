using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.BarterSystem;
using TaleWorlds.CampaignSystem.BarterSystem.Barterables;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.Inventory;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	public class BanditsCampaignBehavior : CampaignBehaviorBase
	{
		private int IdealBanditPartyCount
		{
			get
			{
				return this._numberOfMaxHideoutsAtEachBanditFaction * (this._numberOfMaxBanditPartiesAroundEachHideout + this._numberOfMaximumBanditPartiesInEachHideout) + this._numberOfMaximumLooterParties;
			}
		}

		private int _numberOfMaximumLooterParties
		{
			get
			{
				return Campaign.Current.Models.BanditDensityModel.NumberOfMaximumLooterParties;
			}
		}

		private float _radiusAroundPlayerPartySquared
		{
			get
			{
				return MobileParty.MainParty.SeeingRange * MobileParty.MainParty.SeeingRange * 1.25f;
			}
		}

		private float _numberOfMinimumBanditPartiesInAHideoutToInfestIt
		{
			get
			{
				return (float)Campaign.Current.Models.BanditDensityModel.NumberOfMinimumBanditPartiesInAHideoutToInfestIt;
			}
		}

		private int _numberOfMaxBanditPartiesAroundEachHideout
		{
			get
			{
				return Campaign.Current.Models.BanditDensityModel.NumberOfMaximumBanditPartiesAroundEachHideout;
			}
		}

		private int _numberOfMaxHideoutsAtEachBanditFaction
		{
			get
			{
				return Campaign.Current.Models.BanditDensityModel.NumberOfMaximumHideoutsAtEachBanditFaction;
			}
		}

		private int _numberOfInitialHideoutsAtEachBanditFaction
		{
			get
			{
				return Campaign.Current.Models.BanditDensityModel.NumberOfInitialHideoutsAtEachBanditFaction;
			}
		}

		private int _numberOfMaximumBanditPartiesInEachHideout
		{
			get
			{
				return Campaign.Current.Models.BanditDensityModel.NumberOfMaximumBanditPartiesInEachHideout;
			}
		}

		public override void RegisterEvents()
		{
			CampaignEvents.SettlementEntered.AddNonSerializedListener(this, new Action<MobileParty, Settlement, Hero>(this.OnSettlementEntered));
			CampaignEvents.WeeklyTickEvent.AddNonSerializedListener(this, new Action(this.WeeklyTick));
			CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, new Action(this.DailyTick));
			CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new Action(this.HourlyTick));
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
			CampaignEvents.OnNewGameCreatedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnNewGameCreated));
			CampaignEvents.OnNewGameCreatedPartialFollowUpEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter, int>(this.OnNewGameCreatedPartialFollowUp));
			CampaignEvents.MobilePartyDestroyed.AddNonSerializedListener(this, new Action<MobileParty, PartyBase>(this.OnPartyDestroyed));
		}

		private void OnNewGameCreated(CampaignGameStarter starter)
		{
		}

		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<Dictionary<MobileParty, BanditsCampaignBehavior.PlayerInteraction>>("_interactedBandits", ref this._interactedBandits);
			dataStore.SyncData<bool>("_hideoutsAndBanditsAreInited", ref this._hideoutsAndBanditsAreInitialized);
		}

		private void OnNewGameCreatedPartialFollowUp(CampaignGameStarter starter, int i)
		{
			if (i == 0)
			{
				this.MakeBanditFactionsEnemyToKingdomFactions();
				this._hideoutsAndBanditsAreInitialized = false;
			}
			if (i < 10)
			{
				if (this._numberOfMaxHideoutsAtEachBanditFaction > 0)
				{
					int num = Clan.BanditFactions.Count((Clan x) => !BanditsCampaignBehavior.IsLooterFaction(x));
					int num2 = num / 10 + ((num % 10 > i) ? 1 : 0);
					int num3 = num / 10 * i;
					for (int j = 0; j < i; j++)
					{
						num3 += ((num % 10 > j) ? 1 : 0);
					}
					for (int k = 0; k < num2; k++)
					{
						this.SpawnHideoutsAndBanditsPartiallyOnNewGame(Clan.BanditFactions.ElementAt(num3 + k));
					}
					return;
				}
			}
			else
			{
				int num4 = i - 10;
				int idealBanditPartyCount = this.IdealBanditPartyCount;
				int num5 = idealBanditPartyCount / 90 + ((idealBanditPartyCount % 90 > num4) ? 1 : 0);
				int num6 = idealBanditPartyCount / 90 * num4;
				for (int l = 0; l < num4; l++)
				{
					num6 += ((idealBanditPartyCount % 90 > l) ? 1 : 0);
				}
				for (int m = 0; m < num5; m++)
				{
					this.SpawnBanditOrLooterPartiesAroundAHideoutOrSettlement(num6 + m);
				}
			}
		}

		private void SpawnHideoutsAndBanditsPartiallyOnNewGame(Clan banditClan)
		{
			if (!BanditsCampaignBehavior.IsLooterFaction(banditClan))
			{
				for (int i = 0; i < this._numberOfInitialHideoutsAtEachBanditFaction; i++)
				{
					this.FillANewHideoutWithBandits(banditClan);
				}
			}
		}

		private void MakeBanditFactionsEnemyToKingdomFactions()
		{
			foreach (Clan clan in Clan.BanditFactions)
			{
				if (clan.IsBanditFaction && !clan.IsMinorFaction)
				{
					foreach (Kingdom kingdom in Kingdom.All)
					{
						FactionManager.DeclareWar(kingdom, clan, false);
					}
					FactionManager.DeclareWar(Hero.MainHero.Clan, clan, false);
				}
			}
		}

		private void OnPartyDestroyed(MobileParty mobileParty, PartyBase destroyerParty)
		{
			if (this._interactedBandits.ContainsKey(mobileParty))
			{
				this._interactedBandits.Remove(mobileParty);
			}
		}

		private void SetPlayerInteraction(MobileParty mobileParty, BanditsCampaignBehavior.PlayerInteraction interaction)
		{
			if (this._interactedBandits.ContainsKey(mobileParty))
			{
				this._interactedBandits[mobileParty] = interaction;
				return;
			}
			this._interactedBandits.Add(mobileParty, interaction);
		}

		private BanditsCampaignBehavior.PlayerInteraction GetPlayerInteraction(MobileParty mobileParty)
		{
			BanditsCampaignBehavior.PlayerInteraction playerInteraction;
			if (this._interactedBandits.TryGetValue(mobileParty, out playerInteraction))
			{
				return playerInteraction;
			}
			return BanditsCampaignBehavior.PlayerInteraction.None;
		}

		public void OnSettlementEntered(MobileParty mobileParty, Settlement settlement, Hero hero)
		{
			this.CheckForSpawningBanditBoss(settlement, mobileParty);
			if (Campaign.Current.GameStarted && mobileParty != null && mobileParty.IsBandit && settlement.IsHideout)
			{
				if (!settlement.Hideout.IsSpotted && settlement.Hideout.IsInfested)
				{
					float lengthSquared = (MobileParty.MainParty.Position2D - settlement.Position2D).LengthSquared;
					float seeingRange = MobileParty.MainParty.SeeingRange;
					float num = seeingRange * seeingRange / lengthSquared;
					float partySpottingDifficulty = Campaign.Current.Models.MapVisibilityModel.GetPartySpottingDifficulty(MobileParty.MainParty, mobileParty);
					if (num / partySpottingDifficulty >= 1f)
					{
						settlement.Hideout.IsSpotted = true;
						settlement.Party.UpdateVisibilityAndInspected(0f, false);
						CampaignEventDispatcher.Instance.OnHideoutSpotted(MobileParty.MainParty.Party, settlement.Party);
					}
				}
				int num2 = 0;
				foreach (ItemRosterElement itemRosterElement in mobileParty.ItemRoster)
				{
					int num3 = (itemRosterElement.EquipmentElement.Item.IsFood ? MBRandom.RoundRandomized((float)mobileParty.MemberRoster.TotalManCount * ((3f + 6f * MBRandom.RandomFloat) / (float)itemRosterElement.EquipmentElement.Item.Value)) : 0);
					if (itemRosterElement.Amount > num3)
					{
						int num4 = itemRosterElement.Amount - num3;
						num2 += num4 * itemRosterElement.EquipmentElement.Item.Value;
					}
				}
				if (num2 > 0)
				{
					if (mobileParty.IsPartyTradeActive)
					{
						mobileParty.PartyTradeGold += (int)(0.25f * (float)num2);
					}
					settlement.SettlementComponent.ChangeGold((int)(0.25f * (float)num2));
				}
			}
		}

		private void CheckForSpawningBanditBoss(Settlement settlement, MobileParty mobileParty)
		{
			if (settlement.IsHideout && settlement.Hideout.IsSpotted)
			{
				if (settlement.Parties.Any((MobileParty x) => x.IsBandit || x.IsBanditBossParty))
				{
					CultureObject culture = settlement.Culture;
					MobileParty mobileParty2 = settlement.Parties.FirstOrDefault((MobileParty x) => x.IsBanditBossParty);
					if (mobileParty2 == null)
					{
						this.AddBossParty(settlement, culture);
						return;
					}
					if (!mobileParty2.MemberRoster.Contains(culture.BanditBoss))
					{
						mobileParty2.MemberRoster.AddToCounts(culture.BanditBoss, 1, false, 0, 0, true, -1);
					}
				}
			}
		}

		private void AddBossParty(Settlement settlement, CultureObject culture)
		{
			PartyTemplateObject banditBossPartyTemplate = culture.BanditBossPartyTemplate;
			if (banditBossPartyTemplate != null)
			{
				this.AddBanditToHideout(settlement.Hideout, banditBossPartyTemplate, true).Ai.DisableAi();
			}
		}

		public void DailyTick()
		{
			foreach (MobileParty mobileParty in MobileParty.AllBanditParties)
			{
				if (mobileParty.IsPartyTradeActive)
				{
					mobileParty.PartyTradeGold = (int)((double)mobileParty.PartyTradeGold * 0.95 + (double)(50f * (float)mobileParty.Party.MemberRoster.TotalManCount * 0.05f));
					if (MBRandom.RandomFloat < 0.03f && mobileParty.MapEvent != null)
					{
						foreach (ItemObject itemObject in Items.All)
						{
							if (itemObject.IsFood)
							{
								int num = (BanditsCampaignBehavior.IsLooterFaction(mobileParty.MapFaction) ? 8 : 16);
								int num2 = MBRandom.RoundRandomized((float)mobileParty.MemberRoster.TotalManCount * (1f / (float)itemObject.Value) * (float)num * MBRandom.RandomFloat * MBRandom.RandomFloat * MBRandom.RandomFloat * MBRandom.RandomFloat);
								if (num2 > 0)
								{
									mobileParty.ItemRoster.AddToCounts(itemObject, num2);
								}
							}
						}
					}
				}
			}
		}

		private void TryToSpawnHideoutAndBanditHourly()
		{
			this._hideoutsAndBanditsAreInitialized = true;
			int num = 0;
			foreach (Clan clan in Clan.BanditFactions)
			{
				if (!BanditsCampaignBehavior.IsLooterFaction(clan))
				{
					for (int i = 0; i < this._numberOfInitialHideoutsAtEachBanditFaction; i++)
					{
						this.FillANewHideoutWithBandits(clan);
						num++;
					}
				}
			}
			int num2 = (int)(0.5f * (float)(this._numberOfMaxBanditPartiesAroundEachHideout * num + this._numberOfMaximumLooterParties));
			if (num2 > 0)
			{
				this.SpawnBanditOrLooterPartiesAroundAHideoutOrSettlement(num2);
			}
		}

		public void HourlyTick()
		{
			if (!this._hideoutsAndBanditsAreInitialized && this._numberOfMaxHideoutsAtEachBanditFaction > 0)
			{
				this.TryToSpawnHideoutAndBanditHourly();
			}
			if (Campaign.Current.IsNight)
			{
				int num = 0;
				foreach (Clan clan in Clan.BanditFactions)
				{
					num += clan.WarPartyComponents.Count;
				}
				int num2 = MBRandom.RoundRandomized((float)(this.IdealBanditPartyCount - num) * 0.01f);
				if (num2 > 0)
				{
					this.SpawnBanditOrLooterPartiesAroundAHideoutOrSettlement(num2);
				}
			}
		}

		public void WeeklyTick()
		{
			this.AddNewHideouts();
		}

		public void OnSessionLaunched(CampaignGameStarter campaignGameStarter)
		{
			this.AddDialogs(campaignGameStarter);
		}

		private void AddNewHideouts()
		{
			foreach (Clan clan in Clan.BanditFactions)
			{
				if (!BanditsCampaignBehavior.IsLooterFaction(clan))
				{
					int num = 0;
					foreach (Hideout hideout in Campaign.Current.AllHideouts)
					{
						if (hideout.IsInfested && hideout.Settlement.Culture == clan.Culture)
						{
							num++;
						}
					}
					float num2 = 0f;
					if ((float)num < (float)this._numberOfMaxHideoutsAtEachBanditFaction * 0.5f)
					{
						num2 = 1f - (float)num / (float)MathF.Ceiling((float)this._numberOfMaxHideoutsAtEachBanditFaction * 0.5f);
						num2 = MathF.Max(0f, num2 * num2);
					}
					if (MBRandom.RandomFloat < num2)
					{
						this.FillANewHideoutWithBandits(clan);
					}
				}
			}
		}

		private void FillANewHideoutWithBandits(Clan faction)
		{
			Hideout hideout = this.SelectARandomHideout(faction, false, true, true);
			if (hideout != null)
			{
				int num = 0;
				while ((float)num < this._numberOfMinimumBanditPartiesInAHideoutToInfestIt)
				{
					this.AddBanditToHideout(hideout, null, false);
					num++;
				}
			}
		}

		public MobileParty AddBanditToHideout(Hideout hideoutComponent, PartyTemplateObject overridenPartyTemplate = null, bool isBanditBossParty = false)
		{
			if (hideoutComponent.Owner.Settlement.Culture.IsBandit)
			{
				Clan clan = null;
				foreach (Clan clan2 in Clan.BanditFactions)
				{
					if (hideoutComponent.Owner.Settlement.Culture == clan2.Culture)
					{
						clan = clan2;
					}
				}
				PartyTemplateObject partyTemplateObject = overridenPartyTemplate ?? clan.DefaultPartyTemplate;
				MobileParty mobileParty = BanditPartyComponent.CreateBanditParty(clan.StringId + "_1", clan, hideoutComponent, isBanditBossParty);
				mobileParty.InitializeMobilePartyAtPosition(partyTemplateObject, hideoutComponent.Owner.Settlement.Position2D, -1);
				this.InitBanditParty(mobileParty, clan, hideoutComponent.Owner.Settlement);
				mobileParty.Ai.SetMoveGoToSettlement(hideoutComponent.Owner.Settlement);
				mobileParty.Ai.RecalculateShortTermAi();
				EnterSettlementAction.ApplyForParty(mobileParty, hideoutComponent.Owner.Settlement);
				return mobileParty;
			}
			return null;
		}

		private Hideout SelectARandomHideout(Clan faction, bool isInfestedHideoutNeeded, bool sameFactionIsNeeded, bool selectingFurtherToOthersNeeded = false)
		{
			float num = Campaign.AverageDistanceBetweenTwoFortifications * 0.33f * Campaign.AverageDistanceBetweenTwoFortifications * 0.33f;
			List<ValueTuple<Hideout, float>> list = new List<ValueTuple<Hideout, float>>();
			foreach (Hideout hideout in Hideout.All)
			{
				if ((!sameFactionIsNeeded || hideout.Settlement.Culture == faction.Culture) && (!isInfestedHideoutNeeded || hideout.IsInfested))
				{
					int num2 = 1;
					if (selectingFurtherToOthersNeeded)
					{
						float num3 = Campaign.MapDiagonalSquared;
						float num4 = Campaign.MapDiagonalSquared;
						foreach (Hideout hideout2 in Hideout.All)
						{
							if (hideout != hideout2 && hideout2.IsInfested)
							{
								float num5 = hideout.Settlement.Position2D.DistanceSquared(hideout2.Settlement.Position2D);
								if (hideout.Settlement.Culture == hideout2.Settlement.Culture && num5 < num3)
								{
									num3 = num5;
								}
								if (num5 < num4)
								{
									num4 = num5;
								}
							}
						}
						num2 = (int)MathF.Max(1f, num3 / num + 5f * (num4 / num));
					}
					list.Add(new ValueTuple<Hideout, float>(hideout, (float)num2));
				}
			}
			return MBRandom.ChooseWeighted<Hideout>(list);
		}

		private void SpawnBanditOrLooterPartiesAroundAHideoutOrSettlement(int numberOfBanditsWillBeSpawned)
		{
			List<Clan> list = Clan.BanditFactions.ToList<Clan>();
			Dictionary<Clan, int> dictionary = new Dictionary<Clan, int>(list.Count);
			foreach (Clan clan in list)
			{
				dictionary.Add(clan, 0);
			}
			foreach (Hideout hideout in Campaign.Current.AllHideouts)
			{
				if (hideout.IsInfested && hideout.MapFaction is Clan)
				{
					Dictionary<Clan, int> dictionary2 = dictionary;
					Clan clan2 = hideout.MapFaction as Clan;
					int num = dictionary2[clan2];
					dictionary2[clan2] = num + 1;
				}
			}
			int num2 = this._numberOfMaxBanditPartiesAroundEachHideout + this._numberOfMaximumBanditPartiesInEachHideout + 1;
			int num3 = this._numberOfMaxHideoutsAtEachBanditFaction * num2;
			int num4 = 0;
			foreach (Clan clan3 in list)
			{
				num4 += clan3.WarPartyComponents.Count;
			}
			numberOfBanditsWillBeSpawned = MathF.Max(0, MathF.Min(numberOfBanditsWillBeSpawned, list.Count((Clan f) => !BanditsCampaignBehavior.IsLooterFaction(f)) * num3 + this._numberOfMaximumLooterParties - num4));
			numberOfBanditsWillBeSpawned = MathF.Ceiling((float)numberOfBanditsWillBeSpawned * 0.667f) + MBRandom.RandomInt(numberOfBanditsWillBeSpawned / 3);
			for (int i = 0; i < numberOfBanditsWillBeSpawned; i++)
			{
				Clan clan4 = null;
				float num5 = 1f;
				for (int j = 0; j < list.Count; j++)
				{
					float num6 = 1f;
					if (BanditsCampaignBehavior.IsLooterFaction(list[j]))
					{
						num6 = (float)list[j].WarPartyComponents.Count / (float)this._numberOfMaximumLooterParties;
					}
					else
					{
						int num7 = dictionary[list[j]];
						if (num7 > 0)
						{
							num6 = (float)list[j].WarPartyComponents.Count / (float)(num7 * num2);
						}
					}
					if (num6 < 1f && (clan4 == null || num6 < num5))
					{
						clan4 = list[j];
						num5 = num6;
					}
				}
				if (clan4 == null)
				{
					return;
				}
				this.SpawnAPartyInFaction(clan4);
			}
		}

		private void SpawnAPartyInFaction(Clan selectedFaction)
		{
			PartyTemplateObject defaultPartyTemplate = selectedFaction.DefaultPartyTemplate;
			Settlement settlement;
			if (BanditsCampaignBehavior.IsLooterFaction(selectedFaction))
			{
				settlement = this.SelectARandomSettlementForLooterParty();
			}
			else
			{
				Hideout hideout = this.SelectARandomHideout(selectedFaction, true, true, false);
				settlement = ((hideout != null) ? hideout.Owner.Settlement : null);
				if (settlement == null)
				{
					hideout = this.SelectARandomHideout(selectedFaction, false, true, false);
					settlement = ((hideout != null) ? hideout.Owner.Settlement : null);
					if (settlement == null)
					{
						hideout = this.SelectARandomHideout(selectedFaction, false, false, false);
						settlement = ((hideout != null) ? hideout.Owner.Settlement : null);
					}
				}
			}
			MobileParty mobileParty = (settlement.IsHideout ? BanditPartyComponent.CreateBanditParty(selectedFaction.StringId + "_1", selectedFaction, settlement.Hideout, false) : BanditPartyComponent.CreateLooterParty(selectedFaction.StringId + "_1", selectedFaction, settlement, false));
			if (settlement != null)
			{
				float num = 45f * (BanditsCampaignBehavior.IsLooterFaction(selectedFaction) ? 1.5f : 1f);
				mobileParty.InitializeMobilePartyAroundPosition(defaultPartyTemplate, settlement.GatePosition, num, 0f, -1);
				Vec2 vec = mobileParty.Position2D;
				float radiusAroundPlayerPartySquared = this._radiusAroundPlayerPartySquared;
				for (int i = 0; i < 15; i++)
				{
					Vec2 vec2 = MobilePartyHelper.FindReachablePointAroundPosition(vec, num, 0f);
					if (vec2.DistanceSquared(MobileParty.MainParty.Position2D) > radiusAroundPlayerPartySquared)
					{
						vec = vec2;
						break;
					}
				}
				if (vec != mobileParty.Position2D)
				{
					mobileParty.Position2D = vec;
				}
				this.InitBanditParty(mobileParty, selectedFaction, settlement);
				mobileParty.Aggressiveness = 1f - 0.2f * MBRandom.RandomFloat;
				mobileParty.Ai.SetMovePatrolAroundPoint(settlement.IsTown ? settlement.GatePosition : settlement.Position2D);
			}
		}

		private static bool IsLooterFaction(IFaction faction)
		{
			return !faction.Culture.CanHaveSettlement;
		}

		private Settlement SelectARandomSettlementForLooterParty()
		{
			int num = 0;
			foreach (Settlement settlement in Settlement.All)
			{
				if (settlement.IsTown || settlement.IsVillage)
				{
					int num2 = BanditsCampaignBehavior.CalculateDistanceScore(settlement.Position2D.DistanceSquared(MobileParty.MainParty.Position2D));
					num += num2;
				}
			}
			int num3 = MBRandom.RandomInt(num);
			foreach (Settlement settlement2 in Settlement.All)
			{
				if (settlement2.IsTown || settlement2.IsVillage)
				{
					int num4 = BanditsCampaignBehavior.CalculateDistanceScore(settlement2.Position2D.DistanceSquared(MobileParty.MainParty.Position2D));
					num3 -= num4;
					if (num3 <= 0)
					{
						return settlement2;
					}
				}
			}
			return null;
		}

		private void InitBanditParty(MobileParty banditParty, Clan faction, Settlement homeSettlement)
		{
			banditParty.Party.Visuals.SetMapIconAsDirty();
			banditParty.ActualClan = faction;
			BanditsCampaignBehavior.CreatePartyTrade(banditParty);
			foreach (ItemObject itemObject in Items.All)
			{
				if (itemObject.IsFood)
				{
					int num = (BanditsCampaignBehavior.IsLooterFaction(banditParty.MapFaction) ? 8 : 16);
					int num2 = MBRandom.RoundRandomized((float)banditParty.MemberRoster.TotalManCount * (1f / (float)itemObject.Value) * (float)num * MBRandom.RandomFloat * MBRandom.RandomFloat * MBRandom.RandomFloat * MBRandom.RandomFloat);
					if (num2 > 0)
					{
						banditParty.ItemRoster.AddToCounts(itemObject, num2);
					}
				}
			}
		}

		private static void CreatePartyTrade(MobileParty banditParty)
		{
			int num = (int)(10f * (float)banditParty.Party.MemberRoster.TotalManCount * (0.5f + 1f * MBRandom.RandomFloat));
			banditParty.InitializePartyTrade(num);
		}

		private static int CalculateDistanceScore(float distance)
		{
			int num = 2;
			if (distance < 10000f)
			{
				num = 8;
			}
			else if (distance < 40000f)
			{
				num = 6;
			}
			else if (distance < 160000f)
			{
				num = 4;
			}
			else if (distance < 420000f)
			{
				num = 3;
			}
			return num;
		}

		protected void AddDialogs(CampaignGameStarter campaignGameSystemStarter)
		{
			campaignGameSystemStarter.AddDialogLine("bandit_start_defender", "start", "bandit_defender", "{=!}{ROBBERY_THREAT}", new ConversationSentence.OnConditionDelegate(this.bandit_start_defender_condition), null, 100, null);
			campaignGameSystemStarter.AddPlayerLine("bandit_start_defender_1", "bandit_defender", "bandit_start_fight", "{=DEnFOGhS}Fight me if you dare!", null, null, 100, null, null);
			campaignGameSystemStarter.AddPlayerLine("bandit_start_defender_2", "bandit_defender", "barter_with_bandit_prebarter", "{=aQYMefHU}Maybe we can work out something.", new ConversationSentence.OnConditionDelegate(this.bandit_start_barter_condition), null, 100, null, null);
			campaignGameSystemStarter.AddDialogLine("bandit_start_fight", "bandit_start_fight", "close_window", "{=!}{ROBBERY_START_FIGHT}[ib:aggressive]", null, new ConversationSentence.OnConsequenceDelegate(this.conversation_bandit_set_hostile_on_consequence), 100, null);
			campaignGameSystemStarter.AddDialogLine("barter_with_bandit_prebarter", "barter_with_bandit_prebarter", "barter_with_bandit_screen", "{=!}{ROBBERY_PAY_AGREEMENT}", null, null, 100, null);
			campaignGameSystemStarter.AddDialogLine("barter_with_bandit_screen", "barter_with_bandit_screen", "barter_with_bandit_postbarter", "{=!}Barter screen goes here", null, new ConversationSentence.OnConsequenceDelegate(this.bandit_start_barter_consequence), 100, null);
			campaignGameSystemStarter.AddDialogLine("barter_with_bandit_postbarter_1", "barter_with_bandit_postbarter", "close_window", "{=!}{ROBBERY_CONCLUSION}", new ConversationSentence.OnConditionDelegate(this.bandit_barter_successful_condition), new ConversationSentence.OnConsequenceDelegate(this.bandit_barter_successful_on_consequence), 100, null);
			campaignGameSystemStarter.AddDialogLine("barter_with_bandit_postbarter_2", "barter_with_bandit_postbarter", "close_window", "{=!}{ROBBERY_START_FIGHT}", () => !this.bandit_barter_successful_condition(), new ConversationSentence.OnConsequenceDelegate(this.conversation_bandit_set_hostile_on_consequence), 100, null);
			campaignGameSystemStarter.AddDialogLine("bandit_start_attacker", "start", "bandit_attacker", "{=!}{BANDIT_NEUTRAL_GREETING}", new ConversationSentence.OnConditionDelegate(this.bandit_neutral_greet_on_condition), new ConversationSentence.OnConsequenceDelegate(this.bandit_neutral_greet_on_consequence), 100, null);
			campaignGameSystemStarter.AddPlayerLine("common_encounter_ultimatum", "bandit_attacker", "common_encounter_ultimatum_answer", "{=!}{BANDIT_ULTIMATUM}", null, null, 100, null, null);
			campaignGameSystemStarter.AddPlayerLine("common_encounter_fight", "bandit_attacker", "bandit_attacker_leave", "{=3W3eEIIZ}Never mind. You can go.", null, null, 100, null, null);
			campaignGameSystemStarter.AddDialogLine("common_encounter_ultimatum_we_can_join_you", "common_encounter_ultimatum_answer", "bandits_we_can_join_you", "{=B5UMlqHc}I'll be honest... We don't want to die. Would you take us on as hired fighters? That way everyone gets what they want.", new ConversationSentence.OnConditionDelegate(this.conversation_bandits_will_join_player_on_condition), null, 100, null);
			campaignGameSystemStarter.AddDialogLine("common_encounter_ultimatum_war", "common_encounter_ultimatum_answer", "close_window", "{=n99VA8KP}You'll never take us alive![if:idle_angry][ib:aggressive]", null, new ConversationSentence.OnConsequenceDelegate(this.conversation_bandit_set_hostile_on_consequence), 100, null);
			campaignGameSystemStarter.AddPlayerLine("common_bandit_join_player_accepted", "bandits_we_can_join_you", "close_window", "{=XdKCuzg1}Very well. You may join us. But I'll be keeping an eye on you lot.", null, delegate
			{
				Campaign.Current.ConversationManager.ConversationEndOneShot += delegate
				{
					this.conversation_bandits_join_player_party_on_consequence();
				};
			}, 100, null, null);
			campaignGameSystemStarter.AddPlayerLine("common_bandit_join_player_declined_1", "bandits_we_can_join_you", "player_do_not_let_bandits_to_join", "{=JZvywHNy}You think I'm daft? I'm not trusting you an inch.", null, null, 100, null, null);
			campaignGameSystemStarter.AddPlayerLine("common_bandit_join_player_declined_2", "bandits_we_can_join_you", "player_do_not_let_bandits_to_join", "{=z0WacPaW}No, justice demands you pay for your crimes.", null, new ConversationSentence.OnConsequenceDelegate(this.conversation_bandit_set_hostile_on_consequence), 100, null, null);
			campaignGameSystemStarter.AddPlayerLine("common_bandit_join_player_leave", "bandits_we_can_join_you", "bandit_attacker_leave", "{=D33fIGQe}Never mind.", null, null, 100, null, null);
			campaignGameSystemStarter.AddDialogLine("common_encounter_declined_looters_to_join_war_surrender", "player_do_not_let_bandits_to_join", "close_window", "{=ji2eenPE}All right - we give up. We can't fight you. Maybe the likes of us don't deserve mercy, but... show what mercy you can.", new ConversationSentence.OnConditionDelegate(this.conversation_bandits_surrender_on_condition), delegate
			{
				MobileParty party = MobileParty.ConversationParty;
				Campaign.Current.ConversationManager.ConversationEndOneShot += delegate
				{
					this.conversation_bandits_surrender_on_consequence(party);
				};
			}, 100, null);
			campaignGameSystemStarter.AddDialogLine("common_encounter_ultimatum_war", "player_do_not_let_bandits_to_join", "close_window", "{=LDhU5urT}So that's how it is, is it? Right then - I'll make one of you bleed before I go down.[if:idle_angry][ib:aggressive]", null, null, 100, null);
			campaignGameSystemStarter.AddDialogLine("bandit_attacker_try_leave_success", "bandit_attacker_leave", "close_window", "{=IDdyHef9}We'll be on our way, then!", new ConversationSentence.OnConditionDelegate(this.bandit_attacker_try_leave_condition), delegate
			{
				PlayerEncounter.LeaveEncounter = true;
			}, 100, null);
			campaignGameSystemStarter.AddDialogLine("bandit_attacker_try_leave_fail", "bandit_attacker_leave", "bandit_defender", "{=6Wc1XErN}Wait, wait... You're not going anywhere just yet.", () => !this.bandit_attacker_try_leave_condition(), null, 100, null);
			campaignGameSystemStarter.AddPlayerLine("common_encounter_cheat1", "bandit_attacker", "close_window", "{=4Wvdk30M}Cheat: Follow me", new ConversationSentence.OnConditionDelegate(this.bandit_cheat_conversations_condition), delegate
			{
				PlayerEncounter.EncounteredMobileParty.Ai.SetMoveEscortParty(MobileParty.MainParty);
				PlayerEncounter.EncounteredMobileParty.Ai.SetInitiative(0f, 0f, 48f);
				PlayerEncounter.LeaveEncounter = true;
			}, 100, null, null);
			campaignGameSystemStarter.AddPlayerLine("common_encounter_cheat2", "bandit_attacker", "close_window", "{=ORj5F5il}Cheat: Besiege Town", new ConversationSentence.OnConditionDelegate(this.bandit_cheat_conversations_condition), delegate
			{
				PlayerEncounter.EncounteredMobileParty.Ai.SetMoveBesiegeSettlement(Settlement.FindFirst((Settlement s) => s.IsTown && (double)s.Position2D.Distance(PlayerEncounter.EncounteredMobileParty.Position2D) < 80.0));
				PlayerEncounter.EncounteredMobileParty.Ai.SetInitiative(0f, 0f, 48f);
				PlayerEncounter.LeaveEncounter = true;
			}, 100, null, null);
			campaignGameSystemStarter.AddPlayerLine("common_encounter_cheat3", "bandit_attacker", "close_window", "{=RxuM5RzJ}Cheat: Raid Nearby Village", new ConversationSentence.OnConditionDelegate(this.bandit_cheat_conversations_condition), delegate
			{
				PlayerEncounter.EncounteredMobileParty.Ai.SetMoveRaidSettlement(Settlement.FindFirst((Settlement s) => s.IsVillage && (double)s.Position2D.Distance(PlayerEncounter.EncounteredMobileParty.Position2D) < 50.0));
				PlayerEncounter.LeaveEncounter = true;
			}, 100, null, null);
			campaignGameSystemStarter.AddPlayerLine("common_encounter_cheat4", "bandit_attacker", "close_window", "{=DIfTkzJJ}Cheat: Besiege Nearby Town", new ConversationSentence.OnConditionDelegate(this.bandit_cheat_conversations_condition), delegate
			{
				PlayerEncounter.EncounteredMobileParty.Ai.SetMoveBesiegeSettlement(Settlement.FindFirst((Settlement s) => s.IsTown && (double)s.Position2D.Distance(PlayerEncounter.EncounteredMobileParty.Position2D) < 50.0));
				PlayerEncounter.LeaveEncounter = true;
				PlayerEncounter.EncounteredMobileParty.Ai.SetInitiative(0f, 0f, 72f);
			}, 100, null, null);
			campaignGameSystemStarter.AddPlayerLine("common_encounter_cheat5", "bandit_attacker", "close_window", "{=eXaiRXF9}Cheat: Besiege Nearby Castle", new ConversationSentence.OnConditionDelegate(this.bandit_cheat_conversations_condition), delegate
			{
				PlayerEncounter.EncounteredMobileParty.Ai.SetMoveBesiegeSettlement(Settlement.FindFirst((Settlement s) => s.IsCastle && (double)s.Position2D.Distance(PlayerEncounter.EncounteredMobileParty.Position2D) < 50.0));
				PlayerEncounter.LeaveEncounter = true;
				PlayerEncounter.EncounteredMobileParty.Ai.SetInitiative(0f, 0f, 72f);
			}, 100, null, null);
			campaignGameSystemStarter.AddDialogLine("minor_faction_hostile", "start", "minor_faction_talk_hostile_response", "{=!}{MINOR_FACTION_ENCOUNTER}", new ConversationSentence.OnConditionDelegate(this.conversation_minor_faction_hostile_on_condition), null, 100, null);
			campaignGameSystemStarter.AddPlayerLine("minor_faction_talk_hostile_response_1", "minor_faction_talk_hostile_response", "close_window", "{=aaf5R99a}I'll give you nothing but cold steel, you scum!", null, null, 100, null, null);
			campaignGameSystemStarter.AddPlayerLine("minor_faction_talk_hostile_response_2", "minor_faction_talk_hostile_response", "minor_faction_talk_background", "{=EVLzPv1t}Hold - tell me more about yourselves.", null, null, 100, null, null);
			campaignGameSystemStarter.AddDialogLine("minor_faction_talk_background", "minor_faction_talk_background", "minor_faction_talk_background_next", "{=!}{MINOR_FACTION_SELFDESCRIPTION}", new ConversationSentence.OnConditionDelegate(this.conversation_minor_faction_set_selfdescription), null, 100, null);
			campaignGameSystemStarter.AddPlayerLine("minor_faction_talk_background_next_1", "minor_faction_talk_background_next", "minor_faction_talk_how_to_befriend", "{=vEsmC6M6}Is there any way we could not be enemies?", null, null, 100, null, null);
			campaignGameSystemStarter.AddPlayerLine("minor_faction_talk_background_next_2", "minor_faction_talk_background_next", "close_window", "{=p2WPU1CU}Very good then. Now I know whom I slay.", null, null, 100, null, null);
			campaignGameSystemStarter.AddDialogLine("minor_faction_talk_how_to_befriend", "minor_faction_talk_how_to_befriend", "minor_faction_talk_background_repeat_threat", "{=!}{MINOR_FACTION_HOWTOBEFRIEND}", new ConversationSentence.OnConditionDelegate(this.conversation_minor_faction_set_how_to_befriend), null, 100, null);
			campaignGameSystemStarter.AddDialogLine("minor_faction_talk_background_repeat_threat", "minor_faction_talk_background_repeat_threat", "minor_faction_talk_hostile_response", "{=ByOYHslS}That's enough talking for now. Make your choice.[if:idle_angry][ib:normal][ib:aggressive]", null, null, 100, null);
		}

		private bool bandit_barter_successful_condition()
		{
			return Campaign.Current.BarterManager.LastBarterIsAccepted;
		}

		private bool bandit_cheat_conversations_condition()
		{
			return Game.Current.IsDevelopmentMode;
		}

		private bool conversation_bandits_will_join_player_on_condition()
		{
			if (Hero.MainHero.GetPerkValue(DefaultPerks.Roguery.PartnersInCrime))
			{
				return true;
			}
			int num = (PartyBaseHelper.DoesSurrenderIsLogicalForParty(MobileParty.ConversationParty, MobileParty.MainParty, 0.06f) ? 33 : 67);
			if (Hero.MainHero.GetPerkValue(DefaultPerks.Roguery.Scarface))
			{
				num = MathF.Round((float)num * (1f + DefaultPerks.Roguery.Scarface.PrimaryBonus));
			}
			return MobileParty.ConversationParty.Party.RandomIntWithSeed(3U, 100) <= 100 - num && PartyBaseHelper.DoesSurrenderIsLogicalForParty(MobileParty.ConversationParty, MobileParty.MainParty, 0.09f);
		}

		private bool conversation_bandits_surrender_on_condition()
		{
			int num = (PartyBaseHelper.DoesSurrenderIsLogicalForParty(MobileParty.ConversationParty, MobileParty.MainParty, 0.04f) ? 33 : 67);
			if (Hero.MainHero.GetPerkValue(DefaultPerks.Roguery.Scarface))
			{
				num = MathF.Round((float)num * (1f + DefaultPerks.Roguery.Scarface.PrimaryBonus));
			}
			return MobileParty.ConversationParty.Party.RandomIntWithSeed(4U, 100) <= 100 - num && PartyBaseHelper.DoesSurrenderIsLogicalForParty(MobileParty.ConversationParty, MobileParty.MainParty, 0.06f);
		}

		private bool bandit_neutral_greet_on_condition()
		{
			if (Campaign.Current.CurrentConversationContext == ConversationContext.PartyEncounter && PlayerEncounter.Current != null && PlayerEncounter.EncounteredMobileParty != null && PlayerEncounter.EncounteredMobileParty.MapFaction.IsBanditFaction && PlayerEncounter.PlayerIsAttacker && MobileParty.ConversationParty != null)
			{
				MBTextManager.SetTextVariable("BANDIT_NEUTRAL_GREETING", "{=ZPj0ZAO7}Yeah? What do you want with us?", false);
				MBTextManager.SetTextVariable("BANDIT_ULTIMATUM", "{=5zUIQtTa}I want you to surrender or die, brigand!", false);
				int num = MBRandom.RandomInt(8);
				BanditsCampaignBehavior.PlayerInteraction playerInteraction = this.GetPlayerInteraction(MobileParty.ConversationParty);
				if (playerInteraction == BanditsCampaignBehavior.PlayerInteraction.PaidOffParty)
				{
					MBTextManager.SetTextVariable("BANDIT_NEUTRAL_GREETING", "{=Bm7U7TgG}If you're going to keep pestering us, traveller, we might need to take a bit more coin from you.", false);
					MBTextManager.SetTextVariable("BANDIT_ULTIMATUM", "{=KRfcro26}We're here to fight. Surrender or die!", false);
				}
				else if (playerInteraction != BanditsCampaignBehavior.PlayerInteraction.None)
				{
					if (PlayerEncounter.PlayerIsAttacker)
					{
						MBTextManager.SetTextVariable("BANDIT_NEUTRAL_GREETING", "{=38DvG2ba}Yeah? What is it now?", false);
					}
					else
					{
						MBTextManager.SetTextVariable("BANDIT_NEUTRAL_GREETING", "{=5laJ37D8}Back for more, are you?", false);
					}
					MBTextManager.SetTextVariable("BANDIT_ULTIMATUM", "{=KRfcro26}We're here to fight. Surrender or die!", false);
				}
				else if (num == 1)
				{
					MBTextManager.SetTextVariable("BANDIT_NEUTRAL_GREETING", "{=cO61R3va}We've got no quarrel with you.", false);
					MBTextManager.SetTextVariable("BANDIT_ULTIMATUM", "{=oJ6lpXmp}But I have one with you, brigand! Give up now.", false);
				}
				else if (num == 2)
				{
					MBTextManager.SetTextVariable("BANDIT_NEUTRAL_GREETING", "{=6XdHP9Pv}We're not looking for a fight.", false);
					MBTextManager.SetTextVariable("BANDIT_ULTIMATUM", "{=fiLWg11t}Neither am I, if you surrender. Otherwise...", false);
				}
				else if (num == 3)
				{
					MBTextManager.SetTextVariable("BANDIT_NEUTRAL_GREETING", "{=GUiT211X}You got a problem?", false);
					MBTextManager.SetTextVariable("BANDIT_ULTIMATUM", "{=idwOxnX5}Not if you give up now. If not, prepare to fight!", false);
				}
				else if (num == 4)
				{
					MBTextManager.SetTextVariable("BANDIT_NEUTRAL_GREETING", "{=mHBHKacJ}We're just harmless travellers...", false);
					MBTextManager.SetTextVariable("BANDIT_ULTIMATUM", "{=A5IJmN0X}I think not, brigand. Surrender or die!", false);
					if (PlayerEncounter.EncounteredMobileParty.MapFaction.StringId == "mountain_bandits")
					{
						MBTextManager.SetTextVariable("BANDIT_NEUTRAL_GREETING", "{=8rgH8CGc}We're just harmless shepherds...", false);
					}
					else if (PlayerEncounter.EncounteredMobileParty.MapFaction.StringId == "forest_bandits")
					{
						MBTextManager.SetTextVariable("BANDIT_NEUTRAL_GREETING", "{=kRASveAC}We're just harmless foresters...", false);
					}
					else if (PlayerEncounter.EncounteredMobileParty.MapFaction.StringId == "sea_raiders")
					{
						MBTextManager.SetTextVariable("BANDIT_NEUTRAL_GREETING", "{=k96R57KM}We're just harmless traders...", false);
					}
					else if (PlayerEncounter.EncounteredMobileParty.MapFaction.StringId == "steppe_bandits")
					{
						MBTextManager.SetTextVariable("BANDIT_NEUTRAL_GREETING", "{=odzS6rhH}We're just harmless herdsmen...", false);
					}
					else if (PlayerEncounter.EncounteredMobileParty.MapFaction.StringId == "desert_bandits")
					{
						MBTextManager.SetTextVariable("BANDIT_NEUTRAL_GREETING", "{=Vttb0P15}We're just harmless nomads...", false);
					}
				}
				else if (num == 5)
				{
					MBTextManager.SetTextVariable("BANDIT_NEUTRAL_GREETING", "{=wSwzyr6M}Mess with us and we'll sell our lives dearly.", false);
					MBTextManager.SetTextVariable("BANDIT_ULTIMATUM", "{=GLqb67cg}I don't care, brigand. Surrender or die!", false);
				}
				else if (num == 6)
				{
					MBTextManager.SetTextVariable("BANDIT_NEUTRAL_GREETING", "{=xQ0aBavD}Back off, stranger, unless you want trouble.", false);
					MBTextManager.SetTextVariable("BANDIT_ULTIMATUM", "{=BwIT8F0k}I don't mind, brigand. Surrender or die!", false);
				}
				else if (num == 7)
				{
					MBTextManager.SetTextVariable("BANDIT_NEUTRAL_GREETING", "{=8yPqbZmm}You best back off. There's dozens more of us hiding, just waiting for our signal.", false);
					MBTextManager.SetTextVariable("BANDIT_ULTIMATUM", "{=ASRpFaGF}Nice try, brigand. Surrender or die!", false);
					if (PlayerEncounter.EncounteredMobileParty.MapFaction.StringId == "mountain_bandits")
					{
						MBTextManager.SetTextVariable("BANDIT_NEUTRAL_GREETING", "{=TXzZwb7n}You best back off. Scores of our brothers are just over that ridge over there, waiting for our signal.", false);
					}
					else if (PlayerEncounter.EncounteredMobileParty.MapFaction.StringId == "forest_bandits")
					{
						MBTextManager.SetTextVariable("BANDIT_NEUTRAL_GREETING", "{=lZj61xTm}You don't know who you're messing with. There are scores of our brothers hiding in the woods, just waiting for our signal to pepper you with arrows.", false);
					}
					else if (PlayerEncounter.EncounteredMobileParty.MapFaction.StringId == "sea_raiders")
					{
						MBTextManager.SetTextVariable("BANDIT_NEUTRAL_GREETING", "{=7Sp6aNYo}You best let us be. There's dozens more of us hiding here, just waiting for our signal.", false);
					}
					else if (PlayerEncounter.EncounteredMobileParty.MapFaction.StringId == "steppe_bandits")
					{
						MBTextManager.SetTextVariable("BANDIT_NEUTRAL_GREETING", "{=EUbdov2r}Back off, stranger. There's dozens more of us hiding in that gully over there, just waiting for our signal.", false);
					}
					else if (PlayerEncounter.EncounteredMobileParty.MapFaction.StringId == "desert_bandits")
					{
						MBTextManager.SetTextVariable("BANDIT_NEUTRAL_GREETING", "{=RWxYalkR}Be warned, stranger. There's dozens more of us hiding in that wadi over there, just waiting for our signal.", false);
					}
				}
				return true;
			}
			return false;
		}

		private void bandit_barter_successful_on_consequence()
		{
			this.SetPlayerInteraction(MobileParty.ConversationParty, BanditsCampaignBehavior.PlayerInteraction.PaidOffParty);
		}

		private void bandit_neutral_greet_on_consequence()
		{
			if (this.GetPlayerInteraction(MobileParty.ConversationParty) != BanditsCampaignBehavior.PlayerInteraction.PaidOffParty)
			{
				this.SetPlayerInteraction(MobileParty.ConversationParty, BanditsCampaignBehavior.PlayerInteraction.Friendly);
			}
		}

		private void conversation_bandit_set_hostile_on_consequence()
		{
			this.SetPlayerInteraction(MobileParty.ConversationParty, BanditsCampaignBehavior.PlayerInteraction.Hostile);
		}

		private void conversation_bandits_surrender_on_consequence(MobileParty conversationParty)
		{
			Dictionary<PartyBase, ItemRoster> dictionary = new Dictionary<PartyBase, ItemRoster>();
			ItemRoster itemRoster = new ItemRoster(conversationParty.ItemRoster);
			dictionary.Add(PartyBase.MainParty, itemRoster);
			bool flag = false;
			int num = 0;
			while (num < dictionary.Values.Count && !flag)
			{
				int num2 = 0;
				while (num2 < dictionary.Values.ElementAt(num).Count && !flag)
				{
					if (dictionary.Values.ElementAt(num).GetElementNumber(num2) > 0)
					{
						flag = true;
					}
					num2++;
				}
				num++;
			}
			if (flag)
			{
				InventoryManager.OpenScreenAsLoot(dictionary);
				conversationParty.ItemRoster.Clear();
			}
			int partyTradeGold = conversationParty.PartyTradeGold;
			if (partyTradeGold > 0)
			{
				GiveGoldAction.ApplyForPartyToCharacter(conversationParty.Party, Hero.MainHero, partyTradeGold, false);
			}
			TroopRoster troopRoster = TroopRoster.CreateDummyTroopRoster();
			foreach (TroopRosterElement troopRosterElement in conversationParty.MemberRoster.GetTroopRoster())
			{
				troopRoster.AddToCounts(troopRosterElement.Character, troopRosterElement.Number, false, 0, 0, true, -1);
			}
			PartyScreenManager.OpenScreenAsLoot(TroopRoster.CreateDummyTroopRoster(), troopRoster, conversationParty.Name, troopRoster.TotalManCount, null);
			DestroyPartyAction.Apply(MobileParty.MainParty.Party, conversationParty);
			PlayerEncounter.LeaveEncounter = true;
		}

		private TroopRoster GetTroopsToJoinPlayerParty(List<MobileParty> parties)
		{
			TroopRoster troopRoster = TroopRoster.CreateDummyTroopRoster();
			foreach (MobileParty mobileParty in parties)
			{
				if (mobileParty.IsBandit && !mobileParty.IsLordParty)
				{
					for (int i = 0; i < mobileParty.MemberRoster.Count; i++)
					{
						if (!mobileParty.MemberRoster.GetCharacterAtIndex(i).IsHero)
						{
							troopRoster.AddToCounts(mobileParty.MemberRoster.GetCharacterAtIndex(i), mobileParty.MemberRoster.GetElementNumber(i), false, 0, 0, true, -1);
						}
					}
					for (int j = 0; j < mobileParty.PrisonRoster.Count; j++)
					{
						if (!mobileParty.PrisonRoster.GetCharacterAtIndex(j).IsHero)
						{
							troopRoster.AddToCounts(mobileParty.PrisonRoster.GetCharacterAtIndex(j), mobileParty.PrisonRoster.GetElementNumber(j), false, 0, 0, true, -1);
						}
					}
				}
			}
			return troopRoster;
		}

		private void conversation_bandits_join_player_party_on_consequence()
		{
			List<MobileParty> list = new List<MobileParty> { MobileParty.MainParty };
			List<MobileParty> list2 = new List<MobileParty>();
			if (PlayerEncounter.EncounteredMobileParty != null)
			{
				list2.Add(PlayerEncounter.EncounteredMobileParty);
			}
			if (PlayerEncounter.Current != null)
			{
				PlayerEncounter.Current.FindAllNpcPartiesWhoWillJoinEvent(ref list, ref list2);
			}
			TroopRoster troopsToJoinPlayerParty = this.GetTroopsToJoinPlayerParty(list2);
			PartyScreenManager.OpenScreenAsLoot(troopsToJoinPlayerParty, TroopRoster.CreateDummyTroopRoster(), PlayerEncounter.EncounteredParty.Name, troopsToJoinPlayerParty.TotalManCount, null);
			for (int i = list2.Count - 1; i >= 0; i--)
			{
				MobileParty mobileParty = list2[i];
				CampaignEventDispatcher.Instance.OnBanditPartyRecruited(mobileParty);
				DestroyPartyAction.Apply(MobileParty.MainParty.Party, mobileParty);
			}
			PlayerEncounter.LeaveEncounter = true;
		}

		private bool bandit_start_defender_condition()
		{
			PartyBase encounteredParty = PlayerEncounter.EncounteredParty;
			if ((Hero.OneToOneConversationHero != null && Hero.OneToOneConversationHero.MapFaction != null && !Hero.OneToOneConversationHero.MapFaction.IsBanditFaction) || encounteredParty == null || !encounteredParty.IsMobile || !encounteredParty.MapFaction.IsBanditFaction)
			{
				return false;
			}
			List<TextObject> list = new List<TextObject>();
			List<TextObject> list2 = new List<TextObject>();
			List<TextObject> list3 = new List<TextObject>();
			List<TextObject> list4 = new List<TextObject>();
			for (int i = 1; i <= 6; i++)
			{
				TextObject textObject;
				if (GameTexts.TryGetText("str_robbery_threat", out textObject, i.ToString()))
				{
					list.Add(textObject);
					list2.Add(GameTexts.FindText("str_robbery_pay_agreement", i.ToString()));
					list3.Add(GameTexts.FindText("str_robbery_conclusion", i.ToString()));
					list4.Add(GameTexts.FindText("str_robbery_start_fight", i.ToString()));
				}
			}
			for (int j = 1; j <= 6; j++)
			{
				string text = encounteredParty.MapFaction.StringId + "_" + j.ToString();
				TextObject textObject2;
				if (GameTexts.TryGetText("str_robbery_threat", out textObject2, text))
				{
					for (int k = 0; k < 3; k++)
					{
						list.Add(textObject2);
						list2.Add(GameTexts.FindText("str_robbery_pay_agreement", text));
						list3.Add(GameTexts.FindText("str_robbery_conclusion", text));
						list4.Add(GameTexts.FindText("str_robbery_start_fight", text));
					}
				}
			}
			int num = MBRandom.RandomInt(0, list.Count);
			MBTextManager.SetTextVariable("ROBBERY_THREAT", list[num], false);
			MBTextManager.SetTextVariable("ROBBERY_PAY_AGREEMENT", list2[num], false);
			MBTextManager.SetTextVariable("ROBBERY_CONCLUSION", list3[num], false);
			MBTextManager.SetTextVariable("ROBBERY_START_FIGHT", list4[num], false);
			List<MobileParty> list5 = new List<MobileParty> { MobileParty.MainParty };
			List<MobileParty> list6 = new List<MobileParty>();
			if (MobileParty.ConversationParty != null)
			{
				list6.Add(MobileParty.ConversationParty);
			}
			if (PlayerEncounter.Current != null)
			{
				PlayerEncounter.Current.FindAllNpcPartiesWhoWillJoinEvent(ref list5, ref list6);
			}
			float num2 = 0f;
			foreach (MobileParty mobileParty in list5)
			{
				num2 += mobileParty.Party.TotalStrength;
			}
			float num3 = 0f;
			foreach (MobileParty mobileParty2 in list6)
			{
				num3 += mobileParty2.Party.TotalStrength;
			}
			float num4 = (num3 + 1f) / (num2 + 1f);
			int num5 = Hero.MainHero.Gold / 100;
			double num6 = 2.0 * (double)MathF.Max(0f, MathF.Min(6f, num4 - 1f));
			float num7 = 0f;
			Settlement settlement = SettlementHelper.FindNearestSettlement((Settlement x) => x.IsTown || x.IsVillage, null);
			SettlementComponent settlementComponent;
			if (settlement.IsTown)
			{
				settlementComponent = settlement.Town;
			}
			else
			{
				settlementComponent = settlement.Village;
			}
			foreach (ItemRosterElement itemRosterElement in MobileParty.MainParty.ItemRoster)
			{
				num7 += (float)(settlementComponent.GetItemPrice(itemRosterElement.EquipmentElement, MobileParty.MainParty, true) * itemRosterElement.Amount);
			}
			float num8 = num7 / 100f;
			float num9 = 1f + 2f * MathF.Max(0f, MathF.Min(6f, num4 - 1f));
			BanditsCampaignBehavior._goldAmount = (int)((double)num5 * num6 + (double)(num8 * num9) + 100.0);
			MBTextManager.SetTextVariable("AMOUNT", BanditsCampaignBehavior._goldAmount.ToString(), false);
			return encounteredParty.IsMobile && encounteredParty.MapFaction.IsBanditFaction && PlayerEncounter.PlayerIsDefender;
		}

		private bool bandit_start_barter_condition()
		{
			return PlayerEncounter.Current != null && PlayerEncounter.Current.PlayerSide == BattleSideEnum.Defender;
		}

		private void bandit_start_barter_consequence()
		{
			BarterManager instance = BarterManager.Instance;
			Hero mainHero = Hero.MainHero;
			Hero oneToOneConversationHero = Hero.OneToOneConversationHero;
			PartyBase mainParty = PartyBase.MainParty;
			MobileParty conversationParty = MobileParty.ConversationParty;
			PartyBase partyBase = ((conversationParty != null) ? conversationParty.Party : null);
			Hero hero = null;
			BarterManager.BarterContextInitializer barterContextInitializer = new BarterManager.BarterContextInitializer(BarterManager.Instance.InitializeSafePassageBarterContext);
			int num = 0;
			bool flag = false;
			Barterable[] array = new Barterable[1];
			int num2 = 0;
			Hero hero2 = null;
			Hero mainHero2 = Hero.MainHero;
			MobileParty conversationParty2 = MobileParty.ConversationParty;
			array[num2] = new SafePassageBarterable(hero2, mainHero2, (conversationParty2 != null) ? conversationParty2.Party : null, PartyBase.MainParty);
			instance.StartBarterOffer(mainHero, oneToOneConversationHero, mainParty, partyBase, hero, barterContextInitializer, num, flag, array);
		}

		private bool conversation_minor_faction_hostile_on_condition()
		{
			if (MapEvent.PlayerMapEvent != null)
			{
				foreach (PartyBase partyBase in MapEvent.PlayerMapEvent.InvolvedParties)
				{
					if (PartyBase.MainParty.Side == BattleSideEnum.Attacker && partyBase.IsMobile && partyBase.MobileParty.IsBandit && partyBase.MapFaction.IsMinorFaction)
					{
						string text = partyBase.MapFaction.StringId + "_encounter";
						if (FactionManager.IsAtWarAgainstFaction(partyBase.MapFaction, Hero.MainHero.MapFaction))
						{
							text += "_hostile";
						}
						else
						{
							text += "_neutral";
						}
						MBTextManager.SetTextVariable("MINOR_FACTION_ENCOUNTER", GameTexts.FindText(text, null), false);
						return true;
					}
				}
				return false;
			}
			return false;
		}

		private bool conversation_minor_faction_set_selfdescription()
		{
			foreach (PartyBase partyBase in MapEvent.PlayerMapEvent.InvolvedParties)
			{
				if (PartyBase.MainParty.Side == BattleSideEnum.Attacker && partyBase.IsMobile && partyBase.MobileParty.IsBandit && partyBase.MapFaction.IsMinorFaction)
				{
					string text = partyBase.MapFaction.StringId + "_selfdescription";
					MBTextManager.SetTextVariable("MINOR_FACTION_SELFDESCRIPTION", GameTexts.FindText(text, null), false);
					return true;
				}
			}
			return true;
		}

		private bool conversation_minor_faction_set_how_to_befriend()
		{
			foreach (PartyBase partyBase in MapEvent.PlayerMapEvent.InvolvedParties)
			{
				if (PartyBase.MainParty.Side == BattleSideEnum.Attacker && partyBase.IsMobile && partyBase.MobileParty.IsBandit && partyBase.MapFaction.IsMinorFaction)
				{
					string text = partyBase.MapFaction.StringId + "_how_to_befriend";
					MBTextManager.SetTextVariable("MINOR_FACTION_HOWTOBEFRIEND", GameTexts.FindText(text, null), false);
					return true;
				}
			}
			return true;
		}

		private bool bandit_attacker_try_leave_condition()
		{
			return PlayerEncounter.EncounteredParty != null && (PlayerEncounter.EncounteredParty.TotalStrength <= PartyBase.MainParty.TotalStrength || this.GetPlayerInteraction(PlayerEncounter.EncounteredMobileParty) == BanditsCampaignBehavior.PlayerInteraction.PaidOffParty || this.GetPlayerInteraction(PlayerEncounter.EncounteredMobileParty) == BanditsCampaignBehavior.PlayerInteraction.Friendly);
		}

		private const float BanditSpawnRadius = 45f;

		private const float BanditStartGoldPerBandit = 10f;

		private const float BanditLongTermGoldPerBandit = 50f;

		private bool _hideoutsAndBanditsAreInitialized;

		private Dictionary<MobileParty, BanditsCampaignBehavior.PlayerInteraction> _interactedBandits = new Dictionary<MobileParty, BanditsCampaignBehavior.PlayerInteraction>();

		private static int _goldAmount;

		public class BanditsCampaignBehaviorTypeDefiner : CampaignBehaviorBase.SaveableCampaignBehaviorTypeDefiner
		{
			public BanditsCampaignBehaviorTypeDefiner()
				: base(70000)
			{
			}

			protected override void DefineEnumTypes()
			{
				base.AddEnumDefinition(typeof(BanditsCampaignBehavior.PlayerInteraction), 1, null);
			}

			protected override void DefineContainerDefinitions()
			{
				base.ConstructContainerDefinition(typeof(Dictionary<MobileParty, BanditsCampaignBehavior.PlayerInteraction>));
			}
		}

		private enum PlayerInteraction
		{
			None,
			Friendly,
			PaidOffParty,
			Hostile
		}
	}
}
