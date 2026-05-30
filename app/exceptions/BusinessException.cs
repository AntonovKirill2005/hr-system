namespace rut_shop.net.exceptions;

public class BusinessException : Exception
{
    public BusinessException(string message) : base(message) { }
}