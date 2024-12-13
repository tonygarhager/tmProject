namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// This interface is implemented by all objects that can be secured using permissions
	/// and allows the user to query whether the object has a certain permission.
	/// </summary>
	public interface IPermissionCheck
	{
		/// <summary>
		/// Gets whether this object has the permission with the specified name.
		/// </summary>
		/// <param name="permission">The permission name.</param>
		/// <returns><code>true</code> is the object has the specified permission.</returns>
		bool HasPermission(string permission);
	}
}
