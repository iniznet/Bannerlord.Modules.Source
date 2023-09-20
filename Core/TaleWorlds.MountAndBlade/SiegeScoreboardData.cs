using System;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond.MultiplayerBadges;

namespace TaleWorlds.MountAndBlade
{
	public class SiegeScoreboardData : IScoreboardData
	{
		public MissionScoreboardComponent.ScoreboardHeader[] GetScoreboardHeaders()
		{
			GameNetwork.MyPeer.GetComponent<MissionRepresentativeBase>();
			MissionScoreboardComponent.ScoreboardHeader[] array = new MissionScoreboardComponent.ScoreboardHeader[8];
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
			array[4] = new MissionScoreboardComponent.ScoreboardHeader("kill", (MissionPeer missionPeer) => missionPeer.KillCount.ToString(), (BotData bot) => bot.KillCount.ToString());
			array[5] = new MissionScoreboardComponent.ScoreboardHeader("death", (MissionPeer missionPeer) => missionPeer.DeathCount.ToString(), (BotData bot) => bot.DeathCount.ToString());
			array[6] = new MissionScoreboardComponent.ScoreboardHeader("assist", (MissionPeer missionPeer) => missionPeer.AssistCount.ToString(), (BotData bot) => bot.AssistCount.ToString());
			array[7] = new MissionScoreboardComponent.ScoreboardHeader("score", (MissionPeer missionPeer) => missionPeer.Score.ToString().ToString(), (BotData bot) => bot.Score.ToString());
			return array;
		}
	}
}
