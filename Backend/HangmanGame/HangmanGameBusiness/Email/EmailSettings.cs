namespace HangmanGameBusiness.Email
{
    public class EmailSettings
    {
        public string SenderAddress { get; set; }

        public string SenderPassword { get; set; }

        public string SmtpHost { get; set; }

        public int SmtpPort { get; set; }

        public string SenderDisplayName { get; set; }
    }
}
