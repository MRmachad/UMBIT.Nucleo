using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ObjectiveC;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace UMBIT.Nucleo.Conventions
{
    public class ControllerConventions : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {

            controller.Selectors.Add(new SelectorModel() {
                AttributeRouteModel = new AttributeRouteModel(new RouteAttribute(controller.DisplayName.Substring(0,10).Replace('.','-').ToString()+ "/{action=Index}/{id?}"))
            ,
            });

        }
    }
}
