using System;
using TaleWorlds.Core;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class UpdateCharacterMessage : Message
	{
		public BodyProperties BodyProperties { get; private set; }

		public bool IsFemale { get; private set; }

		public UpdateCharacterMessage(BodyProperties bodyProperties, bool isFemale)
		{
			this.BodyProperties = bodyProperties;
			this.IsFemale = isFemale;
		}
	}
}
