using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TaleWorlds.Diamond.Rest
{
	[DataContract]
	[Serializable]
	public sealed class RestResponse : RestData
	{
		[DataMember]
		public bool Successful { get; private set; }

		[DataMember]
		public string SuccessfulReason { get; private set; }

		[DataMember]
		public RestFunctionResult FunctionResult { get; set; }

		[DataMember]
		public byte[] UserCertificate { get; set; }

		public int RemainingMessageCount
		{
			get
			{
				if (this._responseMessages != null)
				{
					return this._responseMessages.Count;
				}
				return 0;
			}
		}

		public RestResponse()
		{
			this._responseMessages = new List<RestResponseMessage>();
		}

		public void SetSuccessful(bool successful, string succressfulReason)
		{
			this.Successful = successful;
			this.SuccessfulReason = succressfulReason;
		}

		public RestResponseMessage TryDequeueMessage()
		{
			if (this._responseMessages != null && this._responseMessages.Count > 0)
			{
				RestResponseMessage restResponseMessage = this._responseMessages[0];
				this._responseMessages.RemoveAt(0);
				return restResponseMessage;
			}
			return null;
		}

		public void ClearMessageQueue()
		{
			this._responseMessages.Clear();
		}

		public void EnqueueMessage(RestResponseMessage message)
		{
			this._responseMessages.Add(message);
		}

		[DataMember]
		private List<RestResponseMessage> _responseMessages;
	}
}
