using System;

namespace TaleWorlds.MountAndBlade.Diamond
{
	[Serializable]
	public class ClanHomeInfo
	{
		public bool IsInClan { get; private set; }

		public bool CanCreateClan { get; private set; }

		public ClanInfo ClanInfo { get; private set; }

		public NotEnoughPlayersInfo NotEnoughPlayersInfo { get; private set; }

		public PlayerNotEligibleInfo[] PlayerNotEligibleInfos { get; private set; }

		public ClanPlayerInfo[] ClanPlayerInfos { get; private set; }

		public ClanHomeInfo(bool isInClan, bool canCreateClan, ClanInfo clanInfo, NotEnoughPlayersInfo notEnoughPlayersInfo, PlayerNotEligibleInfo[] playerNotEligibleInfos, ClanPlayerInfo[] clanPlayerInfos)
		{
			this.IsInClan = isInClan;
			this.CanCreateClan = canCreateClan;
			this.ClanInfo = clanInfo;
			this.NotEnoughPlayersInfo = notEnoughPlayersInfo;
			this.PlayerNotEligibleInfos = playerNotEligibleInfos;
			this.ClanPlayerInfos = clanPlayerInfos;
		}

		public static ClanHomeInfo CreateInClanInfo(ClanInfo clanInfo, ClanPlayerInfo[] clanPlayerInfos)
		{
			return new ClanHomeInfo(true, false, clanInfo, null, null, clanPlayerInfos);
		}

		public static ClanHomeInfo CreateCanCreateClanInfo()
		{
			return new ClanHomeInfo(false, true, null, null, null, null);
		}

		public static ClanHomeInfo CreateCantCreateClanInfo(NotEnoughPlayersInfo notEnoughPlayersInfo, PlayerNotEligibleInfo[] playerNotEligibleInfos)
		{
			return new ClanHomeInfo(false, false, null, notEnoughPlayersInfo, playerNotEligibleInfos, null);
		}

		public static ClanHomeInfo CreateInvalidStateClanInfo()
		{
			return new ClanHomeInfo(false, false, null, null, null, null);
		}
	}
}
