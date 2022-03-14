namespace ValueTypeGenerator.Validation
{
    public interface IValueValidator<T>
    {
        T GetValidationState();
    }
}
