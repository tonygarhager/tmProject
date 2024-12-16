using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Native;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.Bilingual
{
	public class NativeToBilingualConverter : IBilingualFileTypeComponent, INativeFileTypeComponent, IBilingualContentMessageReporter, IBasicMessageReporter, INativeToBilingualConverter, INativeExtractionContentHandler, IAbstractNativeContentHandler, INativeContentCycleAware, ISharedObjectsAware
	{
		private INativeExtractionContentHandler _output;

		private List<INativeExtractionContentProcessor> _preProcessors = new List<INativeExtractionContentProcessor>();

		private NativeToBilingualConverterImpl _impl = new NativeToBilingualConverterImpl();

		public virtual IBilingualContentHandler Output
		{
			get
			{
				return _impl.Output;
			}
			set
			{
				_impl.Output = value;
			}
		}

		public virtual IDocumentProperties DocumentInfo
		{
			get
			{
				return _impl.DocumentInfo;
			}
			set
			{
				_impl.DocumentInfo = value;
			}
		}

		public virtual IFileProperties FileInfo
		{
			get
			{
				return _impl.FileInfo;
			}
			set
			{
				_impl.FileInfo = value;
			}
		}

		INativeContentStreamMessageReporter INativeFileTypeComponent.MessageReporter
		{
			get
			{
				using (IEnumerator<INativeFileTypeComponent> enumerator = GetComponentsOfType<INativeFileTypeComponent>().GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						INativeFileTypeComponent current = enumerator.Current;
						return current.MessageReporter;
					}
				}
				return null;
			}
			set
			{
				foreach (INativeFileTypeComponent item in GetComponentsOfType<INativeFileTypeComponent>())
				{
					item.MessageReporter = value;
				}
			}
		}

		public IPropertiesFactory PropertiesFactory
		{
			get
			{
				return _impl.PropertiesFactory;
			}
			set
			{
				foreach (INativeFileTypeComponent item in GetComponentsOfType<INativeFileTypeComponent>())
				{
					item.PropertiesFactory = value;
				}
			}
		}

		public virtual IDocumentItemFactory ItemFactory
		{
			get
			{
				return _impl.ItemFactory;
			}
			set
			{
				foreach (IBilingualFileTypeComponent item in GetComponentsOfType<IBilingualFileTypeComponent>())
				{
					item.ItemFactory = value;
				}
			}
		}

		public virtual IBilingualContentMessageReporter MessageReporter
		{
			get
			{
				return _impl.MessageReporter;
			}
			set
			{
				foreach (IBilingualFileTypeComponent item in GetComponentsOfType<IBilingualFileTypeComponent>())
				{
					item.MessageReporter = value;
				}
			}
		}

		public NativeToBilingualConverter()
		{
			_preProcessors.Add(new StructureBoundaryIdentifier());
			_preProcessors.Add(new NativeTextNormalizer());
			ReconnectComponents();
		}

		public virtual void ReconnectComponents()
		{
			INativeExtractionContentHandler output = _impl;
			for (int num = _preProcessors.Count - 1; num >= 0; num--)
			{
				_preProcessors[num].Output = output;
				output = _preProcessors[num];
			}
			_output = output;
		}

		private IEnumerable<T> GetComponentsOfType<T>() where T : class
		{
			foreach (INativeExtractionContentProcessor preProcessor in _preProcessors)
			{
				T val = preProcessor as T;
				if (val != null)
				{
					yield return val;
				}
			}
			T val2 = _impl as T;
			if (val2 != null)
			{
				yield return val2;
			}
		}

		public virtual void StructureTag(IStructureTagProperties tagInfo)
		{
			_output.StructureTag(tagInfo);
		}

		public virtual void InlineStartTag(IStartTagProperties tagInfo)
		{
			_output.InlineStartTag(tagInfo);
		}

		public virtual void InlineEndTag(IEndTagProperties tagInfo)
		{
			_output.InlineEndTag(tagInfo);
		}

		public virtual void InlinePlaceholderTag(IPlaceholderTagProperties tagInfo)
		{
			_output.InlinePlaceholderTag(tagInfo);
		}

		public virtual void Text(ITextProperties textInfo)
		{
			_output.Text(textInfo);
		}

		public virtual void CustomInfo(ICustomInfoProperties info)
		{
			_output.CustomInfo(info);
		}

		public virtual void LocationMark(LocationMarkerId markerId)
		{
			_output.LocationMark(markerId);
		}

		public virtual void ChangeContext(IContextProperties contexts)
		{
			_output.ChangeContext(contexts);
		}

		public virtual void LockedContentStart(ILockedContentProperties lockedContentInfo)
		{
			_output.LockedContentStart(lockedContentInfo);
		}

		public virtual void LockedContentEnd()
		{
			_output.LockedContentEnd();
		}

		public virtual void RevisionStart(IRevisionProperties revisionInfo)
		{
			_output.RevisionStart(revisionInfo);
		}

		public virtual void RevisionEnd()
		{
			_output.RevisionEnd();
		}

		public virtual void CommentStart(ICommentProperties commentInfo)
		{
			_output.CommentStart(commentInfo);
		}

		public virtual void CommentEnd()
		{
			_output.CommentEnd();
		}

		public virtual void ParagraphComments(ICommentProperties commentInfo)
		{
			_output.ParagraphComments(commentInfo);
		}

		public virtual void SetFileProperties(IFileProperties properties)
		{
			foreach (INativeContentCycleAware item in GetComponentsOfType<INativeContentCycleAware>())
			{
				item.SetFileProperties(properties);
			}
		}

		public virtual void StartOfInput()
		{
			foreach (INativeContentCycleAware item in GetComponentsOfType<INativeContentCycleAware>())
			{
				item.StartOfInput();
			}
		}

		public virtual void EndOfInput()
		{
			foreach (INativeContentCycleAware item in GetComponentsOfType<INativeContentCycleAware>())
			{
				item.EndOfInput();
			}
		}

		public virtual void ReportMessage(object source, string origin, ErrorLevel level, string message, TextLocation fromLocation, TextLocation uptoLocation)
		{
			_impl.ReportMessage(source, origin, level, message, fromLocation, uptoLocation);
		}

		public virtual void ReportMessage(object source, string origin, ErrorLevel level, string message, string locationDescription)
		{
			_impl.ReportMessage(source, origin, level, message, locationDescription);
		}

		public void SetSharedObjects(ISharedObjects sharedObjects)
		{
			foreach (ISharedObjectsAware item in GetComponentsOfType<ISharedObjectsAware>())
			{
				item.SetSharedObjects(sharedObjects);
			}
		}
	}
}
