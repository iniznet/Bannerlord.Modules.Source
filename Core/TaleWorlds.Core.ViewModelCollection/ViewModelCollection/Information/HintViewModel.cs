using System;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.Core.ViewModelCollection.Information
{
	// Token: 0x02000016 RID: 22
	public class HintViewModel : ViewModel
	{
		// Token: 0x06000100 RID: 256 RVA: 0x00003F04 File Offset: 0x00002104
		public HintViewModel()
		{
			this.HintText = TextObject.Empty;
		}

		// Token: 0x06000101 RID: 257 RVA: 0x00003F17 File Offset: 0x00002117
		public HintViewModel(TextObject hintText, string uniqueName = null)
		{
			this.HintText = hintText;
			this._uniqueName = uniqueName;
		}

		// Token: 0x06000102 RID: 258 RVA: 0x00003F2D File Offset: 0x0000212D
		public void ExecuteBeginHint()
		{
			if (!TextObject.IsNullOrEmpty(this.HintText))
			{
				MBInformationManager.ShowHint(this.HintText.ToString());
			}
		}

		// Token: 0x06000103 RID: 259 RVA: 0x00003F4C File Offset: 0x0000214C
		public void ExecuteEndHint()
		{
			MBInformationManager.HideInformations();
		}

		// Token: 0x04000065 RID: 101
		public TextObject HintText;

		// Token: 0x04000066 RID: 102
		private readonly string _uniqueName;
	}
}
