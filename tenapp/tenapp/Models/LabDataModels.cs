using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace tenapp.Models
{
    public class LabDataModels
    {
        [DataType(DataType.Text)]
        public String DeviceId { get; set; }

        [DataType(DataType.Text)]
        public String Temp { get; set; }

        [DataType(DataType.Text)]
        public String Hump { get; set; }
    }
}