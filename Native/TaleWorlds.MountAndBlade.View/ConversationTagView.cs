using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade.View
{
	// Token: 0x0200000A RID: 10
	public class ConversationTagView
	{
		// Token: 0x06000044 RID: 68 RVA: 0x000039C5 File Offset: 0x00001BC5
		public static string GetSkillMeshName(SkillObject skillEnum, bool isOn = false)
		{
			if (isOn)
			{
				return "skill_icon_" + skillEnum.StringId.ToLower() + "_on";
			}
			return "skill_icon_" + skillEnum.StringId.ToLower() + "_off";
		}
	}
}
