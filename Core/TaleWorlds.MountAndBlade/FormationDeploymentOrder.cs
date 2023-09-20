using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public struct FormationDeploymentOrder
	{
		public int Key { get; private set; }

		public int Offset { get; private set; }

		private FormationDeploymentOrder(FormationClass formationClass, int offset = 0)
		{
			int formationClassPriority = FormationDeploymentOrder.GetFormationClassPriority(formationClass);
			this.Offset = MathF.Max(0, offset);
			this.Key = formationClassPriority + this.Offset * 11;
		}

		public static FormationDeploymentOrder GetDeploymentOrder(FormationClass fClass, int offset = 0)
		{
			return new FormationDeploymentOrder(fClass, offset);
		}

		public static FormationDeploymentOrder.DeploymentOrderComparer GetComparer()
		{
			FormationDeploymentOrder.DeploymentOrderComparer deploymentOrderComparer;
			if ((deploymentOrderComparer = FormationDeploymentOrder._comparer) == null)
			{
				deploymentOrderComparer = (FormationDeploymentOrder._comparer = new FormationDeploymentOrder.DeploymentOrderComparer());
			}
			return deploymentOrderComparer;
		}

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

		private static FormationDeploymentOrder.DeploymentOrderComparer _comparer;

		public class DeploymentOrderComparer : IComparer<FormationDeploymentOrder>
		{
			public int Compare(FormationDeploymentOrder a, FormationDeploymentOrder b)
			{
				return a.Key - b.Key;
			}
		}
	}
}
