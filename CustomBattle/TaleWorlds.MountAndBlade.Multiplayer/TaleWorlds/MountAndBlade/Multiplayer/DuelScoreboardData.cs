﻿using System;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond.MultiplayerBadges;
using TaleWorlds.MountAndBlade.MissionRepresentatives;

namespace TaleWorlds.MountAndBlade.Multiplayer
{
	public class DuelScoreboardData : IScoreboardData
	{
		public MissionScoreboardComponent.ScoreboardHeader[] GetScoreboardHeaders()
		{
			PeerExtensions.GetComponent<MissionRepresentativeBase>(GameNetwork.MyPeer);
			MissionScoreboardComponent.ScoreboardHeader[] array = new MissionScoreboardComponent.ScoreboardHeader[7];
			array[0] = new MissionScoreboardComponent.ScoreboardHeader("ping", (MissionPeer missionPeer) => MathF.Round(PeerExtensions.GetNetworkPeer(missionPeer).AveragePingInMilliseconds).ToString(), (BotData bot) => "");
			array[1] = new MissionScoreboardComponent.ScoreboardHeader("avatar", (MissionPeer missionPeer) => "", (BotData bot) => "");
			array[2] = new MissionScoreboardComponent.ScoreboardHeader("badge", delegate(MissionPeer missionPeer)
			{
				Badge byIndex = BadgeManager.GetByIndex(PeerExtensions.GetPeer(missionPeer).ChosenBadgeIndex);
				if (byIndex == null)
				{
					return null;
				}
				return byIndex.StringId;
			}, (BotData bot) => "");
			array[3] = new MissionScoreboardComponent.ScoreboardHeader("name", (MissionPeer missionPeer) => missionPeer.GetComponent<MissionPeer>().DisplayedName, (BotData bot) => new TextObject("{=hvQSOi79}Bot", null).ToString());
			array[4] = new MissionScoreboardComponent.ScoreboardHeader("winstreak", (MissionPeer missionPeer) => missionPeer.GetComponent<DuelMissionRepresentative>().NumberOfWins.ToString(), (BotData bot) => "");
			array[5] = new MissionScoreboardComponent.ScoreboardHeader("bounty", (MissionPeer missionPeer) => missionPeer.GetComponent<DuelMissionRepresentative>().Bounty.ToString(), (BotData bot) => "");
			array[6] = new MissionScoreboardComponent.ScoreboardHeader("score", (MissionPeer missionPeer) => missionPeer.GetComponent<DuelMissionRepresentative>().Score.ToString(), (BotData bot) => "");
			return array;
		}
	}
}
