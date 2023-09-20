using System;
using Galaxy.Api;

namespace TaleWorlds.PlatformService.GOG
{
	public class FriendListListener : IFriendListListener
	{
		public bool GotResult { get; private set; }

		public override void OnFriendListRetrieveSuccess()
		{
			this.GotResult = true;
		}

		public override void OnFriendListRetrieveFailure(IFriendListListener.FailureReason failureReason)
		{
			this.GotResult = true;
		}
	}
}
