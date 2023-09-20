﻿using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	[ScriptingInterfaceBase]
	internal interface IMBVoiceManager
	{
		[EngineMethod("get_voice_type_index", false)]
		int GetVoiceTypeIndex(string voiceType);

		[EngineMethod("get_voice_definition_count_with_monster_sound_and_collision_info_class_name", false)]
		int GetVoiceDefinitionCountWithMonsterSoundAndCollisionInfoClassName(string className);

		[EngineMethod("get_voice_definitions_with_monster_sound_and_collision_info_class_name", false)]
		void GetVoiceDefinitionListWithMonsterSoundAndCollisionInfoClassName(string className, int[] definitionIndices);
	}
}
