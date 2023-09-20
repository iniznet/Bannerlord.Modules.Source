using System;

namespace TaleWorlds.Engine.Options
{
	public interface IOptionData
	{
		float GetDefaultValue();

		void Commit();

		float GetValue(bool forceRefresh);

		void SetValue(float value);

		object GetOptionType();

		bool IsNative();

		bool IsAction();

		ValueTuple<string, bool> GetIsDisabledAndReasonID();
	}
}
