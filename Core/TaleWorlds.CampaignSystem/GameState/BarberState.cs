using System;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.GameState
{
	// Token: 0x0200032E RID: 814
	public class BarberState : GameState
	{
		// Token: 0x17000AF0 RID: 2800
		// (get) Token: 0x06002E0B RID: 11787 RVA: 0x000BFC62 File Offset: 0x000BDE62
		public override bool IsMenuState
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000AF1 RID: 2801
		// (get) Token: 0x06002E0C RID: 11788 RVA: 0x000BFC65 File Offset: 0x000BDE65
		// (set) Token: 0x06002E0D RID: 11789 RVA: 0x000BFC6D File Offset: 0x000BDE6D
		public IFaceGeneratorCustomFilter Filter { get; private set; }

		// Token: 0x06002E0E RID: 11790 RVA: 0x000BFC76 File Offset: 0x000BDE76
		public BarberState()
		{
		}

		// Token: 0x06002E0F RID: 11791 RVA: 0x000BFC7E File Offset: 0x000BDE7E
		public BarberState(BasicCharacterObject character, IFaceGeneratorCustomFilter filter)
		{
			this.Character = character;
			this.Filter = filter;
		}

		// Token: 0x04000DDC RID: 3548
		public BasicCharacterObject Character;
	}
}
