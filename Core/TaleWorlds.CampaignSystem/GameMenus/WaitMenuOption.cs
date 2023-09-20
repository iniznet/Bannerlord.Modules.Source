using System;
using System.Reflection;
using System.Xml;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.GameMenus
{
	public class WaitMenuOption
	{
		public int Priority { get; private set; }

		internal WaitMenuOption()
		{
			this.Priority = 100;
			this._text = TextObject.Empty;
			this._tooltip = "";
		}

		internal WaitMenuOption(string idString, TextObject text, WaitMenuOption.OnConditionDelegate condition, WaitMenuOption.OnConsequenceDelegate consequence, int priority = 100, string tooltip = "")
		{
			this._idstring = idString;
			this._text = text;
			this.OnCondition = condition;
			this.OnConsequence = consequence;
			this.Priority = priority;
			this._tooltip = tooltip;
		}

		public bool GetConditionsHold(Game game, MapState mapState)
		{
			if (this.OnCondition != null)
			{
				MenuCallbackArgs menuCallbackArgs = new MenuCallbackArgs(mapState, this.Text);
				return this.OnCondition(menuCallbackArgs);
			}
			return true;
		}

		public TextObject Text
		{
			get
			{
				return this._text;
			}
		}

		public string IdString
		{
			get
			{
				return this._idstring;
			}
		}

		public string Tooltip
		{
			get
			{
				return this._tooltip;
			}
		}

		public bool IsLeave
		{
			get
			{
				return this._isLeave;
			}
		}

		public void RunConsequence(Game game, MapState mapState)
		{
			if (this.OnConsequence != null)
			{
				MenuCallbackArgs menuCallbackArgs = new MenuCallbackArgs(mapState, this.Text);
				this.OnConsequence(menuCallbackArgs);
			}
		}

		public void Deserialize(XmlNode node, Type typeOfWaitMenusCallbacks)
		{
			if (node.Attributes == null)
			{
				throw new TWXmlLoadException("node.Attributes != null");
			}
			this._idstring = node.Attributes["id"].Value;
			XmlNode xmlNode = node.Attributes["text"];
			if (xmlNode != null)
			{
				this._text = new TextObject(xmlNode.InnerText, null);
			}
			if (node.Attributes["is_leave"] != null)
			{
				this._isLeave = true;
			}
			XmlNode xmlNode2 = node.Attributes["on_condition"];
			if (xmlNode2 != null)
			{
				string innerText = xmlNode2.InnerText;
				this._methodOnCondition = typeOfWaitMenusCallbacks.GetMethod(innerText);
				if (this._methodOnCondition == null)
				{
					throw new MBNotFoundException("Can not find WaitMenuOption condition:" + innerText);
				}
				this.OnCondition = (WaitMenuOption.OnConditionDelegate)Delegate.CreateDelegate(typeof(WaitMenuOption.OnConditionDelegate), null, this._methodOnCondition);
			}
			XmlNode xmlNode3 = node.Attributes["on_consequence"];
			if (xmlNode3 != null)
			{
				string innerText2 = xmlNode3.InnerText;
				this._methodOnConsequence = typeOfWaitMenusCallbacks.GetMethod(innerText2);
				if (this._methodOnConsequence == null)
				{
					throw new MBNotFoundException("Can not find WaitMenuOption consequence:" + innerText2);
				}
				this.OnConsequence = (WaitMenuOption.OnConsequenceDelegate)Delegate.CreateDelegate(typeof(WaitMenuOption.OnConsequenceDelegate), null, this._methodOnConsequence);
			}
		}

		private string _idstring;

		private TextObject _text;

		private string _tooltip;

		private MethodInfo _methodOnCondition;

		public WaitMenuOption.OnConditionDelegate OnCondition;

		private MethodInfo _methodOnConsequence;

		public WaitMenuOption.OnConsequenceDelegate OnConsequence;

		private bool _isLeave;

		public delegate bool OnConditionDelegate(MenuCallbackArgs args);

		public delegate void OnConsequenceDelegate(MenuCallbackArgs args);
	}
}
