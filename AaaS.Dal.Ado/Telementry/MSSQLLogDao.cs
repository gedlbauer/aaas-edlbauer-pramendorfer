﻿using AaaS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AaaS.Dal.Ado.Telementry
{
    public class MSSQLLogDao : AdoLogDao
    {
        protected override string LastInsertedIdQuery => "SELECT CAST(scope_identity() AS int)";

        public MSSQLLogDao(IConnectionFactory factory) : base(factory, new MSSQLClientDao(factory)) { }
    }
}
