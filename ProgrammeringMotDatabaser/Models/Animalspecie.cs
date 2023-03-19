﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammeringMotDatabaser.Models
{
    internal class AnimalSpecie
    {
        public int AnimalSpecieId { get; set; }

        public string AnimalSpecieName { get; set; }

        public string? LatinName { get; set; }
       
        public string Display => $"Total number  of animalspecies {AnimalSpecieId}";

        public AnimalClass AnimalClass { get; set; }

        
        public override string ToString()
        {
          
            
                return $"{AnimalSpecieName} {AnimalClass} {AnimalClass.AnimalClassId}"; 


        }



    }
}
