using System;
using System.Runtime.Serialization;

namespace TaleWorlds.Diamond
{
	[DataContract]
	[Serializable]
	public sealed class SessionCredentials
	{
		[DataMember]
		public PeerId PeerId { get; private set; }

		[DataMember]
		public SessionKey SessionKey { get; private set; }

		public SessionCredentials(PeerId peerId, SessionKey sessionKey)
		{
			this.PeerId = peerId;
			this.SessionKey = sessionKey;
		}
	}
}
