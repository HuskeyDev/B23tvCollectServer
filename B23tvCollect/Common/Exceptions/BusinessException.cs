namespace B23tvCollect.Common.Exceptions
{
    public class BusinessException : Exception
    {
        public int ErrCode { get; }
        public BusinessException(int errCode, string? message=null, Exception? innerException=null) : base(message, innerException)
        {
            ErrCode = errCode;
        }
    }
}
