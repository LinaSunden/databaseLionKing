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

        public string Display => $"{Animalspecie.AnimalSpecieName} Count: {AnimalId}";

        public string Display1 => $"You have successfully created {CharacterName} who is a {Animalspecie.AnimalSpecieName} with animal id: {AnimalId}";

        public string CountSpeciesInClass => $"{Animalspecie.Animalclass.AnimalClassName} Count: {Animalspecie.AnimalSpecieId}";

        public int AnimalSpecieId { get; set; }

        public Animalspecie Animalspecie { get; set; }

        public override string ToString()
        {
            return $" {CharacterName} Animal Id: {AnimalId}, Animal specie: {Animalspecie.AnimalSpecieName} {Animalspecie.Animalclass}";
        }


    }

    
}
