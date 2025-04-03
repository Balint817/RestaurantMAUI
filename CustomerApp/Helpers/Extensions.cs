using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerApp.Helpers
{
    public static class Extensions
    {
        public static ImageSource? ToImageSourceFromUrl(this string url)
        {
            if (!Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
            {
                return null;
            }
            try
            {
                return new UriImageSource()
                {
                    Uri = new(url),
                    CachingEnabled = true,
                    CacheValidity = TimeSpan.FromDays(1)
                };
            }
            catch (Exception ex)
            {
                Debug.Fail(ex.ToString());
                return null;
            }
        }
    }
}
