using Sdl.FileTypeSupport.Framework.BilingualApi;
using System;

namespace Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi
{
	public class CharacterCountingIterator
	{
		public interface ICharacterCountingVisitor : IMarkupDataVisitor
		{
			int Count
			{
				get;
				set;
			}
		}

		protected class StartOfItemCharacterCounterVisitor : ICharacterCountingVisitor, IMarkupDataVisitor
		{
			private int _count;

			private bool _inLockedContent;

			public int Count
			{
				get
				{
					return _count;
				}
				set
				{
					_count = value;
				}
			}

			public virtual void VisitTagPair(ITagPair tagPair)
			{
				_count += tagPair.StartTagProperties.TagContent.Length;
				if (_inLockedContent)
				{
					_count += tagPair.EndTagProperties.TagContent.Length;
				}
			}

			public virtual void VisitPlaceholderTag(IPlaceholderTag tag)
			{
				_count += tag.Properties.TagContent.Length;
			}

			public virtual void VisitText(IText text)
			{
				_count += text.Properties.Text.Length;
			}

			public virtual void VisitSegment(ISegment segment)
			{
			}

			public virtual void VisitLocationMarker(ILocationMarker location)
			{
			}

			public virtual void VisitCommentMarker(ICommentMarker commentMarker)
			{
			}

			public virtual void VisitOtherMarker(IOtherMarker marker)
			{
			}

			public virtual void VisitLockedContent(ILockedContent lockedContent)
			{
				if (!_inLockedContent)
				{
					_inLockedContent = true;
					foreach (IAbstractMarkupData allSubItem in lockedContent.Content.AllSubItems)
					{
						allSubItem.AcceptVisitor(this);
					}
					_inLockedContent = false;
				}
			}

			public virtual void VisitRevisionMarker(IRevisionMarker revisionMarker)
			{
			}
		}

		protected class EndOfItemCharacterCounterVisitor : ICharacterCountingVisitor, IMarkupDataVisitor
		{
			private int _count;

			public int Count
			{
				get
				{
					return _count;
				}
				set
				{
					_count = value;
				}
			}

			public virtual void VisitTagPair(ITagPair tagPair)
			{
				_count += tagPair.EndTagProperties.TagContent.Length;
			}

			public virtual void VisitPlaceholderTag(IPlaceholderTag tag)
			{
				throw new NotImplementedException("Should never happen, we only expect containers");
			}

			public virtual void VisitText(IText text)
			{
				throw new NotImplementedException("Should never happen, we only expect containers");
			}

			public virtual void VisitSegment(ISegment segment)
			{
			}

			public virtual void VisitLocationMarker(ILocationMarker location)
			{
				throw new NotImplementedException("Should never happen, we only expect containers");
			}

			public virtual void VisitCommentMarker(ICommentMarker commentMarker)
			{
			}

			public virtual void VisitOtherMarker(IOtherMarker marker)
			{
			}

			public virtual void VisitLockedContent(ILockedContent lockedContent)
			{
				throw new NotImplementedException("Should never happen, we only expect containers!");
			}

			public virtual void VisitRevisionMarker(IRevisionMarker revisionMarker)
			{
			}
		}

		private int _characterCount;

		private Location _currentLocation;

		private Func<ICharacterCountingVisitor> _startCounterFactory;

		private Func<ICharacterCountingVisitor> _endCounterFactory;

		private ICharacterCountingVisitor _startItemVisitor;

		private ICharacterCountingVisitor _endItemVisitor;

		public int CharacterCount
		{
			get
			{
				return _characterCount;
			}
			set
			{
				_characterCount = value;
			}
		}

		public Location CurrentLocation
		{
			get
			{
				return _currentLocation;
			}
			set
			{
				_currentLocation = value;
			}
		}

		public int CharactersToNextLocation
		{
			get
			{
				if (_currentLocation.IsAtEnd)
				{
					return 0;
				}
				IAbstractMarkupData itemAtLocation = _currentLocation.ItemAtLocation;
				if (itemAtLocation != null)
				{
					_startItemVisitor.Count = 0;
					itemAtLocation.AcceptVisitor(_startItemVisitor);
					return _startItemVisitor.Count;
				}
				_endItemVisitor.Count = 0;
				IAbstractMarkupData abstractMarkupData = _currentLocation.BottomLevel.Parent as IAbstractMarkupData;
				if (abstractMarkupData != null)
				{
					abstractMarkupData.AcceptVisitor(_endItemVisitor);
					return _endItemVisitor.Count;
				}
				return 0;
			}
		}

		public int CharactersToPreviousLocation
		{
			get
			{
				Location location = (Location)_currentLocation.Clone();
				if (!location.MovePrevious())
				{
					return 0;
				}
				CharacterCountingIterator characterCountingIterator = new CharacterCountingIterator(location, _startCounterFactory, _endCounterFactory);
				return characterCountingIterator.CharactersToNextLocation;
			}
		}

		public CharacterCountingIterator(Location startLocation)
			: this(startLocation, () => new StartOfItemCharacterCounterVisitor(), () => new EndOfItemCharacterCounterVisitor())
		{
		}

		public CharacterCountingIterator(Location startLocation, Func<ICharacterCountingVisitor> startCounterFactory, Func<ICharacterCountingVisitor> endCounterFactory)
		{
			_startCounterFactory = startCounterFactory;
			_endCounterFactory = endCounterFactory;
			_currentLocation = startLocation;
			_startItemVisitor = startCounterFactory();
			_endItemVisitor = endCounterFactory();
		}

		public bool MoveNext()
		{
			_characterCount += CharactersToNextLocation;
			return _currentLocation.MoveNext();
		}

		public bool MovePrevious()
		{
			_characterCount -= CharactersToPreviousLocation;
			return _currentLocation.MovePrevious();
		}
	}
}
