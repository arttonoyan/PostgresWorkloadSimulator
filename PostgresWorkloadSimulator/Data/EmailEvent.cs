namespace PgScalabilityTest.Data;

public class EmailEvent
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public DateTime Timestamp { get; set; }
    public int Value { get; set; }
}
