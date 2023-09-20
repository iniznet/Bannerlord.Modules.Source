using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.CharacterCreationContent;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ViewModelCollection.EscapeMenu;
using TaleWorlds.ScreenSystem;

namespace SandBox.View.CharacterCreation
{
	// Token: 0x0200005D RID: 93
	public abstract class CharacterCreationStageViewBase : ICharacterCreationStageListener
	{
		// Token: 0x060003FD RID: 1021 RVA: 0x00022648 File Offset: 0x00020848
		protected CharacterCreationStageViewBase(ControlCharacterCreationStage affirmativeAction, ControlCharacterCreationStage negativeAction, ControlCharacterCreationStage refreshAction, ControlCharacterCreationStageReturnInt getCurrentStageIndexAction, ControlCharacterCreationStageReturnInt getTotalStageCountAction, ControlCharacterCreationStageReturnInt getFurthestIndexAction, ControlCharacterCreationStageWithInt goToIndexAction)
		{
			this._affirmativeAction = affirmativeAction;
			this._negativeAction = negativeAction;
			this._refreshAction = refreshAction;
			this._getTotalStageCountAction = getTotalStageCountAction;
			this._getCurrentStageIndexAction = getCurrentStageIndexAction;
			this._getFurthestIndexAction = getFurthestIndexAction;
			this._goToIndexAction = goToIndexAction;
		}

		// Token: 0x060003FE RID: 1022 RVA: 0x000226AF File Offset: 0x000208AF
		public virtual void SetGenericScene(Scene scene)
		{
		}

		// Token: 0x060003FF RID: 1023 RVA: 0x000226B1 File Offset: 0x000208B1
		protected virtual void OnRefresh()
		{
			this._refreshAction.Invoke();
		}

		// Token: 0x06000400 RID: 1024
		public abstract IEnumerable<ScreenLayer> GetLayers();

		// Token: 0x06000401 RID: 1025
		public abstract void NextStage();

		// Token: 0x06000402 RID: 1026
		public abstract void PreviousStage();

		// Token: 0x06000403 RID: 1027 RVA: 0x000226BE File Offset: 0x000208BE
		void ICharacterCreationStageListener.OnStageFinalize()
		{
			this.OnFinalize();
		}

		// Token: 0x06000404 RID: 1028 RVA: 0x000226C6 File Offset: 0x000208C6
		protected virtual void OnFinalize()
		{
		}

		// Token: 0x06000405 RID: 1029 RVA: 0x000226C8 File Offset: 0x000208C8
		public virtual void Tick(float dt)
		{
		}

		// Token: 0x06000406 RID: 1030
		public abstract int GetVirtualStageCount();

		// Token: 0x06000407 RID: 1031 RVA: 0x000226CA File Offset: 0x000208CA
		public virtual void GoToIndex(int index)
		{
			this._goToIndexAction.Invoke(index);
		}

		// Token: 0x06000408 RID: 1032
		public abstract void LoadEscapeMenuMovie();

		// Token: 0x06000409 RID: 1033
		public abstract void ReleaseEscapeMenuMovie();

		// Token: 0x0600040A RID: 1034 RVA: 0x000226D8 File Offset: 0x000208D8
		public void HandleEscapeMenu(CharacterCreationStageViewBase view, ScreenLayer screenLayer)
		{
			if (screenLayer.Input.IsHotKeyReleased("ToggleEscapeMenu"))
			{
				if (this._isEscapeOpen)
				{
					this.RemoveEscapeMenu(view);
					return;
				}
				this.OpenEscapeMenu(view);
			}
		}

		// Token: 0x0600040B RID: 1035 RVA: 0x00022703 File Offset: 0x00020903
		private void OpenEscapeMenu(CharacterCreationStageViewBase view)
		{
			view.LoadEscapeMenuMovie();
			this._isEscapeOpen = true;
		}

		// Token: 0x0600040C RID: 1036 RVA: 0x00022712 File Offset: 0x00020912
		private void RemoveEscapeMenu(CharacterCreationStageViewBase view)
		{
			view.ReleaseEscapeMenuMovie();
			this._isEscapeOpen = false;
		}

		// Token: 0x0600040D RID: 1037 RVA: 0x00022724 File Offset: 0x00020924
		public List<EscapeMenuItemVM> GetEscapeMenuItems(CharacterCreationStageViewBase view)
		{
			TextObject characterCreationDisabledReason = GameTexts.FindText("str_pause_menu_disabled_hint", "CharacterCreation");
			List<EscapeMenuItemVM> list = new List<EscapeMenuItemVM>();
			list.Add(new EscapeMenuItemVM(new TextObject("{=5Saniypu}Resume", null), delegate(object o)
			{
				this.RemoveEscapeMenu(view);
			}, null, () => new Tuple<bool, TextObject>(false, TextObject.Empty), true));
			list.Add(new EscapeMenuItemVM(new TextObject("{=PXT6aA4J}Campaign Options", null), delegate(object o)
			{
			}, null, () => new Tuple<bool, TextObject>(true, characterCreationDisabledReason), false));
			list.Add(new EscapeMenuItemVM(new TextObject("{=NqarFr4P}Options", null), delegate(object o)
			{
			}, null, () => new Tuple<bool, TextObject>(true, characterCreationDisabledReason), false));
			list.Add(new EscapeMenuItemVM(new TextObject("{=bV75iwKa}Save", null), delegate(object o)
			{
			}, null, () => new Tuple<bool, TextObject>(true, characterCreationDisabledReason), false));
			list.Add(new EscapeMenuItemVM(new TextObject("{=e0KdfaNe}Save As", null), delegate(object o)
			{
			}, null, () => new Tuple<bool, TextObject>(true, characterCreationDisabledReason), false));
			list.Add(new EscapeMenuItemVM(new TextObject("{=9NuttOBC}Load", null), delegate(object o)
			{
			}, null, () => new Tuple<bool, TextObject>(true, characterCreationDisabledReason), false));
			list.Add(new EscapeMenuItemVM(new TextObject("{=AbEh2y8o}Save And Exit", null), delegate(object o)
			{
			}, null, () => new Tuple<bool, TextObject>(true, characterCreationDisabledReason), false));
			list.Add(new EscapeMenuItemVM(new TextObject("{=RamV6yLM}Exit to Main Menu", null), delegate(object o)
			{
				this.RemoveEscapeMenu(view);
				view.OnFinalize();
				MBGameManager.EndGame();
			}, null, () => new Tuple<bool, TextObject>(false, TextObject.Empty), false));
			return list;
		}

		// Token: 0x04000223 RID: 547
		protected readonly ControlCharacterCreationStage _affirmativeAction;

		// Token: 0x04000224 RID: 548
		protected readonly ControlCharacterCreationStage _negativeAction;

		// Token: 0x04000225 RID: 549
		protected readonly ControlCharacterCreationStage _refreshAction;

		// Token: 0x04000226 RID: 550
		protected readonly ControlCharacterCreationStageReturnInt _getTotalStageCountAction;

		// Token: 0x04000227 RID: 551
		protected readonly ControlCharacterCreationStageReturnInt _getCurrentStageIndexAction;

		// Token: 0x04000228 RID: 552
		protected readonly ControlCharacterCreationStageReturnInt _getFurthestIndexAction;

		// Token: 0x04000229 RID: 553
		protected readonly ControlCharacterCreationStageWithInt _goToIndexAction;

		// Token: 0x0400022A RID: 554
		protected readonly Vec3 _cameraPosition = new Vec3(6.45f, 4.35f, 1.6f, -1f);

		// Token: 0x0400022B RID: 555
		private bool _isEscapeOpen;
	}
}
