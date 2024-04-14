namespace Domain.Objects.Responses.Asset
{
    public record LogInResponse
    {
        public LogInResponse(DateTime? lastLogin, string sessionToken)
        {
            LastLogin = lastLogin;
            SessionToken = sessionToken;
        }

        public DateTime? LastLogin { get; set; }
        public string SessionToken { get; set; }
    }
}
