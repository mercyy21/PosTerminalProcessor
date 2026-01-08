namespace PosTerminalProcessor.Domain.Common
{
    public class Result
    {
        public string ResponseCode { get; set; }
        public string Description { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }

    }
}
