using ValueTypeGenerator.Validation;

namespace ValueTypeExample.Models
{
    [ValueType]
    public partial class Bloodpressure : IValueValidator<ValidationState>, IValueType
    {
        public int Sys { get; set; }
        public byte Dia { get; set; }
        public byte HR { get; set; }

        public ValidationState GetValidationState()
        {
            throw new NotImplementedException();
        }
        public void Validate()
        {
            Console.WriteLine($"Sys: {Sys}, Dia: {Dia}, HR: {HR}");
        }
    }
}