public class RouteResponse
{
    public decimal TotalCost { get; set; }
    public List<string> Path { get; set; } = default!;
    public string Formatted { get; set; }
}