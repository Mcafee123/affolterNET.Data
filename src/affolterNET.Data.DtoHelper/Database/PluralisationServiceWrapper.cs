using System.Globalization;
using affolterNET.Data.DtoHelper.Interfaces;
using PluralizationService;
using PluralizationService.English;

namespace affolterNET.Data.DtoHelper.Database
{
    public class PluralizationServiceWrapper : IPluralizer
    {
        private readonly IPluralizationApi api;

        private readonly CultureInfo cultureInfo;

        public PluralizationServiceWrapper(CultureInfo ci = null)
        {
            var builder = new PluralizationApiBuilder();
            builder.AddEnglishProvider();

            api = builder.Build();
            if (ci == null)
            {
                ci = new CultureInfo("en-US");
            }

            cultureInfo = ci;
        }

        public string Pluralize(string name)
        {
            return api.Pluralize(name, cultureInfo) ?? name;
        }

        public string Singularize(string name)
        {
            return api.Singularize(name, cultureInfo) ?? name;
        }
    }
}