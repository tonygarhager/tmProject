using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Sdl.Core.Settings
{
	public abstract class AbstractSettingsGroupBase : ISettingsGroup, INotifyPropertyChanged, IEditableObject
	{
		protected EventHandler<SettingsChangedEventArgs> _settingChangedDelegate;

		protected PropertyChangedEventHandler _propertyChangedDelegate;

		protected bool _parentEventHandlerAttached;

		protected bool _parentPropertyChangedEventHandlerAttached;

		protected int _suspended;

		protected List<string> _suspendedEvents;

		public abstract ISettingsGroup Parent
		{
			get;
		}

		public bool EventsSuspended => _suspended > 0;

		public abstract string Id
		{
			get;
			set;
		}

		public abstract ISettingsBundle SettingsBundle
		{
			get;
			set;
		}

		public event EventHandler<SettingsChangedEventArgs> SettingsChanged
		{
			add
			{
				if (!_parentEventHandlerAttached)
				{
					ISettingsGroup parent = Parent;
					if (parent != null)
					{
						parent.SettingsChanged += parent_SettingsChanged;
					}
					_parentEventHandlerAttached = true;
				}
				_settingChangedDelegate = (EventHandler<SettingsChangedEventArgs>)Delegate.Combine(_settingChangedDelegate, value);
			}
			remove
			{
				_settingChangedDelegate = (EventHandler<SettingsChangedEventArgs>)Delegate.Remove(_settingChangedDelegate, value);
			}
		}

		public event PropertyChangedEventHandler PropertyChanged
		{
			add
			{
				if (!_parentPropertyChangedEventHandlerAttached)
				{
					ISettingsGroup parent = Parent;
					if (parent != null)
					{
						parent.PropertyChanged += parent_PropertyChanged;
					}
					_parentPropertyChangedEventHandlerAttached = true;
				}
				_propertyChangedDelegate = (PropertyChangedEventHandler)Delegate.Combine(_propertyChangedDelegate, value);
			}
			remove
			{
				_propertyChangedDelegate = (PropertyChangedEventHandler)Delegate.Remove(_propertyChangedDelegate, value);
			}
		}

		protected abstract void parent_SettingsChanged(object sender, SettingsChangedEventArgs e);

		protected abstract void parent_PropertyChanged(object sender, PropertyChangedEventArgs e);

		protected abstract void CollectSettings(ISettingsGroup settingsGroup, List<string> keys);

		internal void OnParentChanging()
		{
			if (_parentEventHandlerAttached)
			{
				ISettingsGroup parent = Parent;
				if (parent != null)
				{
					parent.SettingsChanged -= parent_SettingsChanged;
				}
				_parentEventHandlerAttached = false;
			}
			if (_parentPropertyChangedEventHandlerAttached)
			{
				ISettingsGroup parent2 = Parent;
				if (parent2 != null)
				{
					parent2.PropertyChanged -= parent_PropertyChanged;
				}
				_parentPropertyChangedEventHandlerAttached = false;
			}
		}

		internal void OnParentChanged()
		{
			ISettingsGroup parent = Parent;
			if (_parentEventHandlerAttached && parent != null)
			{
				parent.SettingsChanged += parent_SettingsChanged;
			}
			if (parent != null)
			{
				List<string> list = new List<string>();
				CollectSettings(parent, list);
				OnSettingsChanged(new List<string>(list), isResumingEvents: false);
			}
		}

		public void SuspendEvents()
		{
			_suspended++;
		}

		public void ResumeEvents()
		{
			if (_suspended > 0)
			{
				_suspended--;
				if (_suspended == 0 && _suspendedEvents != null && _suspendedEvents.Count > 0)
				{
					ReadOnlyCollection<string> settingIds = _suspendedEvents.AsReadOnly();
					_suspendedEvents = null;
					OnSettingsChanged(settingIds, isResumingEvents: true);
				}
			}
		}

		protected void DiscardEvents()
		{
			if (_suspended > 0)
			{
				_suspended--;
				if (_suspended == 0)
				{
					_suspendedEvents = null;
				}
			}
		}

		protected virtual void OnSettingChanged(string settingId)
		{
			List<string> list = new List<string>(1);
			list.Add(settingId);
			OnSettingsChanged(list.AsReadOnly(), isResumingEvents: false);
		}

		protected virtual void OnSettingsChanged(IList<string> settingIds, bool isResumingEvents)
		{
			if (!EventsSuspended)
			{
				if (_settingChangedDelegate != null)
				{
					_settingChangedDelegate(this, new SettingsChangedEventArgs(this, settingIds));
				}
			}
			else
			{
				if (_suspendedEvents == null)
				{
					_suspendedEvents = new List<string>(5);
				}
				_suspendedEvents.AddRange(settingIds);
			}
			if (!isResumingEvents && _propertyChangedDelegate != null)
			{
				foreach (string settingId in settingIds)
				{
					_propertyChangedDelegate(this, new PropertyChangedEventArgs(settingId));
				}
			}
		}

		public abstract bool ContainsSetting(string settingId);

		public abstract Setting<T> GetSetting<T>(string id);

		public abstract bool GetSetting<T>(string settingId, out Setting<T> setting);

		public abstract bool GetSetting<T>(string settingId, out T value);

		public abstract Setting<T> GetSetting<T>(string settingId, T defaultValue);

		public abstract void ImportSettings(ISettingsGroup otherGroup);

		public abstract bool RemoveSetting(string settingId);

		public abstract void Reset();

		public abstract IEnumerable<string> GetSettingIds();

		public abstract void BeginEdit();

		public abstract void EndEdit();

		public abstract void CancelEdit();
	}
}
