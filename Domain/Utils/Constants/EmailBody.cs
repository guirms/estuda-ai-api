namespace Domain.Utils.Constants
{
    public static class EmailBody
    {
        public static string GetPasswordRecoveryBody(string recoveryCode)
        {
            return $"<body style=\"width: 100%; text-align: center\">\r\n    <h2>Seu código de recuperação é:</h2>\r\n    <h1 style=\"background: #d1cfcf; display:inline-block;\">{recoveryCode}</h1>\r\n  </body>";
        }
    }
}
