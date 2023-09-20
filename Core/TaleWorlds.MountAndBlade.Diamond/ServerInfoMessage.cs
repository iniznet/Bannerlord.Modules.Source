using System;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x0200014C RID: 332
	public enum ServerInfoMessage
	{
		// Token: 0x040003CE RID: 974
		Success,
		// Token: 0x040003CF RID: 975
		LoginMuted,
		// Token: 0x040003D0 RID: 976
		DestroySessionPremadeGameCancellation,
		// Token: 0x040003D1 RID: 977
		DestroySessionPartyInvitationCancellation,
		// Token: 0x040003D2 RID: 978
		DestroySessionPartyAutoDisband,
		// Token: 0x040003D3 RID: 979
		PlayerNotFound,
		// Token: 0x040003D4 RID: 980
		PlayerNotInLobby,
		// Token: 0x040003D5 RID: 981
		MustBeInLobby,
		// Token: 0x040003D6 RID: 982
		NoTextGiven,
		// Token: 0x040003D7 RID: 983
		TextTooLong,
		// Token: 0x040003D8 RID: 984
		FindGameBlockedFromMatchmaking,
		// Token: 0x040003D9 RID: 985
		FindGamePartyMemberBlockedFromMatchmaking,
		// Token: 0x040003DA RID: 986
		FindGameNoGameTypeSelected,
		// Token: 0x040003DB RID: 987
		FindGameDisabledGameTypesSelected,
		// Token: 0x040003DC RID: 988
		FindGamePlayerCountNotAllowed,
		// Token: 0x040003DD RID: 989
		FindGameNotPartyLeader,
		// Token: 0x040003DE RID: 990
		FindGameNotAllPlayersReady,
		// Token: 0x040003DF RID: 991
		FindGameRegionNotAvailable,
		// Token: 0x040003E0 RID: 992
		FindGamePunished,
		// Token: 0x040003E1 RID: 993
		RejoinGame,
		// Token: 0x040003E2 RID: 994
		RejoinGameNotFound,
		// Token: 0x040003E3 RID: 995
		RejoinGameNotAllowed,
		// Token: 0x040003E4 RID: 996
		AddFriendCantAddSelf,
		// Token: 0x040003E5 RID: 997
		AddFriendRequestSent,
		// Token: 0x040003E6 RID: 998
		AddFriendRequestReceived,
		// Token: 0x040003E7 RID: 999
		AddFriendAlreadyFriends,
		// Token: 0x040003E8 RID: 1000
		AddFriendRequestPending,
		// Token: 0x040003E9 RID: 1001
		AddFriendRequestAccepted,
		// Token: 0x040003EA RID: 1002
		AddFriendRequestDeclined,
		// Token: 0x040003EB RID: 1003
		AddFriendRequestBlocked,
		// Token: 0x040003EC RID: 1004
		RemoveFriendSuccess,
		// Token: 0x040003ED RID: 1005
		FriendRequestAccepted,
		// Token: 0x040003EE RID: 1006
		FriendRequestDeclined,
		// Token: 0x040003EF RID: 1007
		FriendRequestNotFound,
		// Token: 0x040003F0 RID: 1008
		MustBeInParty,
		// Token: 0x040003F1 RID: 1009
		MustBePartyLeader,
		// Token: 0x040003F2 RID: 1010
		InvitePartyHasModules,
		// Token: 0x040003F3 RID: 1011
		InvitePartyOtherPlayerHasModules,
		// Token: 0x040003F4 RID: 1012
		InvitePartyCantInviteSelf,
		// Token: 0x040003F5 RID: 1013
		InvitePartyOtherPlayerAlreadyInParty,
		// Token: 0x040003F6 RID: 1014
		InvitePartyPartyIsFull,
		// Token: 0x040003F7 RID: 1015
		InvitePartyOnlyLeaderCanInvite,
		// Token: 0x040003F8 RID: 1016
		InvitePartySuccess,
		// Token: 0x040003F9 RID: 1017
		SuggestPartyMustBeInParty,
		// Token: 0x040003FA RID: 1018
		SuggestPartyMustBeMember,
		// Token: 0x040003FB RID: 1019
		SuggestPartyCantSuggestSelf,
		// Token: 0x040003FC RID: 1020
		SuggestPartyOtherPlayerAlreadyInParty,
		// Token: 0x040003FD RID: 1021
		SuggestPartySuccess,
		// Token: 0x040003FE RID: 1022
		DisbandPartySuccess,
		// Token: 0x040003FF RID: 1023
		KickPlayerOtherPlayerMustBeInParty,
		// Token: 0x04000400 RID: 1024
		KickPartyPlayerMustBeLeader,
		// Token: 0x04000401 RID: 1025
		PromotePartyLeaderOngoingClanCreation,
		// Token: 0x04000402 RID: 1026
		PromotePartyLeaderCantPromoteSelf,
		// Token: 0x04000403 RID: 1027
		PromotePartyLeaderCantPromoteNonMember,
		// Token: 0x04000404 RID: 1028
		PromotePartyLeaderMustBeLeader,
		// Token: 0x04000405 RID: 1029
		PromotePartyLeaderSuccess,
		// Token: 0x04000406 RID: 1030
		PromotePartyLeaderAuto,
		// Token: 0x04000407 RID: 1031
		MustBeInClan,
		// Token: 0x04000408 RID: 1032
		MustBeClanLeader,
		// Token: 0x04000409 RID: 1033
		MustBePrivilegedClanMember,
		// Token: 0x0400040A RID: 1034
		ClanCreationNameIsInvalid,
		// Token: 0x0400040B RID: 1035
		ClanCreationTagIsInvalid,
		// Token: 0x0400040C RID: 1036
		ClanCreationSigilIsInvalid,
		// Token: 0x0400040D RID: 1037
		ClanCreationCultureIsInvalid,
		// Token: 0x0400040E RID: 1038
		ClanCreationNotAllPlayersReady,
		// Token: 0x0400040F RID: 1039
		ClanCreationNotEnoughPlayers,
		// Token: 0x04000410 RID: 1040
		ClanCreationAlreadyInAClan,
		// Token: 0x04000411 RID: 1041
		ClanCreationHaveToBeInAParty,
		// Token: 0x04000412 RID: 1042
		SetClanInformationSuccess,
		// Token: 0x04000413 RID: 1043
		AddClanAnnouncementSuccess,
		// Token: 0x04000414 RID: 1044
		EditClanAnnouncementNotFound,
		// Token: 0x04000415 RID: 1045
		EditClanAnnouncementSuccess,
		// Token: 0x04000416 RID: 1046
		DeleteClanAnnouncementNotFound,
		// Token: 0x04000417 RID: 1047
		DeleteClanAnnouncementSuccess,
		// Token: 0x04000418 RID: 1048
		ChangeClanSigilInvalid,
		// Token: 0x04000419 RID: 1049
		ChangeClanSigilSuccess,
		// Token: 0x0400041A RID: 1050
		ChangeClanCultureSuccess,
		// Token: 0x0400041B RID: 1051
		InviteClanPlayerAlreadyInvited,
		// Token: 0x0400041C RID: 1052
		InviteClanPlayerAlreadyInClan,
		// Token: 0x0400041D RID: 1053
		InviteClanPlayerIsNotOnline,
		// Token: 0x0400041E RID: 1054
		InviteClanPlayerFeatureNotSupported,
		// Token: 0x0400041F RID: 1055
		InviteClanCantInviteSelf,
		// Token: 0x04000420 RID: 1056
		InviteClanSuccess,
		// Token: 0x04000421 RID: 1057
		AcceptClanInvitationSuccess,
		// Token: 0x04000422 RID: 1058
		DeclineClanInvitationSuccess,
		// Token: 0x04000423 RID: 1059
		PromoteClanRolePlayerNotInClan,
		// Token: 0x04000424 RID: 1060
		PromoteClanLeaderCantPromoteSelf,
		// Token: 0x04000425 RID: 1061
		PromoteClanLeaderSuccess,
		// Token: 0x04000426 RID: 1062
		PromoteClanOfficerRoleLimitReached,
		// Token: 0x04000427 RID: 1063
		PromoteClanOfficerCantPromoteSelf,
		// Token: 0x04000428 RID: 1064
		PromoteClanOfficerSuccess,
		// Token: 0x04000429 RID: 1065
		RemoveClanOfficerMustBeOfficerToMember,
		// Token: 0x0400042A RID: 1066
		RemoveClanOfficerMustBeOfficerToLeader,
		// Token: 0x0400042B RID: 1067
		RemoveClanOfficerSuccessFromLeader,
		// Token: 0x0400042C RID: 1068
		RemoveClanOfficerSuccessFromMember,
		// Token: 0x0400042D RID: 1069
		RemoveClanMemberToMember,
		// Token: 0x0400042E RID: 1070
		RemoveClanMemberToLeader,
		// Token: 0x0400042F RID: 1071
		RemoveClanMemberLeaderCantLeave,
		// Token: 0x04000430 RID: 1072
		PremadeGameCreationCanceled,
		// Token: 0x04000431 RID: 1073
		PremadeGameCreationMustBeCreating,
		// Token: 0x04000432 RID: 1074
		PremadeGameCreationMapNotAvailable,
		// Token: 0x04000433 RID: 1075
		PremadeGameCreationPartyNotEligible,
		// Token: 0x04000434 RID: 1076
		PremadeGameCreationInvalidGameType,
		// Token: 0x04000435 RID: 1077
		PremadeGameJoinIncorrectPassword,
		// Token: 0x04000436 RID: 1078
		PremadeGameJoinGameNotFound,
		// Token: 0x04000437 RID: 1079
		PremadeGameJoinPartyNotEligible,
		// Token: 0x04000438 RID: 1080
		GetPremadeGameListNotEligible,
		// Token: 0x04000439 RID: 1081
		ReportPlayerGameNotFound,
		// Token: 0x0400043A RID: 1082
		ReportPlayerPlayerNotFound,
		// Token: 0x0400043B RID: 1083
		ReportPlayerServerIsUnofficial,
		// Token: 0x0400043C RID: 1084
		ReportPlayerSuccess,
		// Token: 0x0400043D RID: 1085
		ChangeBannerlordIDFailure,
		// Token: 0x0400043E RID: 1086
		ChangeBannerlordIDSuccess,
		// Token: 0x0400043F RID: 1087
		ChangeBannerlordIDEmpty,
		// Token: 0x04000440 RID: 1088
		ChangeBannerlordIDTooShort,
		// Token: 0x04000441 RID: 1089
		ChangeBannerlordIDTooLong,
		// Token: 0x04000442 RID: 1090
		ChangeBannerlordIDInvalidCharacters,
		// Token: 0x04000443 RID: 1091
		ChangeBannerlordIDProfanity,
		// Token: 0x04000444 RID: 1092
		GameInvitationCantInviteSelf,
		// Token: 0x04000445 RID: 1093
		GameInvitationPlayerAlreadyInGame,
		// Token: 0x04000446 RID: 1094
		GameInvitationSuccess,
		// Token: 0x04000447 RID: 1095
		ChangeRegionFailed,
		// Token: 0x04000448 RID: 1096
		ChangeGameModeFailed,
		// Token: 0x04000449 RID: 1097
		BattleServerKickFriendlyFire,
		// Token: 0x0400044A RID: 1098
		ChatServerDisconnectedFromRoom,
		// Token: 0x0400044B RID: 1099
		CustomizationServiceIsUnavailable,
		// Token: 0x0400044C RID: 1100
		CustomizationNotEnoughLoot,
		// Token: 0x0400044D RID: 1101
		CustomizationItemIsUnavailable,
		// Token: 0x0400044E RID: 1102
		CustomizationItemIsFree,
		// Token: 0x0400044F RID: 1103
		CustomizationItemAlreadyOwned,
		// Token: 0x04000450 RID: 1104
		CustomizationItemIsNotOwned,
		// Token: 0x04000451 RID: 1105
		CustomizationChangeSigilSuccess,
		// Token: 0x04000452 RID: 1106
		CustomizationTroopIsNotValid,
		// Token: 0x04000453 RID: 1107
		CustomizationCantUseMoreThanOneForSingleSlot,
		// Token: 0x04000454 RID: 1108
		CustomizationCantUpdateBadge,
		// Token: 0x04000455 RID: 1109
		CustomizationInvalidBadge,
		// Token: 0x04000456 RID: 1110
		CustomizationCantDowngradeBadge,
		// Token: 0x04000457 RID: 1111
		CustomizationBadgeNotAvailable
	}
}
