using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200013D RID: 317
	// (Invoke) Token: 0x06000FE6 RID: 4070
	public delegate void OnOrderIssuedDelegate(OrderType orderType, MBReadOnlyList<Formation> appliedFormations, params object[] delegateParams);
}
