namespace Pinterest.Api.Accounts.Requests;

public class NewPostRequest
{
    public string Title { get; set; } = string.Empty;
    public IFormFile FileContent { get; set; } 
}