using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public delegate void OnOrderIssuedDelegate(OrderType orderType, MBReadOnlyList<Formation> appliedFormations, params object[] delegateParams);
}
