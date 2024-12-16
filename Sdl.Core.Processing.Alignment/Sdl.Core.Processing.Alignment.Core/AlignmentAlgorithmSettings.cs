using Sdl.Core.Processing.Alignment.Common;
using Sdl.LanguagePlatform.Core.Resources;
using System;
using System.Globalization;

namespace Sdl.Core.Processing.Alignment.Core
{
	internal class AlignmentAlgorithmSettings
	{
		internal const AlignmentAlgorithmType DefaultAlgorithmType = AlignmentAlgorithmType.Fast;

		internal const bool DefaultUpdateBilingualDictionary = true;

		internal const double DefaultTagWeight = 0.5;

		private readonly CultureInfo _leftCulture;

		private readonly CultureInfo _rightCulture;

		private IBilingualDictionary _bilingualDictionary;

		private bool _updateBilingualDictionary;

		private double _tagWeight;

		private bool _useDocumentStructure;

		private IResourceDataAccessor _resourceDataAccessor;

		public CultureInfo LeftCulture => _leftCulture;

		public CultureInfo RightCulture => _rightCulture;

		public AlignmentAlgorithmType AlgorithmType
		{
			get;
			set;
		}

		public IBilingualDictionary BilingualDictionary
		{
			get
			{
				return _bilingualDictionary;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("BilingualDictionary");
				}
				if (!object.Equals(value.SourceCulture.Name, LeftCulture.Name))
				{
					throw new ArgumentException("BilingualDictionary must have same source culture as alignment settings", "BilingualDictionary");
				}
				if (!object.Equals(value.TargetCulture.Name, RightCulture.Name))
				{
					throw new ArgumentException("BilingualDictionary must have same target culture as alignment settings", "BilingualDictionary");
				}
				_bilingualDictionary = value;
			}
		}

		public bool UpdateBilingualDictionary
		{
			get
			{
				return _updateBilingualDictionary;
			}
			set
			{
				_updateBilingualDictionary = value;
			}
		}

		public bool UseDocumentStructure
		{
			get
			{
				return _useDocumentStructure;
			}
			set
			{
				_useDocumentStructure = value;
			}
		}

		public double TagWeight
		{
			get
			{
				return _tagWeight;
			}
			set
			{
				if (value < 0.0)
				{
					throw new ArgumentOutOfRangeException("TagWeight", "TagWeight must be > 0.0");
				}
				if (value > 1.0)
				{
					throw new ArgumentOutOfRangeException("TagWeight", "TagWeight must be <= 1.0");
				}
				_tagWeight = value;
			}
		}

		public IResourceDataAccessor ResourceDataAccessor
		{
			get
			{
				return _resourceDataAccessor;
			}
			set
			{
				_resourceDataAccessor = value;
			}
		}

		public AlignmentAlgorithmSettings(CultureInfo leftCulture, CultureInfo rightCulture)
		{
			if (leftCulture == null)
			{
				throw new ArgumentNullException("leftCulture");
			}
			if (rightCulture == null)
			{
				throw new ArgumentNullException("rightCulture");
			}
			_leftCulture = leftCulture;
			_rightCulture = rightCulture;
			AlgorithmType = AlignmentAlgorithmType.Fast;
			_updateBilingualDictionary = true;
			_bilingualDictionary = new DefaultBilingualDictionary(leftCulture, rightCulture);
			_tagWeight = 0.5;
			_useDocumentStructure = true;
		}
	}
}
