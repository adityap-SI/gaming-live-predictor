using Microsoft.AspNetCore.Mvc;
using System;

namespace ICC.Predictor.Admin.ViewComponents
{
    public class MessageViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string component, object message)
        {
            if (!string.IsNullOrEmpty(component))
                return View($"/Views/Partial/Message/{component}.cshtml", message);
            else
                return Content("");
        }
    }
}