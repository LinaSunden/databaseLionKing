using Microsoft.Extensions.Configuration;
using Npgsql;
using Npgsql.Internal.TypeHandlers.LTreeHandlers;
using ProgrammeringMotDatabaser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
        /// Method that searches for a specific animal by animalname, animal id, animalname, animalspeciename
        /// </summary>
        /// <param name="characterName"></param>
        /// <returns></returns>
        public async Task<Animal> GetAnimalByName(string characterName) //testa lowercase i metoden så att man kan söka på Simba och simba oavsett stor eller liten bokstav
                                                                        
        {
                string sqlQuestion = "SELECT * FROM animal JOIN animalspecie ON animalspecie.animalspecieid = animal.animalspecieid WHERE animal.charactername= @name";
                          
               
                await using var dataSource = NpgsqlDataSource.Create(_connectionString);
                await using var command = dataSource.CreateCommand(sqlQuestion);
                command.Parameters.AddWithValue("name", characterName);
                await using var reader = await command.ExecuteReaderAsync();


                Animal animal = new();
                while (await reader.ReadAsync()) 
                {
                    animal = new()

                    {
                        AnimalId = reader.GetInt32(0),
                        CharacterName = (string)reader["charactername"],

                        Animalspecie = new()
                        {
                            AnimalSpecieName = (string)reader["animalspeciename"]
                        }
                        
                    };
                                    
                }        
               
            return animal;
        }

       





        /// <summary>
        /// Method show all species sorted by animalspeciename. Used in comboboxes
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Animalspecie>> GetAnimalSpecie()
        {
            List <Animalspecie> animalSpecies = new List <Animalspecie>();
            string sqlQ = "SELECT * FROM animalspecie ORDER BY animalspeciename ASC";
            
            await using var dataSource = NpgsqlDataSource.Create(_connectionString);
            await using var command = dataSource.CreateCommand(sqlQ);
            await using var reader = await command.ExecuteReaderAsync();
            Animalspecie animalspecie = new Animalspecie();
            while (await reader.ReadAsync())
            {
           
                    animalspecie = new Animalspecie()
                    {
                        AnimalSpecieId = reader.GetInt32(0),
                        AnimalSpecieName = (string)reader["animalspeciename"],
                        AnimalClassId = reader.GetInt32(3)
                    };
            
                
                animalSpecies.Add(animalspecie);
            }
            return animalSpecies;
        }


        public async Task<IEnumerable<Animalspecie>> GetAnimalBySpeficClass(Animalclass animalclass)
        {
            List<Animalspecie> animalSpecies = new List<Animalspecie>();
            //string sqlQ = "SELECT * FROM animalspecie ORDER BY animalspeciename ASC";

            var sqlJoin = $"SELECT animal.animalid, animalspecie.animalspeciename, animalclass.animalclassname FROM animalclass JOIN animalspecie ON animalspecie.animalclassid = animalclass.animalclassid JOIN animal ON animal.animalspecieid = animalspecie.animalspecieid WHERE animalclass.animalclassname = @animalclassname ORDER BY animalspeciename ASC";

            await using var dataSource = NpgsqlDataSource.Create(_connectionString);
            await using var command = dataSource.CreateCommand(sqlJoin);
            command.Parameters.AddWithValue("animalclassname", animalclass.AnimalClassName);
            await using var reader = await command.ExecuteReaderAsync();        

            Animalspecie animalspecie = new Animalspecie();
            

            while (await reader.ReadAsync())  
            {
                animalspecie = new Animalspecie()
                {
                    AnimalSpecieName = (string)reader["animalspeciename"],
                    Animalclass = new()
                    {
                        AnimalClassName = (string)reader["animalclassname"]
                    }
                      //Försök få in Animalid också.               
                };
                animalSpecies.Add(animalspecie);
            }
            return animalSpecies;
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
            command.Parameters.AddWithValue("animalspecieid", animal.Animalspecie.AnimalSpecieId);
            await command.ExecuteNonQueryAsync();

        }

        public async Task AddAnimalClass(Animalclass animalclass)
        {
            string sqlCommand = "insert into animalclass(animalclassname) values(@animalclassname)";

            await using var dataSource = NpgsqlDataSource.Create(_connectionString);
            await using var command = dataSource.CreateCommand(sqlCommand);
            command.Parameters.AddWithValue("animalclassname", animalclass.AnimalClassName);
            await command.ExecuteNonQueryAsync();


        }

        public async Task AddAnimalSpecie(Animalspecie animalSpecie)
        {
            string sqlCommand = "insert into animalspecie(animalspeciename, animalclassid) values(@animalspeciename, @animalclassid)";

            await using var dataSource = NpgsqlDataSource.Create(_connectionString);
            await using var command = dataSource.CreateCommand(sqlCommand);
            command.Parameters.AddWithValue("animalspeciename", animalSpecie.AnimalSpecieName);
            command.Parameters.AddWithValue("animalclassid", animalSpecie.Animalclass.AnimalClassId);
            await command.ExecuteNonQueryAsync();


        }

        public async Task<IEnumerable<Animalclass>> GetAnimalClass()
        {
            List<Animalclass> animalClass = new List<Animalclass>();
            string sqlQ = "SELECT * FROM animalclass ";

            await using var dataSource = NpgsqlDataSource.Create(_connectionString);
            await using var command = dataSource.CreateCommand(sqlQ);
            await using var reader = await command.ExecuteReaderAsync();

            Animalclass animalclass = new Animalclass();
            while (await reader.ReadAsync())
            {
                animalclass = new Animalclass()
                {
                    AnimalClassId = reader.GetInt32(0),
                    AnimalClassName = (string)reader["animalclassname"]

                };
                animalClass.Add(animalclass);
            }
            return animalClass;
        }


        public async Task<IEnumerable<Animal>> GetAllAnimalsSortedBySpecie()
        {
            List<Animal> animals = new List<Animal>();
            string sqlQ = "SELECT a.animalid, s.animalspeciename, s.latinname FROM animal a JOIN animalspecie s ON s.animalspecieid = a.animalspecieid ORDER BY animalspeciename ASC";
                     


            await using var dataSource = NpgsqlDataSource.Create(_connectionString);
            await using var command = dataSource.CreateCommand(sqlQ);
            await using var reader = await command.ExecuteReaderAsync();

            Animal animal = new Animal();
            //Animal animal = new Animal();

            while (await reader.ReadAsync())
            {

                animal = new Animal()
                {
                    AnimalId = reader.GetInt32(0),

                    //AnimalSpecieId = reader.GetInt32(0),
                    Animalspecie = new()
                    {
                        AnimalSpecieName = (string)reader["animalspeciename"],                                       
                        
                    }

                    
                    
                    //LatinName = (string)reader["latinname"] //kolla hur man kan hantera nullvärden. 
                   
                   //hur ska vi nå animalid med C# kod.. Vi har animalspecieid som FK som kopplar ihop tabellerna Animal och Animalspecie.
                   //
                };

                //animal = new Animal()
                //{
                //    AnimalId = reader.GetInt32(0),

                //};

                //animalSpecies.Add(animal);

                animals.Add(animal);
            }
            return animals;
        }



    }
}