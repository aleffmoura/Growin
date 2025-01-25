namespace Growin.ApplicationService.ViewModels;

public record OrderResumeViewModel
{
    public Identifier Id { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Quantity { get; set; } = string.Empty;
}
