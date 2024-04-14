namespace Domain.Objects.Responses.Machine
{
    public record UserSessionResponse
    {
        public bool IsAuthenticated { get; set; }
        public bool HasAsset { get; set; }
    }
}

