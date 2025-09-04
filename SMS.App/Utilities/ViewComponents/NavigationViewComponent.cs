using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using SMS.App.ViewModels.ModuleSubModuleVM;
using System.Collections.Generic;
using System.Linq;

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
        var user = _httpContextAccessor.HttpContext?.User;
        if (user == null) return View(new SiteMap { Modules = new List<Module>() });

        // Get current route for active highlighting
        var routeData = _httpContextAccessor.HttpContext?.GetRouteData();
        var currentController = routeData?.Values["controller"]?.ToString();
        var currentAction = routeData?.Values["action"]?.ToString();

        ViewData["CurrentController"] = currentController;
        ViewData["CurrentAction"] = currentAction;

        // Filter modules based on Permission
        var filteredModules = _siteMap.Modules
            .Where(m => string.IsNullOrEmpty(m.Permission) || user.HasClaim("Permission", m.Permission))
            .Select(m => new Module
            {
                SystemName = m.SystemName,
                DisplayName = m.DisplayName,
                Icon = m.Icon,
                Permission = m.Permission,
                Submodules = m.Submodules.Select(s => new Submodule
                {
                    SystemName = s.SystemName,
                    DisplayName = s.DisplayName,
                    Icon = s.Icon,
                    Items = s.Items
                        .Where(i => string.IsNullOrEmpty(i.Claim) || user.HasClaim("Permission", i.Claim))
                        .ToList()
                }).ToList()
            })
            .ToList();

        var filteredSiteMap = new SiteMap { Modules = filteredModules };

        return View(filteredSiteMap);
    }
}
