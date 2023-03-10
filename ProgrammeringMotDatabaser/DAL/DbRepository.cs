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
        public async Task<Animal> GetAnimalByName(string characterName)
        {
          
            string sqlQuestion = $"SELECT * FROM animal WHERE charactername='{characterName}'";  
            
            await using var dataSource = NpgsqlDataSource.Create(_connectionString);
            await using var command = dataSource.CreateCommand(sqlQuestion);
            await using var reader = await command.ExecuteReaderAsync();

            Animal animal = new Animal();

          

            while(await reader.ReadAsync())   //doublecheck if we should use switch 
            {
                animal = new Animal()

                {
                    AnimalId = reader.GetInt32(0),
                    CharacterName = (string)reader["charactername"]
                    
                };
                                

            }

            if (animal.AnimalId == 0)
            {
                MessageBox.Show($"There is no animal called {characterName}");

            }

            return animal;
        }

    }
}
