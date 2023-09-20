using System;
using System.Collections.Generic;
using Helpers;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	public class HeroKnownInformationCampaignBehavior : CampaignBehaviorBase
	{
		public override void RegisterEvents()
		{
			CampaignEvents.DailyTickHeroEvent.AddNonSerializedListener(this, new Action<Hero>(this.OnDailyTickHero));
			CampaignEvents.ConversationEnded.AddNonSerializedListener(this, new Action<IEnumerable<CharacterObject>>(this.ConversationEnded));
			CampaignEvents.OnAgentJoinedConversationEvent.AddNonSerializedListener(this, new Action<IAgent>(this.OnAgentJoinedConversation));
			CampaignEvents.OnPlayerMetHeroEvent.AddNonSerializedListener(this, new Action<Hero>(this.OnPlayerMetHero));
			CampaignEvents.HeroesMarried.AddNonSerializedListener(this, new Action<Hero, Hero, bool>(this.OnHeroesMarried));
			CampaignEvents.HeroCreated.AddNonSerializedListener(this, new Action<Hero, bool>(this.OnHeroCreated));
			CampaignEvents.OnGameLoadFinishedEvent.AddNonSerializedListener(this, new Action(this.OnGameLoadFinishedEvent));
			CampaignEvents.OnCharacterCreationIsOverEvent.AddNonSerializedListener(this, new Action(this.OnCharacterCreationIsOver));
			CampaignEvents.OnPlayerLearnsAboutHeroEvent.AddNonSerializedListener(this, new Action<Hero>(this.OnPlayerLearnsAboutHero));
			CampaignEvents.NearbyPartyAddedToPlayerMapEvent.AddNonSerializedListener(this, new Action<MobileParty>(this.OnNearbyPartyAddedToPlayerMapEvent));
			CampaignEvents.GameMenuOpened.AddNonSerializedListener(this, new Action<MenuCallbackArgs>(this.OnGameMenuChanged));
			CampaignEvents.AfterMissionStarted.AddNonSerializedListener(this, new Action<IMission>(this.OnAfterMissionStarted));
			CampaignEvents.MapEventStarted.AddNonSerializedListener(this, new Action<MapEvent, PartyBase, PartyBase>(this.OnMapEventStarted));
			CampaignEvents.PartyAttachedAnotherParty.AddNonSerializedListener(this, new Action<MobileParty>(this.OnPartyAttachedAnotherParty));
			CampaignEvents.OnPlayerJoinedTournamentEvent.AddNonSerializedListener(this, new Action<Town, bool>(this.OnPlayerJoinedTournament));
		}

		private void OnPlayerJoinedTournament(Town town, bool isParticipant)
		{
			foreach (CharacterObject characterObject in Campaign.Current.TournamentManager.GetTournamentGame(town).GetParticipantCharacters(town.Settlement, false))
			{
				if (characterObject.IsHero && !characterObject.HeroObject.IsKnownToPlayer)
				{
					characterObject.HeroObject.IsKnownToPlayer = true;
				}
			}
		}

		private void OnNearbyPartyAddedToPlayerMapEvent(MobileParty mobileParty)
		{
			if (mobileParty.LeaderHero != null)
			{
				mobileParty.LeaderHero.IsKnownToPlayer = true;
			}
		}

		private void OnPartyAttachedAnotherParty(MobileParty party)
		{
			if (party == MobileParty.MainParty)
			{
				if (party.AttachedTo.LeaderHero != null)
				{
					party.AttachedTo.LeaderHero.IsKnownToPlayer = true;
				}
				using (List<MobileParty>.Enumerator enumerator = party.AttachedTo.AttachedParties.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						MobileParty mobileParty = enumerator.Current;
						if (mobileParty.LeaderHero != null)
						{
							mobileParty.LeaderHero.IsKnownToPlayer = true;
						}
					}
					return;
				}
			}
			if ((party.AttachedTo == MobileParty.MainParty || party.AttachedTo == MobileParty.MainParty.AttachedTo) && party.LeaderHero != null)
			{
				party.LeaderHero.IsKnownToPlayer = true;
			}
		}

		private void OnPartyAttachedToAnotherParty(MobileParty mobileParty)
		{
			if (mobileParty == MobileParty.MainParty)
			{
				if (mobileParty.AttachedTo.LeaderHero != null)
				{
					mobileParty.AttachedTo.LeaderHero.IsKnownToPlayer = true;
				}
				using (List<MobileParty>.Enumerator enumerator = mobileParty.AttachedTo.AttachedParties.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						MobileParty mobileParty2 = enumerator.Current;
						if (mobileParty2.LeaderHero != null)
						{
							mobileParty2.LeaderHero.IsKnownToPlayer = true;
						}
					}
					return;
				}
			}
			if ((mobileParty.AttachedTo == MobileParty.MainParty || mobileParty.AttachedTo == MobileParty.MainParty.AttachedTo) && mobileParty.LeaderHero != null)
			{
				mobileParty.LeaderHero.IsKnownToPlayer = true;
			}
		}

		private void OnMapEventStarted(MapEvent mapEvent, PartyBase attackerParty, PartyBase defenderParty)
		{
			if (MapEvent.PlayerMapEvent == mapEvent)
			{
				foreach (PartyBase partyBase in mapEvent.InvolvedParties)
				{
					if (partyBase.LeaderHero != null)
					{
						partyBase.LeaderHero.IsKnownToPlayer = true;
					}
				}
			}
		}

		private void OnPlayerLearnsAboutHero(Hero hero)
		{
			if (hero.Clan != Clan.PlayerClan)
			{
				TextObject textObject = new TextObject("{=lLMlPcl4}You have discovered {HERO.NAME}", null);
				textObject.SetCharacterProperties("HERO", hero.CharacterObject, false);
				InformationManager.DisplayMessage(new InformationMessage(textObject.ToString()));
			}
		}

		private void OnAfterMissionStarted(IMission mission)
		{
			if (CampaignMission.Current.Location != null)
			{
				this.LearnAboutLocationCharacters(CampaignMission.Current.Location);
			}
		}

		private void OnGameMenuChanged(MenuCallbackArgs args)
		{
			foreach (Location location in Campaign.Current.GameMenuManager.MenuLocations)
			{
				this.LearnAboutLocationCharacters(location);
			}
		}

		private void LearnAboutLocationCharacters(Location location)
		{
			foreach (LocationCharacter locationCharacter in location.GetCharacterList())
			{
				if (locationCharacter.Character.IsHero && !locationCharacter.IsHidden && locationCharacter.Character.HeroObject.CurrentSettlement == Settlement.CurrentSettlement)
				{
					locationCharacter.Character.HeroObject.IsKnownToPlayer = true;
				}
			}
		}

		private void OnPlayerMetHero(Hero hero)
		{
			this.UpdateHeroLocation(hero);
			hero.IsKnownToPlayer = true;
		}

		private void OnDailyTickHero(Hero hero)
		{
			this.UpdateHeroLocation(hero);
		}

		private void OnAgentJoinedConversation(IAgent agent)
		{
			CharacterObject characterObject = (CharacterObject)agent.Character;
			if (characterObject.IsHero)
			{
				this.UpdateHeroLocation(characterObject.HeroObject);
				characterObject.HeroObject.IsKnownToPlayer = true;
			}
			MobileParty conversationParty = MobileParty.ConversationParty;
			Hero hero;
			if (conversationParty == null)
			{
				hero = null;
			}
			else
			{
				CaravanPartyComponent caravanPartyComponent = conversationParty.CaravanPartyComponent;
				hero = ((caravanPartyComponent != null) ? caravanPartyComponent.Owner : null);
			}
			Hero hero2 = hero;
			if (hero2 != null)
			{
				hero2.IsKnownToPlayer = true;
			}
		}

		private void UpdateHeroLocation(Hero hero)
		{
			if (hero.IsActive || hero.IsPrisoner)
			{
				Settlement closestSettlement = HeroHelper.GetClosestSettlement(hero);
				if (closestSettlement != null)
				{
					hero.UpdateLastKnownClosestSettlement(closestSettlement);
				}
			}
		}

		private void OnCharacterCreationIsOver()
		{
			foreach (Hero hero in Hero.AllAliveHeroes)
			{
				this.UpdateHeroLocation(hero);
			}
		}

		private void OnGameLoadFinishedEvent()
		{
			if (MBSaveLoad.IsUpdatingGameVersion && MBSaveLoad.LastLoadedGameVersion < ApplicationVersion.FromString("e1.8.1.0", 26219))
			{
				foreach (Hero hero in Clan.PlayerClan.Heroes)
				{
					hero.SetHasMet();
				}
				foreach (Hero hero2 in Hero.AllAliveHeroes)
				{
					if (hero2.LastKnownClosestSettlement == null)
					{
						this.UpdateHeroLocation(hero2);
					}
					if (hero2.HasMet)
					{
						hero2.IsKnownToPlayer = true;
					}
				}
			}
		}

		private void OnHeroesMarried(Hero hero1, Hero hero2, bool showNotification)
		{
			if (hero1 == Hero.MainHero)
			{
				hero2.SetHasMet();
			}
			if (hero2 == Hero.MainHero)
			{
				hero1.SetHasMet();
			}
		}

		private void OnHeroCreated(Hero hero, bool isBornNaturally)
		{
			if (hero.Clan == Clan.PlayerClan)
			{
				hero.SetHasMet();
			}
		}

		private void ConversationEnded(IEnumerable<CharacterObject> conversationCharacters)
		{
			foreach (CharacterObject characterObject in conversationCharacters)
			{
				if (characterObject.IsHero)
				{
					characterObject.HeroObject.SetHasMet();
				}
			}
		}

		public override void SyncData(IDataStore dataStore)
		{
		}
	}
}
