using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.GameState
{
	// Token: 0x02000335 RID: 821
	public class EducationState : GameState
	{
		// Token: 0x17000AFF RID: 2815
		// (get) Token: 0x06002E33 RID: 11827 RVA: 0x000BFDE4 File Offset: 0x000BDFE4
		public override bool IsMenuState
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000B00 RID: 2816
		// (get) Token: 0x06002E34 RID: 11828 RVA: 0x000BFDE7 File Offset: 0x000BDFE7
		// (set) Token: 0x06002E35 RID: 11829 RVA: 0x000BFDEF File Offset: 0x000BDFEF
		public Hero Child { get; private set; }

		// Token: 0x17000B01 RID: 2817
		// (get) Token: 0x06002E36 RID: 11830 RVA: 0x000BFDF8 File Offset: 0x000BDFF8
		// (set) Token: 0x06002E37 RID: 11831 RVA: 0x000BFE00 File Offset: 0x000BE000
		public IEducationStateHandler Handler
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

		// Token: 0x06002E38 RID: 11832 RVA: 0x000BFE09 File Offset: 0x000BE009
		public EducationState()
		{
			Debug.FailedAssert("Do not use EducationState with default constructor!", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\GameState\\EducationState.cs", ".ctor", 22);
		}

		// Token: 0x06002E39 RID: 11833 RVA: 0x000BFE27 File Offset: 0x000BE027
		public EducationState(Hero child)
		{
			this.Child = child;
		}

		// Token: 0x04000DE9 RID: 3561
		private IEducationStateHandler _handler;
	}
}
