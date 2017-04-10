namespace EcoSim.Models
{
    public class ServerResponse
    {
        public bool Error { get; set; }
        public string Message { get; set; }

        public ServerResponse(bool error, string message)
        {
            Error = error;
            Message = message;
        }
    }
}