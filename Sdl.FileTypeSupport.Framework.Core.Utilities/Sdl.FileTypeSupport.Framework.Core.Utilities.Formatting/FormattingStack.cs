using Sdl.FileTypeSupport.Framework.Formatting;
using System;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.Core.Utilities.Formatting
{
	[Serializable]
	public class FormattingStack : List<IFormattingGroup>, ICloneable
	{
		public IFormattingGroup EffectiveFormatting
		{
			get
			{
				FormattingGroup formattingGroup = new FormattingGroup();
				using (Enumerator enumerator = GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						IFormattingGroup current = enumerator.Current;
						formattingGroup.UnderrideWith(current);
					}
					return formattingGroup;
				}
			}
		}

		public FormattingStack()
		{
		}

		protected FormattingStack(FormattingStack other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			IFormattingGroup[] array = other.ToArray();
			for (int num = other.Count - 1; num >= 0; num--)
			{
				Push((IFormattingGroup)array[num].Clone());
			}
		}

		public virtual void Push(IFormattingGroup newLayer)
		{
			Insert(0, newLayer);
		}

		public virtual IFormattingGroup Pop()
		{
			if (base.Count > 0)
			{
				IFormattingGroup result = base[0];
				RemoveAt(0);
				return result;
			}
			return null;
		}

		public virtual object Clone()
		{
			return new FormattingStack(this);
		}
	}
}
