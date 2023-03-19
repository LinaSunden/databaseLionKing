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
        /// <summary> 
        /// primary key
        /// </summary>
        private readonly string _connectionString;

        public Animal()  //constructor, access to connectionstring
        {
            var config = new ConfigurationBuilder().AddUserSecrets<DbRepository>().Build();
            _connectionString = config.GetConnectionString("develop");
        }

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


        public async Task<IEnumerable<Animal>> GetAllAnimals()
        {
            List<Animal> animals = new List<Animal>();

            var sqlJoin = "SELECT a.animalid, a.charactername, s.animalspecieid, s.animalspeciename, s.latinname, c.animalclassid, c.animalclassname FROM animal a JOIN animalspecie s ON s.animalspecieid = a.animalspecieid JOIN animalclass c ON c.animalclassid = s.animalclassid GROUP BY a.animalid, s.animalspeciename,  s.latinname, c.animalclassname,  s.animalspecieid, c.animalclassid ORDER BY animalspeciename ASC";

            await using var dataSource = NpgsqlDataSource.Create(_connectionString);
            await using var command = dataSource.CreateCommand(sqlJoin);
            await using var reader = await command.ExecuteReaderAsync();

            Animal animal = new Animal();
            while (await reader.ReadAsync())
            {
                animal = new()
                {
                    AnimalId = reader.GetInt32(0),
                    CharacterName = reader["charactername"] == DBNull.Value ? null : (string)reader["charactername"],


                    AnimalSpecie = new()
                    {
                        AnimalSpecieId = reader.GetInt32(2),
                        AnimalSpecieName = (string)reader["animalspeciename"],
                        LatinName = reader["latinname"] == DBNull.Value ? null : (string)reader["latinname"],

                        AnimalClass = new()
                        {
                            AnimalClassId = reader.GetInt32(5),
                            AnimalClassName = (string)reader["animalclassname"]

                        }
                    }
                };
                animals.Add(animal);
            }
            return animals;
        }
    }

    
}
