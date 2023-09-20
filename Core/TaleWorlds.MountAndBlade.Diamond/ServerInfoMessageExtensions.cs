using System;
using Messages.FromLobbyServer.ToClient;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.Diamond
{
	public static class ServerInfoMessageExtensions
	{
		public static TextObject GetDescription(this SystemMessage message)
		{
			for (int i = 0; i < message.Parameters.Count; i++)
			{
				GameTexts.SetVariable("ARG" + (i + 1), message.Parameters[i]);
			}
			switch (message.Message)
			{
			case ServerInfoMessage.Success:
				return new TextObject("{=NaTaGiB1}Success.", null);
			case ServerInfoMessage.LoginMuted:
				return new TextObject("{=h84jSMrT}You are muted until {ARG1}. Reason: {ARG2}", null);
			case ServerInfoMessage.DestroySessionPremadeGameCancellation:
				return new TextObject("{=T5gR6XCJ}Premade game no longer exists.", null);
			case ServerInfoMessage.DestroySessionPartyInvitationCancellation:
				return new TextObject("{=a1RXhy6A}Your party invitation is no longer valid.", null);
			case ServerInfoMessage.DestroySessionPartyAutoDisband:
				return new TextObject("{=BwBR7TJB}No one left in your party and it has been disbanded!", null);
			case ServerInfoMessage.PlayerNotFound:
				return new TextObject("{=ysfEQO6c}Player not found", null);
			case ServerInfoMessage.PlayerNotInLobby:
				if (message.Parameters.Count <= 0 || !(message.Parameters[0] == string.Empty))
				{
					return new TextObject("{=SWZaqPx4}{ARG1} is not in lobby.", null);
				}
				return new TextObject("{=uDbGN6fp}Player is not in lobby.", null);
			case ServerInfoMessage.MustBeInLobby:
				return new TextObject("{=DJFaYeWm}You must be in lobby to perform this action.", null);
			case ServerInfoMessage.NoTextGiven:
				return new TextObject("{=hav7yZyB}No text entered.", null);
			case ServerInfoMessage.TextTooLong:
				return new TextObject("{=aId8kA8r}Given text is longer than the limit.", null);
			case ServerInfoMessage.FindGameBlockedFromMatchmaking:
				return new TextObject("{=P0aRStg4}Cannot queue to battle because you are blocked from matchmaking.", null);
			case ServerInfoMessage.FindGamePartyMemberBlockedFromMatchmaking:
				return new TextObject("{=AIgw3aFG}Cannot queue to battle because some of the players are blocked from matchmaking.", null);
			case ServerInfoMessage.FindGameNoGameTypeSelected:
				return new TextObject("{=caMylVF7}Cannot queue to battle because no enabled game types are selected.", null);
			case ServerInfoMessage.FindGameDisabledGameTypesSelected:
				return new TextObject("{=4YuWC1Jg}Disabled game types selected.", null);
			case ServerInfoMessage.FindGamePlayerCountNotAllowed:
				return new TextObject("{=vi3lFSBW}Parties of {ARG1} are not allowed in party queue.", null);
			case ServerInfoMessage.FindGameNotPartyLeader:
				return new TextObject("{=wNubP2BF}Only party leader can queue for games.", null);
			case ServerInfoMessage.FindGameNotAllPlayersReady:
				return new TextObject("{=OByu0fjQ}Cannot queue to battle because not all players are ready in your party.", null);
			case ServerInfoMessage.FindGameRegionNotAvailable:
				return new TextObject("{=E2ILp86t}Cannot queue to battle because region is not available.", null);
			case ServerInfoMessage.FindGamePunished:
				return new TextObject("{=ND2abg9k}You are blocked from matchmaking for {ARG1} seconds.", null);
			case ServerInfoMessage.RejoinGame:
				return new TextObject("{=lcq2bKRk}Rejoining battle.", null);
			case ServerInfoMessage.RejoinGameNotFound:
				return new TextObject("{=MowSn4ch}Game not found.", null);
			case ServerInfoMessage.RejoinGameNotAllowed:
				return new TextObject("{=Ebp5T7vh}Can't rejoin game you've abandoned.", null);
			case ServerInfoMessage.AddFriendCantAddSelf:
				return new TextObject("{=SIT1tREw}You cannot add yourself as friend", null);
			case ServerInfoMessage.AddFriendRequestSent:
				if (message.Parameters.Count <= 0 || !(message.Parameters[0] == string.Empty))
				{
					return new TextObject("{=CyeaniWe}Friend request is sent to {ARG1}.", null);
				}
				return new TextObject("{=yvT1gK1g}Friend request is sent.", null);
			case ServerInfoMessage.AddFriendRequestReceived:
				return new TextObject("{=shIV99LZ}Friend request received from {ARG1}.", null);
			case ServerInfoMessage.AddFriendAlreadyFriends:
				return new TextObject("{=dabogDDj}You are already friends", null);
			case ServerInfoMessage.AddFriendRequestPending:
				if (message.Parameters.Count <= 0 || !(message.Parameters[0] == string.Empty))
				{
					return new TextObject("{=bzcI3ei2}You already have a pending request for {ARG1}.", null);
				}
				return new TextObject("{=rwcA5RMK}You already have a pending request for this player.", null);
			case ServerInfoMessage.AddFriendRequestAccepted:
				return new TextObject("{=TaLNjfao}You are now friends with {ARG1}.", null);
			case ServerInfoMessage.AddFriendRequestDeclined:
				if (message.Parameters.Count <= 0 || !(message.Parameters[0] == string.Empty))
				{
					return new TextObject("{=Ds01FoSn}{ARG1} declined your friend request.", null);
				}
				return new TextObject("{=ASgVFGs1}Your friend request is declined.", null);
			case ServerInfoMessage.AddFriendRequestBlocked:
				return new TextObject("{=GIiGZOxP}Cannot add friend. Privacy settings failed.", null);
			case ServerInfoMessage.RemoveFriendSuccess:
				return new TextObject("{=M0Cgjgb0}{ARG1} has been removed as a friend.", null);
			case ServerInfoMessage.FriendRequestAccepted:
				return new TextObject("{=3NwGpJwe}Friend request accepted.", null);
			case ServerInfoMessage.FriendRequestDeclined:
				return new TextObject("{=ruC5qF1H}Friend request declined.", null);
			case ServerInfoMessage.FriendRequestNotFound:
				return new TextObject("{=oFfMEy1S}Friend request cannot be found", null);
			case ServerInfoMessage.MustBeInParty:
				return new TextObject("{=2BtJYizu}You must be in a party to perform this action.", null);
			case ServerInfoMessage.MustBePartyLeader:
				return new TextObject("{=IRWReNWu}You must be the leader of your party to perform this action.", null);
			case ServerInfoMessage.InvitePartyHasModules:
				return new TextObject("{=vd8dEfvZ}You can't create a party while having mods!", null);
			case ServerInfoMessage.InvitePartyOtherPlayerHasModules:
				return new TextObject("{=l1YfVXF3}You can't invite a player that has mods!", null);
			case ServerInfoMessage.InvitePartyCantInviteSelf:
				return new TextObject("{=fD2SyeA7}You cannot invite yourself to party.", null);
			case ServerInfoMessage.InvitePartyOtherPlayerAlreadyInParty:
			case ServerInfoMessage.SuggestPartyOtherPlayerAlreadyInParty:
				return new TextObject("{=hcJumPEg}{ARG1} is already in party.", null);
			case ServerInfoMessage.InvitePartyPartyIsFull:
				return new TextObject("{=mvQKpHXH}Your party is full.", null);
			case ServerInfoMessage.InvitePartyOnlyLeaderCanInvite:
				return new TextObject("{=l6a9vI9z}Only party leader can invite other players.", null);
			case ServerInfoMessage.InvitePartySuccess:
				if (message.Parameters.Count <= 0 || !(message.Parameters[0] == string.Empty))
				{
					return new TextObject("{=vfsaTZKg}{ARG1} invited to your party.", null);
				}
				return new TextObject("{=x5vn4FPi}Player invited to your party.", null);
			case ServerInfoMessage.SuggestPartyMustBeInParty:
				return new TextObject("{=mdH4S8Kx}You must be in a party to suggest someone to your party.", null);
			case ServerInfoMessage.SuggestPartyMustBeMember:
				return new TextObject("{=4lNl8zS2}You cannot suggest someone to party as party leader.", null);
			case ServerInfoMessage.SuggestPartyCantSuggestSelf:
				return new TextObject("{=S7Y5Suho}You cannot suggest yourself to party.", null);
			case ServerInfoMessage.SuggestPartySuccess:
				if (message.Parameters.Count <= 0 || !(message.Parameters[0] == string.Empty))
				{
					return new TextObject("{=9kzik0vP}{ARG1} suggested to your party.", null);
				}
				return new TextObject("{=DOkblbrz}Player suggested to your party.", null);
			case ServerInfoMessage.DisbandPartySuccess:
				return new TextObject("{=Pn13ZKwI}Your party has been disbanded!", null);
			case ServerInfoMessage.KickPlayerOtherPlayerMustBeInParty:
				return new TextObject("{=VdyJtjSG}{ARG1} is not in your party!", null);
			case ServerInfoMessage.KickPartyPlayerMustBeLeader:
				return new TextObject("{=4XDu4Qcd}Only party leader can kick players from the party.", null);
			case ServerInfoMessage.PromotePartyLeaderOngoingClanCreation:
				return new TextObject("{=yrCBeRMf}You can not change the party leader when in the process of creating a clan.", null);
			case ServerInfoMessage.PromotePartyLeaderCantPromoteSelf:
				return new TextObject("{=bivF3RXY}You cannot promote yourself", null);
			case ServerInfoMessage.PromotePartyLeaderCantPromoteNonMember:
				return new TextObject("{=wJsR1aGI}{ARG1} is not a party member.", null);
			case ServerInfoMessage.PromotePartyLeaderMustBeLeader:
				return new TextObject("{=emx7txM9}Member promotion action can only be performed by the party leader.", null);
			case ServerInfoMessage.PromotePartyLeaderSuccess:
				return new TextObject("{=IWr2ZmWX}{ARG1} was promoted to party leader.", null);
			case ServerInfoMessage.PromotePartyLeaderAuto:
				return new TextObject("{=zPvuvrBe}{ARG1} was assigned as party leader.", null);
			case ServerInfoMessage.MustBeInClan:
				return new TextObject("{=bMLqfPRv}You are not in a clan.", null);
			case ServerInfoMessage.MustBeClanLeader:
				return new TextObject("{=PzUrwWnO}You are not the leader of your clan.", null);
			case ServerInfoMessage.MustBePrivilegedClanMember:
				return new TextObject("{=Y7wWhSsO}You are not a privileged member of your clan.", null);
			case ServerInfoMessage.ClanCreationNameIsInvalid:
				return new TextObject("{=nlYK3i5a}Clan name is invalid.", null);
			case ServerInfoMessage.ClanCreationTagIsInvalid:
				return new TextObject("{=APQ0kNVj}Clan tag is invalid.", null);
			case ServerInfoMessage.ClanCreationSigilIsInvalid:
				return new TextObject("{=bB5p8KpK}Clan sigil is invalid.", null);
			case ServerInfoMessage.ClanCreationCultureIsInvalid:
				return new TextObject("{=OfCk5NMA}Clan faction is invalid.", null);
			case ServerInfoMessage.ClanCreationNotAllPlayersReady:
				return new TextObject("{=SvHP9TXr}Not all players are ready.", null);
			case ServerInfoMessage.ClanCreationNotEnoughPlayers:
				return new TextObject("{=K0mSh8Hw}Your party does not have enough players to create a clan.", null);
			case ServerInfoMessage.ClanCreationAlreadyInAClan:
				return new TextObject("{=dHlxVbMw}You are already in a clan.", null);
			case ServerInfoMessage.ClanCreationHaveToBeInAParty:
				return new TextObject("{=b72mIrOl}You have to be in a party to create a clan.", null);
			case ServerInfoMessage.SetClanInformationSuccess:
				return new TextObject("{=iG72AMy8}Information text changed successfully.", null);
			case ServerInfoMessage.AddClanAnnouncementSuccess:
				return new TextObject("{=baHOuMTb}Announcement added successfully.", null);
			case ServerInfoMessage.EditClanAnnouncementNotFound:
				return new TextObject("{=e9LrrgAH}Announcement doesn't exist.", null);
			case ServerInfoMessage.EditClanAnnouncementSuccess:
				return new TextObject("{=y0dKQ8pL}Announcement changed successfully.", null);
			case ServerInfoMessage.DeleteClanAnnouncementNotFound:
				return new TextObject("{=e9LrrgAH}Announcement doesn't exist.", null);
			case ServerInfoMessage.DeleteClanAnnouncementSuccess:
				return new TextObject("{=rI8gE2Qh}Announcement deleted successfully.", null);
			case ServerInfoMessage.ChangeClanSigilInvalid:
				return new TextObject("{=qHGkxNWJ}Invalid clan sigil.", null);
			case ServerInfoMessage.ChangeClanSigilSuccess:
				return new TextObject("{=rfaIXasP}Clan sigil changed successfully.", null);
			case ServerInfoMessage.ChangeClanCultureSuccess:
				return new TextObject("{=KPMxDIgb}Clan culture changed successfully.", null);
			case ServerInfoMessage.InviteClanPlayerAlreadyInvited:
				if (message.Parameters.Count <= 0 || !(message.Parameters[0] == string.Empty))
				{
					return new TextObject("{=uJ7527FB}{ARG1} is already invited to another clan.", null);
				}
				return new TextObject("{=Np8V0jcX}Player is already invited to another clan.", null);
			case ServerInfoMessage.InviteClanPlayerAlreadyInClan:
				if (message.Parameters.Count <= 0 || !(message.Parameters[0] == string.Empty))
				{
					return new TextObject("{=DcbVwkit}{ARG1} is already in a clan.", null);
				}
				return new TextObject("{=ba4CJpja}Player is already in a clan.", null);
			case ServerInfoMessage.InviteClanPlayerIsNotOnline:
				if (message.Parameters.Count <= 0 || !(message.Parameters[0] == string.Empty))
				{
					return new TextObject("{=gQ8deZ17}{ARG1} is not online.", null);
				}
				return new TextObject("{=rkxTEFGp}Player is not online.", null);
			case ServerInfoMessage.InviteClanPlayerFeatureNotSupported:
				return new TextObject("{=8UmHXSnO}Player does not have the Clan feature available.", null);
			case ServerInfoMessage.InviteClanCantInviteSelf:
				return new TextObject("{=XgQQPZsn}You can't invite yourself.", null);
			case ServerInfoMessage.InviteClanSuccess:
				if (message.Parameters.Count <= 0 || !(message.Parameters[0] == string.Empty))
				{
					return new TextObject("{=ZB6cxznt}{ARG1} invited to clan.", null);
				}
				return new TextObject("{=Dmz3JLyp}Player invited to clan.", null);
			case ServerInfoMessage.AcceptClanInvitationSuccess:
				return new TextObject("{=T2Q35w3d}{ARG1} accepted clan invitation.", null);
			case ServerInfoMessage.DeclineClanInvitationSuccess:
				return new TextObject("{=7cEXe02I}{ARG1} declined clan invitation.", null);
			case ServerInfoMessage.PromoteClanRolePlayerNotInClan:
				if (message.Parameters.Count <= 0 || !(message.Parameters[0] == string.Empty))
				{
					return new TextObject("{=tam7tCdb}{ARG1} is not in your clan.", null);
				}
				return new TextObject("{=M8x38ZZZ}Player is not in your clan.", null);
			case ServerInfoMessage.PromoteClanLeaderCantPromoteSelf:
				return new TextObject("{=P3JTNI9P}You can not promote yourself to clan leader.", null);
			case ServerInfoMessage.PromoteClanLeaderSuccess:
				return new TextObject("{=jbaOH6qf}{ARG1} successfully promoted to clan leader.", null);
			case ServerInfoMessage.PromoteClanOfficerRoleLimitReached:
				return new TextObject("{=bvWo9N6k}Officer limit of the clan is reached.", null);
			case ServerInfoMessage.PromoteClanOfficerCantPromoteSelf:
				return new TextObject("{=eozBcVaB}You can not promote yourself as clan officer.", null);
			case ServerInfoMessage.PromoteClanOfficerSuccess:
				return new TextObject("{=FFPyGnZg}{ARG1} successfully promoted to clan officer.", null);
			case ServerInfoMessage.RemoveClanOfficerMustBeOfficerToMember:
				return new TextObject("{=3NxaSDgg}You are not an officer.", null);
			case ServerInfoMessage.RemoveClanOfficerMustBeOfficerToLeader:
				return new TextObject("{=yR7avbUg}{ARG1} is not an officer.", null);
			case ServerInfoMessage.RemoveClanOfficerSuccessFromLeader:
				return new TextObject("{=3lNHtNWa}Officer role of {ARG1} is removed.", null);
			case ServerInfoMessage.RemoveClanOfficerSuccessFromMember:
				return new TextObject("{=TuORvMMq}Left officer role.", null);
			case ServerInfoMessage.RemoveClanMemberToMember:
				return new TextObject("{=nZlu3Moe}Left clan.", null);
			case ServerInfoMessage.RemoveClanMemberToLeader:
				return new TextObject("{=tKlQbZaJ}{ARG1} removed from clan.", null);
			case ServerInfoMessage.RemoveClanMemberLeaderCantLeave:
				return new TextObject("{=FEeVGSO6}Clan leader can't leave the clan.", null);
			case ServerInfoMessage.PremadeGameCreationCanceled:
				return new TextObject("{=zKK3LDbF}Premade game creation canceled", null);
			case ServerInfoMessage.PremadeGameCreationMustBeCreating:
				return new TextObject("{=sa138Btz}You have to be creating a clan game to cancel one.", null);
			case ServerInfoMessage.PremadeGameCreationMapNotAvailable:
				return new TextObject("{=MgGvnobq}Selected map is not available.", null);
			case ServerInfoMessage.PremadeGameCreationPartyNotEligible:
				return new TextObject("{=yQyozBBX}Your party is not eligible to create a clan game.", null);
			case ServerInfoMessage.PremadeGameCreationInvalidGameType:
				return new TextObject("{=hqK9565f}Invalid game type.", null);
			case ServerInfoMessage.PremadeGameJoinIncorrectPassword:
				return new TextObject("{=Ajw0d4dW}Incorrect password.", null);
			case ServerInfoMessage.PremadeGameJoinGameNotFound:
				return new TextObject("{=MowSn4ch}Game not found.", null);
			case ServerInfoMessage.PremadeGameJoinPartyNotEligible:
				return new TextObject("{=uqqqWl6f}Party not eligible for clan game.", null);
			case ServerInfoMessage.GetPremadeGameListNotEligible:
				return new TextObject("{=0XY1VKVB}Your party is not eligible for clan games.", null);
			case ServerInfoMessage.ReportPlayerGameNotFound:
				return new TextObject("{=xHqjCamt}Could not report player. Game cannot be found.", null);
			case ServerInfoMessage.ReportPlayerPlayerNotFound:
				return new TextObject("{=6cMSAe2Z}Could not report player. Player cannot be found.", null);
			case ServerInfoMessage.ReportPlayerServerIsUnofficial:
				return new TextObject("{=t8K21Vmc}Could not report player. Server is unofficial.", null);
			case ServerInfoMessage.ReportPlayerSuccess:
				return new TextObject("{=kuM16dg2}{ARG1} has been reported.", null);
			case ServerInfoMessage.ChangeBannerlordIDFailure:
				return new TextObject("{=R7z6X8bT}Could not update Bannerlord ID.", null);
			case ServerInfoMessage.ChangeBannerlordIDSuccess:
				return new TextObject("{=bFSVAiel}Your Bannerlord ID is updated successfully.", null);
			case ServerInfoMessage.ChangeBannerlordIDEmpty:
				return new TextObject("{=RKioOKi4}Bannerlord ID cannot be empty.", null);
			case ServerInfoMessage.ChangeBannerlordIDTooShort:
				return new TextObject("{=1CO3GGEk}Given ID is too short.", null);
			case ServerInfoMessage.ChangeBannerlordIDTooLong:
				return new TextObject("{=RuKlb0Li}Given ID is too long.", null);
			case ServerInfoMessage.ChangeBannerlordIDInvalidCharacters:
				return new TextObject("{=fQCaY3da}Given ID contains invalid characters.", null);
			case ServerInfoMessage.ChangeBannerlordIDProfanity:
				return new TextObject("{=BpwNljJF}Bannerlord Id cannot contain profanity.", null);
			case ServerInfoMessage.GameInvitationCantInviteSelf:
				return new TextObject("{=0MDEP0jy}You cannot invite yourself", null);
			case ServerInfoMessage.GameInvitationPlayerAlreadyInGame:
				return new TextObject("{=UcUqJcr9}You cannot invite an online player to the game", null);
			case ServerInfoMessage.GameInvitationSuccess:
				return new TextObject("{=J3dazru7}Invited {ARG1} to the game", null);
			case ServerInfoMessage.ChangeRegionFailed:
				return new TextObject("{=YGfHWmNS}Could not change region", null);
			case ServerInfoMessage.ChangeGameModeFailed:
				return new TextObject("{=AAnoAhox}Could not change selected game mode", null);
			case ServerInfoMessage.BattleServerKickFriendlyFire:
				return new TextObject("{=InUAmnX4}You are kicked due to friendly damage", null);
			case ServerInfoMessage.ChatServerDisconnectedFromRoom:
				return new TextObject("{=DMPzhEK0}Disconnected from chat room: /{ARG1}", null);
			case ServerInfoMessage.CustomizationServiceIsUnavailable:
				return new TextObject("{=o76rZw0U}Service is not available at the moment", null);
			case ServerInfoMessage.CustomizationNotEnoughLoot:
				return new TextObject("{=CyrOnYI2}You do not have enough loot.", null);
			case ServerInfoMessage.CustomizationItemIsUnavailable:
				return new TextObject("{=SsCPw38T}Selected cosmetic does not exist", null);
			case ServerInfoMessage.CustomizationItemIsFree:
				return new TextObject("{=cwF03dOR}You cannot buy a free cosmetic item", null);
			case ServerInfoMessage.CustomizationItemAlreadyOwned:
				return new TextObject("{=hhUKkNR6}You already own selected cosmetic item", null);
			case ServerInfoMessage.CustomizationItemIsNotOwned:
				return new TextObject("{=4HQNKMUZ}You do not own selected cosmetic item.", null);
			case ServerInfoMessage.CustomizationChangeSigilSuccess:
				return new TextObject("{=2KsbeJHa}You have successfully updated your sigil preference", null);
			case ServerInfoMessage.CustomizationTroopIsNotValid:
				return new TextObject("{=kVcDXjUx}Invalid troop", null);
			case ServerInfoMessage.CustomizationCantUseMoreThanOneForSingleSlot:
				return new TextObject("{=96e2tZea}You cannot use multiple cosmetic items changing same item", null);
			case ServerInfoMessage.CustomizationCantUpdateBadge:
				return new TextObject("{=9TxM5UQS}Could not update shown badge.", null);
			case ServerInfoMessage.CustomizationInvalidBadge:
				return new TextObject("{=bl2dNFmT}Invalid badge.", null);
			case ServerInfoMessage.CustomizationCantDowngradeBadge:
				return new TextObject("{=w0FSTU2h}You cannot pick a lower tier badge.", null);
			case ServerInfoMessage.CustomizationBadgeNotAvailable:
				return new TextObject("{=HTFNfODt}You cannot pick a badge you have not earned.", null);
			default:
				return TextObject.Empty;
			}
		}
	}
}
