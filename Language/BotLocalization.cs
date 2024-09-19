using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Language
{
    public class BotLocalization
    {
        private ResourceManager resourceManager;

        public BotLocalization(string languageCode)
        {
            CultureInfo culture = new CultureInfo(languageCode);
            resourceManager = new ResourceManager("Language.Resources.String", typeof(BotLocalization).Assembly);
            CultureInfo.CurrentUICulture = culture;
        }

        public string GetLocalizedString(string key, params object[] args)
        {
            string localizedString = resourceManager.GetString(key);
            if (localizedString != null)
            {
                return string.Format(localizedString, args);
            }
            return key;
        }
    }
}
