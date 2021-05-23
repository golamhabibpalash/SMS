using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolManagementSystem.Controllers
{
    public class ServicesController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ServicesController(ApplicationDbContext context)
        {
            _context = context;
        }



        public async Task<JsonResult> GetClassListBySessionId(int id)
        {
            var classList = await _context.AcademicClass
                .OrderBy(s => s.ClassSerial)
                .ToListAsync();
            return Json(classList);
        }
        public async Task<JsonResult> GetSectionListByClassId(int id)
        {
            var sectionList = await _context.AcademicSection
                .Where(s => s.AcademicClassId == id)
                .OrderBy(s => s.Name)
                .ToListAsync();
            return Json(sectionList);
        }
        public async Task<JsonResult> GetDistrictListByDivisionId(int id)
        {
            var districtList = await _context.District.
                Where(s => s.DivisionId == id)
                .OrderBy(d => d.Name)
                .ToListAsync();
            return Json(districtList);
        }
        public async Task<JsonResult> GetUpazilaListByDistrictId(int id)
        {
            var upazilaList = await _context.Upazila
                .Where(s => s.DistrictId == id)
                .OrderBy(s => s.Name)
                .ToListAsync();

            return Json(upazilaList);
        }
    }
}
