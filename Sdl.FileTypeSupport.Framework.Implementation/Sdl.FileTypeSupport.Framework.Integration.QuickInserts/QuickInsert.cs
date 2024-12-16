using Sdl.FileTypeSupport.Framework.Bilingual;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Formatting;
using Sdl.FileTypeSupport.Framework.IntegrationApi.QuickInserts;
using System;

namespace Sdl.FileTypeSupport.Framework.Integration.QuickInserts
{
	public sealed class QuickInsert : IQuickInsert, ICloneable
	{
		private QuickInsertIds _id;

		private string _name;

		private string _description;

		private IAbstractMarkupDataContainer _markupData;

		private IFormattingGroup _formatting;

		private bool _displayOnToolBar;

		public QuickInsertIds Id => _id;

		public string Name => _name;

		public string Description => _description;

		public IAbstractMarkupDataContainer MarkupData => _markupData;

		public IFormattingGroup Formatting => _formatting;

		public bool DisplayOnToolBar => _displayOnToolBar;

		public QuickInsert()
		{
		}

		public QuickInsert(QuickInsertIds id, string name, string description, IAbstractMarkupDataContainer markupData, IFormattingGroup formatting, bool displayOnToolbar)
		{
			_id = id;
			_name = name;
			_description = description;
			_markupData = markupData;
			_formatting = formatting;
			_displayOnToolBar = displayOnToolbar;
		}

		public QuickInsert(QuickInsert old)
		{
			_id = old.Id;
			_name = (old.Name.Clone() as string);
			_description = (old.Description.Clone() as string);
			_markupData = CloneMarkupData(old.MarkupData);
			_formatting = (old.Formatting?.Clone() as IFormattingGroup);
			_displayOnToolBar = old.DisplayOnToolBar;
		}

		private IAbstractMarkupDataContainer CloneMarkupData(IAbstractMarkupDataContainer markupData)
		{
			MarkupDataContainer markupDataContainer = new MarkupDataContainer();
			foreach (IAbstractMarkupData allSubItem in markupData.AllSubItems)
			{
				IAbstractMarkupData item = allSubItem.Clone() as IAbstractMarkupData;
				markupDataContainer.Add(item);
			}
			return markupDataContainer;
		}

		public object Clone()
		{
			return new QuickInsert(this);
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
			QuickInsert quickInsert = obj as QuickInsert;
			if (Id != quickInsert.Id)
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return _id.GetHashCode();
		}
	}
}
