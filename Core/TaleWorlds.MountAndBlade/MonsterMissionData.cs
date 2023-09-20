using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020002D5 RID: 725
	public class MonsterMissionData : IMonsterMissionData
	{
		// Token: 0x1700073F RID: 1855
		// (get) Token: 0x0600280C RID: 10252 RVA: 0x0009B38C File Offset: 0x0009958C
		// (set) Token: 0x0600280D RID: 10253 RVA: 0x0009B394 File Offset: 0x00099594
		public Monster Monster { get; private set; }

		// Token: 0x17000740 RID: 1856
		// (get) Token: 0x0600280E RID: 10254 RVA: 0x0009B3A0 File Offset: 0x000995A0
		public CapsuleData BodyCapsule
		{
			get
			{
				return new CapsuleData
				{
					Radius = this.Monster.BodyCapsuleRadius,
					P1 = this.Monster.BodyCapsulePoint1,
					P2 = this.Monster.BodyCapsulePoint2
				};
			}
		}

		// Token: 0x17000741 RID: 1857
		// (get) Token: 0x0600280F RID: 10255 RVA: 0x0009B3EC File Offset: 0x000995EC
		public CapsuleData CrouchedBodyCapsule
		{
			get
			{
				return new CapsuleData
				{
					Radius = this.Monster.CrouchedBodyCapsuleRadius,
					P1 = this.Monster.CrouchedBodyCapsulePoint1,
					P2 = this.Monster.CrouchedBodyCapsulePoint2
				};
			}
		}

		// Token: 0x17000742 RID: 1858
		// (get) Token: 0x06002810 RID: 10256 RVA: 0x0009B438 File Offset: 0x00099638
		public MBActionSet ActionSet
		{
			get
			{
				if (!this._actionSet.IsValid && !string.IsNullOrEmpty(this.Monster.ActionSetCode))
				{
					this._actionSet = MBActionSet.GetActionSet(this.Monster.ActionSetCode);
				}
				return this._actionSet;
			}
		}

		// Token: 0x17000743 RID: 1859
		// (get) Token: 0x06002811 RID: 10257 RVA: 0x0009B475 File Offset: 0x00099675
		public MBActionSet FemaleActionSet
		{
			get
			{
				if (!this._femaleActionSet.IsValid && !string.IsNullOrEmpty(this.Monster.FemaleActionSetCode))
				{
					this._femaleActionSet = MBActionSet.GetActionSet(this.Monster.FemaleActionSetCode);
				}
				return this._femaleActionSet;
			}
		}

		// Token: 0x06002812 RID: 10258 RVA: 0x0009B4B2 File Offset: 0x000996B2
		public MonsterMissionData(Monster monster)
		{
			this._actionSet = MBActionSet.InvalidActionSet;
			this._femaleActionSet = MBActionSet.InvalidActionSet;
			this.Monster = monster;
		}

		// Token: 0x04000EBF RID: 3775
		private MBActionSet _actionSet;

		// Token: 0x04000EC0 RID: 3776
		private MBActionSet _femaleActionSet;
	}
}
