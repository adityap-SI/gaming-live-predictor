using Microsoft.AspNetCore.Mvc;
using System;

namespace ICC.Predictor.Admin.ViewComponents
{
    public class ControlsViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string component, object model)
        {
            return View($"/Views/Partial/{component}.cshtml", model);
        }
    }
}