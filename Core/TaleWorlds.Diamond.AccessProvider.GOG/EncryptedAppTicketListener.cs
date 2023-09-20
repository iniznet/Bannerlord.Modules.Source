using System;
using Galaxy.Api;

namespace TaleWorlds.Diamond.AccessProvider.GOG
{
	internal class EncryptedAppTicketListener : IEncryptedAppTicketListener
	{
		public bool GotResult { get; private set; }

		public override void OnEncryptedAppTicketRetrieveFailure(IEncryptedAppTicketListener.FailureReason failureReason)
		{
			this.GotResult = true;
		}

		public override void OnEncryptedAppTicketRetrieveSuccess()
		{
			this.GotResult = true;
		}
	}
}
