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
        /// the name of the animal, can be null
        /// </summary>
        public string CharacterName { get; set; }

        public AnimalSpecie AnimalSpecie { get; set; }


        #region DisplayMemberPath
        public string CreateAnimalSuccess => $"You have successfully created {CharacterName} who is a {AnimalSpecie.AnimalSpecieName} with animal id: {AnimalId}";

        public string CountAnimalsInClass => $"Class: {AnimalSpecie.AnimalClass.AnimalClassName} Count: {AnimalId}";

        public string DeleteAnimals => $"Animal id: {AnimalId}, Charactername: {CharacterName}, Specie: {AnimalSpecie.AnimalSpecieName}"; //refererar till att listan där vi väljer delete visas

        public string CountAnimalInEachSpecie => $"Specie: {AnimalSpecie.AnimalSpecieName} Count: {AnimalId}";

        public string AllAnimalsWithAName => $"Charactername: {CharacterName}, Specie: {AnimalSpecie.AnimalSpecieName}, Latin name: {AnimalSpecie.LatinName}, Class: {AnimalSpecie.AnimalClass.AnimalClassName}";

        public string AllInfoAboutAnimals => $"Charactername: {CharacterName}, Specie: {AnimalSpecie.AnimalSpecieName} Latin name: {AnimalSpecie.LatinName}, Class: {AnimalSpecie.AnimalClass.AnimalClassName}";

        public string AnimalsInEachClass => $"Animal id: {AnimalId}, Specie: {AnimalSpecie.AnimalSpecieName}, Class: {AnimalSpecie.AnimalClass.AnimalClassName}";

        #endregion



    }

    
}
