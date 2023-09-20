using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.Siege
{
	public class DefaultSiegeStrategies
	{
		private static DefaultSiegeStrategies Instance
		{
			get
			{
				return Campaign.Current.DefaultSiegeStrategies;
			}
		}

		public static SiegeStrategy PreserveStrength
		{
			get
			{
				return DefaultSiegeStrategies.Instance._preserveStrength;
			}
		}

		public static SiegeStrategy PrepareAgainstAssault
		{
			get
			{
				return DefaultSiegeStrategies.Instance._prepareAgainstAssault;
			}
		}

		public static SiegeStrategy CounterBombardment
		{
			get
			{
				return DefaultSiegeStrategies.Instance._counterBombardment;
			}
		}

		public static SiegeStrategy PrepareAssault
		{
			get
			{
				return DefaultSiegeStrategies.Instance._prepareAssault;
			}
		}

		public static SiegeStrategy BreachWalls
		{
			get
			{
				return DefaultSiegeStrategies.Instance._breachWalls;
			}
		}

		public static SiegeStrategy WearOutDefenders
		{
			get
			{
				return DefaultSiegeStrategies.Instance._wearOutDefenders;
			}
		}

		public static SiegeStrategy Custom
		{
			get
			{
				return DefaultSiegeStrategies.Instance._custom;
			}
		}

		public static IEnumerable<SiegeStrategy> AllAttackerStrategies
		{
			get
			{
				yield return DefaultSiegeStrategies.PrepareAssault;
				yield return DefaultSiegeStrategies.BreachWalls;
				yield return DefaultSiegeStrategies.WearOutDefenders;
				yield return DefaultSiegeStrategies.PreserveStrength;
				yield return DefaultSiegeStrategies.Custom;
				yield break;
			}
		}

		public static IEnumerable<SiegeStrategy> AllDefenderStrategies
		{
			get
			{
				yield return DefaultSiegeStrategies.PrepareAgainstAssault;
				yield return DefaultSiegeStrategies.CounterBombardment;
				yield return DefaultSiegeStrategies.PreserveStrength;
				yield return DefaultSiegeStrategies.Custom;
				yield break;
			}
		}

		public DefaultSiegeStrategies()
		{
			this.RegisterAll();
		}

		private void RegisterAll()
		{
			this._preserveStrength = this.Create("siege_strategy_preserve_strength");
			this._prepareAgainstAssault = this.Create("siege_strategy_prepare_against_assault");
			this._counterBombardment = this.Create("siege_strategy_counter_bombardment");
			this._prepareAssault = this.Create("siege_strategy_prepare_assault");
			this._breachWalls = this.Create("siege_strategy_breach_walls");
			this._wearOutDefenders = this.Create("siege_strategy_wear_out_defenders");
			this._custom = this.Create("siege_strategy_custom");
			this.InitializeAll();
		}

		private SiegeStrategy Create(string stringId)
		{
			return Game.Current.ObjectManager.RegisterPresumedObject<SiegeStrategy>(new SiegeStrategy(stringId));
		}

		private void InitializeAll()
		{
			this._custom.Initialize(new TextObject("{=!}Custom", null), new TextObject("{=!}Custom strategy that can be managed entirely.", null));
			this._preserveStrength.Initialize(new TextObject("{=!}Preserve Strength", null), new TextObject("{=!}Priority is set to preserving our strength.", null));
			this._prepareAgainstAssault.Initialize(new TextObject("{=!}Prepare Against Assault", null), new TextObject("{=!}Priority is set to keep advantage when the enemies' assault starts.", null));
			this._counterBombardment.Initialize(new TextObject("{=!}Counter Bombardment", null), new TextObject("{=!}Priority is set to countering enemy bombardment.", null));
			this._prepareAssault.Initialize(new TextObject("{=!}Prepare Assault", null), new TextObject("{=!}Priority is set to assaulting the walls.", null));
			this._breachWalls.Initialize(new TextObject("{=!}Breach Walls", null), new TextObject("{=!}Priority is set to breaching the walls.", null));
			this._wearOutDefenders.Initialize(new TextObject("{=!}Wear out Defenders", null), new TextObject("{=!}Priority is set to destroying engines of the enemy.", null));
		}

		private SiegeStrategy _preserveStrength;

		private SiegeStrategy _prepareAgainstAssault;

		private SiegeStrategy _counterBombardment;

		private SiegeStrategy _prepareAssault;

		private SiegeStrategy _breachWalls;

		private SiegeStrategy _wearOutDefenders;

		private SiegeStrategy _custom;
	}
}
