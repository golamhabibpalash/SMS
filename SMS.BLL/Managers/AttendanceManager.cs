﻿using BLL.Managers.Base;
using SMS.BLL.Contracts;
using SMS.DAL.Contracts;
using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.BLL.Managers
{
    public class AttendanceManager : Manager<Attendance>, IAttendanceManager
    {
        private readonly IAttendanceRepository _attendanceRepository;
        public AttendanceManager(IAttendanceRepository attendanceRepository) : base(attendanceRepository)
        {
            _attendanceRepository = attendanceRepository;
        }

        public async Task<List<Attendance>> GetTodaysAllAttendanceAsync()
        {
            return await _attendanceRepository.GetTodaysAllAttendanceAsync();
        }
    }
}
