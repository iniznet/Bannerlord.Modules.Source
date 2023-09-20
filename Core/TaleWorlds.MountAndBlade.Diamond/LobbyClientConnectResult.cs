using System;
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
	}
}
