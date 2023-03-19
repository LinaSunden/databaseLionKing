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
    internal class AnimalClass
    {
        //private readonly string _connectionString;

        //public AnimalClass()  //constructor, access to connectionstring
        //{
        //    var config = new ConfigurationBuilder().AddUserSecrets<DbRepository>().Build();
        //    _connectionString = config.GetConnectionString("develop");
        //}

        public int AnimalClassId { get; set; }

        public string AnimalClassName { get; set; }

        public string Display => $"{AnimalClassName }";

        public override string ToString()
        {
            return $" Animal class: {AnimalClassName}";
        }


        //public async Task<IEnumerable<AnimalClass>> GetAnimalClassTest()
        //{
        //    List<AnimalClass> animalClassList = new List<AnimalClass>();


        //    var sqlJoin = "SELECT animalclassid, animalclassname FROM animalclass ORDER BY animalclassname ASC";

        //    await using var dataSource = NpgsqlDataSource.Create(_connectionString);
        //    await using var command = dataSource.CreateCommand(sqlJoin);
        //    await using var reader = await command.ExecuteReaderAsync();

        //    AnimalClass animalclass = new ();
        //    while (await reader.ReadAsync())
        //    {
        //        animalclass = new()
        //        {                                          
        //                    AnimalClassId = reader.GetInt32(0),
        //                    AnimalClassName = (string)reader["animalclassname"]                                         
        //        };
        //        animalClassList.Add(animalclass);
        //    }
        //    return animalClassList;
        //}


    

    }
}
