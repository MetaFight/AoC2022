bool isInTestMode = false;

var input =
    isInTestMode
        ? "input-testing.txt"
        : "input.txt";

var data =
    File.ReadAllLines(input);


// Part 1
{
    //var vertices = GetVertices(data, out var width, out var height);
    //var start = vertices.Single(item => item.IsStart);
    //var end = vertices.Single(item => item.IsEnd);

    //var part1 = SolvePathLength(vertices, start, end);
    //Console.WriteLine($"Part 1: {part1}");
}

// Part 2
{
    var vertices = GetVertices(data, out var width, out var height);
    var end = vertices.Single(item => item.IsEnd);
    var startCandidates = vertices.Where(item => item.Height == 0).OrderBy(start => Math.Abs(end.X - start.X) + Math.Abs(end.Y - start.Y)).ToList();

    uint minPathLength = int.MaxValue;
    foreach (var start in startCandidates)
    {
        foreach (var item in vertices)
        {
            item.DistanceFromStart = int.MaxValue;
        }
        start.DistanceFromStart = 0;

        var length = SolvePathLength(vertices, start, end, abortThreshold: minPathLength);
        if (length < minPathLength)
        {
            minPathLength = length;
        }
    }


    var part2 = minPathLength;
    Console.WriteLine($"Part 2: {part2}");
}

static uint SolvePathLength(IEnumerable<Vertex> vertices, Vertex start, Vertex end, uint abortThreshold = int.MaxValue)
{
    var unvisited = new HashSet<Vertex>(vertices);
    unvisited.Remove(start);

    var current = start;
    while (unvisited.Count > 0)
    {
        uint newDistance = current.DistanceFromStart + 1;

        if (newDistance > abortThreshold)
        {
            return int.MaxValue;
        }

        var unvisitedNeighbors = current.NavigableNeighbours.Intersect(unvisited).ToList();
        foreach (var neighbour in unvisitedNeighbors)
        {
            if (neighbour.DistanceFromStart > newDistance)
            {
                neighbour.DistanceFromStart = newDistance;
            }
        }

        if (current == end)
        {
            break;
        }

        current = unvisited.OrderBy(item => item.DistanceFromStart).First();
        unvisited.Remove(current);
    }

    return end.DistanceFromStart;
}

static IEnumerable<Vertex> GetVertices(string[] data, out int width, out int height)
{
    var vertices =
        data.Select((line, y) => line.Select((value, x) => new Vertex(x, y, value)).ToArray())
            .ToArray();

    width = vertices[0].Length;
    height = vertices.Length;

    var result = new List<Vertex>();

    for (int y = 0; y < height; y++)
    {
        for (int x = 0; x < width; x++)
        {
            var vertex = vertices[y][x];

            var north = y >= 1 ? vertices[y - 1][x] : null;
            var east = x < width - 1 ? vertices[y][x + 1] : null;
            var south = y < vertices.Length - 1 ? vertices[y + 1][x] : null;
            var west = x >= 1 ? vertices[y][x - 1] : null;

            vertex.Connect(north, east, south, west);

            //yield return vertex;
            result.Add(vertex);
        }
    }

    return result;
}

class Vertex
{
    public int Height { get; }
    public uint DistanceFromStart { get; set; }
    public List<Vertex> NavigableNeighbours { get; private set; }
    public bool IsStart { get; }
    public bool IsEnd { get; }
    public int X { get; }
    public int Y { get; }

    public Vertex(int x, int y, char value)
    {
        this.Height = value switch
        {
            'S' => 0,
            'E' => 25,
            >= 'a' and <= 'z' => value - 'a',
            _ => throw new ArgumentOutOfRangeException(nameof(value)),
        };
        this.NavigableNeighbours = new List<Vertex>();

        this.IsStart = value == 'S';
        this.IsEnd = value == 'E';

        this.DistanceFromStart = this.IsStart ? 0 : (uint)int.MaxValue;
        this.X = x;
        this.Y = y;
    }

    internal void Connect(params Vertex?[] neighbours)
    {
        this.NavigableNeighbours.AddRange(neighbours.Where(item => item is not null && item.Height <= this.Height + 1)!);
    }
}