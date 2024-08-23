public class RouteRequest
{
    public int Id { get; set; }
    public string Origin { get; set; } = default!;
    public string Destination { get; set; } = default!;
    public int Price { get; set; }
}