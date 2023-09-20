using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.ViewModelCollection.Inquiries;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.MountAndBlade.GauntletUI
{
	// Token: 0x0200000E RID: 14
	public class GauntletQueryManager : GlobalLayer
	{
		// Token: 0x06000056 RID: 86 RVA: 0x00003E98 File Offset: 0x00002098
		public void Initialize()
		{
			if (!this._isInitialized)
			{
				this._isInitialized = true;
				this._inquiryQueue = new Queue<Tuple<Type, object, bool, bool>>();
				InformationManager.OnShowInquiry += this.CreateQuery;
				InformationManager.OnShowTextInquiry += this.CreateTextQuery;
				MBInformationManager.OnShowMultiSelectionInquiry += this.CreateMultiSelectionQuery;
				InformationManager.OnHideInquiry += this.CloseQuery;
				InformationManager.IsAnyInquiryActive = (Func<bool>)Delegate.Combine(InformationManager.IsAnyInquiryActive, new Func<bool>(this.OnIsAnyInquiryActiveQuery));
				this._singleQueryPopupVM = new SingleQueryPopUpVM(new Action(this.CloseQuery));
				this._multiSelectionQueryPopUpVM = new MultiSelectionQueryPopUpVM(new Action(this.CloseQuery));
				this._textQueryPopUpVM = new TextQueryPopUpVM(new Action(this.CloseQuery));
				this._gauntletLayer = new GauntletLayer(4500, "GauntletLayer", false);
				this._createQueryActions = new Dictionary<Type, Action<object, bool, bool>>
				{
					{
						typeof(InquiryData),
						delegate(object data, bool pauseState, bool prioritize)
						{
							this.CreateQuery((InquiryData)data, pauseState, prioritize);
						}
					},
					{
						typeof(TextInquiryData),
						delegate(object data, bool pauseState, bool prioritize)
						{
							this.CreateTextQuery((TextInquiryData)data, pauseState, prioritize);
						}
					},
					{
						typeof(MultiSelectionInquiryData),
						delegate(object data, bool pauseState, bool prioritize)
						{
							this.CreateMultiSelectionQuery((MultiSelectionInquiryData)data, pauseState, prioritize);
						}
					}
				};
				base.Layer = this._gauntletLayer;
				this._gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
				ScreenManager.AddGlobalLayer(this, true);
			}
			ScreenManager.SetSuspendLayer(base.Layer, true);
		}

		// Token: 0x06000057 RID: 87 RVA: 0x00004019 File Offset: 0x00002219
		private bool OnIsAnyInquiryActiveQuery()
		{
			return GauntletQueryManager._activeDataSource != null;
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00004024 File Offset: 0x00002224
		internal void InitializeKeyVisuals()
		{
			this._singleQueryPopupVM.SetCancelInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Exit"));
			this._singleQueryPopupVM.SetDoneInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Confirm"));
			this._multiSelectionQueryPopUpVM.SetCancelInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Exit"));
			this._multiSelectionQueryPopUpVM.SetDoneInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Confirm"));
			this._textQueryPopUpVM.SetCancelInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Exit"));
			this._textQueryPopUpVM.SetDoneInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Confirm"));
		}

		// Token: 0x06000059 RID: 89 RVA: 0x000040EC File Offset: 0x000022EC
		protected override void OnEarlyTick(float dt)
		{
			base.OnEarlyTick(dt);
			if (GauntletQueryManager._activeDataSource != null)
			{
				if (ScreenManager.FocusedLayer != base.Layer)
				{
					this.SetLayerFocus(true);
				}
				if (GauntletQueryManager._activeDataSource.IsButtonOkShown && GauntletQueryManager._activeDataSource.IsButtonOkEnabled && this._gauntletLayer.Input.IsHotKeyDownAndReleased("Confirm"))
				{
					GauntletQueryManager._activeDataSource.ExecuteAffirmativeAction();
					return;
				}
				if (GauntletQueryManager._activeDataSource.IsButtonCancelShown && this._gauntletLayer.Input.IsHotKeyDownAndReleased("Exit"))
				{
					GauntletQueryManager._activeDataSource.ExecuteNegativeAction();
				}
			}
		}

		// Token: 0x0600005A RID: 90 RVA: 0x00004182 File Offset: 0x00002382
		protected override void OnLateTick(float dt)
		{
			base.OnLateTick(dt);
			PopUpBaseVM activeDataSource = GauntletQueryManager._activeDataSource;
			if (activeDataSource == null)
			{
				return;
			}
			activeDataSource.OnTick(dt);
		}

		// Token: 0x0600005B RID: 91 RVA: 0x0000419C File Offset: 0x0000239C
		private void CreateQuery(InquiryData data, bool pauseGameActiveState, bool prioritize)
		{
			if (GauntletQueryManager._activeDataSource == null)
			{
				this._singleQueryPopupVM.SetData(data);
				this._movie = this._gauntletLayer.LoadMovie("SingleQueryPopup", this._singleQueryPopupVM);
				GauntletQueryManager._activeDataSource = this._singleQueryPopupVM;
				GauntletQueryManager._activeQueryData = data;
				this.HandleQueryCreated(data.SoundEventPath, pauseGameActiveState);
				return;
			}
			if (prioritize)
			{
				this.QueueAndShowNewData(data, pauseGameActiveState, prioritize);
				return;
			}
			this._inquiryQueue.Enqueue(new Tuple<Type, object, bool, bool>(typeof(InquiryData), data, pauseGameActiveState, prioritize));
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00004224 File Offset: 0x00002424
		private void CreateTextQuery(TextInquiryData data, bool pauseGameActiveState, bool prioritize)
		{
			if (GauntletQueryManager._activeDataSource == null)
			{
				this._textQueryPopUpVM.SetData(data);
				this._movie = this._gauntletLayer.LoadMovie("TextQueryPopup", this._textQueryPopUpVM);
				GauntletQueryManager._activeDataSource = this._textQueryPopUpVM;
				GauntletQueryManager._activeQueryData = data;
				this.HandleQueryCreated(data.SoundEventPath, pauseGameActiveState);
				return;
			}
			if (prioritize)
			{
				this.QueueAndShowNewData(data, pauseGameActiveState, prioritize);
				return;
			}
			this._inquiryQueue.Enqueue(new Tuple<Type, object, bool, bool>(typeof(TextInquiryData), data, pauseGameActiveState, prioritize));
		}

		// Token: 0x0600005D RID: 93 RVA: 0x000042AC File Offset: 0x000024AC
		private void CreateMultiSelectionQuery(MultiSelectionInquiryData data, bool pauseGameActiveState, bool prioritize)
		{
			if (GauntletQueryManager._activeDataSource == null)
			{
				this._multiSelectionQueryPopUpVM.SetData(data);
				this._movie = this._gauntletLayer.LoadMovie("MultiSelectionQueryPopup", this._multiSelectionQueryPopUpVM);
				GauntletQueryManager._activeDataSource = this._multiSelectionQueryPopUpVM;
				GauntletQueryManager._activeQueryData = data;
				this.HandleQueryCreated(data.SoundEventPath, pauseGameActiveState);
				return;
			}
			if (prioritize)
			{
				this.QueueAndShowNewData(data, pauseGameActiveState, prioritize);
				return;
			}
			this._inquiryQueue.Enqueue(new Tuple<Type, object, bool, bool>(typeof(MultiSelectionInquiryData), data, pauseGameActiveState, prioritize));
		}

		// Token: 0x0600005E RID: 94 RVA: 0x00004334 File Offset: 0x00002534
		private void QueueAndShowNewData(object newInquiryData, bool pauseGameActiveState, bool prioritize)
		{
			Queue<Tuple<Type, object, bool, bool>> queue = new Queue<Tuple<Type, object, bool, bool>>();
			queue.Enqueue(new Tuple<Type, object, bool, bool>(newInquiryData.GetType(), newInquiryData, pauseGameActiveState, prioritize));
			queue.Enqueue(new Tuple<Type, object, bool, bool>(GauntletQueryManager._activeQueryData.GetType(), GauntletQueryManager._activeQueryData, this._isLastActiveGameStatePaused, false));
			this._inquiryQueue = GauntletQueryManager.CombineQueues<Tuple<Type, object, bool, bool>>(queue, this._inquiryQueue);
			this.CloseQuery();
		}

		// Token: 0x0600005F RID: 95 RVA: 0x00004394 File Offset: 0x00002594
		private void HandleQueryCreated(string soundEventPath, bool pauseGameActiveState)
		{
			InformationManager.HideTooltip();
			GauntletQueryManager._activeDataSource.ForceRefreshKeyVisuals();
			this.SetLayerFocus(true);
			this._isLastActiveGameStatePaused = pauseGameActiveState;
			if (this._isLastActiveGameStatePaused)
			{
				GameStateManager.Current.RegisterActiveStateDisableRequest(this);
				MBCommon.PauseGameEngine();
			}
			if (!string.IsNullOrEmpty(soundEventPath))
			{
				SoundEvent.PlaySound2D(soundEventPath);
			}
		}

		// Token: 0x06000060 RID: 96 RVA: 0x000043E8 File Offset: 0x000025E8
		private void CloseQuery()
		{
			if (GauntletQueryManager._activeDataSource == null)
			{
				return;
			}
			this.SetLayerFocus(false);
			if (this._isLastActiveGameStatePaused)
			{
				GameStateManager.Current.UnregisterActiveStateDisableRequest(this);
				MBCommon.UnPauseGameEngine();
			}
			if (this._movie != null)
			{
				this._gauntletLayer.ReleaseMovie(this._movie);
				this._movie = null;
			}
			GauntletQueryManager._activeDataSource.OnClearData();
			GauntletQueryManager._activeDataSource = null;
			GauntletQueryManager._activeQueryData = null;
			if (this._inquiryQueue.Count > 0)
			{
				Tuple<Type, object, bool, bool> tuple = this._inquiryQueue.Dequeue();
				Action<object, bool, bool> action;
				if (this._createQueryActions.TryGetValue(tuple.Item1, out action))
				{
					action(tuple.Item2, tuple.Item3, tuple.Item4);
					return;
				}
				Debug.FailedAssert("Invalid data type for query", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.GauntletUI\\GauntletQueryManager.cs", "CloseQuery", 256);
			}
		}

		// Token: 0x06000061 RID: 97 RVA: 0x000044B4 File Offset: 0x000026B4
		private void SetLayerFocus(bool isFocused)
		{
			if (isFocused)
			{
				ScreenManager.SetSuspendLayer(base.Layer, false);
				base.Layer.IsFocusLayer = true;
				ScreenManager.TrySetFocus(base.Layer);
				base.Layer.InputRestrictions.SetInputRestrictions(true, 7);
				return;
			}
			base.Layer.InputRestrictions.ResetInputRestrictions();
			ScreenManager.SetSuspendLayer(base.Layer, true);
			base.Layer.IsFocusLayer = false;
			ScreenManager.TryLoseFocus(base.Layer);
		}

		// Token: 0x06000062 RID: 98 RVA: 0x00004530 File Offset: 0x00002730
		private static Queue<T> CombineQueues<T>(Queue<T> t1, Queue<T> t2)
		{
			Queue<T> queue = new Queue<T>();
			int num = t1.Count;
			for (int i = 0; i < num; i++)
			{
				queue.Enqueue(t1.Dequeue());
			}
			num = t2.Count;
			for (int j = 0; j < num; j++)
			{
				queue.Enqueue(t2.Dequeue());
			}
			return queue;
		}

		// Token: 0x04000038 RID: 56
		private bool _isInitialized;

		// Token: 0x04000039 RID: 57
		private Queue<Tuple<Type, object, bool, bool>> _inquiryQueue;

		// Token: 0x0400003A RID: 58
		private bool _isLastActiveGameStatePaused;

		// Token: 0x0400003B RID: 59
		private GauntletLayer _gauntletLayer;

		// Token: 0x0400003C RID: 60
		private SingleQueryPopUpVM _singleQueryPopupVM;

		// Token: 0x0400003D RID: 61
		private MultiSelectionQueryPopUpVM _multiSelectionQueryPopUpVM;

		// Token: 0x0400003E RID: 62
		private TextQueryPopUpVM _textQueryPopUpVM;

		// Token: 0x0400003F RID: 63
		private static PopUpBaseVM _activeDataSource;

		// Token: 0x04000040 RID: 64
		private static object _activeQueryData;

		// Token: 0x04000041 RID: 65
		private IGauntletMovie _movie;

		// Token: 0x04000042 RID: 66
		private Dictionary<Type, Action<object, bool, bool>> _createQueryActions;
	}
}
