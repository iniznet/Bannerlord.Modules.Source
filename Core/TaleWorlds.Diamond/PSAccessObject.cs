using System;

namespace TaleWorlds.Diamond
{
	[Serializable]
	public class PSAccessObject
	{
		public int IssuerId { get; }

		public string AuthCode { get; }

		public PSAccessObject(int issuerId, string authCode)
		{
			this.IssuerId = issuerId;
			this.AuthCode = authCode;
		}
	}
}
