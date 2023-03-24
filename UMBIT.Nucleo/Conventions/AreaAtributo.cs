using Microsoft.AspNetCore.Mvc;

namespace UMBIT.Nucleo.Conventions
{
    public class AreaAtributo : AreaAttribute
    {
        public AreaAtributo(string areaName) : base(areaName)
        {
        }
    }
}
