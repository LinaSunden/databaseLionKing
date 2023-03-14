using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammeringMotDatabaser.Models
{
    internal class Animalclass
    {
        public int AnimalClassId { get; set; }

        public string AnimalClassName { get; set; }

        public override string ToString()
        {
            return $" {AnimalClassName}";
        }




    }
}
