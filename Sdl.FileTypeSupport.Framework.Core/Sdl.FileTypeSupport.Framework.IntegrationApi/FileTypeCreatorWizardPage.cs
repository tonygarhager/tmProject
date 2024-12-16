namespace Sdl.FileTypeSupport.Framework.IntegrationApi
{
	public class FileTypeCreatorWizardPage
	{
		public string Id
		{
			get;
			private set;
		}

		public string Name
		{
			get;
			private set;
		}

		public string Description
		{
			get;
			private set;
		}

		public string HelpTopic
		{
			get;
			private set;
		}

		public IFileTypeDefinitionConfiguringControl Control
		{
			get;
			private set;
		}

		public FileTypeCreatorWizardPage(string id, string name, string description, string helpTopic, IFileTypeDefinitionConfiguringControl control)
		{
			Id = id;
			Name = name;
			Description = description;
			HelpTopic = helpTopic;
			Control = control;
		}
	}
}
