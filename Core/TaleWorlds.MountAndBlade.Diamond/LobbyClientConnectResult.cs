﻿using System;
using System.Collections.Generic;
using TaleWorlds.Diamond;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.Diamond
{
	public class LobbyClientConnectResult
	{
		public bool Connected { get; private set; }

		public TextObject Error { get; private set; }

		public LobbyClientConnectResult(bool connected, TextObject error)
		{
			this.Connected = connected;
			this.Error = error;
		}

		public static LobbyClientConnectResult FromServerConnectResult(string errorCode, Dictionary<string, string> parameters)
		{
			if (LobbyClientConnectResult._serverErrorCodes.ContainsKey(errorCode))
			{
				string text = LobbyClientConnectResult._serverErrorCodes[errorCode];
				if (parameters != null)
				{
					foreach (string text2 in parameters.Keys)
					{
						text = text.Replace("{" + text2 + "}", parameters[text2]);
					}
				}
				TextObject textObject = new TextObject(text, null);
				return new LobbyClientConnectResult(errorCode == LoginErrorCode.None.ToString(), textObject);
			}
			return new LobbyClientConnectResult(false, new TextObject("{=tzQxtv27}Unknown error.", null));
		}

		private static Dictionary<string, string> _serverErrorCodes = new Dictionary<string, string>
		{
			{
				LoginErrorCode.None.ToString(),
				""
			},
			{
				LoginErrorCode.Failed.ToString(),
				"{=Rj8RhD7F}Failed"
			},
			{
				LoginErrorCode.LoginRequestFailed.ToString(),
				"{=ahobSLlo}Login request failed."
			},
			{
				LoginErrorCode.PlatformServiceNoAccess.ToString(),
				"{=NRa52uaF}Could not get access from your platform service."
			},
			{
				LoginErrorCode.PlatformServiceNoAccessError.ToString(),
				"{=DLKRQOuM}Could not get access from your platform service (Error: {ACCESSERROR})."
			},
			{ "CouldNotLogin", "{=kY2oXMng}Could not login." },
			{ "VersionMismatch", "{=Tauk2JzA}Version mismatch, Server Version: {SERVERVERSION} - Your Version: {CLIENTVERSION}." },
			{ "IncorrectPassword", "{=X6N9nSn0}You are not allowed to login into this server. Please try official public servers." },
			{ "FamilyShareNotAllowed", "{=dbLB0tuT}Family share is not allowed." },
			{ "BannedFromGame", "{=0W4dgUho}Banned from game until {BANNEDUNTIL}, Reason: {BANREASON}" },
			{ "NoAuthenticationToken", "{=iWLq9hMg}No Authentication Token is provided." },
			{ "AuthTokenExpired", "{=39f0eu3Y}Provided AuthToken has expired." },
			{ "BannedFromHostingServers", "{=xriaEwgN}You're banned from hosting servers until {BAN_EXPIRATION_TIME}. Reason: {BAN_REASON}." },
			{ "CustomBattleServerIncompatibleVersion", "{=satFJMMu}Custom Battle Server has incompatible version." },
			{ "ReachedMaxNumberofCustomBattleServers", "{=YmPyx3qi}Player reached maximum allowed number of Custom Battle Servers." },
			{ "CouldNotDestroyOldSession", "{=p5K0ATtZ}Could not destroy your old session." }
		};
	}
}
