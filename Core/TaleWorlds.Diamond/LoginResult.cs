using System;
using System.Runtime.Serialization;
using TaleWorlds.Localization;

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
		public TextObject ErrorCode { get; private set; }

		[DataMember]
		public LoginResultObject LoginResultObject { get; private set; }

		public LoginResult(PeerId peerId, SessionKey sessionKey, LoginResultObject loginResultObject)
		{
			this.PeerId = peerId;
			this.SessionKey = sessionKey;
			this.Successful = true;
			this.ErrorCode = new TextObject("", null);
			this.LoginResultObject = loginResultObject;
		}

		public LoginResult(PeerId peerId, SessionKey sessionKey)
			: this(peerId, sessionKey, null)
		{
		}

		public LoginResult(TextObject errorCode)
		{
			this.ErrorCode = errorCode;
			this.Successful = false;
		}
	}
}
