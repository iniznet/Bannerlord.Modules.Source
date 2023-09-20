using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020001FA RID: 506
	public struct FormationDeploymentOrder
	{
		// Token: 0x17000598 RID: 1432
		// (get) Token: 0x06001C23 RID: 7203 RVA: 0x000648F5 File Offset: 0x00062AF5
		// (set) Token: 0x06001C24 RID: 7204 RVA: 0x000648FD File Offset: 0x00062AFD
		public int Key { get; private set; }

		// Token: 0x17000599 RID: 1433
		// (get) Token: 0x06001C25 RID: 7205 RVA: 0x00064906 File Offset: 0x00062B06
		// (set) Token: 0x06001C26 RID: 7206 RVA: 0x0006490E File Offset: 0x00062B0E
		public int Offset { get; private set; }

		// Token: 0x06001C27 RID: 7207 RVA: 0x00064918 File Offset: 0x00062B18
		private FormationDeploymentOrder(FormationClass formationClass, int offset = 0)
		{
			int formationClassPriority = FormationDeploymentOrder.GetFormationClassPriority(formationClass);
			this.Offset = MathF.Max(0, offset);
			this.Key = formationClassPriority + this.Offset * 11;
		}

		// Token: 0x06001C28 RID: 7208 RVA: 0x0006494A File Offset: 0x00062B4A
		public static FormationDeploymentOrder GetDeploymentOrder(FormationClass fClass, int offset = 0)
		{
			return new FormationDeploymentOrder(fClass, offset);
		}

		// Token: 0x06001C29 RID: 7209 RVA: 0x00064953 File Offset: 0x00062B53
		public static FormationDeploymentOrder.DeploymentOrderComparer GetComparer()
		{
			FormationDeploymentOrder.DeploymentOrderComparer deploymentOrderComparer;
			if ((deploymentOrderComparer = FormationDeploymentOrder._comparer) == null)
			{
				deploymentOrderComparer = (FormationDeploymentOrder._comparer = new FormationDeploymentOrder.DeploymentOrderComparer());
			}
			return deploymentOrderComparer;
		}

		// Token: 0x06001C2A RID: 7210 RVA: 0x0006496C File Offset: 0x00062B6C
		private static int GetFormationClassPriority(FormationClass fClass)
		{
			switch (fClass)
			{
			case FormationClass.Infantry:
				return 2;
			case FormationClass.Ranged:
				return 5;
			case FormationClass.Cavalry:
				return 4;
			case FormationClass.HorseArcher:
				return 6;
			case FormationClass.NumberOfDefaultFormations:
				return 0;
			case FormationClass.HeavyInfantry:
				return 1;
			case FormationClass.LightCavalry:
				return 7;
			case FormationClass.HeavyCavalry:
				return 3;
			case FormationClass.NumberOfRegularFormations:
				return 9;
			case FormationClass.Bodyguard:
				return 8;
			default:
				return 10;
			}
		}

		// Token: 0x0400092D RID: 2349
		private static FormationDeploymentOrder.DeploymentOrderComparer _comparer;

		// Token: 0x0200052A RID: 1322
		public class DeploymentOrderComparer : IComparer<FormationDeploymentOrder>
		{
			// Token: 0x06003998 RID: 14744 RVA: 0x000E8D16 File Offset: 0x000E6F16
			public int Compare(FormationDeploymentOrder a, FormationDeploymentOrder b)
			{
				return a.Key - b.Key;
			}
		}
	}
}
