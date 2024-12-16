using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using System;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.Integration
{
	public class QuickTag : IQuickTag
	{
		private IQuickTagContent _MarkupDataContainer = new QuickTagContent();

		private string _CommandId;

		private LocalizableString _CommandName;

		private LocalizableString _Description;

		private IconDescriptor _Icon;

		private bool _DisplayOnToolbar;

		public IQuickTagContent MarkupDataContent => _MarkupDataContainer;

		public string CommandId
		{
			get
			{
				return _CommandId;
			}
			set
			{
				_CommandId = value;
			}
		}

		public LocalizableString CommandName
		{
			get
			{
				return _CommandName;
			}
			set
			{
				_CommandName = value;
			}
		}

		public LocalizableString Description
		{
			get
			{
				return _Description;
			}
			set
			{
				_Description = value;
			}
		}

		public IconDescriptor Icon
		{
			get
			{
				return _Icon;
			}
			set
			{
				_Icon = value;
			}
		}

		public bool DisplayOnToolBar
		{
			get
			{
				return _DisplayOnToolbar;
			}
			set
			{
				_DisplayOnToolbar = value;
			}
		}

		public bool IsDefaultQuickTag
		{
			get
			{
				string[] names = Enum.GetNames(typeof(QuickTagDefaultId));
				foreach (string b in names)
				{
					if (CommandId == b)
					{
						return true;
					}
				}
				return false;
			}
		}

		public QuickTag(params IAbstractMarkupData[] markupData)
		{
			_Icon = new IconDescriptor();
			ReadMarkupData(markupData);
		}

		public QuickTag()
		{
			_Icon = new IconDescriptor();
		}

		public QuickTag(QuickTagDefaultId qtDefaultId)
		{
			_CommandId = qtDefaultId.ToString();
		}

		public static QuickTag Create(params object[] markupData)
		{
			QuickTag quickTag = new QuickTag();
			foreach (object obj in markupData)
			{
				quickTag.MarkupDataContent.Add((IAbstractMarkupData)obj);
			}
			return quickTag;
		}

		private void ReadMarkupData(IList<IAbstractMarkupData> markupDataList)
		{
			foreach (IAbstractMarkupData markupData in markupDataList)
			{
				_MarkupDataContainer.Add(markupData);
			}
		}

		public override bool Equals(object obj)
		{
			if (!base.Equals(obj))
			{
				return false;
			}
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			IQuickTag quickTag = obj as IQuickTag;
			if (CommandId != quickTag.CommandId)
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return CommandId.GetHashCode();
		}
	}
}
