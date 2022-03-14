namespace ValueTypeExample.Models.Exceptions;

[Serializable]
public class InvalidTemperatureException : ArgumentOutOfRangeException
{
    public InvalidTemperatureException() { }
    public InvalidTemperatureException(string? paramName) : base(paramName) { }
    public InvalidTemperatureException(string? message, Exception? innerException)
        : base(message, innerException) { }
    public InvalidTemperatureException(string? paramName, string? message)
        : base(paramName, message) { }
    public InvalidTemperatureException(string? paramName, object? actualValue, string? message)
        : base(paramName, actualValue, message) { }

    protected InvalidTemperatureException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}
