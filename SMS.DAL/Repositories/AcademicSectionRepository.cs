﻿using Microsoft.EntityFrameworkCore;
using SMS.DAL.Contracts;
using SMS.DAL.Contracts.Base;
using SMS.DAL.Repositories.Base;
using SMS.DB;
using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.DAL.Repositories
{
    public class AcademicSectionRepository : Repository<AcademicSection>, IAcademicSectionRepositoy 
    {
        
        public AcademicSectionRepository(ApplicationDbContext db):base(db)
        {

        }
        public override async Task<IReadOnlyCollection<AcademicSection>> GetAllAsync()
        {
            return await _context.AcademicSection
                .Include(m => m.AcademicClass)
                .Include(m => m.AcademicSession)
                .ToListAsync();
        }

        public async Task<IReadOnlyCollection<AcademicSection>> GetAllByClassWithSessionId(int classId, int sessionId)
        {
            return await _context.AcademicSection
                .Where(s => s.AcademicClassId == classId && s.AcademicSessionId == sessionId)
                .ToListAsync();
        }
    }
}
