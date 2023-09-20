using System;
using System.Collections.Generic;

namespace TaleWorlds.GauntletUI
{
	public class SoundProperties
	{
		public IEnumerable<KeyValuePair<string, AudioProperty>> RegisteredStateSounds
		{
			get
			{
				foreach (KeyValuePair<string, AudioProperty> keyValuePair in this._stateSounds)
				{
					yield return keyValuePair;
				}
				Dictionary<string, AudioProperty>.Enumerator enumerator = default(Dictionary<string, AudioProperty>.Enumerator);
				yield break;
				yield break;
			}
		}

		public IEnumerable<KeyValuePair<string, AudioProperty>> RegisteredEventSounds
		{
			get
			{
				foreach (KeyValuePair<string, AudioProperty> keyValuePair in this._eventSounds)
				{
					yield return keyValuePair;
				}
				Dictionary<string, AudioProperty>.Enumerator enumerator = default(Dictionary<string, AudioProperty>.Enumerator);
				yield break;
				yield break;
			}
		}

		public SoundProperties()
		{
			this._stateSounds = new Dictionary<string, AudioProperty>();
			this._eventSounds = new Dictionary<string, AudioProperty>();
		}

		public void AddStateSound(string state, AudioProperty audioProperty)
		{
			this._stateSounds.Add(state, audioProperty);
		}

		public void AddEventSound(string state, AudioProperty audioProperty)
		{
			if (this._eventSounds.ContainsKey(state))
			{
				this._eventSounds[state] = audioProperty;
				return;
			}
			this._eventSounds.Add(state, audioProperty);
		}

		public void FillFrom(SoundProperties soundProperties)
		{
			this._stateSounds = new Dictionary<string, AudioProperty>();
			this._eventSounds = new Dictionary<string, AudioProperty>();
			foreach (KeyValuePair<string, AudioProperty> keyValuePair in soundProperties._stateSounds)
			{
				string key = keyValuePair.Key;
				AudioProperty value = keyValuePair.Value;
				AudioProperty audioProperty = new AudioProperty();
				audioProperty.FillFrom(value);
				this._stateSounds.Add(key, audioProperty);
			}
			foreach (KeyValuePair<string, AudioProperty> keyValuePair2 in soundProperties._eventSounds)
			{
				string key2 = keyValuePair2.Key;
				AudioProperty value2 = keyValuePair2.Value;
				AudioProperty audioProperty2 = new AudioProperty();
				audioProperty2.FillFrom(value2);
				this._eventSounds.Add(key2, audioProperty2);
			}
		}

		public AudioProperty GetEventAudioProperty(string eventName)
		{
			if (this._eventSounds.ContainsKey(eventName))
			{
				return this._eventSounds[eventName];
			}
			return null;
		}

		public AudioProperty GetStateAudioProperty(string stateName)
		{
			if (this._stateSounds.ContainsKey(stateName))
			{
				return this._stateSounds[stateName];
			}
			return null;
		}

		private Dictionary<string, AudioProperty> _stateSounds;

		private Dictionary<string, AudioProperty> _eventSounds;
	}
}
