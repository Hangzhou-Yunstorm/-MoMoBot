namespace MoMoBot.Infrastructure.Luis
{
    public class ServiceException
    {
        public Error Error { get; set; }
        public string Message { get; set; }
    }

    public class Error
    {
        public string Code { get; set; }
        public string Message { get; set; }
    }
}
