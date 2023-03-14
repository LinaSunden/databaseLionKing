using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammeringMotDatabaser.Models
{
    internal class Animal
    {
        /// <summary> 
        /// primary key
        /// </summary>
        public int AnimalId { get; set; } 
                                              
        /// <summary>
        /// the name of the character, can be null
        /// </summary>
        public string CharacterName { get; set; }

        public int AnimalSpecieId { get; set; }

        public Animalspecie Animalspecie { get; set; }

        
    }
}
