using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.GameState
{
	// Token: 0x02000340 RID: 832
	public class PartyState : PlayerGameState
	{
		// Token: 0x17000B15 RID: 2837
		// (get) Token: 0x06002EA7 RID: 11943 RVA: 0x000C048F File Offset: 0x000BE68F
		public override bool IsMenuState
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000B16 RID: 2838
		// (get) Token: 0x06002EA8 RID: 11944 RVA: 0x000C0492 File Offset: 0x000BE692
		// (set) Token: 0x06002EA9 RID: 11945 RVA: 0x000C049A File Offset: 0x000BE69A
		public PartyScreenLogic PartyScreenLogic { get; private set; }

		// Token: 0x17000B17 RID: 2839
		// (get) Token: 0x06002EAA RID: 11946 RVA: 0x000C04A3 File Offset: 0x000BE6A3
		// (set) Token: 0x06002EAB RID: 11947 RVA: 0x000C04AB File Offset: 0x000BE6AB
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

		// Token: 0x06002EAC RID: 11948 RVA: 0x000C04B4 File Offset: 0x000BE6B4
		public void InitializeLogic(PartyScreenLogic partyScreenLogic)
		{
			this.PartyScreenLogic = partyScreenLogic;
		}

		// Token: 0x06002EAD RID: 11949 RVA: 0x000C04BD File Offset: 0x000BE6BD
		public void RequestUserInput(string text, Action accept, Action cancel)
		{
			if (this.Handler != null)
			{
				this.Handler.RequestUserInput(text, accept, cancel);
			}
		}

		// Token: 0x04000DFA RID: 3578
		private IPartyScreenLogicHandler _handler;
	}
}
