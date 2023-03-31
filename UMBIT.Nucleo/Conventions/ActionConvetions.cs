using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace UMBIT.Nucleo.Conventions
{
    public class ActionConvetions : IActionModelConvention
    {
        public void Apply(ActionModel action)
        {
            action.Selectors.Add(new SelectorModel()
            {
                AttributeRouteModel = new AttributeRouteModel(new RouteAttribute(action.ActionName))
            });

            action.RouteValues.Add("modulo", action.Controller.ControllerType.Assembly.GetName().Name);
        }
    }
}
