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
        public int Character_id { get; set; } //eventuellt om vi slår ihop animal i logiska modellen till animal + character så det blir 
                                              // animal id (pk) animal_specie_id(fk) character_name, då ändrar vi denna till Animal_id
        /// <summary>
        /// the name of the character, can be null
        /// </summary>
        public string Character_name { get; set; }


      
    }
}
