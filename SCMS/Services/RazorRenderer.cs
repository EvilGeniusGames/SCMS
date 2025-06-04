using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;

namespace SCMS.Services
{
    public class RazorRenderer
    {
        private readonly IRazorViewEngine _viewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;

        public RazorRenderer(IRazorViewEngine viewEngine,
                             ITempDataProvider tempDataProvider,
                             IServiceProvider serviceProvider)
        {
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
        }

        public async Task<string> RenderViewAsync<TModel>(HttpContext httpContext, string viewPath, TModel model, Dictionary<string, object>? viewData = null)
        {
            var actionContext = new ActionContext(httpContext, httpContext.GetRouteData(), new ActionDescriptor());

            using var sw = new StringWriter();

            var viewResult = _viewEngine.GetView(viewPath, viewPath, false);
            if (!viewResult.Success) throw new FileNotFoundException($"View '{viewPath}' not found.");

            var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
            {
                Model = model
            };

            if (viewData != null)
            {
                foreach (var kv in viewData)
                {
                    viewDictionary[kv.Key] = kv.Value;
                }
            }


            var viewContext = new ViewContext(actionContext, viewResult.View, viewDictionary,
                new TempDataDictionary(httpContext, _tempDataProvider), sw, new HtmlHelperOptions());

            await viewResult.View.RenderAsync(viewContext);
            return sw.ToString();
        }
    }

}
