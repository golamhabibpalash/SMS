﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.Entities
{
    public class Gender : CommonProps
    {
        public string Name { get; set; }
        public bool Status { get; set; }

    }
}
