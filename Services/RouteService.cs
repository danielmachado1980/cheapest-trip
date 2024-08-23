public class RouteService
{
    private readonly RouteContext _context;

    public RouteService(RouteContext context)
    {
        _context = context;
    }

    public RouteResponse? FindCheapestRoute(string origin, string destination)
    {
        var allRoutes = _context.Routes.ToList();
        var graph = BuildGraph(allRoutes);

        var allPaths = new List<List<string>>();
        var path = new List<string>();
        var costs = new List<int>();

        FindAllPaths(graph, origin, destination, path, allPaths, 0, costs);

        // Encontra o caminho mais barato
        int minCostIndex = costs.IndexOf(costs.Min());
        var cheapestPath = allPaths[minCostIndex];
        var cheapestCost = costs[minCostIndex];

        return new RouteResponse
        {
            TotalCost = cheapestCost,
            Path = cheapestPath,
            Formatted = $"{string.Join(" - ", cheapestPath)} ao custo de {cheapestCost}"
        };
    }

    private Dictionary<string, List<(string Destination, int Cost)>> BuildGraph(List<RouteRequest> routes)
    {
        var graph = new Dictionary<string, List<(string Destination, int Cost)>>();

        foreach (var route in routes)
        {
            if (!graph.ContainsKey(route.Origin))
            {
                graph[route.Origin] = new List<(string Destination, int Cost)>();
            }

            graph[route.Origin].Add((route.Destination, route.Price));
        }

        return graph;
    }

    private void FindAllPaths(Dictionary<string, List<(string, int)>> graph, string current, string destination, List<string> path, List<List<string>> allPaths, int currentCost, List<int> costs)
    {
        // Adiciona o nó atual ao caminho
        path.Add(current);

        // Se o nó atual é o destino, salva o caminho e o custo
        if (current == destination)
        {
            allPaths.Add(new List<string>(path));
            costs.Add(currentCost);
        }
        else
        {
            // Se não for o destino, continua explorando os vizinhos
            foreach (var neighbor in graph[current])
            {
                if (!path.Contains(neighbor.Item1)) // Evita ciclos
                {
                    FindAllPaths(graph, neighbor.Item1, destination, path, allPaths, currentCost + neighbor.Item2, costs);
                }
            }
        }

        // Remove o nó atual do caminho para explorar outras rotas
        path.RemoveAt(path.Count - 1);
    }
}