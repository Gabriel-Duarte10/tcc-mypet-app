﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tcc_mypet_app.Models.Dto
{
    public class BreedDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public AnimalTypeDTO AnimalType { get; set; }
    }
}
