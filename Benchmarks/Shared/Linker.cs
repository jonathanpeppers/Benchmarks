using System;
using Mono.Linker;

namespace MonoDroid.Tuner
{
	class Linker
	{
		public static I18nAssemblies ParseI18nAssemblies (string i18n)
		{
			if (string.IsNullOrWhiteSpace (i18n))
				return I18nAssemblies.None;

			var assemblies = I18nAssemblies.None;

			foreach (var part in i18n.Split (new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)) {
				var assembly = part.Trim ();
				if (string.IsNullOrEmpty (assembly))
					continue;

				try {
					assemblies |= (I18nAssemblies) Enum.Parse (typeof (I18nAssemblies), assembly, true);
				} catch {
					throw new FormatException ("Unknown value for i18n: " + assembly);
				}
			}

			return assemblies;
		}
	}
}
