namespace API.DTO.Requests.User
{
    public class CreateUserDto
    {
        public Guid Id { get; } =  Guid.NewGuid();
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string? JobTitle { get; set; }
        public string? Department { get; set; }
    }
}
