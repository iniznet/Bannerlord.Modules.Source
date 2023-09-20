using System;

namespace TaleWorlds.MountAndBlade.Diamond
{
	public class DisconnectInfo
	{
		public DisconnectType Type { get; set; }

		public DisconnectInfo()
		{
			this.Type = DisconnectType.Unknown;
		}
	}
}
