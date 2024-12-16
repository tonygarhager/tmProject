using Sdl.LanguagePlatform.Core;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemory.EditScripts
{
	[DataContract]
	public class EditActionDeleteTags : EditAction
	{
		[DataMember]
		public Segment.DeleteTagsAction DeleteTagsAction
		{
			get;
			set;
		}

		public EditActionDeleteTags()
			: this(Segment.DeleteTagsAction.KeepTextPlaceholders)
		{
		}

		public EditActionDeleteTags(Segment.DeleteTagsAction mode)
		{
			DeleteTagsAction = mode;
		}

		public override bool Validate(IFieldDefinitions fields, bool throwIfInvalid)
		{
			return true;
		}

		public override bool Apply(TranslationUnit tu)
		{
			if ((0 | (tu.SourceSegment.DeleteTags(DeleteTagsAction) ? 1 : 0) | (tu.TargetSegment.DeleteTags(DeleteTagsAction) ? 1 : 0)) == 0)
			{
				return false;
			}
			tu.SourceSegment.Tokens = null;
			tu.TargetSegment.Tokens = null;
			return true;
		}
	}
}
