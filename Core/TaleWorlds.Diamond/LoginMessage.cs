using System;
using System.Runtime.Serialization;

namespace TaleWorlds.Diamond
{
	[DataContract]
	[Serializable]
	public abstract class LoginMessage : Message
	{
		[DataMember]
		public PeerId PeerId { get; set; }

		public LoginMessage()
		{
		}

		protected LoginMessage(PeerId peerId)
		{
			this.PeerId = peerId;
		}
	}
}
