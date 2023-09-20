using System;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond.MultiplayerBadges;
using TaleWorlds.MountAndBlade.MissionRepresentatives;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020002BC RID: 700
	public class DuelScoreboardData : IScoreboardData
	{
		// Token: 0x060026E7 RID: 9959 RVA: 0x00092510 File Offset: 0x00090710
		public MissionScoreboardComponent.ScoreboardHeader[] GetScoreboardHeaders()
		{
			GameNetwork.MyPeer.GetComponent<MissionRepresentativeBase>();
			MissionScoreboardComponent.ScoreboardHeader[] array = new MissionScoreboardComponent.ScoreboardHeader[7];
			array[0] = new MissionScoreboardComponent.ScoreboardHeader("ping", (MissionPeer missionPeer) => MathF.Round(missionPeer.GetNetworkPeer().AveragePingInMilliseconds).ToString(), (BotData bot) => "");
			array[1] = new MissionScoreboardComponent.ScoreboardHeader("avatar", (MissionPeer missionPeer) => "", (BotData bot) => "");
			array[2] = new MissionScoreboardComponent.ScoreboardHeader("badge", delegate(MissionPeer missionPeer)
			{
				Badge byIndex = BadgeManager.GetByIndex(missionPeer.GetPeer().ChosenBadgeIndex);
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
