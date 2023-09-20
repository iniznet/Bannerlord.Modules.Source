using System;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;

namespace TaleWorlds.CampaignSystem
{
	// Token: 0x020000A5 RID: 165
	public class VisualCreator
	{
		// Token: 0x170004D3 RID: 1235
		// (get) Token: 0x06001189 RID: 4489 RVA: 0x00050900 File Offset: 0x0004EB00
		// (set) Token: 0x0600118A RID: 4490 RVA: 0x00050908 File Offset: 0x0004EB08
		public IPartyVisualCreator PartyVisualCreator
		{
			get
			{
				return this._partyVisualCreator;
			}
			set
			{
				this._partyVisualCreator = value;
			}
		}

		// Token: 0x0600118B RID: 4491 RVA: 0x00050911 File Offset: 0x0004EB11
		public IPartyVisual CreatePartyVisual()
		{
			IPartyVisualCreator partyVisualCreator = this.PartyVisualCreator;
			if (partyVisualCreator == null)
			{
				return null;
			}
			return partyVisualCreator.CreatePartyVisual();
		}

		// Token: 0x170004D4 RID: 1236
		// (get) Token: 0x0600118C RID: 4492 RVA: 0x00050924 File Offset: 0x0004EB24
		// (set) Token: 0x0600118D RID: 4493 RVA: 0x0005092C File Offset: 0x0004EB2C
		public IMapEventVisualCreator MapEventVisualCreator
		{
			get
			{
				return this._mapEventVisualCreator;
			}
			set
			{
				this._mapEventVisualCreator = value;
			}
		}

		// Token: 0x0600118E RID: 4494 RVA: 0x00050935 File Offset: 0x0004EB35
		public IMapEventVisual CreateMapEventVisual(MapEvent mapEvent)
		{
			IMapEventVisualCreator mapEventVisualCreator = this.MapEventVisualCreator;
			if (mapEventVisualCreator == null)
			{
				return null;
			}
			return mapEventVisualCreator.CreateMapEventVisual(mapEvent);
		}

		// Token: 0x04000618 RID: 1560
		private IPartyVisualCreator _partyVisualCreator;

		// Token: 0x04000619 RID: 1561
		private IMapEventVisualCreator _mapEventVisualCreator;
	}
}
