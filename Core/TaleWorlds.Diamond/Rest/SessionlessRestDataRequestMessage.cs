using System;
using System.Runtime.Serialization;
using TaleWorlds.Library;

namespace TaleWorlds.Diamond.Rest
{
	[DataContract]
	[Serializable]
	public class SessionlessRestDataRequestMessage : SessionlessRestRequestMessage
	{
		[DataMember]
		public MessageType MessageType { get; private set; }

		[DataMember]
		public byte[] MessageData { get; private set; }

		public Message GetMessage()
		{
			return (Message)Common.DeserializeObject(this.MessageData);
		}

		public SessionlessRestDataRequestMessage()
		{
		}

		public SessionlessRestDataRequestMessage(Message message, MessageType messageType)
		{
			this.MessageData = Common.SerializeObject(message);
			this.MessageType = messageType;
		}
	}
}
