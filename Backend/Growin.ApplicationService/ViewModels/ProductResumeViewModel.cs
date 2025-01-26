namespace Growin.ApplicationService.ViewModels;
public record ProductResumeViewModel
{
    public Identifier Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public long QuantityInStock { get; set; }
}
