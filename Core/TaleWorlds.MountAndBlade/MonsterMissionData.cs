using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
	public class MonsterMissionData : IMonsterMissionData
	{
		public Monster Monster { get; private set; }

		public CapsuleData BodyCapsule
		{
			get
			{
				return new CapsuleData(this.Monster.BodyCapsuleRadius, this.Monster.BodyCapsulePoint1, this.Monster.BodyCapsulePoint2);
			}
		}

		public CapsuleData CrouchedBodyCapsule
		{
			get
			{
				return new CapsuleData(this.Monster.CrouchedBodyCapsuleRadius, this.Monster.CrouchedBodyCapsulePoint1, this.Monster.CrouchedBodyCapsulePoint2);
			}
		}

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

		public MonsterMissionData(Monster monster)
		{
			this._actionSet = MBActionSet.InvalidActionSet;
			this._femaleActionSet = MBActionSet.InvalidActionSet;
			this.Monster = monster;
		}

		private MBActionSet _actionSet;

		private MBActionSet _femaleActionSet;
	}
}
