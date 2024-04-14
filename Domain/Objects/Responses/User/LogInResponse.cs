namespace Domain.Objects.Responses.Asset
{
    public record LogInResponse
    {
        public LogInResponse(string token)
        {
            Token = token;
        }

        public string Token { get; set; }
    }
}
