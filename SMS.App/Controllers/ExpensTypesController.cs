using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem;
using SMS.App.ViewModels.Accounts;
using SMS.DAL.Contracts;
using SMS.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SMS.App.Controllers
{
    [Authorize(Roles = "SuperAdmin, Admin")]
    public class ExpensTypesController : Controller
    {
        private readonly IExpenseTypeRepository _expenseTypeRepository;
        public ExpensTypesController(IExpenseTypeRepository expenseTypeRepository)
        {
            _expenseTypeRepository = expenseTypeRepository;
        }
        // GET: ExpensTypesController
        public async Task<ActionResult> Index()
        {
            GlobalUI.PageTitle = "Expense Type";
            ExpenseTypesVM expenseTypesVM = new ExpenseTypesVM();

            List<ExpenseType> objExpenseTypes = (List<ExpenseType>)await _expenseTypeRepository.GetAllAsync();
            expenseTypesVM.ExpenseTypes = objExpenseTypes;
            return View(expenseTypesVM);
        }

        // GET: ExpensTypesController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ExpensTypesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ExpensTypesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ExpensTypesController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ExpensTypesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ExpensTypesController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ExpensTypesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
