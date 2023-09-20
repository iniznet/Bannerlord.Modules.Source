using System;
using System.Runtime.Serialization;

namespace TaleWorlds.Diamond.Rest
{
	[DataContract]
	[Serializable]
	public class AliveMessage : RestRequestMessage
	{
		[DataMember]
		public SessionCredentials SessionCredentials { get; private set; }

		public AliveMessage()
		{
		}

		public AliveMessage(SessionCredentials sessionCredentials)
		{
			this.SessionCredentials = sessionCredentials;
		}
	}
}
