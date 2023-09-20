using System;

namespace SandBox.View.CharacterCreation
{
	// Token: 0x0200005C RID: 92
	public sealed class CharacterCreationStageViewAttribute : Attribute
	{
		// Token: 0x060003FC RID: 1020 RVA: 0x00022639 File Offset: 0x00020839
		public CharacterCreationStageViewAttribute(Type stageType)
		{
			this.StageType = stageType;
		}

		// Token: 0x04000222 RID: 546
		public readonly Type StageType;
	}
}
