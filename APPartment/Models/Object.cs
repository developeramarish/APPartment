﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APPartment.Models
{
    public class Object
    {
        [Key]
        public long ObjectId { get; set; }

        [ForeignKey("ObjectType")]
        public long ObjectTypeId { get; set; }
    }
}
