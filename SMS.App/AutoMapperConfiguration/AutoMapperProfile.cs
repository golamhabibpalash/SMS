using AutoMapper;
using SMS.App.ViewModels.Students;
using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.App.AutoMapperConfiguration
{
    public class AutoMapperProfile:Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<StudentCreateVM, Student>();
            CreateMap<Student, StudentCreateVM>();
        }
    }
}
