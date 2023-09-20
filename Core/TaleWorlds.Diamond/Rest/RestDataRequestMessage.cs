using System;
using System.Runtime.Serialization;
using TaleWorlds.Library;

namespace TaleWorlds.Diamond.Rest
{
	[DataContract]
	[Serializable]
	public class RestDataRequestMessage : RestRequestMessage
	{
		[DataMember]
		public MessageType MessageType { get; private set; }

		[DataMember]
		public SessionCredentials SessionCredentials { get; private set; }

		[DataMember]
		public byte[] MessageData { get; private set; }

		[DataMember]
		public string MessageName { get; private set; }

		public Message GetMessage()
		{
			return (Message)Common.DeserializeObject(this.MessageData);
		}

		public RestDataRequestMessage()
		{
		}

		public RestDataRequestMessage(SessionCredentials sessionCredentials, Message message, MessageType messageType)
		{
			this.MessageData = Common.SerializeObject(message);
			this.MessageType = messageType;
			this.SessionCredentials = sessionCredentials;
			this.MessageName = message.GetType().Name;
		}

		public override string ToString()
		{
			return string.Concat(new object[] { "Rest Data Request Message: ", this.MessageName, "-", this.MessageType });
		}
	}
}
