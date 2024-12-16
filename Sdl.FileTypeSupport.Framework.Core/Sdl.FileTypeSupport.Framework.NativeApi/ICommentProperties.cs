using System;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.NativeApi
{
	public interface ICommentProperties : ICloneable
	{
		int Count
		{
			get;
		}

		IEnumerable<IComment> Comments
		{
			get;
		}

		string Xml
		{
			get;
			set;
		}

		void Add(IComment comment);

		void AddComments(ICommentProperties comments);

		IComment GetItem(int index);

		void Delete(IComment comment);
	}
}
