﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlCheck
{
    public class ReferCount<T1, T2>
    {
        public ReferCount(T1 obj, T2 count)
        {
            Obj = obj;
            Count = count;
        }
        public T1 Obj { get; set; }
        public T2 Count { get; set; }
    }
}
