using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using SMS.App.ViewModels.ModuleSubModuleVM;
using SMS.BLL.Contracts;
using SMS.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.App.Utilities.ViewComponents;

public class NavigationViewComponent : ViewComponent
{
    private readonly SiteMap _siteMap;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IProjectModuleManager _projectModuleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public NavigationViewComponent(
        SiteMap siteMap,
        IHttpContextAccessor httpContextAccessor,
        IProjectModuleManager projectModuleManager,
        UserManager<ApplicationUser> userManager)
    {
        _siteMap = siteMap;
        _httpContextAccessor = httpContextAccessor;
        _projectModuleManager = projectModuleManager;
        _userManager = userManager;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user == null)
            return View(new SiteMap { Modules = new List<Module>() });

        // Get all active modules from DB
        var modules = await _projectModuleManager.GetAllAsync();
        var allActiveModules = modules.Where(m => m.Status).ToList();

        // Filter sitemap modules by active DB modules
        var activeModulesFromSiteMap = _siteMap.Modules
            .Where(m => allActiveModules.Any(am => am.ModuleName == m.SystemName))
            .ToList();

        // Collect user claims (values only)
        var userClaims = user.Claims
            .Select(c => c.Value)
            .ToHashSet();

        // Get current route for active highlighting
        var routeData = _httpContextAccessor.HttpContext?.GetRouteData();
        var currentController = routeData?.Values["controller"]?.ToString();
        var currentAction = routeData?.Values["action"]?.ToString();

        ViewData["CurrentController"] = currentController;
        ViewData["CurrentAction"] = currentAction;

        // Filter modules → submodules → items
        var filteredModules = activeModulesFromSiteMap
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
                            .Where(i => string.IsNullOrEmpty(i.Claim) || user.Claims.Any(c => c.Value == i.Claim))
                            .ToList()
                }).ToList()
            })
            .ToList();

        var filteredSiteMap = new SiteMap { Modules = filteredModules };

        return View(filteredSiteMap);
    }
}
