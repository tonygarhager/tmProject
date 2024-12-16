using Sdl.Core.PluginFramework.Validation;
using System;
using System.Globalization;

namespace Sdl.Core.PluginFramework
{
	[AttributeUsage(AttributeTargets.Class)]
	public class ExtensionAttribute : Attribute
	{
		private string _id;

		private string _name;

		private string _description;

		private string _icon;

		public string Id
		{
			get
			{
				return _id;
			}
			set
			{
				_id = value;
			}
		}

		[PluginResource]
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
				if (string.IsNullOrEmpty(_description))
				{
					_description = value;
				}
			}
		}

		[PluginResource]
		public string Description
		{
			get
			{
				return _description;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					_description = value;
				}
			}
		}

		public string Icon
		{
			get
			{
				return _icon;
			}
			set
			{
				_icon = value;
			}
		}

		public ExtensionAttribute()
		{
		}

		public ExtensionAttribute(string id, string name, string description)
		{
			_id = id;
			_name = name;
			_description = description;
		}

		public virtual void Validate(IExtensionAttributeInfo info, IExtensionValidationContext context)
		{
			ValidateId(info, context);
			ValidateName(info, context);
		}

		protected virtual void ValidateId(IExtensionAttributeInfo info, IExtensionValidationContext context)
		{
			if (string.IsNullOrEmpty(Id))
			{
				context.ReportError("PF103", string.Format(CultureInfo.CurrentUICulture, "Extension attribute '{0}' on class '{1}' has no valid Id.", new object[2]
				{
					GetType().FullName,
					info.ExtensionType.FullName
				}));
			}
		}

		protected virtual void ValidateName(IExtensionAttributeInfo info, IExtensionValidationContext context)
		{
			if (string.IsNullOrEmpty(Name))
			{
				context.ReportError("PF104", string.Format(CultureInfo.CurrentUICulture, "Extension attribute '{0}' on class '{1}' has no valid Name.", new object[2]
				{
					GetType().FullName,
					info.ExtensionType.FullName
				}));
			}
		}
	}
}
