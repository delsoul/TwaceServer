namespace TwaceServer.Shared.Models
{
    public class RegisterResult
    {
        public bool Successful { get; set; }
        public string Error { get; set; }
        public string VerifyCode { get; set; }
    }
}
