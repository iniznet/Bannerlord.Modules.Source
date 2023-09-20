using System;

namespace TaleWorlds.Diamond
{
	[Serializable]
	public class GOGAccessObject
	{
		public ulong GogId { get; private set; }

		public string UserName { get; private set; }

		public byte[] Ticket { get; private set; }

		public GOGAccessObject(string userName, ulong gogId, byte[] ticket)
		{
			this.UserName = userName;
			this.GogId = gogId;
			this.Ticket = ticket;
		}
	}
}
