using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200019D RID: 413
	[ScriptingInterfaceBase]
	internal interface IMBVoiceManager
	{
		// Token: 0x060016E4 RID: 5860
		[EngineMethod("get_voice_type_index", false)]
		int GetVoiceTypeIndex(string voiceType);

		// Token: 0x060016E5 RID: 5861
		[EngineMethod("get_voice_definition_count_with_monster_sound_and_collision_info_class_name", false)]
		int GetVoiceDefinitionCountWithMonsterSoundAndCollisionInfoClassName(string className);

		// Token: 0x060016E6 RID: 5862
		[EngineMethod("get_voice_definitions_with_monster_sound_and_collision_info_class_name", false)]
		void GetVoiceDefinitionListWithMonsterSoundAndCollisionInfoClassName(string className, int[] definitionIndices);
	}
}
