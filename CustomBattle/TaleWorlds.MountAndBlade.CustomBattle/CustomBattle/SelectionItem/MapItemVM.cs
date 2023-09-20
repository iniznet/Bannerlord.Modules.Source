using System;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.CustomBattle.CustomBattle.SelectionItem
{
	public class MapItemVM : SelectorItemVM
	{
		public string MapName { get; private set; }

		public string MapId { get; private set; }

		public MapItemVM(string mapName, string mapId)
			: base(mapName)
		{
			this.MapName = mapName;
			this.MapId = mapId;
			this.NameText = mapName;
		}

		public void UpdateSearchedText(string searchedText)
		{
			this._searchedText = searchedText;
			string text = null;
			if (this.MapName.IndexOf(this._searchedText, StringComparison.OrdinalIgnoreCase) != -1)
			{
				text = this.MapName.Substring(this.MapName.IndexOf(this._searchedText, StringComparison.OrdinalIgnoreCase), this._searchedText.Length);
			}
			if (!string.IsNullOrEmpty(text))
			{
				this.NameText = this.MapName.Replace(text, "<a>" + text + "</a>");
				return;
			}
			this.NameText = this.MapName;
		}

		[DataSourceProperty]
		public string NameText
		{
			get
			{
				return this._nameText;
			}
			set
			{
				if (this._nameText != value)
				{
					this._nameText = value;
					base.OnPropertyChangedWithValue<string>(value, "NameText");
				}
			}
		}

		private string _searchedText;

		public string _nameText;
	}
}
