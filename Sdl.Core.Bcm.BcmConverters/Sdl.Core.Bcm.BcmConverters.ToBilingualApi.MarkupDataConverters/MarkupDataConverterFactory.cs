using Sdl.Core.Bcm.BcmConverters.Common;
using Sdl.Core.Bcm.BcmModel;
using Sdl.Core.Bcm.BcmModel.Annotations;
using Sdl.Core.Bcm.BcmModel.Skeleton;
using Sdl.FileTypeSupport.Framework.Bilingual;
using Sdl.FileTypeSupport.Framework.Native;
using System;
using System.Collections.Generic;

namespace Sdl.Core.Bcm.BcmConverters.ToBilingualApi.MarkupDataConverters
{
	internal class MarkupDataConverterFactory
	{
		private readonly Dictionary<Type, MarkupDataConverter> _converters = new Dictionary<Type, MarkupDataConverter>();

		public ContextTable ContextTable
		{
			get;
			set;
		}

		internal MarkupDataConverterFactory(PropertiesFactory propertiesFactory, DocumentItemFactory documentItemFactory, FileSkeleton fileSkeleton)
		{
			ContextTable = new ContextTable();
			_converters.Add(typeof(Sdl.Core.Bcm.BcmModel.Segment), new SegmentConverter(documentItemFactory).SetFactory(this));
			_converters.Add(typeof(TextMarkup), new TextMarkupConverter(propertiesFactory).SetFactory(this));
			_converters.Add(typeof(Sdl.Core.Bcm.BcmModel.TagPair), new TagPairConverter(propertiesFactory, documentItemFactory, fileSkeleton).SetFactory(this));
			_converters.Add(typeof(Sdl.Core.Bcm.BcmModel.PlaceholderTag), new PlaceholderTagConverter(propertiesFactory, documentItemFactory, fileSkeleton).SetFactory(this));
			_converters.Add(typeof(Sdl.Core.Bcm.BcmModel.StructureTag), new StructureTagConverter(propertiesFactory, documentItemFactory, fileSkeleton).SetFactory(this));
			_converters.Add(typeof(LockedContentContainer), new LockedContentConverter(propertiesFactory).SetFactory(this));
			_converters.Add(typeof(CommentContainer), new CommentContainerConverter(propertiesFactory, fileSkeleton).SetFactory(this));
			_converters.Add(typeof(RevisionContainer), new RevisionConverter(propertiesFactory).SetFactory(this));
			_converters.Add(typeof(FeedbackContainer), new FeedbackConverter(propertiesFactory, fileSkeleton).SetFactory(this));
		}

		internal void ResetSegmentProperties(DocumentItemFactory documentItemFactory)
		{
			_converters[typeof(Sdl.Core.Bcm.BcmModel.Segment)] = new SegmentConverter(documentItemFactory)
			{
				ConverterFactory = this
			};
		}

		internal MarkupDataConverter GetConverter(Type type)
		{
			if (!_converters.ContainsKey(type))
			{
				return null;
			}
			return _converters[type];
		}
	}
}
