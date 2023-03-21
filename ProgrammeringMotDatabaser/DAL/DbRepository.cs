﻿using Microsoft.Extensions.Configuration;
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


        #region Create - Animal class, Animal specie and Animal

    /// <summary>
    /// Create a animal class
    /// </summary>
    /// <param name="animalclass"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
        public async Task<AnimalClass> AddAnimalClass(AnimalClass animalclass)
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

        /// <summary>
        /// Create Animal specie
        /// </summary>
        /// <param name="animalSpecieName"></param>
        /// <param name="latinname"></param>
        /// <param name="animalClassId"></param>
        /// <returns></returns>
        public async Task<AnimalSpecie> AddAnimalSpecie(string animalSpecieName, string latinname, int animalClassId)
        {
            try
            {
                string sqlCommand = "insert into animalspecie(animalspeciename, latinname, animalclassid) values(@animalspeciename, @latinname, @animalclassid)";

                await using var dataSource = NpgsqlDataSource.Create(_connectionString);
                await using var command = dataSource.CreateCommand(sqlCommand);
                command.Parameters.AddWithValue("animalspeciename", animalSpecieName);
                command.Parameters.AddWithValue("latinname", (object)latinname ?? DBNull.Value);
                command.Parameters.AddWithValue("animalclassid", animalClassId);
                await command.ExecuteNonQueryAsync();

                var animalspecie = new AnimalSpecie()
                {
                    AnimalSpecieName = animalSpecieName,
                    LatinName = latinname,

                    AnimalClass = new()
                    {
                        AnimalClassId = animalClassId,


                    }

                };
                return animalspecie;
            }
            catch (PostgresException ex)
            {

                string errorMessage = "Something went wrong";
                string errorCode = ex.SqlState;


                switch (errorCode)
                {
                    case PostgresErrorCodes.ForeignKeyViolation:
                        errorMessage = $"This value has connections that must be deleted before you can delete it.";
                        break;

                    case PostgresErrorCodes.UniqueViolation:
                        errorMessage = "The value already exists. The value must be unique.";
                        break;

                    case PostgresErrorCodes.StringDataRightTruncation:
                        errorMessage = "The value has too many characters. Max 255 characters.";
                        break;

                    default:
                        break;
                }

                throw new Exception(errorMessage, ex);
            }
        }

        /// <summary>
        /// Create an animal  
        /// </summary>
        /// <param name="characterName"></param>
        /// <param name="specieId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task <Animal> AddAnimal(Animal animal)                                                                   
        {
            try
            {
                string sqlCommand = "insert into animal(charactername, animalspecieid) values(@charactername, @animalspecieid) returning animalid";

                await using var dataSource = NpgsqlDataSource.Create(_connectionString);
                await using var command = dataSource.CreateCommand(sqlCommand);
                command.Parameters.AddWithValue("charactername", (object)animal.CharacterName ?? DBNull.Value); 
                command.Parameters.AddWithValue("animalspecieid", animal.AnimalSpecie.AnimalSpecieId);
                
                animal.AnimalId = (int) await command.ExecuteScalarAsync();

               return animal;
                

                
                //await command.ExecuteNonQueryAsync();


                //var animal = GetAnimalByCharacterName(characterName); //Någonting blir fel här när characterName är null.Vet inte varför Men får upp olika felmeddelanden. 
                //return await animal;
            }
            catch (PostgresException ex)
            {
                string errorMessage = "Something went wrong";
                string errorCode = ex.SqlState;

                switch (errorCode)
                {
                    case PostgresErrorCodes.UniqueViolation:
                        errorMessage = "There is already an animal with that name. The animal name must be unique";
                        break;
                    default:
                        break;
                }
                throw new Exception(errorMessage, ex);
            }

        }


        #endregion


        #region Read - All Methods that are conncted to read


        public async Task<Animal> GetAnimalById(int animalId)
        {

            string sql = "Select a.animalid, a.charactername, s.animalspeciename from animal a Join animalspecie s on s.animalspecieid = a.animalspecieid where a.animalid = @animalid";

            await using var dataSource = NpgsqlDataSource.Create(_connectionString);
            await using var command = dataSource.CreateCommand(sql);
            command.Parameters.AddWithValue("animalid", animalId);
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
                      
                        AnimalSpecieName = (string) reader["animalspeciename"],

                    }
                    
                  
                };
            }


            return animal;
        }




    

    //Animal animal = new();
    //            while (await reader.ReadAsync())
    //            {
    //                animal = new ()
    //                {
    //                    AnimalId = reader.GetInt32(0),
    //                    CharacterName = reader["charactername"] == DBNull.Value? null : (string) reader["charactername"],

    //                    AnimalSpecie = new()
    //                    {
    //                        AnimalSpecieName = (string)reader["animalspeciename"],
    //                        LatinName = reader["latinname"] == DBNull.Value ? null : (string)reader["latinname"],

    //                        AnimalClass = new()
    //                        {
    //                            AnimalClassName = (string)reader["animalclassname"]

    //                        }
    //                    }


    //                };
    //            }
    //            return animal;



/// <summary>
/// This method make a list with all Animals and all info about them. Using every propertie in class Animal, AnimalSpecie and Animal Class
/// </summary>
/// <returns></returns>
public async Task<IEnumerable<Animal>> AllInfoAboutAllAnimals()
        {
            try
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
            catch(PostgresException ex)
            {

                string errorMessage = "Something went wrong";
                string errorCode = ex.SqlState;


                switch (errorCode)
                {
                    case PostgresErrorCodes.ForeignKeyViolation:
                        errorMessage = $"This value has connections that must be deleted before you can delete it.";
                        break;

                    case PostgresErrorCodes.UniqueViolation:
                        errorMessage = "The value already exists. The value must be unique.";
                        break;

                    case PostgresErrorCodes.StringDataRightTruncation:
                        errorMessage = "The value has too many characters. Max 255 characters.";
                        break;

                    default:
                        break;
                }

                throw new Exception(errorMessage, ex);
            }
             
        }

        /// <summary>
        /// Method returns all animals that have a character name
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Animal>> GetAnimalWithCharacterName()
        {
            try
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
                        CharacterName = reader["charactername"] == DBNull.Value ? null : (string)reader["charactername"], 


                        AnimalSpecie = new()
                        {
                            AnimalSpecieName = (string)reader["animalspeciename"],

                            AnimalClass = new()
                            {
                                AnimalClassName = (string)reader["animalclassname"]

                            }

                        }
                    };

                    animals.Add(animal);
                }
                return animals;
            }

            catch (PostgresException ex)
            {
                string errorMessage = "Something went wrong";
                string errorCode = ex.SqlState;


                switch (errorCode)
                {
                    case PostgresErrorCodes.ForeignKeyViolation:
                        errorMessage = $"This value has connections that must be deleted before you can delete it.";
                        break;

                    case PostgresErrorCodes.UniqueViolation:
                        errorMessage = "The value already exists. The value must be unique.";
                        break;

                    case PostgresErrorCodes.StringDataRightTruncation:
                        errorMessage = "The value has too many characters. Max 255 characters.";
                        break;

                    default:
                        break;
                }

                throw new Exception(errorMessage, ex);
            }
        }


        /// <summary>
        /// Method that searches for a specific animal by charactername
        /// </summary>
        /// <param name="characterName"></param>
        /// <returns></returns>
        public async Task<Animal> GetAnimalByCharacterName(string characterName) //testa lowercase i metoden så att man kan söka på Simba och simba oavsett stor eller liten bokstav                                                                        
        {
            try
            {
            string sqlQuestion = "SELECT animalid, charactername, animalspeciename, latinname, animalclassname FROM animal JOIN animalspecie ON animalspecie.animalspecieid = animal.animalspecieid JOIN animalclass ON animalclass.animalclassid =animalspecie.animalclassid WHERE animal.charactername= @charactername";
             
               
            await using var dataSource = NpgsqlDataSource.Create(_connectionString);
            await using var command = dataSource.CreateCommand(sqlQuestion);
            command.Parameters.AddWithValue ("charactername", (object)characterName ?? DBNull.Value);
                await using var reader = await command.ExecuteReaderAsync();


            
                Animal animal = new();
                while (await reader.ReadAsync())
                {
                    animal = new()
                    {
                        AnimalId = reader.GetInt32(0),
                        CharacterName = reader["charactername"] == DBNull.Value ? null : (string)reader["charactername"],

                        AnimalSpecie = new()
                        {
                            AnimalSpecieName = (string)reader["animalspeciename"],
                            LatinName = reader["latinname"] == DBNull.Value ? null : (string)reader["latinname"],

                            AnimalClass = new()
                            {
                                AnimalClassName = (string)reader["animalclassname"]

                            }
                        }


                    };
                }
                return animal;
            }

            catch (PostgresException ex)
            {
                string errorMessage = "Something went wrong";
                string errorCode = ex.SqlState;


                switch (errorCode)
                {
                    case PostgresErrorCodes.ForeignKeyViolation:
                        errorMessage = $"This value has connections that must be deleted before you can delete it.";
                        break;

                    case PostgresErrorCodes.UniqueViolation:
                        errorMessage = "The value already exists. The value must be unique.";
                        break;

                    case PostgresErrorCodes.StringDataRightTruncation:
                        errorMessage = "The value has too many characters. Max 255 characters.";
                        break;

                    default:
                        break;
                }

                throw new Exception(errorMessage, ex);
            }
        }

        /// <summary>
        /// Count all animals in each specie
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Animal>> CountAnimalInEachSpecie()
        {
            try
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



                        AnimalSpecie = new()
                        {
                            AnimalSpecieName = (string)reader["animalspeciename"],


                        }

                    };


                    animals.Add(animal);

                }
                return animals;
            }

            catch (PostgresException ex)
            {
                string errorMessage = "Something went wrong";
                string errorCode = ex.SqlState;


                switch (errorCode)
                {
                    case PostgresErrorCodes.ForeignKeyViolation:
                        errorMessage = $"This value has connections that must be deleted before you can delete it.";
                        break;

                    case PostgresErrorCodes.UniqueViolation:
                        errorMessage = "The value already exists. The value must be unique.";
                        break;

                    case PostgresErrorCodes.StringDataRightTruncation:
                        errorMessage = "The value has too many characters. Max 255 characters.";
                        break;

                    default:
                        break;
                }

                throw new Exception(errorMessage, ex);
            }

        }

        /// <summary>
        /// Count how many species there are 
        /// </summary>
        /// <returns></returns>
        public async Task<AnimalSpecie> CountSpecie()
        {
            try
            {
                string sqlQ = "SELECT COUNT (s.animalspecieid) as AmountOfSpecies FROM animalspecie s";

                await using var dataSource = NpgsqlDataSource.Create(_connectionString);
                await using var command = dataSource.CreateCommand(sqlQ);
                await using var reader = await command.ExecuteReaderAsync();

                AnimalSpecie animalspecie = new();
                while (await reader.ReadAsync())
                {
                    animalspecie = new()
                    {

                        AnimalSpecieId = reader.GetInt32(0),

                    };

                }

                return animalspecie;
            }

            catch (PostgresException ex)
            {
                string errorMessage = "Something went wrong";
                string errorCode = ex.SqlState;


                switch (errorCode)
                {
                    case PostgresErrorCodes.ForeignKeyViolation:
                        errorMessage = $"This value has connections that must be deleted before you can delete it.";
                        break;

                    case PostgresErrorCodes.UniqueViolation:
                        errorMessage = "The value already exists. The value must be unique.";
                        break;

                    case PostgresErrorCodes.StringDataRightTruncation:
                        errorMessage = "The value has too many characters. Max 255 characters.";
                        break;

                    default:
                        break;
                }

                throw new Exception(errorMessage, ex);
            }

        }

        /// <summary>
        /// Count how many species there are in each animal class
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Animal>> NumberOfAnimalsInClass()
        {
            try
            {
                List<Animal> animals = new List<Animal>();
                string sqlQ = "SELECT c.animalclassname, COUNT (a.animalid) FROM animalclass c full JOIN animalspecie s ON s.animalclassid = c.animalclassid full JOIN animal a ON s.animalspecieid = a.animalspecieid GROUP BY c.animalclassname ORDER BY COUNT(a.animalid) DESC";

                await using var dataSource = NpgsqlDataSource.Create(_connectionString);
                await using var command = dataSource.CreateCommand(sqlQ);
                await using var reader = await command.ExecuteReaderAsync();
                Animal animal = new();

                while (await reader.ReadAsync())
                {

                    animal = new()
                    {
                        AnimalId= reader.GetInt32(1),

                        AnimalSpecie = new()
                        {
                    
                            AnimalClass = new()
                            {
                                AnimalClassName = (string)reader["animalclassname"]

                            }

                        }

                    };


                    animals.Add(animal);

                }
                return animals;
            }
            catch (PostgresException ex)
            {
                string errorMessage = "Something went wrong";
                string errorCode = ex.SqlState;


                switch (errorCode)
                {
                    case PostgresErrorCodes.ForeignKeyViolation:
                        errorMessage = $"This value has connections that must be deleted before you can delete it.";
                        break;

                    case PostgresErrorCodes.UniqueViolation:
                        errorMessage = "The value already exists. The value must be unique.";
                        break;

                    case PostgresErrorCodes.StringDataRightTruncation:
                        errorMessage = "The value has too many characters. Max 255 characters.";
                        break;

                    default:
                        break;
                }

                throw new Exception(errorMessage, ex);
            }

        }


        /// <summary>
        /// Return a list of all animal classes
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<AnimalClass>> GetAnimalClass()
        {
            try
            {
                List<AnimalClass> animalClass = new List<AnimalClass>();
                string sqlQ = "SELECT * FROM animalclass ";

                await using var dataSource = NpgsqlDataSource.Create(_connectionString);
                await using var command = dataSource.CreateCommand(sqlQ);
                await using var reader = await command.ExecuteReaderAsync();

                AnimalClass animalclass = new AnimalClass();
                while (await reader.ReadAsync())
                {
                    animalclass = new AnimalClass()
                    {
                        AnimalClassId = reader.GetInt32(0),
                        AnimalClassName = (string)reader["animalclassname"]

                    };
                    animalClass.Add(animalclass);
                }
                return animalClass;
            }

            catch (PostgresException ex)
            {
                string errorMessage = "Something went wrong";
                string errorCode = ex.SqlState;


                switch (errorCode)
                {
                    case PostgresErrorCodes.ForeignKeyViolation:
                        errorMessage = $"This value has connections that must be deleted before you can delete it.";
                        break;

                    case PostgresErrorCodes.UniqueViolation:
                        errorMessage = "The value already exists. The value must be unique.";
                        break;

                    case PostgresErrorCodes.StringDataRightTruncation:
                        errorMessage = "The value has too many characters. Max 255 characters.";
                        break;

                    default:
                        break;
                }

                throw new Exception(errorMessage, ex);
            }

        }


        /// <summary>
        /// Find the name of a animal class with the help of animalspeciename
        /// </summary>
        /// <param name="selectAnimalspecie"></param>
        /// <returns></returns>
        public async Task<AnimalSpecie> FindClass(AnimalSpecie selectAnimalspecie)
        {
            try
            {
                string sqlQuestion = "Select s.animalspeciename, c.animalclassname, s.latinname From animalspecie s Join animalclass c ON s.animalclassid = c.animalclassid Where animalspeciename = @animalspeciename";


                await using var dataSource = NpgsqlDataSource.Create(_connectionString);
                await using var command = dataSource.CreateCommand(sqlQuestion);
                command.Parameters.AddWithValue("animalspeciename", selectAnimalspecie.AnimalSpecieName);
                await using var reader = await command.ExecuteReaderAsync();

                AnimalSpecie animalspecie = new();
                while (await reader.ReadAsync())
                {
                    animalspecie = new()

                    {
                        AnimalSpecieName = (string)reader["animalspeciename"],
                        LatinName = reader["latinname"] == DBNull.Value ? null : (string)reader["latinname"],

                        AnimalClass = new()
                        {
                            AnimalClassName = (string)reader["animalclassname"]
                        }


                    };

                }
                return animalspecie;
            }
            catch (PostgresException ex)
            {
                string errorMessage = "Something went wrong";
                string errorCode = ex.SqlState;


                switch (errorCode)
                {
                    case PostgresErrorCodes.ForeignKeyViolation:
                        errorMessage = $"This value has connections that must be deleted before you can delete it.";
                        break;

                    case PostgresErrorCodes.UniqueViolation:
                        errorMessage = "The value already exists. The value must be unique.";
                        break;

                    case PostgresErrorCodes.StringDataRightTruncation:
                        errorMessage = "The value has too many characters. Max 255 characters.";
                        break;

                    default:
                        break;
                }

                throw new Exception(errorMessage, ex);
            }


            //Lägg in try catch. Någon kan ta bort en klass och då finns han inte längre när du söker.

        }





        /// <summary>
        /// Method show all species sorted by animalspeciename. Used in comboboxes
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<AnimalSpecie>> GetAnimalSpecie()
        {
            try
            {
                List<AnimalSpecie> animalSpecies = new List<AnimalSpecie>();
                string sqlQ = "SELECT * FROM animalspecie ORDER BY animalspeciename ASC";

                await using var dataSource = NpgsqlDataSource.Create(_connectionString);

                await using var command = dataSource.CreateCommand(sqlQ);
                await using var reader = await command.ExecuteReaderAsync();
                AnimalSpecie animalspecie = new AnimalSpecie();
                while (await reader.ReadAsync())
                {

                    animalspecie = new AnimalSpecie()
                    {
                        AnimalSpecieId = reader.GetInt32(0),
                        AnimalSpecieName = (string)reader["animalspeciename"],

                        AnimalClass = new()
                        {
                            AnimalClassId = reader.GetInt32(3)
                        }


                    };


                    animalSpecies.Add(animalspecie);
                }
                return animalSpecies;
            }
            catch (PostgresException ex)
            {
                string errorMessage = "Something went wrong";
                string errorCode = ex.SqlState;


                switch (errorCode)
                {
                    case PostgresErrorCodes.ForeignKeyViolation:
                        errorMessage = $"This value has connections that must be deleted before you can delete it.";
                        break;

                    case PostgresErrorCodes.UniqueViolation:
                        errorMessage = "The value already exists. The value must be unique.";
                        break;

                    case PostgresErrorCodes.StringDataRightTruncation:
                        errorMessage = "The value has too many characters. Max 255 characters.";
                        break;

                    default:
                        break;
                }

                throw new Exception(errorMessage, ex);
            }
        }



        /// <summary>
        /// Method gives all animal how belongs to a specific animal class
        /// </summary>
        /// <param name="animalclass"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Animal>> GetAnimalBySpeficClass(AnimalClass animalclass)
        {
            try
            {
                List<Animal> animals = new List<Animal>();

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

                        AnimalSpecie = new()
                        {
                            AnimalSpecieName = (string)reader["animalspeciename"],

                            AnimalClass = new()
                            {
                                AnimalClassName = (string)reader["animalclassname"]
                            }

                        }
                    };

                    animals.Add(animal);
                }
                return animals;
            }
            catch (PostgresException ex)
            {
                string errorMessage = "Something went wrong";
                string errorCode = ex.SqlState;


                switch (errorCode)
                {
                    case PostgresErrorCodes.ForeignKeyViolation:
                        errorMessage = $"This value has connections that must be deleted before you can delete it.";
                        break;

                    case PostgresErrorCodes.UniqueViolation:
                        errorMessage = "The value already exists. The value must be unique.";
                        break;

                    case PostgresErrorCodes.StringDataRightTruncation:
                        errorMessage = "The value has too many characters. Max 255 characters.";
                        break;

                    default:
                        break;
                }

                throw new Exception(errorMessage, ex);
            }
        }

        #endregion


        #region Update - Character name, Latin name and Animal specie

        /// <summary>
        /// Update character name of an animal
        /// </summary>
        /// <param name="newCharaternamne"></param>
        /// <param name="animal"></param>
        /// <returns></returns>
        public async Task<Animal> UpdateCharacterName(string newCharaternamne, Animal animal)
        {
            try
            {
                string sqlCommand = "UPDATE animal SET charactername = @charactername WHERE animalid = @animalid";

                await using var dataSource = NpgsqlDataSource.Create(_connectionString);
                await using var command = dataSource.CreateCommand(sqlCommand);
                command.Parameters.AddWithValue("charactername", (object)newCharaternamne ?? DBNull.Value);
                command.Parameters.AddWithValue("animalid", animal.AnimalId);
                await command.ExecuteNonQueryAsync();

                var newAnimal = new Animal()
                {
                    AnimalId = animal.AnimalId,
                    CharacterName = newCharaternamne,

                };

                return newAnimal;
            }
            catch (PostgresException ex)
            {
                string errorMessage = "Something went wrong";
                string errorCode = ex.SqlState;


                switch (errorCode)
                {
                    case PostgresErrorCodes.ForeignKeyViolation:
                        errorMessage = $"This value has connections that must be deleted before you can delete it.";
                        break;

                    case PostgresErrorCodes.UniqueViolation:
                        errorMessage = "The value already exists. The value must be unique.";
                        break;

                    case PostgresErrorCodes.StringDataRightTruncation:
                        errorMessage = "The value has too many characters. Max 255 characters.";
                        break;

                    default:
                        break;
                }

                throw new Exception(errorMessage, ex);
            }

        }

        /// <summary>
        /// Update Latin name of a animal specie
        /// </summary>
        /// <param name="newLatinName"></param>
        /// <param name="animalSN"></param>
        /// <returns></returns>
        public async Task<AnimalSpecie> UpdateLatinName(string newLatinName, string animalSN)
        {
            try
            {

                string sqlCommand = "UPDATE animalspecie SET latinname = @latinname WHERE animalspeciename = @animalspeciename";

                await using var dataSource = NpgsqlDataSource.Create(_connectionString);
                await using var command = dataSource.CreateCommand(sqlCommand);
                command.Parameters.AddWithValue("latinname", (object)newLatinName ?? DBNull.Value);
                command.Parameters.AddWithValue("animalspeciename", animalSN);
                await command.ExecuteNonQueryAsync();

                var newAnimal = new AnimalSpecie()
                {

                    AnimalSpecieName = animalSN,
                    LatinName = newLatinName

                };

                return newAnimal;
            }
            catch (PostgresException ex)
            {
                string errorMessage = "Something went wrong";
                string errorCode = ex.SqlState;


                switch (errorCode)
                {
                    case PostgresErrorCodes.ForeignKeyViolation:
                        errorMessage = $"This value has connections that must be deleted before you can delete it.";
                        break;

                    case PostgresErrorCodes.UniqueViolation:
                        errorMessage = "The value already exists. The value must be unique.";
                        break;

                    case PostgresErrorCodes.StringDataRightTruncation:
                        errorMessage = "The value has too many characters. Max 255 characters.";
                        break;

                    default:
                        break;
                }

                throw new Exception(errorMessage, ex);
            }
        }

        /// <summary>
        /// Update animal specie 
        /// </summary>
        /// <param name="animal"></param>
        /// <param name="newAnimalSpecieId"></param>
        /// <returns></returns>
        public async Task<Animal> UpdateAnimalSpecie(Animal animal, int newAnimalSpecieId)
        {
            try
            {

                string sqlCommand = "UPDATE animal SET animalspecieid = @animalspecieid WHERE animalid = @animalid";

                await using var dataSource = NpgsqlDataSource.Create(_connectionString);
                await using var command = dataSource.CreateCommand(sqlCommand);
                command.Parameters.AddWithValue("animalspecieid", newAnimalSpecieId);
                command.Parameters.AddWithValue("animalid", animal.AnimalId);
                await command.ExecuteNonQueryAsync();

                var newAnimal = new Animal()
                {
                    AnimalId = animal.AnimalId,

                    AnimalSpecie = new()
                    {
                        AnimalSpecieId = newAnimalSpecieId,

                    }
                };

                return newAnimal;
            }
            catch (PostgresException ex)
            {
                string errorMessage = "Something went wrong";
                string errorCode = ex.SqlState;


                switch (errorCode)
                {
                    case PostgresErrorCodes.ForeignKeyViolation:
                        errorMessage = $"This value has connections that must be deleted before you can delete it.";
                        break;

                    case PostgresErrorCodes.UniqueViolation:
                        errorMessage = "The value already exists. The value must be unique.";
                        break;

                    case PostgresErrorCodes.StringDataRightTruncation:
                        errorMessage = "The value has too many characters. Max 255 characters.";
                        break;

                    default:
                        break;
                }

                throw new Exception(errorMessage, ex);
            }


        }
        #endregion


        #region Delete - Animal, animal specie and animal class

        /// <summary>
        /// Delete an animal
        /// </summary>
        /// <param name="animal"></param>
        /// <returns></returns>
        public async Task DeleteAnimal(Animal animal)
        {
        try
        {
            string sqlCommand = "DELETE FROM animal WHERE animalid = @animalid";

            await using var dataSource = NpgsqlDataSource.Create(_connectionString);
            await using var command = dataSource.CreateCommand(sqlCommand);
            command.Parameters.AddWithValue("animalid", animal.AnimalId);
            await command.ExecuteNonQueryAsync();

        }
        catch (PostgresException ex)
        {

            string errorMessage = "Something went wrong";
            string errorCode = ex.SqlState;


            switch (errorCode)
            {
                case PostgresErrorCodes.ForeignKeyViolation:
                    errorMessage = $"This value has connections that must be deleted before you can delete it.";
                    break;

                case PostgresErrorCodes.UniqueViolation:
                    errorMessage = "The value already exists. The value must be unique.";
                    break;

                case PostgresErrorCodes.StringDataRightTruncation:
                    errorMessage = "The value has too many characters. Max 255 characters.";
                    break;

                default:
                    break;
            }

            throw new Exception(errorMessage, ex);
        }

    }

        /// <summary>
        /// Delete an animal in animal specie 
        /// </summary>
        /// <param name="animalSpecie"></param>
        /// <returns></returns>
        public async Task DeleteAnimalInSpecie(AnimalSpecie animalSpecie)
        {
            try
            {
                string sqlCommand = "DELETE FROM animal WHERE animalspecieid = @animalspecieid";

                await using var dataSource = NpgsqlDataSource.Create(_connectionString);
                await using var command = dataSource.CreateCommand(sqlCommand);
                command.Parameters.AddWithValue("animalspecieid", animalSpecie.AnimalSpecieId);
                await command.ExecuteNonQueryAsync();
            }
                catch (PostgresException ex)
                {

                    string errorMessage = "Something went wrong";
                    string errorCode = ex.SqlState;


                    switch (errorCode)
                    {
                        case PostgresErrorCodes.ForeignKeyViolation:
                            errorMessage = $"This value has connections that must be deleted before you can delete it.";
                            break;

                        case PostgresErrorCodes.UniqueViolation:
                            errorMessage = "The value already exists. The value must be unique.";
                            break;

                        case PostgresErrorCodes.StringDataRightTruncation:
                            errorMessage = "The value has too many characters. Max 255 characters.";
                            break;

                        default:
                            break;
                    }

                    throw new Exception(errorMessage, ex);
                }


            }

        /// <summary>
        /// Delete a animal specie
        /// </summary>
        /// <param name="animalSpecie"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task DeleteAnimalSpecie(AnimalSpecie animalSpecie)
        {
            try
            {
                string sqlCommand = "DELETE FROM animalspecie WHERE animalspecieid = @animalspecieid";

                await using var dataSource = NpgsqlDataSource.Create(_connectionString);
                await using var command = dataSource.CreateCommand(sqlCommand);
                command.Parameters.AddWithValue("animalspecieid", animalSpecie.AnimalSpecieId);
                await command.ExecuteNonQueryAsync();
            }
            catch (PostgresException ex)
            {

                string errorMessage = "Something went wrong";
                string errorCode = ex.SqlState;


                switch (errorCode)
                {
                    case PostgresErrorCodes.ForeignKeyViolation:
                        errorMessage = $"This value has connections that must be deleted before you can delete it. Do you wish to delete all animals in this specie?";
                        break;

                    case PostgresErrorCodes.UniqueViolation:
                        errorMessage = "The value already exists. The value must be unique.";
                        break;

                    case PostgresErrorCodes.StringDataRightTruncation:
                        errorMessage = "The value has too many characters. Max 255 characters.";
                        break;

                    default:
                        break;
                }

                throw new Exception(errorMessage, ex);
            }
        }

        /// <summary>
        /// Delete a animal class
        /// </summary>
        /// <param name="animalClass"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task DeleteAnimalClass(AnimalClass animalClass)//22001, 23503, 23505
        {
            try
            {
                string sqlCommand = "DELETE FROM animalclass WHERE animalclassid = @animalclassid";

                await using var dataSource = NpgsqlDataSource.Create(_connectionString);
                await using var command = dataSource.CreateCommand(sqlCommand);
                command.Parameters.AddWithValue("animalclassid", animalClass.AnimalClassId);
                await command.ExecuteNonQueryAsync();
            }
            catch (PostgresException ex) 
            {

                string errorMessage = "Something went wrong";
                string errorCode = ex.SqlState;


                switch (errorCode)
                {
                    case PostgresErrorCodes.ForeignKeyViolation:
                        errorMessage = $"This value has connections that must be deleted before you can delete it.";
                        break;

                    case PostgresErrorCodes.UniqueViolation:
                        errorMessage = "The value already exists. The value must be unique.";
                        break;

                    case PostgresErrorCodes.StringDataRightTruncation:
                        errorMessage = "The value has too many characters. Max 255 characters.";
                        break;

                    default:
                        break;
                }

                throw new Exception(errorMessage, ex);
            }
        }
        #endregion





       //Kolla på föreläsningen igen och se om vi fattar hur vi kan använda oss av dessa metoder på ett smart sätt.

        private static T? ConvertFromDBVal<T>(object obj)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return default;
            }
            return (T)obj;
        }
        private static object ConvertToDBVal<T>(object obj)
        {
            if (obj == null || obj == string.Empty)
            {
                return DBNull.Value;
            }
            return (T)obj;
        }


    }
}