using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SMS.App.ViewModels.QuestionBank;
using SMS.BLL.Contracts;
using System.Threading.Tasks;

namespace SMS.App.Utilities.ViewComponents
{
    public class QuestionCreateViewComponent : ViewComponent
    {
        private readonly IAcademicClassManager _academicClassManager;
        public QuestionCreateViewComponent(IAcademicClassManager academicClassManager)
        {
            _academicClassManager = academicClassManager;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            ViewData["AcademicClassId"] = new SelectList(await _academicClassManager.GetAllAsync(), "Id", "Name");
            return View("Create",new QuestionVM());
        }
    }
}
