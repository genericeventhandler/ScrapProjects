using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrapProject
{
   public class TestClass
    {
       public void TestInfiniteLoop()
        {
            var x = 10;
            while (x-- > 0)
            {
                Console.WriteLine(".");
            }
        }
    }
}
