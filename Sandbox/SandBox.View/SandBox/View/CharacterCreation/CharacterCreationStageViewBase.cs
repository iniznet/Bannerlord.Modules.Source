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
	public abstract class CharacterCreationStageViewBase : ICharacterCreationStageListener
	{
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

		public virtual void SetGenericScene(Scene scene)
		{
		}

		protected virtual void OnRefresh()
		{
			this._refreshAction.Invoke();
		}

		public abstract IEnumerable<ScreenLayer> GetLayers();

		public abstract void NextStage();

		public abstract void PreviousStage();

		void ICharacterCreationStageListener.OnStageFinalize()
		{
			this.OnFinalize();
		}

		protected virtual void OnFinalize()
		{
		}

		public virtual void Tick(float dt)
		{
		}

		public abstract int GetVirtualStageCount();

		public virtual void GoToIndex(int index)
		{
			this._goToIndexAction.Invoke(index);
		}

		public abstract void LoadEscapeMenuMovie();

		public abstract void ReleaseEscapeMenuMovie();

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

		private void OpenEscapeMenu(CharacterCreationStageViewBase view)
		{
			view.LoadEscapeMenuMovie();
			this._isEscapeOpen = true;
		}

		private void RemoveEscapeMenu(CharacterCreationStageViewBase view)
		{
			view.ReleaseEscapeMenuMovie();
			this._isEscapeOpen = false;
		}

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

		protected readonly ControlCharacterCreationStage _affirmativeAction;

		protected readonly ControlCharacterCreationStage _negativeAction;

		protected readonly ControlCharacterCreationStage _refreshAction;

		protected readonly ControlCharacterCreationStageReturnInt _getTotalStageCountAction;

		protected readonly ControlCharacterCreationStageReturnInt _getCurrentStageIndexAction;

		protected readonly ControlCharacterCreationStageReturnInt _getFurthestIndexAction;

		protected readonly ControlCharacterCreationStageWithInt _goToIndexAction;

		protected readonly Vec3 _cameraPosition = new Vec3(6.45f, 4.35f, 1.6f, -1f);

		private bool _isEscapeOpen;
	}
}
