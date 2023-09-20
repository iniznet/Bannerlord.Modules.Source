using System;

namespace SandBox.View.CharacterCreation
{
	public sealed class CharacterCreationStageViewAttribute : Attribute
	{
		public CharacterCreationStageViewAttribute(Type stageType)
		{
			this.StageType = stageType;
		}

		public readonly Type StageType;
	}
}
