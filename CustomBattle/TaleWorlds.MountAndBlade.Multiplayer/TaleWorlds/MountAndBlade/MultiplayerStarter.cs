using System;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade
{
	public class MultiplayerStarter
	{
		public MultiplayerStarter(MBObjectManager objectManager)
		{
			this._objectManager = objectManager;
		}

		public void LoadXMLFromFile(string xmlPath, string xsdPath)
		{
			this._objectManager.LoadOneXmlFromFile(xmlPath, xsdPath, false);
		}

		public void ClearEmptyObjects()
		{
			this._objectManager.UnregisterNonReadyObjects();
		}

		private readonly MBObjectManager _objectManager;
	}
}
