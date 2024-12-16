using Newtonsoft.Json;
using Sdl.Core.Bcm.BcmConverters.Common;
using Sdl.Core.Bcm.BcmModel;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using Sdl.FileTypeSupport.Framework.Integration;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.Native;
using Sdl.LanguagePlatform.Core.Resources;
using System.Globalization;
using System.Text;

namespace Sdl.Core.Bcm.BcmConverters
{
	public class TextToBcmConverter
	{
		private readonly string _text;

		private readonly CultureInfo _sourceCulture;

		private readonly CultureInfo _targetCulture;

		private readonly IResourceDataAccessor _accessor;

		public TextToBcmConverter(string text, CultureInfo sourceCulture, CultureInfo targetCulture)
		{
			_text = text;
			_sourceCulture = sourceCulture;
			_targetCulture = targetCulture;
		}

		public TextToBcmConverter(string text, IResourceDataAccessor accessor, CultureInfo sourceCulture, CultureInfo targetCulture)
			: this(text, sourceCulture, targetCulture)
		{
			_accessor = accessor;
		}

		public string ConvertToJson()
		{
			Document value = ConvertToBcmDocument();
			return JsonConvert.SerializeObject(value);
		}

		public Document ConvertToBcmDocument()
		{
			IFileExtractor fileExtractor = new FileExtractor();
			fileExtractor.FileConversionProperties.OriginalFilePath = "";
			fileExtractor.NativeExtractor = new NativeExtractor
			{
				Parser = new TextInjector
				{
					Text = _text,
					Encoding = Encoding.GetEncoding("UTF-8")
				}
			};
			MultiFileConverter multiFileConverter = new MultiFileConverter();
			multiFileConverter.DocumentInfo = multiFileConverter.ItemFactory.CreateDocumentProperties();
			multiFileConverter.DocumentInfo.SourceLanguage = new Language(_sourceCulture);
			multiFileConverter.DocumentInfo.TargetLanguage = new Language(_targetCulture);
			multiFileConverter.AddExtractor(fileExtractor);
			multiFileConverter.AddBilingualProcessor(new BilingualContentHandlerAdapter(DefaultBuilder.GetDefaultSegmentor(_accessor)));
			BcmExtractor bcmExtractor = new BcmExtractor();
			multiFileConverter.AddBilingualProcessor(new BilingualContentHandlerAdapter(bcmExtractor));
			multiFileConverter.Parse();
			multiFileConverter.Dispose();
			return bcmExtractor.OutputDocument;
		}
	}
}
