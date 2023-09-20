using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.CampaignSystem.Encyclopedia
{
	public class EncyclopediaManager
	{
		public IViewDataTracker ViewDataTracker { get; private set; }

		public void CreateEncyclopediaPages()
		{
			this._pages = new Dictionary<Type, EncyclopediaPage>();
			this.ViewDataTracker = Campaign.Current.GetCampaignBehavior<IViewDataTracker>();
			List<Type> list = new List<Type>();
			List<Assembly> list2 = new List<Assembly>();
			Assembly assembly = typeof(EncyclopediaModelBase).Assembly;
			list2.Add(assembly);
			foreach (Assembly assembly2 in AppDomain.CurrentDomain.GetAssemblies())
			{
				AssemblyName[] referencedAssemblies = assembly2.GetReferencedAssemblies();
				for (int j = 0; j < referencedAssemblies.Length; j++)
				{
					if (referencedAssemblies[j].ToString() == assembly.GetName().ToString())
					{
						list2.Add(assembly2);
						break;
					}
				}
			}
			foreach (Assembly assembly3 in list2)
			{
				list.AddRange(assembly3.GetTypesSafe(null));
			}
			foreach (Type type in list)
			{
				if (typeof(EncyclopediaPage).IsAssignableFrom(type))
				{
					object[] array = type.GetCustomAttributes(typeof(OverrideEncyclopediaModel), false);
					for (int i = 0; i < array.Length; i++)
					{
						OverrideEncyclopediaModel overrideEncyclopediaModel = array[i] as OverrideEncyclopediaModel;
						if (overrideEncyclopediaModel != null)
						{
							EncyclopediaPage encyclopediaPage = Activator.CreateInstance(type) as EncyclopediaPage;
							foreach (Type type2 in overrideEncyclopediaModel.PageTargetTypes)
							{
								this._pages.Add(type2, encyclopediaPage);
							}
						}
					}
				}
			}
			foreach (Type type3 in list)
			{
				if (typeof(EncyclopediaPage).IsAssignableFrom(type3))
				{
					object[] array = type3.GetCustomAttributes(typeof(EncyclopediaModel), false);
					for (int i = 0; i < array.Length; i++)
					{
						EncyclopediaModel encyclopediaModel = array[i] as EncyclopediaModel;
						if (encyclopediaModel != null)
						{
							EncyclopediaPage encyclopediaPage2 = Activator.CreateInstance(type3) as EncyclopediaPage;
							foreach (Type type4 in encyclopediaModel.PageTargetTypes)
							{
								if (!this._pages.ContainsKey(type4))
								{
									this._pages.Add(type4, encyclopediaPage2);
								}
							}
						}
					}
				}
			}
		}

		public IEnumerable<EncyclopediaPage> GetEncyclopediaPages()
		{
			return this._pages.Values.Distinct<EncyclopediaPage>();
		}

		public EncyclopediaPage GetPageOf(Type type)
		{
			return this._pages[type];
		}

		public string GetIdentifier(Type type)
		{
			return this._pages[type].GetIdentifier(type);
		}

		public void GoToLink(string pageType, string stringID)
		{
			if (this._executeLink == null || string.IsNullOrEmpty(pageType))
			{
				return;
			}
			if (pageType == "Home" || pageType == "LastPage")
			{
				this._executeLink(pageType, null);
				return;
			}
			if (pageType == "ListPage")
			{
				EncyclopediaPage encyclopediaPage = Campaign.Current.EncyclopediaManager.GetEncyclopediaPages().FirstOrDefault((EncyclopediaPage e) => e.HasIdentifier(stringID));
				this._executeLink(pageType, encyclopediaPage);
				return;
			}
			EncyclopediaPage encyclopediaPage2 = Campaign.Current.EncyclopediaManager.GetEncyclopediaPages().FirstOrDefault((EncyclopediaPage e) => e.HasIdentifier(pageType));
			MBObjectBase @object = encyclopediaPage2.GetObject(pageType, stringID);
			if (encyclopediaPage2 != null && encyclopediaPage2.IsValidEncyclopediaItem(@object))
			{
				this._executeLink(pageType, @object);
			}
		}

		public void GoToLink(string link)
		{
			string[] array = link.ToString().Split(new char[] { '-' });
			this.GoToLink(array[0], array[1]);
		}

		public void SetLinkCallback(Action<string, object> ExecuteLink)
		{
			this._executeLink = ExecuteLink;
		}

		private Dictionary<Type, EncyclopediaPage> _pages;

		public const string HOME_ID = "Home";

		public const string LIST_PAGE_ID = "ListPage";

		public const string LAST_PAGE_ID = "LastPage";

		private Action<string, object> _executeLink;
	}
}
