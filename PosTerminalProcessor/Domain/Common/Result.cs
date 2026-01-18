namespace PosTerminalProcessor.Domain.Common
{
    public class Result
    {
        //Create a separate result class for different operations if necessary (without data in this case)
        public string ResponseCode { get; set; }
        public string Description { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }

    }
}
