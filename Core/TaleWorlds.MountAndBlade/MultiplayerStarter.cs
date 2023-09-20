using System;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200030E RID: 782
	public class MultiplayerStarter
	{
		// Token: 0x06002A5A RID: 10842 RVA: 0x000A4AC1 File Offset: 0x000A2CC1
		public MultiplayerStarter(MBObjectManager objectManager)
		{
			this._objectManager = objectManager;
		}

		// Token: 0x06002A5B RID: 10843 RVA: 0x000A4AD0 File Offset: 0x000A2CD0
		public void LoadXMLFromFile(string xmlPath, string xsdPath)
		{
			this._objectManager.LoadOneXmlFromFile(xmlPath, xsdPath, false);
		}

		// Token: 0x06002A5C RID: 10844 RVA: 0x000A4AE0 File Offset: 0x000A2CE0
		public void ClearEmptyObjects()
		{
			this._objectManager.UnregisterNonReadyObjects();
		}

		// Token: 0x0400103E RID: 4158
		private readonly MBObjectManager _objectManager;
	}
}
