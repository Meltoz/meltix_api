using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public static class SlugGenerator
    {
        public static string Generate(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return string.Empty;

            var withoutExtension = title.Split('.')[0];

            return withoutExtension
                .ToLowerInvariant()
                .Replace(" ", "-")
                .Replace("'", "")
                .Normalize(NormalizationForm.FormD);
        }
    }
}
