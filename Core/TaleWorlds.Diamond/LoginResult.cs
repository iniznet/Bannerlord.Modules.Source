using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TaleWorlds.Diamond
{
	[DataContract]
	[Serializable]
	public sealed class LoginResult : FunctionResult
	{
		[DataMember]
		public PeerId PeerId { get; private set; }

		[DataMember]
		public SessionKey SessionKey { get; private set; }

		[DataMember]
		public bool Successful { get; private set; }

		[DataMember]
		public string ErrorCode { get; private set; }

		[DataMember]
		public Dictionary<string, string> ErrorParameters { get; private set; }

		[DataMember]
		public string ProviderResponse { get; private set; }

		[DataMember]
		public LoginResultObject LoginResultObject { get; private set; }

		public LoginResult()
		{
		}

		public LoginResult(PeerId peerId, SessionKey sessionKey, LoginResultObject loginResultObject)
		{
			this.PeerId = peerId;
			this.SessionKey = sessionKey;
			this.Successful = true;
			this.ErrorCode = "";
			this.LoginResultObject = loginResultObject;
		}

		public LoginResult(PeerId peerId, SessionKey sessionKey)
			: this(peerId, sessionKey, null)
		{
		}

		public LoginResult(string errorCode, Dictionary<string, string> parameters = null)
		{
			this.ErrorCode = errorCode;
			this.Successful = false;
			this.ErrorParameters = parameters;
		}
	}
}
