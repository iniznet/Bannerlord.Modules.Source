using System;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.CustomBattle
{
	public class SelectionGroup : ViewModel
	{
		public SelectionGroup(string name, List<string> textList = null)
		{
			this._name = name;
			if (textList != null)
			{
				this._textList = textList;
			}
			this.Text = ((this._textList.Count > 0) ? this._textList[0] : "");
		}

		protected virtual void ClickSelectionLeft()
		{
			this._index--;
			if (this._index < 0)
			{
				this._index = this._textList.Count - 1;
			}
			this.Text = ((this._textList.Count > 0) ? this._textList[this._index] : "");
		}

		protected virtual void ClickSelectionRight()
		{
			this._index++;
			this._index %= this._textList.Count;
			this.Text = ((this._textList.Count > 0) ? this._textList[this._index] : "");
		}

		public string Text
		{
			get
			{
				return this._text;
			}
			set
			{
				if (value != this._text)
				{
					this._text = value;
					base.OnPropertyChangedWithValue<string>(value, "Text");
				}
			}
		}

		public List<string> TextList
		{
			get
			{
				return this._textList;
			}
			set
			{
				if (value != this._textList)
				{
					this._textList = value;
					this.Text = ((this._textList.Count > 0) ? this._textList[this._index] : "");
				}
			}
		}

		public int Index
		{
			get
			{
				return this._index;
			}
			private set
			{
				value = this._index;
			}
		}

		protected List<string> _textList = new List<string>();

		private int _index;

		private string _name;

		private string _text;
	}
}
