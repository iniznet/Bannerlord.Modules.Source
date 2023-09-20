using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.GameState
{
	public class PartyState : PlayerGameState
	{
		public override bool IsMenuState
		{
			get
			{
				return true;
			}
		}

		public PartyScreenLogic PartyScreenLogic { get; private set; }

		public IPartyScreenLogicHandler Handler
		{
			get
			{
				return this._handler;
			}
			set
			{
				this._handler = value;
			}
		}

		public void InitializeLogic(PartyScreenLogic partyScreenLogic)
		{
			this.PartyScreenLogic = partyScreenLogic;
		}

		public void RequestUserInput(string text, Action accept, Action cancel)
		{
			if (this.Handler != null)
			{
				this.Handler.RequestUserInput(text, accept, cancel);
			}
		}

		private IPartyScreenLogicHandler _handler;
	}
}
