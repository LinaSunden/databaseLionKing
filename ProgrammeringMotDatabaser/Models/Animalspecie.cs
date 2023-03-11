using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammeringMotDatabaser.Models
{
    internal class Animalspecie
    {
        public int AnimalSpecieId { get; set; }

        public string AnimalSpecieName { get; set; }

        public string LatinName { get; set; }

        public string AnimalClassName { get; set; }

        public Animalclass AnimalClassId { get; set; }    

        


    }
}
