﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine._98.Headers
{
    public class GENERIC_MGR<T> where T : GENERIC_MGR<T>, new()
    {
        private static readonly Lazy<T> _instance = new Lazy<T>(() => new T());//쓰레드 환경에서 안전한 Lazy 
        public static T GetInstance() => _instance.Value;

    }
}
