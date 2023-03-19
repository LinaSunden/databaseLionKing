using Microsoft.Extensions.Configuration;
using Npgsql;
using ProgrammeringMotDatabaser.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammeringMotDatabaser.Models
{
    internal class Animal
    {
        
        public int AnimalId { get; set; } 
                                              
        /// <summary>
        /// the name of the character, can be null
        /// </summary>
        public string CharacterName { get; set; }

        public string Display => $"{AnimalSpecie.AnimalSpecieName} Count: {AnimalId}";

        public string Display1 => $"You have successfully created {CharacterName} who is a {AnimalSpecie.AnimalSpecieName} with animal id: {AnimalId}";

        public string CountSpeciesInClass => $"{AnimalSpecie.AnimalClass.AnimalClassName} Count: {AnimalSpecie.AnimalSpecieId}";

        public string AllAnimalsSortedBySpecie => $"{CharacterName} {AnimalSpecie.AnimalSpecieName} {AnimalSpecie.LatinName} {AnimalSpecie.AnimalClass.AnimalClassName}";

        //public int AnimalSpecieId { get; set; }  

        public AnimalSpecie AnimalSpecie { get; set; }

        public override string ToString()
        {
            return $"Charactername: {CharacterName}, Animal specie: {AnimalSpecie.AnimalSpecieName} Latin name: {AnimalSpecie.LatinName} Animal class name: {AnimalSpecie.AnimalClass.AnimalClassName}";
        }


       
    }

    
}
