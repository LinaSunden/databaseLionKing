using Microsoft.Extensions.Configuration;
using Npgsql;
using Npgsql.Internal.TypeHandlers.LTreeHandlers;
using ProgrammeringMotDatabaser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ProgrammeringMotDatabaser.DAL
{
    internal class DbRepository
    {
        private readonly string _connectionString;

        public DbRepository()  //constructor, access to connectionstring
        {
            var config = new ConfigurationBuilder().AddUserSecrets<DbRepository>().Build();
            _connectionString = config.GetConnectionString("develop");
        }

        /// <summary>
        /// Method that searches for a specific animalname
        /// </summary>
        /// <param name="characterName"></param>
        /// <returns></returns>
        public async Task<Animal> GetAnimalByName(string characterName)
        {
            
                string sqlQuestion = $"SELECT * FROM animal WHERE charactername= @name";

                await using var dataSource = NpgsqlDataSource.Create(_connectionString);
                await using var command = dataSource.CreateCommand(sqlQuestion);
                command.Parameters.AddWithValue("name", characterName);
                await using var reader = await command.ExecuteReaderAsync();


                Animal animal = new Animal();
                while (await reader.ReadAsync()) 
                {
                    animal = new Animal()

                    {
                        AnimalId = reader.GetInt32(0),
                        CharacterName = (string)reader["charactername"],
                    };

                    
                }
         
               
            return animal;

        }








        /// <summary>
        /// Method to add animalnam and animalspecieid, connected to the task of creating a new animal with connection to specie and class 
        /// </summary>
        /// <param name="animal"></param>
        /// <returns></returns>
        public async Task AddAnimal(Animal animal)  //Adds a name to an animal and gives it specieID and animalID 
        {
            string sqlCommand = "insert into animal(charactername, animalspecieid) values(@charactername, @animalspecieid)";

            await using var dataSource = NpgsqlDataSource.Create(_connectionString);
            await using var command = dataSource.CreateCommand(sqlCommand);
            command.Parameters.AddWithValue("charactername", animal.CharacterName);
            command.Parameters.AddWithValue("animalspecieid", animal.AnimalSpecieid);
            await command.ExecuteNonQueryAsync();

        }

        public async Task AddAnimalClass()
        {





        }



        /// <summary>
        /// method to retrieve the database values in animalspecienames and display in combobox
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Animalspecie>> AddAnimalSpecieToCombox()
        {

            List<Animalspecie> animalspecies = new List<Animalspecie>();

            string sqlCommand = "SELECT * FROM animalspecie";
            await using var dataSource = NpgsqlDataSource.Create(_connectionString);

            await using var command = dataSource.CreateCommand(sqlCommand);
            await using var reader = await command.ExecuteReaderAsync();

            Animalspecie animalspecie = new Animalspecie();

            while (await reader.ReadAsync())
            {
                animalspecie = new Animalspecie()
                {
                    AnimalSpecieId = reader.GetInt32(0),
                    AnimalSpecieName = (string)reader["animalspeciename"],
                    AnimalClassName = (string)reader["animalclassname"]

                };

                animalspecies.Add(animalspecie);

            }

            return animalspecies;
        }



    }
}