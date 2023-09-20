using System;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Diamond
{
	[Serializable]
	public class PlayerNotEligibleInfo
	{
		public PlayerId PlayerId { get; private set; }

		public PlayerNotEligibleError[] Errors { get; private set; }

		public PlayerNotEligibleInfo(PlayerId playerId, PlayerNotEligibleError[] errors)
		{
			this.PlayerId = playerId;
			this.Errors = errors;
		}
	}
}
