using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STOL_Training_Tool.Model
{
    public class Ident
    {
        public string Model { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }

        public string ToString()
        {
            return $"{this.Type}|{this.Model}|{this.Title}";
        }
    }
}
