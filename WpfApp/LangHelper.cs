using System.Globalization;
using System.Threading;

namespace Leosac.WpfApp
{
    public class LangHelper
    {
        public static void ChangeLanguage(string lang)
        {
            var culture = CultureInfo.GetCultureInfo(lang);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }
    }
}
