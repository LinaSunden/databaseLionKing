﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammeringMotDatabaser.Models
{
    internal class AnimalClass
    {
        public int AnimalClassId { get; set; }

        public string AnimalClassName { get; set; }

        public string Display => $" E: {AnimalClassName }";

        public override string ToString()
        {
            return $" Animal class: {AnimalClassName}";
        }




    }
}
