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
using System.Xml;

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
                string sqlQuestion = "SELECT * FROM animal JOIN animalspecie ON animalspecie.animalspecieid = animal.animalspecieid WHERE animal.charactername= @charactername";
                          
               
                await using var dataSource = NpgsqlDataSource.Create(_connectionString);
                await using var command = dataSource.CreateCommand(sqlQuestion);
                command.Parameters.AddWithValue("charactername", characterName);
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

        public async Task<Animalspecie> FindClass(Animalspecie selectAnimalspecie)
        {
            string sqlQuestion = "Select s.animalspeciename, c.animalclassname From animalspecie s Join animalclass c ON s.animalclassid = c.animalclassid Where animalspeciename = @speciename";

            
            await using var dataSource = NpgsqlDataSource.Create(_connectionString);
            await using var command = dataSource.CreateCommand(sqlQuestion);
            command.Parameters.AddWithValue("speciename", selectAnimalspecie.AnimalSpecieName);
            await using var reader = await command.ExecuteReaderAsync();

            Animalspecie animalspecie = new();
            while (await reader.ReadAsync())
            {
                animalspecie = new()

                {
                    AnimalSpecieName = (string)reader["animalspeciename"],

                    Animalclass= new()
                    {
                        AnimalClassName = (string)reader["animalclassname"]
                    }
                

                };

            }
            return animalspecie;
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
            //hej på dig
            // En kommentar från Moniras dator
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

        public async Task<IEnumerable<Animal>> CountAnimalInEachSpecie()
        {
            List<Animal> animals = new List<Animal>();
            string sqlQ = "SELECT s.animalspeciename, COUNT (a.animalid) FROM animalspecie s JOIN animal a ON s.animalspecieid = a.animalspecieid GROUP BY s.animalspeciename ORDER BY COUNT(a.animalid) DESC";
          
            await using var dataSource = NpgsqlDataSource.Create(_connectionString);
            await using var command = dataSource.CreateCommand(sqlQ);
            await using var reader = await command.ExecuteReaderAsync();
            Animal animal = new();
            
            while (await reader.ReadAsync())
            {
             
                animal = new()
                {
                    AnimalId = reader.GetInt32(1),
                  


                    Animalspecie = new()
                    {
                            AnimalSpecieName = (string)reader["animalspeciename"],
                         

                    }
                   
                };

                
                animals.Add(animal);
                
            }
            return animals;
        }

        public async Task<Animalspecie> CountSpecie() // public async Task<Animal> GetAnimalByName()
        {
            string sqlQ = "SELECT COUNT (s.animalspecieid) as AmountOfSpecies FROM animalspecie s";

            await using var dataSource = NpgsqlDataSource.Create(_connectionString);
            await using var command = dataSource.CreateCommand(sqlQ);            
            await using var reader = await command.ExecuteReaderAsync();

            Animalspecie animalspecie = new();
            while (await reader.ReadAsync())
            {
                animalspecie = new()
                {
                   

                  AnimalSpecieId = reader.GetInt32(0),
                    

                };

            }

            return animalspecie;

            
        }

        public async Task<IEnumerable<Animal>> NumberOfSpecieInClass()
        {
            List<Animal> animals = new List<Animal>();
            string sqlQ = "SELECT c.animalclassname, COUNT(s.animalspecieid) FROM animalclass c FULL JOIN animalspecie s ON s.animalclassid = c.animalclassid FULL JOIN animal a ON s.animalspecieid = a.animalspecieid GROUP BY c.animalclassname ORDER BY COUNT(s.animalspecieid) DESC";

            await using var dataSource = NpgsqlDataSource.Create(_connectionString);
            await using var command = dataSource.CreateCommand(sqlQ);
            await using var reader = await command.ExecuteReaderAsync();
            Animal animal = new();

            while (await reader.ReadAsync())
            {

                animal = new()
                {
                    


                    Animalspecie = new()
                    {
                        AnimalSpecieId = reader.GetInt32(1),
                       
                        
                        Animalclass = new()
                        {
                            AnimalClassName = (string)reader["animalclassname"]

                        }

                    }

                };


                animals.Add(animal);

            }
            return animals;
        }


        public async Task<IEnumerable<Animal>> GetAnimalWithCharacterName()
        {
            List<Animal> animals = new List<Animal>();


            var sqlJoin = $"SELECT animal.charactername, animalspecie.animalspeciename, animalclass.animalclassname FROM animal JOIN animalspecie ON animalspecie.animalspecieid = animal.animalspecieid JOIN animalclass ON animalclass.animalclassid = animalspecie.animalclassid WHERE animal.charactername IS NOT NULL ORDER BY charactername ASC";

            await using var dataSource = NpgsqlDataSource.Create(_connectionString);
            await using var command = dataSource.CreateCommand(sqlJoin);           
            await using var reader = await command.ExecuteReaderAsync();

            Animal animal = new Animal();


            while (await reader.ReadAsync())
            {
                animal = new()
                {
                    CharacterName = (string)reader["charactername"],


                    Animalspecie = new()
                    {
                        AnimalSpecieName = (string)reader["animalspeciename"],

                        Animalclass = new()
                        {
                            AnimalClassName = (string)reader["animalclassname"]

                        }

                    }
                }; 

                animals.Add(animal);
            }
               return animals;
        }


        public async Task<IEnumerable<Animal>> GetAnimalBySpeficClass(Animalclass animalclass)
        {
            List<Animal> animals = new List<Animal>();
            //string sqlQ = "SELECT * FROM animalspecie ORDER BY animalspeciename ASC";

            var sqlJoin = $"SELECT animal.animalid, animalspecie.animalspeciename, animalclass.animalclassname FROM animalclass JOIN animalspecie ON animalspecie.animalclassid = animalclass.animalclassid JOIN animal ON animal.animalspecieid = animalspecie.animalspecieid WHERE animalclass.animalclassname = @animalclassname ORDER BY animalspeciename ASC";

            await using var dataSource = NpgsqlDataSource.Create(_connectionString);
            await using var command = dataSource.CreateCommand(sqlJoin);
            command.Parameters.AddWithValue("animalclassname", animalclass.AnimalClassName);
            await using var reader = await command.ExecuteReaderAsync();

            Animal animal = new Animal();

            while (await reader.ReadAsync())  
            {
                animal = new Animal()
                {
                    AnimalId = reader.GetInt32(0),   

                    Animalspecie = new()
                    {
                        AnimalSpecieName = (string)reader["animalspeciename"],
                        
                        Animalclass = new()
                        {
                            AnimalClassName = (string)reader["animalclassname"]
                        }
                        
                    }                    
                };

                animals.Add(animal);
            }
            return animals;
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

        public async Task<Animal> AddAnimalAndGetValue(string characterName, int specieId) //testa lowercase i metoden så att man kan söka på Simba och simba oavsett stor eller liten bokstav                                                                        
        {
            string sqlCommand = "insert into animal(charactername, animalspecieid) values(@charactername, @animalspecieid)";

            await using var dataSource = NpgsqlDataSource.Create(_connectionString);
            await using var command = dataSource.CreateCommand(sqlCommand);
            command.Parameters.AddWithValue("charactername", characterName);
            command.Parameters.AddWithValue("animalspecieid", specieId);
            await command.ExecuteNonQueryAsync();

            var animal = GetAnimalByName(characterName);
            return await animal;
            
        }

        public async Task <Animalclass> AddAnimalClass(Animalclass animalclass)
        {
            try
            {
                string sqlCommand = "insert into animalclass(animalclassname) values(@animalclassname)";

                await using var dataSource = NpgsqlDataSource.Create(_connectionString);
                await using var command = dataSource.CreateCommand(sqlCommand);
                command.Parameters.AddWithValue("animalclassname", animalclass.AnimalClassName);
                await command.ExecuteNonQueryAsync();
                return animalclass;
            }
            catch (PostgresException ex)
            {
               
                string errorMessage = "Something went wrong";
                string errorCode = ex.SqlState;
                
                switch (errorCode)
                {
                    case PostgresErrorCodes.UniqueViolation:
                        errorMessage = "The class already exists. The class name must be unique";
                        break;
                    default:
                        break;
                }

                throw new Exception(errorMessage, ex);
            }

            
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
        /// <summary>
        /// Method for question 1
        /// </summary>
        /// <param name="animalclass"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Animalspecie>> GetAnimalClassesForQOne(Animalclass animalclass)
        {
            List<Animalspecie> animalspecies = new List<Animalspecie>();
            string sqlQ = $"SELECT animalspeciename, animalclassname, animalspecieid FROM animalspecie JOIN animalclass ON animalspecie.animalclassid = animalclass.animalclassid WHERE animalclassname = @animalclassname";

            await using var dataSource = NpgsqlDataSource.Create(_connectionString);
            await using var command = dataSource.CreateCommand(sqlQ);
            command.Parameters.AddWithValue("animalclassname", animalclass.AnimalClassName);
            await using var reader = await command.ExecuteReaderAsync();

            Animalspecie animalspecie = new();
            while (await reader.ReadAsync())
            {
                animalspecie = new Animalspecie()
                {
                    AnimalSpecieName = (string)reader["animalspeciename"],
                    AnimalSpecieId = reader.GetInt32(2),

                    Animalclass = new()
                    {
                        
                        AnimalClassName = (string)reader["animalclassname"]
                    }


                };

             
                animalspecies.Add(animalspecie);
            }
            return animalspecies;
        }


        public async Task<IEnumerable<Animal>> GetAllAnimalsSortedBySpecie()
        {
            List<Animal> animals = new List<Animal>();
            string sqlQ = "SELECT a.animalid, a.charactername, s.animalspeciename, s.latinname FROM animal a JOIN animalspecie s ON s.animalspecieid = a.animalspecieid ORDER BY animalspeciename ASC";
                     
            await using var dataSource = NpgsqlDataSource.Create(_connectionString);
            await using var command = dataSource.CreateCommand(sqlQ);
            await using var reader = await command.ExecuteReaderAsync();

            Animal animal = new Animal();
          

            while (await reader.ReadAsync())
            {

                animal = new Animal()
                {
                    AnimalId = reader.GetInt32(0),
                    CharacterName = (string)reader["charactername"],

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