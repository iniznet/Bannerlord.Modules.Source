using System;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Profile
{
	// Token: 0x0200006B RID: 107
	public class MPLobbyProfileGameModeSelectorItemVM : SelectorItemVM
	{
		// Token: 0x17000308 RID: 776
		// (get) Token: 0x060009D8 RID: 2520 RVA: 0x00024152 File Offset: 0x00022352
		// (set) Token: 0x060009D9 RID: 2521 RVA: 0x0002415A File Offset: 0x0002235A
		public string GameModeCode { get; private set; }

		// Token: 0x060009DA RID: 2522 RVA: 0x00024163 File Offset: 0x00022363
		public MPLobbyProfileGameModeSelectorItemVM(string gameModeCode, TextObject gameModeName)
			: base(gameModeName)
		{
			this.GameModeCode = gameModeCode;
		}
	}
}
