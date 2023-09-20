using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200034D RID: 845
	[Obsolete]
	public class DestructableMissionObject : MissionObject
	{
		// Token: 0x06002D89 RID: 11657 RVA: 0x000B2C9A File Offset: 0x000B0E9A
		protected internal override void OnEditorInit()
		{
			Debug.FailedAssert("This scene is using old prefabs with the old destruction system, please update!", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Objects\\Siege\\DestructableMissionObject.cs", "OnEditorInit", 18);
		}

		// Token: 0x06002D8A RID: 11658 RVA: 0x000B2CB2 File Offset: 0x000B0EB2
		protected internal override void OnInit()
		{
			Debug.FailedAssert("This scene is using old prefabs with the old destruction system, please update! The game will now close!", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Objects\\Siege\\DestructableMissionObject.cs", "OnInit", 23);
			Environment.Exit(0);
		}
	}
}
