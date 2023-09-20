using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class CustomBattleTroopSupplier : IMissionTroopSupplier
	{
		public CustomBattleTroopSupplier(CustomBattleCombatant customBattleCombatant, bool isPlayerSide, bool isPlayerGeneral, bool isSallyOut, Func<BasicCharacterObject, bool> customAllocationConditions = null)
		{
			this._customBattleCombatant = customBattleCombatant;
			this._customAllocationConditions = customAllocationConditions;
			this._isPlayerSide = isPlayerSide;
			this._isPlayerGeneral = isPlayerSide && isPlayerGeneral;
			this._isSallyOut = isSallyOut;
			this.ArrangePriorities();
		}

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

		public BasicCharacterObject GetGeneralCharacter()
		{
			return this._customBattleCombatant.General;
		}

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

		public void OnTroopWounded()
		{
			this._numWounded++;
		}

		public void OnTroopKilled()
		{
			this._numKilled++;
		}

		public void OnTroopRouted()
		{
			this._numRouted++;
		}

		public int NumRemovedTroops
		{
			get
			{
				return this._numWounded + this._numKilled + this._numRouted;
			}
		}

		public int NumTroopsNotSupplied
		{
			get
			{
				return this._characters.Count - this._numAllocated;
			}
		}

		public int GetNumberOfPlayerControllableTroops()
		{
			return this._numAllocated;
		}

		public bool AnyTroopRemainsToBeSupplied
		{
			get
			{
				return this._anyTroopRemainsToBeSupplied;
			}
		}

		private readonly CustomBattleCombatant _customBattleCombatant;

		private PriorityQueue<float, BasicCharacterObject> _characters;

		private int _numAllocated;

		private int _numWounded;

		private int _numKilled;

		private int _numRouted;

		private Func<BasicCharacterObject, bool> _customAllocationConditions;

		private bool _anyTroopRemainsToBeSupplied = true;

		private readonly bool _isPlayerSide;

		private readonly bool _isPlayerGeneral;

		private readonly bool _isSallyOut;
	}
}
