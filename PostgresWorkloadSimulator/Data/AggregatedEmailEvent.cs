namespace PgScalabilityTest.Data;

public class AggregatedEmailEvent
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public DateTime Date { get; set; }
    public int TotalValue { get; set; }
}
