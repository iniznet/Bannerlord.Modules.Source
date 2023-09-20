using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.GauntletUI
{
	internal class WidgetContainer
	{
		internal int Count
		{
			get
			{
				return this._widgetLists[this._currentBufferIndex].Count;
			}
		}

		internal int RealCount
		{
			get
			{
				return this._widgetLists[this._currentBufferIndex].Count - this._emptyCount;
			}
		}

		internal WidgetContainer(UIContext context, int initialCapacity, WidgetContainer.ContainerType type)
		{
			this._emptyWidget = new EmptyWidget(context);
			this._currentBufferIndex = 0;
			this._widgetLists = new List<Widget>[]
			{
				new List<Widget>(initialCapacity),
				new List<Widget>(initialCapacity)
			};
			this._containerType = type;
			this._emptyCount = 0;
		}

		internal List<Widget> GetCurrentList()
		{
			return this._widgetLists[this._currentBufferIndex];
		}

		internal Widget this[int index]
		{
			get
			{
				return this._widgetLists[this._currentBufferIndex][index];
			}
			set
			{
				this._widgetLists[this._currentBufferIndex][index] = value;
			}
		}

		internal int Add(Widget widget)
		{
			this._widgetLists[this._currentBufferIndex].Add(widget);
			return this._widgetLists[this._currentBufferIndex].Count - 1;
		}

		internal void Remove(Widget widget)
		{
			int num = this._widgetLists[this._currentBufferIndex].IndexOf(widget);
			this._widgetLists[this._currentBufferIndex][num] = this._emptyWidget;
			this._emptyCount++;
		}

		internal void RemoveFromIndex(int index)
		{
			this._widgetLists[this._currentBufferIndex][index] = this._emptyWidget;
			this._emptyCount++;
		}

		internal bool CheckFragmentation()
		{
			int count = this._widgetLists[this._currentBufferIndex].Count;
			return count > 32 && (int)((float)count * 0.1f) < this._emptyCount;
		}

		internal void DoDefragmentation()
		{
			int count = this._widgetLists[this._currentBufferIndex].Count;
			int num = (this._currentBufferIndex + 1) % 2;
			List<Widget> list = this._widgetLists[this._currentBufferIndex];
			List<Widget> list2 = this._widgetLists[num];
			int num2 = 0;
			for (int i = 0; i < count; i++)
			{
				Widget widget = list[i];
				if (widget != this._emptyWidget)
				{
					switch (this._containerType)
					{
					case WidgetContainer.ContainerType.Update:
						widget.OnUpdateListIndex -= num2;
						break;
					case WidgetContainer.ContainerType.ParallelUpdate:
						widget.OnParallelUpdateListIndex -= num2;
						break;
					case WidgetContainer.ContainerType.LateUpdate:
						widget.OnLateUpdateListIndex -= num2;
						break;
					case WidgetContainer.ContainerType.VisualDefinition:
						widget.OnVisualDefinitionListIndex -= num2;
						break;
					case WidgetContainer.ContainerType.TweenPosition:
						widget.OnTweenPositionListIndex -= num2;
						break;
					case WidgetContainer.ContainerType.UpdateBrushes:
						widget.OnUpdateBrushesIndex -= num2;
						break;
					}
					list2.Add(widget);
				}
				else
				{
					num2++;
				}
			}
			list.Clear();
			this._emptyCount = 0;
			this._currentBufferIndex = num;
		}

		private int _currentBufferIndex;

		private List<Widget>[] _widgetLists;

		private EmptyWidget _emptyWidget;

		private int _emptyCount;

		private WidgetContainer.ContainerType _containerType;

		internal enum ContainerType
		{
			None,
			Update,
			ParallelUpdate,
			LateUpdate,
			VisualDefinition,
			TweenPosition,
			UpdateBrushes
		}
	}
}
