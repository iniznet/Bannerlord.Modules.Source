using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.CampaignSystem.Encyclopedia
{
	public abstract class EncyclopediaPage
	{
		protected abstract IEnumerable<EncyclopediaListItem> InitializeListItems();

		protected abstract IEnumerable<EncyclopediaFilterGroup> InitializeFilterItems();

		protected abstract IEnumerable<EncyclopediaSortController> InitializeSortControllers();

		public int HomePageOrderIndex { get; protected set; }

		public EncyclopediaPage Parent { get; }

		public EncyclopediaPage()
		{
			this._filters = this.InitializeFilterItems();
			this._items = this.InitializeListItems();
			this._sortControllers = new List<EncyclopediaSortController>
			{
				new EncyclopediaSortController(new TextObject("{=koX9okuG}None", null), new EncyclopediaListItemNameComparer())
			};
			((List<EncyclopediaSortController>)this._sortControllers).AddRange(this.InitializeSortControllers());
			foreach (object obj in base.GetType().GetCustomAttributes(typeof(EncyclopediaModel), true))
			{
				if (obj is EncyclopediaModel)
				{
					this._identifierTypes = (obj as EncyclopediaModel).PageTargetTypes;
					break;
				}
			}
			this._identifiers = new Dictionary<Type, string>();
			foreach (Type type in this._identifierTypes)
			{
				if (Game.Current.ObjectManager.HasType(type))
				{
					this._identifiers.Add(type, Game.Current.ObjectManager.FindRegisteredClassPrefix(type));
				}
				else
				{
					string text = type.Name.ToString();
					if (text == "Clan")
					{
						text = "Faction";
					}
					this._identifiers.Add(type, text);
				}
			}
		}

		public bool HasIdentifierType(Type identifierType)
		{
			return this._identifierTypes.Contains(identifierType);
		}

		internal bool HasIdentifier(string identifier)
		{
			return this._identifiers.ContainsValue(identifier);
		}

		public string GetIdentifier(Type identifierType)
		{
			if (this._identifiers.ContainsKey(identifierType))
			{
				return this._identifiers[identifierType];
			}
			return "";
		}

		public string[] GetIdentifierNames()
		{
			return this._identifiers.Values.ToArray<string>();
		}

		public bool IsFiltered(object o)
		{
			using (IEnumerator<EncyclopediaFilterGroup> enumerator = this.GetFilterItems().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!enumerator.Current.Predicate(o))
					{
						return true;
					}
				}
			}
			return false;
		}

		public virtual string GetViewFullyQualifiedName()
		{
			return "";
		}

		public virtual string GetStringID()
		{
			return "";
		}

		public virtual TextObject GetName()
		{
			return TextObject.Empty;
		}

		public virtual MBObjectBase GetObject(string typeName, string stringID)
		{
			return MBObjectManager.Instance.GetObject(typeName, stringID);
		}

		public virtual bool IsValidEncyclopediaItem(object o)
		{
			return false;
		}

		public virtual TextObject GetDescriptionText()
		{
			return TextObject.Empty;
		}

		public IEnumerable<EncyclopediaListItem> GetListItems()
		{
			return this._items;
		}

		public IEnumerable<EncyclopediaFilterGroup> GetFilterItems()
		{
			return this._filters;
		}

		public IEnumerable<EncyclopediaSortController> GetSortControllers()
		{
			return this._sortControllers;
		}

		private readonly Type[] _identifierTypes;

		private readonly Dictionary<Type, string> _identifiers;

		private IEnumerable<EncyclopediaFilterGroup> _filters;

		private IEnumerable<EncyclopediaListItem> _items;

		private IEnumerable<EncyclopediaSortController> _sortControllers;
	}
}
