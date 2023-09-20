using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020002D6 RID: 726
	public class MonsterMissionDataCreator : IMonsterMissionDataCreator
	{
		// Token: 0x06002813 RID: 10259 RVA: 0x0009B4D7 File Offset: 0x000996D7
		IMonsterMissionData IMonsterMissionDataCreator.CreateMonsterMissionData(Monster monster)
		{
			return new MonsterMissionData(monster);
		}
	}
}
