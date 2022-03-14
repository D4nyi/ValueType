using ValueTypeExample.Models.Exceptions;

using ValueTypeGenerator.Validation;

namespace ValueTypeExample.Models;

public class Temperature : IValueType
{
    public float Value { get; set; }

    public ValidationState GetValidationState()
    {
        return Value switch
        {
            >= 30f and <= 33f or > 38.5f => ValidationState.InterventionHigh,
            > 33f and <= 35f or > 38f => ValidationState.InterventionLow,
            > 35f and <= 36f or >= 37f and < 37.5f => ValidationState.WarningLow,
            > 37.5f and <= 38f => ValidationState.WarningHigh,
            _ => ValidationState.Invalid
        };
    }

    public void Validate()
    {
        if (Value < 30f || Value > 50f)
        {
            throw new InvalidTemperatureException(nameof(Value), Value, "Nem elfogadható érték!");
        }
    }
}
