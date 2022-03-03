﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("Rooms")]
    public class Room
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        
        public IEnumerable<Desk> Desks { get; set; }
    }
}
