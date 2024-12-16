using Sdl.Core.Bcm.BcmConverters.ToBilingualApi.Extensions;
using Sdl.Core.Bcm.BcmModel;
using Sdl.Core.Bcm.BcmModel.Annotations;
using Sdl.Core.Bcm.BcmModel.Skeleton;
using Sdl.FileTypeSupport.Framework.Bilingual;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System.Linq;

namespace Sdl.Core.Bcm.BcmConverters.ToBilingualApi.MarkupDataConverters
{
	internal class CommentContainerConverter : MarkupDataConverter
	{
		private readonly IPropertiesFactory _propertiesFactory;

		private readonly FileSkeleton _fileSkeleton;

		public CommentContainerConverter(IPropertiesFactory propertiesFactory, FileSkeleton fileSkeleton)
		{
			_propertiesFactory = propertiesFactory;
			_fileSkeleton = fileSkeleton;
		}

		internal override IAbstractMarkupData Convert(MarkupData source)
		{
			CommentContainer commentContainer = source as CommentContainer;
			if (commentContainer == null)
			{
				return null;
			}
			CommentMarker commentMarker = new CommentMarker
			{
				Comments = _propertiesFactory.CreateCommentProperties()
			};
			CommentDefinition commentDefinition = GetCommentDefinition(commentContainer.CommentDefinitionId);
			IComment comment = _propertiesFactory.CreateComment(commentDefinition.Text, commentDefinition.Author, (Severity)commentDefinition.CommentSeverity);
			comment.Date = commentDefinition.Date;
			commentMarker.Comments.Add(comment);
			commentMarker.ConvertAndAddChildren(commentContainer.Children, base.ConverterFactory);
			return commentMarker;
		}

		private CommentDefinition GetCommentDefinition(int commentDefinitionId)
		{
			return _fileSkeleton.CommentDefinitions.First((CommentDefinition x) => x.Id == commentDefinitionId);
		}
	}
}
