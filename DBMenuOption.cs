using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Hashing_Test
{
    public class DBMenuOption
    {
        public string Name { get; set; }
        public Action Function { get; set; }

        public DBMenuOption(string name, Action function)
        {
            Name = name;

            Function = function;
        }
    }
}
