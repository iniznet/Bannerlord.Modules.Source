using System;
using System.Runtime.Serialization;

namespace TaleWorlds.Diamond.Rest
{
	[DataContract]
	[Serializable]
	public class RestObjectResponseMessage : RestResponseMessage
	{
		public override Message GetMessage()
		{
			return this._message;
		}

		public RestObjectResponseMessage()
		{
		}

		public RestObjectResponseMessage(Message message)
		{
			this._message = message;
		}

		[DataMember]
		private Message _message;
	}
}
