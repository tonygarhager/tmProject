using System;

namespace Sdl.Enterprise2.Platform.Contracts.Communication
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public sealed class OperationTimeoutAttribute : Attribute
	{
		private TimeSpan _operationTimeout;

		private string _propertyName;

		public int Hours
		{
			get
			{
				return _operationTimeout.Hours;
			}
			set
			{
				_operationTimeout = TimeSpan.FromHours(value);
			}
		}

		public int Minutes
		{
			get
			{
				return _operationTimeout.Minutes;
			}
			set
			{
				_operationTimeout = TimeSpan.FromMinutes(value);
			}
		}

		public int Seconds
		{
			get
			{
				return _operationTimeout.Seconds;
			}
			set
			{
				_operationTimeout = TimeSpan.FromSeconds(value);
			}
		}

		public string TimeSpanPropertyName
		{
			get
			{
				return _propertyName;
			}
			set
			{
				_propertyName = value;
			}
		}

		public OperationTimeoutAttribute()
		{
			_operationTimeout = default(TimeSpan);
		}
	}
}
