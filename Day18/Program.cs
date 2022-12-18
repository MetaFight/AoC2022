bool isInTestMode = false;

var input =
    isInTestMode
        ? "input-testing.txt"
        : "input.txt";

var data =
    File.ReadAllLines(input);

int totalExposedFaces = 0;
// Part 1
{
    var droplets =
        data.Select(line => line.Split(",").Select(item => int.Parse(item)).ToArray())
            .Select(tokens => (x: tokens[0], y: tokens[1], z: tokens[2]));

    totalExposedFaces = CountExposedFaces(droplets);

    var part1 = totalExposedFaces;
    Console.WriteLine($"Part 1: {part1}");
}

// Part 2
{
    var droplets =
        data.Select(line => line.Split(",").Select(item => int.Parse(item)).ToArray())
            .Select(tokens => (x: tokens[0], y: tokens[1], z: tokens[2]))
            .ToList();

    var minX = droplets.Min(item => item.x);
    var maxX = droplets.Max(item => item.x);
    var minY = droplets.Min(item => item.y);
    var maxY = droplets.Max(item => item.y);
    var minZ = droplets.Min(item => item.z);
    var maxZ = droplets.Max(item => item.z);

    var emptySpaces =
        Enumerable.Range(minX, maxX + 1)
            .SelectMany(x =>
                Enumerable.Range(minY, maxY + 1)
                    .SelectMany(y =>
                        Enumerable.Range(minZ, maxZ + 1).Select(z => (x, y, z))))
            .Except(droplets)
            .ToList();

    // Partition into contiguous spaces
    var domains = new List<List<(int x, int y, int z)>>();
    var unprocessed = new List<(int x, int y, int z)>(emptySpaces);
    while (unprocessed.Count > 0)
    {
        var continguousSpace = new List<(int x, int y, int z)>();

        var start = unprocessed[0];

        unprocessed.RemoveAt(0);
        continguousSpace.Add(start);
        ExpandOn(start);

        void ExpandOn((int x, int y, int z) start)
        {
            foreach (var neighbor in GetNeighbors(start).Intersect(unprocessed))
            {
                unprocessed.Remove(neighbor);
                continguousSpace.Add(neighbor);
                ExpandOn(neighbor);
            }
        }

        domains.Add(continguousSpace);
    }

    // Remove any continguousSpace that touches the total space edges
    var internalSpaces = domains.Where(domain => !domain.Any(TouchesSpaceEdge)).ToList();

    bool TouchesSpaceEdge((int x, int y, int z) position)
    {
        var (x, y, z) = position;

        return x == minX || x == maxX || y == minY || y == maxY || z == minZ || z == maxZ;
    }

    var internalFaces = internalSpaces.Select(space => CountExposedFaces(space.Distinct())).ToList();

    // 1155 is too low

    var part2 = totalExposedFaces - internalFaces.Sum();
    Console.WriteLine($"Part 2: {part2}");
}

static int CountExposedFaces(IEnumerable<(int x, int y, int z)> droplets)
{
    int result = 0;
    foreach (var droplet in droplets)
    {
        var neighbors = GetNeighbors(droplet);
        var exposedFaces = 6 - droplets.Intersect(neighbors).Count();

        result += exposedFaces;
    }

    return result;
}

static (int x, int y, int z)[] GetNeighbors((int x, int y, int z) droplet)
{
    return new[] {
            (droplet.x + 1, droplet.y, droplet.z),
            (droplet.x - 1, droplet.y, droplet.z),
            (droplet.x, droplet.y + 1, droplet.z),
            (droplet.x, droplet.y - 1, droplet.z),
            (droplet.x, droplet.y, droplet.z + 1),
            (droplet.x, droplet.y, droplet.z - 1)
            };
}