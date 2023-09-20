using System;
using System.Runtime.Serialization;

namespace TaleWorlds.Diamond.Rest
{
	[DataContract]
	[Serializable]
	public class RestObjectRequestMessage : RestRequestMessage
	{
		[DataMember]
		public MessageType MessageType { get; private set; }

		[DataMember]
		public SessionCredentials SessionCredentials { get; private set; }

		[DataMember]
		public Message Message { get; private set; }

		public RestObjectRequestMessage()
		{
		}

		public RestObjectRequestMessage(SessionCredentials sessionCredentials, Message message, MessageType messageType)
		{
			this.Message = message;
			this.MessageType = messageType;
			this.SessionCredentials = sessionCredentials;
		}
	}
}
