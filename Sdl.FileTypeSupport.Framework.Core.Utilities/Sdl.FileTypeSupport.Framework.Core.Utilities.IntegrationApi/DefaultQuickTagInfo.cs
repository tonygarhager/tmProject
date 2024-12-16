using Sdl.FileTypeSupport.Framework.Core.Utilities.Formatting;
using Sdl.FileTypeSupport.Framework.Formatting;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using System;

namespace Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi
{
	public class DefaultQuickTagInfo : IDefaultQuickTagInfo
	{
		private string _CommandName;

		private string _CommandID;

		private string _ImageResource;

		private string _ImagePath;

		private string _Description;

		private IFormattingGroup _Formatting = new FormattingGroup();

		public string CommandName => _CommandName;

		public string CommandID => _CommandID;

		public string ImageResource => _ImageResource;

		public string ImagePath => _ImagePath;

		public string Description => _Description;

		public IFormattingGroup Formatting => _Formatting;

		public QuickTagDefaultId DefaultId
		{
			get
			{
				try
				{
					return (QuickTagDefaultId)Enum.Parse(typeof(QuickTagDefaultId), CommandID);
				}
				catch (Exception)
				{
					return QuickTagDefaultId.Undefined;
				}
			}
		}

		public DefaultQuickTagInfo(string commandName, string commandID, string imageResource, string imagePath, string description, IFormattingItem formattingItem)
		{
			_CommandName = commandName;
			_CommandID = commandID;
			_ImageResource = imageResource;
			_ImagePath = imagePath;
			_Description = description;
			if (formattingItem != null)
			{
				_Formatting.Add(formattingItem);
			}
		}
	}
}
