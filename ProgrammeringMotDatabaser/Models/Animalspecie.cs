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

        public int AnimalClassId { get; set; }

        public string Display => $"Total number  of animalspecies {AnimalSpecieId}";

        public Animalclass Animalclass { get; set; }

        
        public override string ToString()
        {
          
            
                return $"{AnimalSpecieName} {Animalclass}"; 



            //{Animalclass.AnimalClassName}";

            //else
            //{
            //    return $"{AnimalSpecieName} {LatinName}";
            //}


        }



    }
}
