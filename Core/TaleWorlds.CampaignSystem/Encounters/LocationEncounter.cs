using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.Encounters
{
	public class LocationEncounter
	{
		public Settlement Settlement { get; }

		public List<AccompanyingCharacter> CharactersAccompanyingPlayer { get; private set; }

		protected LocationEncounter(Settlement settlement)
		{
			this.Settlement = settlement;
			this.CharactersAccompanyingPlayer = new List<AccompanyingCharacter>();
		}

		public void AddAccompanyingCharacter(LocationCharacter locationCharacter, bool isFollowing = false)
		{
			if (!this.CharactersAccompanyingPlayer.Any((AccompanyingCharacter x) => x.LocationCharacter.Character == locationCharacter.Character))
			{
				AccompanyingCharacter accompanyingCharacter = new AccompanyingCharacter(locationCharacter, isFollowing);
				this.CharactersAccompanyingPlayer.Add(accompanyingCharacter);
			}
		}

		public AccompanyingCharacter GetAccompanyingCharacter(LocationCharacter locationCharacter)
		{
			return this.CharactersAccompanyingPlayer.Find((AccompanyingCharacter x) => x.LocationCharacter == locationCharacter);
		}

		public AccompanyingCharacter GetAccompanyingCharacter(CharacterObject character)
		{
			return this.CharactersAccompanyingPlayer.Find(delegate(AccompanyingCharacter x)
			{
				LocationCharacter locationCharacter = x.LocationCharacter;
				return ((locationCharacter != null) ? locationCharacter.Character : null) == character;
			});
		}

		public void RemoveAccompanyingCharacter(LocationCharacter locationCharacter)
		{
			if (this.CharactersAccompanyingPlayer.Any((AccompanyingCharacter x) => x.LocationCharacter == locationCharacter))
			{
				AccompanyingCharacter accompanyingCharacter = this.CharactersAccompanyingPlayer.Find((AccompanyingCharacter x) => x.LocationCharacter == locationCharacter);
				this.CharactersAccompanyingPlayer.Remove(accompanyingCharacter);
			}
		}

		public void RemoveAccompanyingCharacter(Hero hero)
		{
			for (int i = this.CharactersAccompanyingPlayer.Count - 1; i >= 0; i--)
			{
				if (this.CharactersAccompanyingPlayer[i].LocationCharacter.Character.IsHero && this.CharactersAccompanyingPlayer[i].LocationCharacter.Character.HeroObject == hero)
				{
					this.CharactersAccompanyingPlayer.Remove(this.CharactersAccompanyingPlayer[i]);
					return;
				}
			}
		}

		public void RemoveAllAccompanyingCharacters()
		{
			this.CharactersAccompanyingPlayer.Clear();
		}

		public void OnCharacterLocationChanged(LocationCharacter locationCharacter, Location fromLocation, Location toLocation)
		{
			if ((fromLocation == CampaignMission.Current.Location && toLocation == null) || (fromLocation == null && toLocation == CampaignMission.Current.Location))
			{
				CampaignMission.Current.OnCharacterLocationChanged(locationCharacter, fromLocation, toLocation);
			}
		}

		public virtual bool IsWorkshopLocation(Location location)
		{
			return false;
		}

		public virtual bool IsTavern(Location location)
		{
			return false;
		}

		public virtual IMission CreateAndOpenMissionController(Location nextLocation, Location previousLocation = null, CharacterObject talkToChar = null, string playerSpecialSpawnTag = null)
		{
			return null;
		}

		public bool IsInsideOfASettlement;
	}
}
