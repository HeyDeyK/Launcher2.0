using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrvniWPF
{
    class ExeSoubory
    {
        private string name;

        public ExeSoubory(string name)
        {
            this.name = name;
        }
        public override string ToString()
        {
            return name;
        }
    }
}
