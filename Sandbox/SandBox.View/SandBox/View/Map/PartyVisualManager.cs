using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;

namespace SandBox.View.Map
{
	public class PartyVisualManager : CampaignEntityVisualComponent
	{
		public static PartyVisualManager Current
		{
			get
			{
				return Campaign.Current.GetEntityComponent<PartyVisualManager>();
			}
		}

		protected override void OnInitialize()
		{
			base.OnInitialize();
			foreach (MobileParty mobileParty in MobileParty.All)
			{
				this.AddNewPartyVisualForParty(mobileParty.Party);
			}
			foreach (Settlement settlement in Settlement.All)
			{
				this.AddNewPartyVisualForParty(settlement.Party);
			}
			CampaignEvents.MobilePartyCreated.AddNonSerializedListener(this, new Action<MobileParty>(this.OnMobilePartyCreated));
			CampaignEvents.MobilePartyDestroyed.AddNonSerializedListener(this, new Action<MobileParty, PartyBase>(this.OnMobilePartyDestroyed));
		}

		private void OnMobilePartyDestroyed(MobileParty mobileParty, PartyBase destroyerParty)
		{
			this._partiesAndVisuals[mobileParty.Party].OnPartyRemoved();
			this._visualsFlattened.Remove(this._partiesAndVisuals[mobileParty.Party]);
			this._partiesAndVisuals.Remove(mobileParty.Party);
		}

		private void OnMobilePartyCreated(MobileParty mobileParty)
		{
			this.AddNewPartyVisualForParty(mobileParty.Party);
		}

		private void AddNewPartyVisualForParty(PartyBase partyBase)
		{
			PartyVisual partyVisual = new PartyVisual(partyBase);
			partyVisual.OnStartup();
			this._partiesAndVisuals.Add(partyBase, partyVisual);
			this._visualsFlattened.Add(partyVisual);
		}

		public PartyVisual GetVisualOfParty(PartyBase partyBase)
		{
			return this._partiesAndVisuals[partyBase];
		}

		public void OnFinalized()
		{
			foreach (PartyVisual partyVisual in this._partiesAndVisuals.Values)
			{
				partyVisual.ReleaseResources();
			}
		}

		public override void OnTick(float realDt, float dt)
		{
			this._dirtyPartyVisualCount = -1;
			TWParallel.For(0, this._visualsFlattened.Count, delegate(int startInclusive, int endExclusive)
			{
				for (int k = startInclusive; k < endExclusive; k++)
				{
					this._visualsFlattened[k].Tick(dt, ref this._dirtyPartyVisualCount, ref this._dirtyPartiesList);
				}
			}, 16);
			for (int i = 0; i < this._dirtyPartyVisualCount + 1; i++)
			{
				this._dirtyPartiesList[i].ValidateIsDirty(realDt, dt);
			}
			for (int j = this._fadingPartiesFlatten.Count - 1; j >= 0; j--)
			{
				this._fadingPartiesFlatten[j].TickFadingState(realDt, dt);
			}
		}

		internal void RegisterFadingVisual(PartyVisual visual)
		{
			if (!this._fadingPartiesSet.Contains(visual))
			{
				this._fadingPartiesFlatten.Add(visual);
				this._fadingPartiesSet.Add(visual);
			}
		}

		internal void UnRegisterFadingVisual(PartyVisual visual)
		{
			if (this._fadingPartiesSet.Contains(visual))
			{
				int num = this._fadingPartiesFlatten.IndexOf(visual);
				this._fadingPartiesFlatten[num] = this._fadingPartiesFlatten[this._fadingPartiesFlatten.Count - 1];
				this._fadingPartiesFlatten.Remove(this._fadingPartiesFlatten[this._fadingPartiesFlatten.Count - 1]);
				this._fadingPartiesSet.Remove(visual);
			}
		}

		private readonly Dictionary<PartyBase, PartyVisual> _partiesAndVisuals = new Dictionary<PartyBase, PartyVisual>();

		private readonly List<PartyVisual> _visualsFlattened = new List<PartyVisual>();

		private int _dirtyPartyVisualCount;

		private PartyVisual[] _dirtyPartiesList = new PartyVisual[2500];

		private readonly List<PartyVisual> _fadingPartiesFlatten = new List<PartyVisual>();

		private readonly HashSet<PartyVisual> _fadingPartiesSet = new HashSet<PartyVisual>();
	}
}
