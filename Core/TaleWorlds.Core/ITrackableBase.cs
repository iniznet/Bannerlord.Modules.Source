using System;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.Core
{
	// Token: 0x02000091 RID: 145
	public interface ITrackableBase
	{
		// Token: 0x060007D0 RID: 2000
		TextObject GetName();

		// Token: 0x060007D1 RID: 2001
		Vec3 GetPosition();

		// Token: 0x060007D2 RID: 2002
		float GetTrackDistanceToMainAgent();

		// Token: 0x060007D3 RID: 2003
		bool CheckTracked(BasicCharacterObject basicCharacter);
	}
}
