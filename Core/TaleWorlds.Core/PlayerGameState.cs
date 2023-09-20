using System;

namespace TaleWorlds.Core
{
	public abstract class PlayerGameState : GameState
	{
		public VirtualPlayer Peer
		{
			get
			{
				return this._peer;
			}
			private set
			{
				this._peer = value;
			}
		}

		private VirtualPlayer _peer;
	}
}
