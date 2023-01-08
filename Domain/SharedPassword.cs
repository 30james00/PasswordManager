namespace Domain;

public class SharedPassword
{
    public Guid Id { get; set; }

    public Account Account { get; set; }
    public Guid AccountId { get; set; }

    public SavedPassword SavedPassword { get; set; }
    public Guid SavedPasswordId { get; set; }
}