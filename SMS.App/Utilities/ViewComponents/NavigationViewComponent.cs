using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using SMS.App.ViewModels.ModuleSubModuleVM;

namespace SMS.App.Utilities.ViewComponents;

public class NavigationViewComponent : ViewComponent
{
    private readonly SiteMap _siteMap;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public NavigationViewComponent(SiteMap siteMap, IHttpContextAccessor httpContextAccessor)
    {
        _siteMap = siteMap;
        _httpContextAccessor = httpContextAccessor;
    }

    public IViewComponentResult Invoke()
    {
        var routeData = _httpContextAccessor.HttpContext?.GetRouteData();
        var currentController = routeData?.Values["controller"]?.ToString();
        var currentAction = routeData?.Values["action"]?.ToString();

        ViewData["CurrentController"] = currentController;
        ViewData["CurrentAction"] = currentAction;

        return View(_siteMap);
    }
}
