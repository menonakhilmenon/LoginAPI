using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace LoginAPI
{
    public interface IDatabaseHelper
    {
        public string CnnVal(string name);
        public IDbConnection Connection { get; }
    }
}
