using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.GauntletUI
{
	// Token: 0x02000033 RID: 51
	internal class WidgetContainer
	{
		// Token: 0x17000116 RID: 278
		// (get) Token: 0x06000360 RID: 864 RVA: 0x0000EA65 File Offset: 0x0000CC65
		internal int Count
		{
			get
			{
				return this._widgetLists[this._currentBufferIndex].Count;
			}
		}

		// Token: 0x17000117 RID: 279
		// (get) Token: 0x06000361 RID: 865 RVA: 0x0000EA79 File Offset: 0x0000CC79
		internal int RealCount
		{
			get
			{
				return this._widgetLists[this._currentBufferIndex].Count - this._emptyCount;
			}
		}

		// Token: 0x06000362 RID: 866 RVA: 0x0000EA94 File Offset: 0x0000CC94
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

		// Token: 0x06000363 RID: 867 RVA: 0x0000EAE6 File Offset: 0x0000CCE6
		internal List<Widget> GetCurrentList()
		{
			return this._widgetLists[this._currentBufferIndex];
		}

		// Token: 0x17000118 RID: 280
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

		// Token: 0x06000366 RID: 870 RVA: 0x0000EB20 File Offset: 0x0000CD20
		internal int Add(Widget widget)
		{
			this._widgetLists[this._currentBufferIndex].Add(widget);
			return this._widgetLists[this._currentBufferIndex].Count - 1;
		}

		// Token: 0x06000367 RID: 871 RVA: 0x0000EB4C File Offset: 0x0000CD4C
		internal void Remove(Widget widget)
		{
			int num = this._widgetLists[this._currentBufferIndex].IndexOf(widget);
			this._widgetLists[this._currentBufferIndex][num] = this._emptyWidget;
			this._emptyCount++;
		}

		// Token: 0x06000368 RID: 872 RVA: 0x0000EB94 File Offset: 0x0000CD94
		internal void RemoveFromIndex(int index)
		{
			this._widgetLists[this._currentBufferIndex][index] = this._emptyWidget;
			this._emptyCount++;
		}

		// Token: 0x06000369 RID: 873 RVA: 0x0000EBC0 File Offset: 0x0000CDC0
		internal bool CheckFragmentation()
		{
			int count = this._widgetLists[this._currentBufferIndex].Count;
			return count > 32 && (int)((float)count * 0.1f) < this._emptyCount;
		}

		// Token: 0x0600036A RID: 874 RVA: 0x0000EBFC File Offset: 0x0000CDFC
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

		// Token: 0x040001BC RID: 444
		private int _currentBufferIndex;

		// Token: 0x040001BD RID: 445
		private List<Widget>[] _widgetLists;

		// Token: 0x040001BE RID: 446
		private EmptyWidget _emptyWidget;

		// Token: 0x040001BF RID: 447
		private int _emptyCount;

		// Token: 0x040001C0 RID: 448
		private WidgetContainer.ContainerType _containerType;

		// Token: 0x0200007C RID: 124
		internal enum ContainerType
		{
			// Token: 0x0400042E RID: 1070
			None,
			// Token: 0x0400042F RID: 1071
			Update,
			// Token: 0x04000430 RID: 1072
			ParallelUpdate,
			// Token: 0x04000431 RID: 1073
			LateUpdate,
			// Token: 0x04000432 RID: 1074
			VisualDefinition,
			// Token: 0x04000433 RID: 1075
			TweenPosition,
			// Token: 0x04000434 RID: 1076
			UpdateBrushes
		}
	}
}
