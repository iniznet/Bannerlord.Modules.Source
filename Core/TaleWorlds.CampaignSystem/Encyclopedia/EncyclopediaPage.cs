using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.CampaignSystem.Encyclopedia
{
	// Token: 0x0200015E RID: 350
	public abstract class EncyclopediaPage
	{
		// Token: 0x0600185E RID: 6238
		protected abstract IEnumerable<EncyclopediaListItem> InitializeListItems();

		// Token: 0x0600185F RID: 6239
		protected abstract IEnumerable<EncyclopediaFilterGroup> InitializeFilterItems();

		// Token: 0x06001860 RID: 6240
		protected abstract IEnumerable<EncyclopediaSortController> InitializeSortControllers();

		// Token: 0x1700066C RID: 1644
		// (get) Token: 0x06001861 RID: 6241 RVA: 0x0007B889 File Offset: 0x00079A89
		// (set) Token: 0x06001862 RID: 6242 RVA: 0x0007B891 File Offset: 0x00079A91
		public int HomePageOrderIndex { get; protected set; }

		// Token: 0x1700066D RID: 1645
		// (get) Token: 0x06001863 RID: 6243 RVA: 0x0007B89A File Offset: 0x00079A9A
		public EncyclopediaPage Parent { get; }

		// Token: 0x06001864 RID: 6244 RVA: 0x0007B8A4 File Offset: 0x00079AA4
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

		// Token: 0x06001865 RID: 6245 RVA: 0x0007B9D9 File Offset: 0x00079BD9
		public bool HasIdentifierType(Type identifierType)
		{
			return this._identifierTypes.Contains(identifierType);
		}

		// Token: 0x06001866 RID: 6246 RVA: 0x0007B9E7 File Offset: 0x00079BE7
		internal bool HasIdentifier(string identifier)
		{
			return this._identifiers.ContainsValue(identifier);
		}

		// Token: 0x06001867 RID: 6247 RVA: 0x0007B9F5 File Offset: 0x00079BF5
		public string GetIdentifier(Type identifierType)
		{
			if (this._identifiers.ContainsKey(identifierType))
			{
				return this._identifiers[identifierType];
			}
			return "";
		}

		// Token: 0x06001868 RID: 6248 RVA: 0x0007BA17 File Offset: 0x00079C17
		public string[] GetIdentifierNames()
		{
			return this._identifiers.Values.ToArray<string>();
		}

		// Token: 0x06001869 RID: 6249 RVA: 0x0007BA2C File Offset: 0x00079C2C
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

		// Token: 0x0600186A RID: 6250 RVA: 0x0007BA88 File Offset: 0x00079C88
		public virtual string GetViewFullyQualifiedName()
		{
			return "";
		}

		// Token: 0x0600186B RID: 6251 RVA: 0x0007BA8F File Offset: 0x00079C8F
		public virtual string GetStringID()
		{
			return "";
		}

		// Token: 0x0600186C RID: 6252 RVA: 0x0007BA96 File Offset: 0x00079C96
		public virtual TextObject GetName()
		{
			return TextObject.Empty;
		}

		// Token: 0x0600186D RID: 6253 RVA: 0x0007BA9D File Offset: 0x00079C9D
		public virtual MBObjectBase GetObject(string typeName, string stringID)
		{
			return MBObjectManager.Instance.GetObject(typeName, stringID);
		}

		// Token: 0x0600186E RID: 6254 RVA: 0x0007BAAB File Offset: 0x00079CAB
		public virtual bool IsValidEncyclopediaItem(object o)
		{
			return false;
		}

		// Token: 0x0600186F RID: 6255 RVA: 0x0007BAAE File Offset: 0x00079CAE
		public virtual TextObject GetDescriptionText()
		{
			return TextObject.Empty;
		}

		// Token: 0x06001870 RID: 6256 RVA: 0x0007BAB5 File Offset: 0x00079CB5
		public IEnumerable<EncyclopediaListItem> GetListItems()
		{
			return this._items;
		}

		// Token: 0x06001871 RID: 6257 RVA: 0x0007BABD File Offset: 0x00079CBD
		public IEnumerable<EncyclopediaFilterGroup> GetFilterItems()
		{
			return this._filters;
		}

		// Token: 0x06001872 RID: 6258 RVA: 0x0007BAC5 File Offset: 0x00079CC5
		public IEnumerable<EncyclopediaSortController> GetSortControllers()
		{
			return this._sortControllers;
		}

		// Token: 0x040008A3 RID: 2211
		private readonly Type[] _identifierTypes;

		// Token: 0x040008A4 RID: 2212
		private readonly Dictionary<Type, string> _identifiers;

		// Token: 0x040008A5 RID: 2213
		private IEnumerable<EncyclopediaFilterGroup> _filters;

		// Token: 0x040008A6 RID: 2214
		private IEnumerable<EncyclopediaListItem> _items;

		// Token: 0x040008A7 RID: 2215
		private IEnumerable<EncyclopediaSortController> _sortControllers;
	}
}
