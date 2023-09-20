using System;
using System.Collections.Generic;
using System.Linq;
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
	public class GauntletQueryManager : GlobalLayer
	{
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

		private bool OnIsAnyInquiryActiveQuery()
		{
			return GauntletQueryManager._activeDataSource != null;
		}

		internal void InitializeKeyVisuals()
		{
			this._singleQueryPopupVM.SetCancelInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Exit"));
			this._singleQueryPopupVM.SetDoneInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Confirm"));
			this._multiSelectionQueryPopUpVM.SetCancelInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Exit"));
			this._multiSelectionQueryPopUpVM.SetDoneInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Confirm"));
			this._textQueryPopUpVM.SetCancelInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Exit"));
			this._textQueryPopUpVM.SetDoneInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Confirm"));
		}

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
			if (data == null)
			{
				Debug.FailedAssert("Trying to create query with null data!", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.GauntletUI\\GauntletQueryManager.cs", "CreateQuery", 123);
				return;
			}
			if (GauntletQueryManager.CheckIfQueryDataIsEqual(GauntletQueryManager._activeQueryData, data) || this._inquiryQueue.Any((Tuple<Type, object, bool, bool> x) => GauntletQueryManager.CheckIfQueryDataIsEqual(x.Item2, data)))
			{
				Debug.FailedAssert("Trying to create query but it is already present! Title: " + data.TitleText, "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.GauntletUI\\GauntletQueryManager.cs", "CreateQuery", 128);
				return;
			}
			if (prioritize)
			{
				this.QueueAndShowNewData(data, pauseGameActiveState, prioritize);
				return;
			}
			this._inquiryQueue.Enqueue(new Tuple<Type, object, bool, bool>(typeof(InquiryData), data, pauseGameActiveState, prioritize));
		}

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
			if (data == null)
			{
				Debug.FailedAssert("Trying to create textQuery with null data!", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.GauntletUI\\GauntletQueryManager.cs", "CreateTextQuery", 158);
				return;
			}
			if (GauntletQueryManager.CheckIfQueryDataIsEqual(GauntletQueryManager._activeQueryData, data) || this._inquiryQueue.Any((Tuple<Type, object, bool, bool> x) => GauntletQueryManager.CheckIfQueryDataIsEqual(x.Item2, data)))
			{
				Debug.FailedAssert("Trying to create textQuery but it is already present! Title: " + data.TitleText, "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.GauntletUI\\GauntletQueryManager.cs", "CreateTextQuery", 163);
				return;
			}
			if (prioritize)
			{
				this.QueueAndShowNewData(data, pauseGameActiveState, prioritize);
				return;
			}
			this._inquiryQueue.Enqueue(new Tuple<Type, object, bool, bool>(typeof(TextInquiryData), data, pauseGameActiveState, prioritize));
		}

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
			if (data == null)
			{
				Debug.FailedAssert("Trying to create multiSelectionQuery with null data!", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.GauntletUI\\GauntletQueryManager.cs", "CreateMultiSelectionQuery", 193);
				return;
			}
			if (GauntletQueryManager.CheckIfQueryDataIsEqual(GauntletQueryManager._activeQueryData, data) || this._inquiryQueue.Any((Tuple<Type, object, bool, bool> x) => GauntletQueryManager.CheckIfQueryDataIsEqual(x.Item2, data)))
			{
				Debug.FailedAssert("Trying to create multiSelectionQuery but it is already present! Title: " + data.TitleText, "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.GauntletUI\\GauntletQueryManager.cs", "CreateMultiSelectionQuery", 198);
				return;
			}
			if (prioritize)
			{
				this.QueueAndShowNewData(data, pauseGameActiveState, prioritize);
				return;
			}
			this._inquiryQueue.Enqueue(new Tuple<Type, object, bool, bool>(typeof(MultiSelectionInquiryData), data, pauseGameActiveState, prioritize));
		}

		private void QueueAndShowNewData(object newInquiryData, bool pauseGameActiveState, bool prioritize)
		{
			Queue<Tuple<Type, object, bool, bool>> queue = new Queue<Tuple<Type, object, bool, bool>>();
			queue.Enqueue(new Tuple<Type, object, bool, bool>(newInquiryData.GetType(), newInquiryData, pauseGameActiveState, prioritize));
			queue.Enqueue(new Tuple<Type, object, bool, bool>(GauntletQueryManager._activeQueryData.GetType(), GauntletQueryManager._activeQueryData, this._isLastActiveGameStatePaused, false));
			this._inquiryQueue = GauntletQueryManager.CombineQueues<Tuple<Type, object, bool, bool>>(queue, this._inquiryQueue);
			this.CloseQuery();
		}

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
				Debug.FailedAssert("Invalid data type for query", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.GauntletUI\\GauntletQueryManager.cs", "CloseQuery", 290);
			}
		}

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

		private static bool CheckIfQueryDataIsEqual(object queryData1, object queryData2)
		{
			InquiryData inquiryData;
			if ((inquiryData = queryData1 as InquiryData) != null)
			{
				return inquiryData.HasSameContentWith(queryData2);
			}
			TextInquiryData textInquiryData;
			if ((textInquiryData = queryData1 as TextInquiryData) != null)
			{
				return textInquiryData.HasSameContentWith(queryData2);
			}
			MultiSelectionInquiryData multiSelectionInquiryData;
			return (multiSelectionInquiryData = queryData1 as MultiSelectionInquiryData) != null && multiSelectionInquiryData.HasSameContentWith(queryData2);
		}

		private bool _isInitialized;

		private Queue<Tuple<Type, object, bool, bool>> _inquiryQueue;

		private bool _isLastActiveGameStatePaused;

		private GauntletLayer _gauntletLayer;

		private SingleQueryPopUpVM _singleQueryPopupVM;

		private MultiSelectionQueryPopUpVM _multiSelectionQueryPopUpVM;

		private TextQueryPopUpVM _textQueryPopUpVM;

		private static PopUpBaseVM _activeDataSource;

		private static object _activeQueryData;

		private IGauntletMovie _movie;

		private Dictionary<Type, Action<object, bool, bool>> _createQueryActions;
	}
}
