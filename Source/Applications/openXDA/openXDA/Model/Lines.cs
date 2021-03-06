﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSF.Data.Model;

namespace openXDA.Model
{
    [TableName("Line")]
    public class Lines
    {
        [PrimaryKey(true)]
        public int ID { get; set; }

        [StringLength(50)]
        public string AssetKey { get; set; }

        public float VoltageKV { get; set; }
        public float ThermalRating { get; set; }
        public float Length { get; set; }

        public string Description { get; set; }



    }
}
