using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020001F3 RID: 499
	public class CustomBattleTroopSupplier : IMissionTroopSupplier
	{
		// Token: 0x06001BC0 RID: 7104 RVA: 0x00062493 File Offset: 0x00060693
		public CustomBattleTroopSupplier(CustomBattleCombatant customBattleCombatant, bool isPlayerSide, bool isPlayerGeneral, bool isSallyOut, Func<BasicCharacterObject, bool> customAllocationConditions = null)
		{
			this._customBattleCombatant = customBattleCombatant;
			this._customAllocationConditions = customAllocationConditions;
			this._isPlayerSide = isPlayerSide;
			this._isPlayerGeneral = isPlayerSide && isPlayerGeneral;
			this._isSallyOut = isSallyOut;
			this.ArrangePriorities();
		}

		// Token: 0x06001BC1 RID: 7105 RVA: 0x000624D0 File Offset: 0x000606D0
		private void ArrangePriorities()
		{
			this._characters = new PriorityQueue<float, BasicCharacterObject>(new GenericComparer<float>());
			int[] array = new int[8];
			int[] array2 = new int[8];
			int i;
			int j;
			for (i = 0; i < 8; i = j + 1)
			{
				array[i] = this._customBattleCombatant.Characters.Count((BasicCharacterObject character) => character.DefaultFormationClass == (FormationClass)i);
				j = i;
			}
			UnitSpawnPrioritizations unitSpawnPrioritizations = (this._isPlayerSide ? Game.Current.UnitSpawnPrioritization : UnitSpawnPrioritizations.HighLevel);
			int num = array.Sum();
			float num2 = 1000f;
			foreach (BasicCharacterObject basicCharacterObject in this._customBattleCombatant.Characters)
			{
				FormationClass formationClass = basicCharacterObject.GetFormationClass();
				float num3;
				if (this._isSallyOut)
				{
					num3 = this.GetSallyOutAmbushProbabilityOfTroop(basicCharacterObject, num, ref num2);
				}
				else
				{
					num3 = this.GetDefaultProbabilityOfTroop(basicCharacterObject, num, unitSpawnPrioritizations, ref num2, ref array, ref array2);
				}
				array[(int)formationClass]--;
				array2[(int)formationClass]++;
				this._characters.Enqueue(num3, basicCharacterObject);
			}
		}

		// Token: 0x06001BC2 RID: 7106 RVA: 0x0006261C File Offset: 0x0006081C
		private float GetSallyOutAmbushProbabilityOfTroop(BasicCharacterObject character, int troopCountTotal, ref float heroProbability)
		{
			float num = 0f;
			if (character.IsHero)
			{
				float num2 = heroProbability;
				heroProbability = num2 - 1f;
				num = num2;
			}
			else
			{
				num += (float)character.Level;
				if (character.HasMount())
				{
					num += 100f;
				}
			}
			return num;
		}

		// Token: 0x06001BC3 RID: 7107 RVA: 0x00062664 File Offset: 0x00060864
		private float GetDefaultProbabilityOfTroop(BasicCharacterObject character, int troopCountTotal, UnitSpawnPrioritizations unitSpawnPrioritization, ref float heroProbability, ref int[] troopCountByFormationType, ref int[] enqueuedTroopCountByFormationType)
		{
			FormationClass formationClass = character.GetFormationClass();
			float num = (float)troopCountByFormationType[(int)formationClass] / (float)((unitSpawnPrioritization == UnitSpawnPrioritizations.Homogeneous) ? (enqueuedTroopCountByFormationType[(int)formationClass] + 1) : troopCountTotal);
			float num2;
			if (!character.IsHero)
			{
				num2 = num;
			}
			else
			{
				float num3 = heroProbability;
				heroProbability = num3 - 1f;
				num2 = num3;
			}
			float num4 = num2;
			if (!character.IsHero && (unitSpawnPrioritization == UnitSpawnPrioritizations.HighLevel || unitSpawnPrioritization == UnitSpawnPrioritizations.LowLevel))
			{
				num4 += (float)character.Level;
				if (unitSpawnPrioritization == UnitSpawnPrioritizations.LowLevel)
				{
					num4 *= -1f;
				}
			}
			return num4;
		}

		// Token: 0x06001BC4 RID: 7108 RVA: 0x000626D4 File Offset: 0x000608D4
		public IEnumerable<IAgentOriginBase> SupplyTroops(int numberToAllocate)
		{
			List<BasicCharacterObject> list = this.AllocateTroops(numberToAllocate);
			CustomBattleAgentOrigin[] array = new CustomBattleAgentOrigin[list.Count];
			this._numAllocated += list.Count;
			for (int i = 0; i < array.Length; i++)
			{
				UniqueTroopDescriptor uniqueTroopDescriptor = new UniqueTroopDescriptor(Game.Current.NextUniqueTroopSeed);
				array[i] = new CustomBattleAgentOrigin(this._customBattleCombatant, list[i], this, this._isPlayerSide, i, uniqueTroopDescriptor);
			}
			if (array.Length < numberToAllocate)
			{
				this._anyTroopRemainsToBeSupplied = false;
			}
			return array;
		}

		// Token: 0x06001BC5 RID: 7109 RVA: 0x00062754 File Offset: 0x00060954
		public IEnumerable<IAgentOriginBase> GetAllTroops()
		{
			CustomBattleAgentOrigin[] array = new CustomBattleAgentOrigin[this._customBattleCombatant.Characters.Count<BasicCharacterObject>()];
			int num = 0;
			foreach (BasicCharacterObject basicCharacterObject in this._customBattleCombatant.Characters)
			{
				array[num] = new CustomBattleAgentOrigin(this._customBattleCombatant, basicCharacterObject, this, this._isPlayerSide, -1, default(UniqueTroopDescriptor));
				num++;
			}
			return array;
		}

		// Token: 0x06001BC6 RID: 7110 RVA: 0x000627E0 File Offset: 0x000609E0
		public BasicCharacterObject GetGeneralCharacter()
		{
			return this._customBattleCombatant.General;
		}

		// Token: 0x06001BC7 RID: 7111 RVA: 0x000627F0 File Offset: 0x000609F0
		private List<BasicCharacterObject> AllocateTroops(int numberToAllocate)
		{
			if (numberToAllocate > this._characters.Count)
			{
				numberToAllocate = this._characters.Count;
			}
			List<BasicCharacterObject> list = new List<BasicCharacterObject>();
			while (numberToAllocate > 0 && this._characters.Count > 0)
			{
				BasicCharacterObject basicCharacterObject = this._characters.DequeueValue();
				if (this._customAllocationConditions == null || this._customAllocationConditions(basicCharacterObject))
				{
					list.Add(basicCharacterObject);
					numberToAllocate--;
				}
			}
			return list;
		}

		// Token: 0x06001BC8 RID: 7112 RVA: 0x00062861 File Offset: 0x00060A61
		public void OnTroopWounded()
		{
			this._numWounded++;
		}

		// Token: 0x06001BC9 RID: 7113 RVA: 0x00062871 File Offset: 0x00060A71
		public void OnTroopKilled()
		{
			this._numKilled++;
		}

		// Token: 0x06001BCA RID: 7114 RVA: 0x00062881 File Offset: 0x00060A81
		public void OnTroopRouted()
		{
			this._numRouted++;
		}

		// Token: 0x17000583 RID: 1411
		// (get) Token: 0x06001BCB RID: 7115 RVA: 0x00062891 File Offset: 0x00060A91
		public int NumRemovedTroops
		{
			get
			{
				return this._numWounded + this._numKilled + this._numRouted;
			}
		}

		// Token: 0x17000584 RID: 1412
		// (get) Token: 0x06001BCC RID: 7116 RVA: 0x000628A7 File Offset: 0x00060AA7
		public int NumTroopsNotSupplied
		{
			get
			{
				return this._characters.Count - this._numAllocated;
			}
		}

		// Token: 0x06001BCD RID: 7117 RVA: 0x000628BB File Offset: 0x00060ABB
		public int GetNumberOfPlayerControllableTroops()
		{
			return this._numAllocated;
		}

		// Token: 0x17000585 RID: 1413
		// (get) Token: 0x06001BCE RID: 7118 RVA: 0x000628C3 File Offset: 0x00060AC3
		public bool AnyTroopRemainsToBeSupplied
		{
			get
			{
				return this._anyTroopRemainsToBeSupplied;
			}
		}

		// Token: 0x040008F3 RID: 2291
		private readonly CustomBattleCombatant _customBattleCombatant;

		// Token: 0x040008F4 RID: 2292
		private PriorityQueue<float, BasicCharacterObject> _characters;

		// Token: 0x040008F5 RID: 2293
		private int _numAllocated;

		// Token: 0x040008F6 RID: 2294
		private int _numWounded;

		// Token: 0x040008F7 RID: 2295
		private int _numKilled;

		// Token: 0x040008F8 RID: 2296
		private int _numRouted;

		// Token: 0x040008F9 RID: 2297
		private Func<BasicCharacterObject, bool> _customAllocationConditions;

		// Token: 0x040008FA RID: 2298
		private bool _anyTroopRemainsToBeSupplied = true;

		// Token: 0x040008FB RID: 2299
		private readonly bool _isPlayerSide;

		// Token: 0x040008FC RID: 2300
		private readonly bool _isPlayerGeneral;

		// Token: 0x040008FD RID: 2301
		private readonly bool _isSallyOut;
	}
}
