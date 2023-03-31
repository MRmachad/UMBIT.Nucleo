using Microsoft.AspNetCore.Mvc.Razor;
using System.Collections.Generic;
using System.Linq;

namespace UMBIT.Nucleo.Configurate.LocationExpander
{
    internal class UMBITViewLocationExpander : IViewLocationExpander
    {
        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            string text = context.Values.ContainsKey("modulo") ? context.Values["modulo"] : null;
            if (!string.IsNullOrWhiteSpace(text))
            {
                string[] first = new string[2]
                {
                    "/" + text + "/Views/{1}/{0}.cshtml",
                    "/" + text + "/Views/Shared/{0}.cshtml"
                };
                viewLocations = first.Concat(viewLocations);
            }

            return viewLocations;
        }

        public void PopulateValues(ViewLocationExpanderContext context)
        {
            string value = context.ActionContext.RouteData?.Values["modulo"]?.ToString();
            if (!string.IsNullOrEmpty(value))
            {
                context.Values.Add("modulo", value);
            }
        }
    }
}