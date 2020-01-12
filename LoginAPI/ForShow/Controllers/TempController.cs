using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForShow.Controllers
{
    public class TempController : ControllerBase
    {
        [HttpGet]
        [Route("test")]
        public string Hello()
        {
            return "Testing from inside";
        }
        [HttpPost]
        [Route("test")]
        public TestClass Byebye(Test val)
        {
            return new TestClass() { val = val.val,str = "Hello boy!" };
        }       
        public class Test 
        {
            public int val;
        }
    }
}
