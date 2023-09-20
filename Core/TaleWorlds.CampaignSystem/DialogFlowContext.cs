using System;

namespace TaleWorlds.CampaignSystem
{
	internal class DialogFlowContext
	{
		public DialogFlowContext(string token, bool byPlayer, DialogFlowContext parent)
		{
			this.Token = token;
			this.ByPlayer = byPlayer;
			this.Parent = parent;
		}

		internal readonly string Token;

		internal readonly bool ByPlayer;

		internal readonly DialogFlowContext Parent;
	}
}
