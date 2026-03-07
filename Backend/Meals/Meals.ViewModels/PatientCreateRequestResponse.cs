using System;
using System.Collections.Generic;
using System.Text;

namespace Hospital.Meals.ViewModels
{
    public class PatientCreateRequestResponse
    {
        public string Id { get; set; } = "";
        public string StatusString { get; set; } = "";
        public string StatusReason { get; set; } = "";
        public string UnsafeIngredientId { get; set; } = "";
    }
}
