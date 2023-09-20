using System;
using TaleWorlds.Engine;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
	public interface IFocusable
	{
		void OnFocusGain(Agent userAgent);

		void OnFocusLose(Agent userAgent);

		FocusableObjectType FocusableObjectType { get; }

		TextObject GetInfoTextForBeingNotInteractable(Agent userAgent);

		string GetDescriptionText(GameEntity gameEntity = null);
	}
}
