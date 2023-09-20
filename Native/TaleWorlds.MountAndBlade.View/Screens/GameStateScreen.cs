using System;

namespace TaleWorlds.MountAndBlade.View.Screens
{
	// Token: 0x0200002D RID: 45
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class GameStateScreen : Attribute
	{
		// Token: 0x17000020 RID: 32
		// (get) Token: 0x060001BC RID: 444 RVA: 0x0000F034 File Offset: 0x0000D234
		// (set) Token: 0x060001BD RID: 445 RVA: 0x0000F03C File Offset: 0x0000D23C
		public Type GameStateType { get; private set; }

		// Token: 0x060001BE RID: 446 RVA: 0x0000F045 File Offset: 0x0000D245
		public GameStateScreen(Type gameStateType)
		{
			this.GameStateType = gameStateType;
		}
	}
}
