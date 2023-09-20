using System;
using System.Runtime.Serialization;
using TaleWorlds.Library;

namespace TaleWorlds.Diamond.Rest
{
	[DataContract]
	[Serializable]
	public class RestDataResponseMessage : RestResponseMessage
	{
		[DataMember]
		public byte[] MessageData { get; private set; }

		public override Message GetMessage()
		{
			return (Message)Common.DeserializeObject(this.MessageData);
		}

		public RestDataResponseMessage()
		{
		}

		public RestDataResponseMessage(Message message)
		{
			this.MessageData = Common.SerializeObject(message);
		}
	}
}
