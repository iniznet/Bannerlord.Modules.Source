using System;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.CampaignSystem
{
	public struct MeetingSceneData
	{
		public string SceneID { get; private set; }

		public string CultureString { get; private set; }

		public CultureObject Culture
		{
			get
			{
				return MBObjectManager.Instance.GetObject<CultureObject>(this.CultureString);
			}
		}

		public MeetingSceneData(string sceneID, string cultureString)
		{
			this.SceneID = sceneID;
			this.CultureString = cultureString;
		}
	}
}
