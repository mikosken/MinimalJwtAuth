namespace AuthAPI.ViewModels
{
    public class UserViewModel
    {
        // Basic ViewModel for User info that is returned from API actions to clients.
        // Shouldn't contain anything we don't want the client to see.
        // We could add other information for clients ease of display here, but
        // only information in a validated token should be trusted by the server.
        public string? UserName { get; set; }
        //public DateTime Expires { get; set; }
        // This is only for ease of display, actual expiry is validated from JWT.
        public string? Token { get; set; }
    }
}