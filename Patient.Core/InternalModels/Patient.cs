using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;

namespace Patient.Core.InternalModels
{
    internal class Patient
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string DietTypeId { get; set; } = "";
        public string Notes { get; set; } = "";
    }
}
