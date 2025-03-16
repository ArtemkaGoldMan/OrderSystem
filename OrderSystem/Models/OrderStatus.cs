﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderSystem.Models
{
    public enum OrderStatus
    {
        New,
        InWarehouse,
        InShipping,
        ReturnedToCustomer,
        Error,
        Closed,
        Cancelled
    }
}
