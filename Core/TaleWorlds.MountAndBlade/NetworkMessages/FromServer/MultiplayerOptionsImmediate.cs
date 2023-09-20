using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class MultiplayerOptionsImmediate : GameNetworkMessage
	{
		public MultiplayerOptionsImmediate()
		{
			this._optionList = new List<MultiplayerOptions.MultiplayerOption>();
			for (MultiplayerOptions.OptionType optionType = MultiplayerOptions.OptionType.ServerName; optionType < MultiplayerOptions.OptionType.NumOfSlots; optionType++)
			{
				if (optionType.GetOptionProperty().Replication == MultiplayerOptionsProperty.ReplicationOccurrence.Immediately)
				{
					this._optionList.Add(MultiplayerOptions.Instance.GetOptionFromOptionType(optionType, MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions));
				}
			}
		}

		public MultiplayerOptions.MultiplayerOption GetOption(MultiplayerOptions.OptionType optionType)
		{
			return this._optionList.First((MultiplayerOptions.MultiplayerOption x) => x.OptionType == optionType);
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this._optionList = new List<MultiplayerOptions.MultiplayerOption>();
			for (MultiplayerOptions.OptionType optionType = MultiplayerOptions.OptionType.ServerName; optionType < MultiplayerOptions.OptionType.NumOfSlots; optionType++)
			{
				MultiplayerOptionsProperty optionProperty = optionType.GetOptionProperty();
				if (optionProperty.Replication == MultiplayerOptionsProperty.ReplicationOccurrence.Immediately)
				{
					MultiplayerOptions.MultiplayerOption multiplayerOption = MultiplayerOptions.MultiplayerOption.CreateMultiplayerOption(optionType);
					switch (optionProperty.OptionValueType)
					{
					case MultiplayerOptions.OptionValueType.Bool:
						multiplayerOption.UpdateValue(GameNetworkMessage.ReadBoolFromPacket(ref flag));
						break;
					case MultiplayerOptions.OptionValueType.Integer:
					case MultiplayerOptions.OptionValueType.Enum:
						multiplayerOption.UpdateValue(GameNetworkMessage.ReadIntFromPacket(new CompressionInfo.Integer(optionProperty.BoundsMin, optionProperty.BoundsMax, true), ref flag));
						break;
					case MultiplayerOptions.OptionValueType.String:
						multiplayerOption.UpdateValue(GameNetworkMessage.ReadStringFromPacket(ref flag));
						break;
					}
					this._optionList.Add(multiplayerOption);
				}
			}
			return flag;
		}

		protected override void OnWrite()
		{
			foreach (MultiplayerOptions.MultiplayerOption multiplayerOption in this._optionList)
			{
				MultiplayerOptions.OptionType optionType = multiplayerOption.OptionType;
				MultiplayerOptionsProperty optionProperty = optionType.GetOptionProperty();
				switch (optionProperty.OptionValueType)
				{
				case MultiplayerOptions.OptionValueType.Bool:
					GameNetworkMessage.WriteBoolToPacket(optionType.GetBoolValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions));
					break;
				case MultiplayerOptions.OptionValueType.Integer:
				case MultiplayerOptions.OptionValueType.Enum:
					GameNetworkMessage.WriteIntToPacket(optionType.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions), new CompressionInfo.Integer(optionProperty.BoundsMin, optionProperty.BoundsMax, true));
					break;
				case MultiplayerOptions.OptionValueType.String:
					GameNetworkMessage.WriteStringToPacket(optionType.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions));
					break;
				}
			}
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Administration;
		}

		protected override string OnGetLogFormat()
		{
			return "Receiving runtime multiplayer options.";
		}

		private List<MultiplayerOptions.MultiplayerOption> _optionList;
	}
}
