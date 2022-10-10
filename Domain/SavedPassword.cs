namespace Domain;

public class SavedPassword
{
    public Guid Id { get; set; }
    public String Password { get; set; }
    public String WebAdress { get; set; }
    public String Description { get; set; }
    public String Login { get; set; }
    
    public Account Account  { get; set; }
    public String AccountId { get; set; }
}