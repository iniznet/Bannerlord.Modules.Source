using System;

namespace TaleWorlds.MountAndBlade
{
	public class MultiplayerInfo
	{
		public MultiplayerData MultiplayerDataValues
		{
			get
			{
				return this.multiplayerDataValues;
			}
		}

		public MultiplayerInfo()
		{
			this.multiplayerDataValues = new MultiplayerData();
		}

		public bool IsMultiplayerTeamAvailable(int peerNo, int teamNo)
		{
			return true;
		}

		protected MultiplayerData multiplayerDataValues;
	}
}
