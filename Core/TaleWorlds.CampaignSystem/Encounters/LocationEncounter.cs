using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.Encounters
{
	// Token: 0x02000299 RID: 665
	public class LocationEncounter
	{
		// Token: 0x170008DB RID: 2267
		// (get) Token: 0x06002409 RID: 9225 RVA: 0x00099101 File Offset: 0x00097301
		public Settlement Settlement { get; }

		// Token: 0x170008DC RID: 2268
		// (get) Token: 0x0600240A RID: 9226 RVA: 0x00099109 File Offset: 0x00097309
		// (set) Token: 0x0600240B RID: 9227 RVA: 0x00099111 File Offset: 0x00097311
		public List<AccompanyingCharacter> CharactersAccompanyingPlayer { get; private set; }

		// Token: 0x0600240C RID: 9228 RVA: 0x0009911A File Offset: 0x0009731A
		protected LocationEncounter(Settlement settlement)
		{
			this.Settlement = settlement;
			this.CharactersAccompanyingPlayer = new List<AccompanyingCharacter>();
		}

		// Token: 0x0600240D RID: 9229 RVA: 0x00099134 File Offset: 0x00097334
		public void AddAccompanyingCharacter(LocationCharacter locationCharacter, bool isFollowing = false)
		{
			if (!this.CharactersAccompanyingPlayer.Any((AccompanyingCharacter x) => x.LocationCharacter.Character == locationCharacter.Character))
			{
				AccompanyingCharacter accompanyingCharacter = new AccompanyingCharacter(locationCharacter, isFollowing);
				this.CharactersAccompanyingPlayer.Add(accompanyingCharacter);
			}
		}

		// Token: 0x0600240E RID: 9230 RVA: 0x00099180 File Offset: 0x00097380
		public AccompanyingCharacter GetAccompanyingCharacter(LocationCharacter locationCharacter)
		{
			return this.CharactersAccompanyingPlayer.Find((AccompanyingCharacter x) => x.LocationCharacter == locationCharacter);
		}

		// Token: 0x0600240F RID: 9231 RVA: 0x000991B4 File Offset: 0x000973B4
		public AccompanyingCharacter GetAccompanyingCharacter(CharacterObject character)
		{
			return this.CharactersAccompanyingPlayer.Find(delegate(AccompanyingCharacter x)
			{
				LocationCharacter locationCharacter = x.LocationCharacter;
				return ((locationCharacter != null) ? locationCharacter.Character : null) == character;
			});
		}

		// Token: 0x06002410 RID: 9232 RVA: 0x000991E8 File Offset: 0x000973E8
		public void RemoveAccompanyingCharacter(LocationCharacter locationCharacter)
		{
			if (this.CharactersAccompanyingPlayer.Any((AccompanyingCharacter x) => x.LocationCharacter == locationCharacter))
			{
				AccompanyingCharacter accompanyingCharacter = this.CharactersAccompanyingPlayer.Find((AccompanyingCharacter x) => x.LocationCharacter == locationCharacter);
				this.CharactersAccompanyingPlayer.Remove(accompanyingCharacter);
			}
		}

		// Token: 0x06002411 RID: 9233 RVA: 0x00099240 File Offset: 0x00097440
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

		// Token: 0x06002412 RID: 9234 RVA: 0x000992B9 File Offset: 0x000974B9
		public void RemoveAllAccompanyingCharacters()
		{
			this.CharactersAccompanyingPlayer.Clear();
		}

		// Token: 0x06002413 RID: 9235 RVA: 0x000992C6 File Offset: 0x000974C6
		public void OnCharacterLocationChanged(LocationCharacter locationCharacter, Location fromLocation, Location toLocation)
		{
			if ((fromLocation == CampaignMission.Current.Location && toLocation == null) || (fromLocation == null && toLocation == CampaignMission.Current.Location))
			{
				CampaignMission.Current.OnCharacterLocationChanged(locationCharacter, fromLocation, toLocation);
			}
		}

		// Token: 0x06002414 RID: 9236 RVA: 0x000992F5 File Offset: 0x000974F5
		public virtual bool IsWorkshopLocation(Location location)
		{
			return false;
		}

		// Token: 0x06002415 RID: 9237 RVA: 0x000992F8 File Offset: 0x000974F8
		public virtual bool IsTavern(Location location)
		{
			return false;
		}

		// Token: 0x06002416 RID: 9238 RVA: 0x000992FB File Offset: 0x000974FB
		public virtual IMission CreateAndOpenMissionController(Location nextLocation, Location previousLocation = null, CharacterObject talkToChar = null, string playerSpecialSpawnTag = null)
		{
			return null;
		}

		// Token: 0x04000B02 RID: 2818
		public bool IsInsideOfASettlement;
	}
}
