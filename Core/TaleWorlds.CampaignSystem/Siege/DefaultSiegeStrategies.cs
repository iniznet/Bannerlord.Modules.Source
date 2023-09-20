using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.Siege
{
	// Token: 0x02000289 RID: 649
	public class DefaultSiegeStrategies
	{
		// Token: 0x1700087E RID: 2174
		// (get) Token: 0x06002248 RID: 8776 RVA: 0x00091E88 File Offset: 0x00090088
		private static DefaultSiegeStrategies Instance
		{
			get
			{
				return Campaign.Current.DefaultSiegeStrategies;
			}
		}

		// Token: 0x1700087F RID: 2175
		// (get) Token: 0x06002249 RID: 8777 RVA: 0x00091E94 File Offset: 0x00090094
		public static SiegeStrategy PreserveStrength
		{
			get
			{
				return DefaultSiegeStrategies.Instance._preserveStrength;
			}
		}

		// Token: 0x17000880 RID: 2176
		// (get) Token: 0x0600224A RID: 8778 RVA: 0x00091EA0 File Offset: 0x000900A0
		public static SiegeStrategy PrepareAgainstAssault
		{
			get
			{
				return DefaultSiegeStrategies.Instance._prepareAgainstAssault;
			}
		}

		// Token: 0x17000881 RID: 2177
		// (get) Token: 0x0600224B RID: 8779 RVA: 0x00091EAC File Offset: 0x000900AC
		public static SiegeStrategy CounterBombardment
		{
			get
			{
				return DefaultSiegeStrategies.Instance._counterBombardment;
			}
		}

		// Token: 0x17000882 RID: 2178
		// (get) Token: 0x0600224C RID: 8780 RVA: 0x00091EB8 File Offset: 0x000900B8
		public static SiegeStrategy PrepareAssault
		{
			get
			{
				return DefaultSiegeStrategies.Instance._prepareAssault;
			}
		}

		// Token: 0x17000883 RID: 2179
		// (get) Token: 0x0600224D RID: 8781 RVA: 0x00091EC4 File Offset: 0x000900C4
		public static SiegeStrategy BreachWalls
		{
			get
			{
				return DefaultSiegeStrategies.Instance._breachWalls;
			}
		}

		// Token: 0x17000884 RID: 2180
		// (get) Token: 0x0600224E RID: 8782 RVA: 0x00091ED0 File Offset: 0x000900D0
		public static SiegeStrategy WearOutDefenders
		{
			get
			{
				return DefaultSiegeStrategies.Instance._wearOutDefenders;
			}
		}

		// Token: 0x17000885 RID: 2181
		// (get) Token: 0x0600224F RID: 8783 RVA: 0x00091EDC File Offset: 0x000900DC
		public static SiegeStrategy Custom
		{
			get
			{
				return DefaultSiegeStrategies.Instance._custom;
			}
		}

		// Token: 0x17000886 RID: 2182
		// (get) Token: 0x06002250 RID: 8784 RVA: 0x00091EE8 File Offset: 0x000900E8
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

		// Token: 0x17000887 RID: 2183
		// (get) Token: 0x06002251 RID: 8785 RVA: 0x00091EF1 File Offset: 0x000900F1
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

		// Token: 0x06002252 RID: 8786 RVA: 0x00091EFA File Offset: 0x000900FA
		public DefaultSiegeStrategies()
		{
			this.RegisterAll();
		}

		// Token: 0x06002253 RID: 8787 RVA: 0x00091F08 File Offset: 0x00090108
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

		// Token: 0x06002254 RID: 8788 RVA: 0x00091F92 File Offset: 0x00090192
		private SiegeStrategy Create(string stringId)
		{
			return Game.Current.ObjectManager.RegisterPresumedObject<SiegeStrategy>(new SiegeStrategy(stringId));
		}

		// Token: 0x06002255 RID: 8789 RVA: 0x00091FAC File Offset: 0x000901AC
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

		// Token: 0x04000A9C RID: 2716
		private SiegeStrategy _preserveStrength;

		// Token: 0x04000A9D RID: 2717
		private SiegeStrategy _prepareAgainstAssault;

		// Token: 0x04000A9E RID: 2718
		private SiegeStrategy _counterBombardment;

		// Token: 0x04000A9F RID: 2719
		private SiegeStrategy _prepareAssault;

		// Token: 0x04000AA0 RID: 2720
		private SiegeStrategy _breachWalls;

		// Token: 0x04000AA1 RID: 2721
		private SiegeStrategy _wearOutDefenders;

		// Token: 0x04000AA2 RID: 2722
		private SiegeStrategy _custom;
	}
}
