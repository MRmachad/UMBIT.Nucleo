using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.Collections.Generic;
using System.Linq;
using System;

namespace UMBIT.Nucleo.Configurate
{
    public class UMBITViewEngine : IViewEngine
    {
        private IEnumerable<object> _viewLocationFormats;

        public ViewEngineResult FindView(ActionContext context, string viewName, bool isMainPage)
        {
            throw new Exception("Controller route value not found.");
        }

        public ViewEngineResult GetView(string executingFilePath, string viewPath, bool isMainPage)
        {


            return ViewEngineResult.NotFound(viewPath, new List<string> { "0" });
        }
    }
}
