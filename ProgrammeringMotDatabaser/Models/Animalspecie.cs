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
                
        public Animalclass AnimalClass { get; set; }

        
        public override string ToString()
        {
            return $"{AnimalSpecieName}  {LatinName}"; 
        }


        //vi borde kunna skapa en lista av class id och class namn här 

    }
}
