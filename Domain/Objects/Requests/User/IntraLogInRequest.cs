namespace Domain.Objects.Requests.User
{
    public record IntraLogInRequest
    {
        public required string UserName { get; set; }
        public required string Password { get; set; }
    }
}
