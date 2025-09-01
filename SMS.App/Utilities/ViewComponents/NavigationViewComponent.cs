using Microsoft.AspNetCore.Mvc;
using SMS.App.ViewModels.ModuleSubModuleVM;

namespace SMS.App.Utilities.ViewComponents;

public class NavigationViewComponent : ViewComponent
{
    private readonly SiteMap _siteMap;

    public NavigationViewComponent(SiteMap siteMap)
    {
        _siteMap = siteMap;
    }

    public IViewComponentResult Invoke()
    {
        // Pass sitemap to view
        return View(_siteMap);
    }
}
