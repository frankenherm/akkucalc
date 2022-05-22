using System.ComponentModel.DataAnnotations;

namespace AkkuCalc.Model
{
    public class CellCapacity
    {
        [Required(ErrorMessage = "Bitte eine valide Fließkommazahl eingeben")]
        [Range(0,float.MaxValue, ErrorMessage = "Bitte eine valide Fließkommazahl eingeben")]
        public float Value { get; set; }
    }
}
