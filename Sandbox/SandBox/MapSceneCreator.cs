using System;
using TaleWorlds.CampaignSystem.Map;

namespace SandBox
{
	// Token: 0x02000009 RID: 9
	public class MapSceneCreator : IMapSceneCreator
	{
		// Token: 0x06000041 RID: 65 RVA: 0x000042A6 File Offset: 0x000024A6
		IMapScene IMapSceneCreator.CreateMapScene()
		{
			return new MapScene();
		}
	}
}
