using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade
{
	internal class OutputTranslationMemoryCollection : Collection<IOutputTranslationMemory>, IOutputTranslationMemoryCollection, ICollection<IOutputTranslationMemory>, IEnumerable<IOutputTranslationMemory>, IEnumerable
	{
		public IOutputTranslationMemory Add()
		{
			OutputTranslationMemory outputTranslationMemory = new OutputTranslationMemory();
			Add(outputTranslationMemory);
			return outputTranslationMemory;
		}

		public IOutputTranslationMemory Add(IInputTranslationMemory inputTranslationMemory, bool autoPopulateOutputTranslationMemorySetup)
		{
			return Add(new IInputTranslationMemory[1]
			{
				inputTranslationMemory
			}, autoPopulateOutputTranslationMemorySetup);
		}

		public IOutputTranslationMemory Add(IEnumerable<IInputTranslationMemory> inputTranslationMemories, bool autoPopulateOutputTranslationMemorySetup)
		{
			OutputTranslationMemory outputTranslationMemory = new OutputTranslationMemory();
			foreach (IInputTranslationMemory inputTranslationMemory in inputTranslationMemories)
			{
				if (inputTranslationMemory.Setup.LanguageDirections.Length > 1)
				{
					throw new ArgumentException($"Translation memories with multiple language pairs are not supported by this method. Add the individual language pairs manually ({inputTranslationMemory.TranslationMemory.Url}).");
				}
				outputTranslationMemory.InputLanguageDirections.Add(inputTranslationMemory, inputTranslationMemory.Setup.LanguageDirections[0]);
			}
			if (autoPopulateOutputTranslationMemorySetup)
			{
				outputTranslationMemory.PopulateSetup();
			}
			Add(outputTranslationMemory);
			return outputTranslationMemory;
		}
	}
}
