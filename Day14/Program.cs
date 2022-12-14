using System.Text;

bool isInTestMode = false;

var input =
    isInTestMode
        ? "input-testing.txt"
        : "input.txt";

var data =
    File.ReadAllLines(input);



// Part 1
{
    var (map, sourceX, sourceY, width, height) = BuildMap(data);

    var sandCount = Part1Simulation(map, sourceX, sourceY, width, height);

    //PrintMap(map);

    var part1 = sandCount;
    Console.WriteLine($"Part 1: {part1}");
}

// Part 2
{
    var rocks =
        data.Select(line =>
            line.Split(" -> ")
                .Select(pair => pair.Split(",").Select(item => int.Parse(item)).ToArray())
                .Select(pair => (x: pair[0], y: pair[1]))
                .ToList())
            .SelectMany(ToPixels);

    var map = new Map(rocks, new Pixel(500, 0, '+'));


    var part2 = map.Simulate();
    Console.WriteLine($"Part 2: {part2}");
    //File.WriteAllText("./out.txt", map.ToString());
}

IEnumerable<Pixel> ToPixels(List<(int x, int y)> points)
{
    for (int i = 0; i < points.Count - 1; i++)
    {
        var from = points[i];
        var to = points[i + 1];

        int x0 = Math.Min(from.x, to.x);
        int x1 = Math.Max(from.x, to.x);
        int y0 = Math.Min(from.y, to.y);
        int y1 = Math.Max(from.y, to.y);

        for (int x = x0; x <= x1; x++)
        {
            for (int y = y0; y <= y1; y++)
            {
                if (x == from.x && y == from.y && i != 0)
                {
                    continue;
                }
                yield return new Pixel(x, y, '#');
            }
        }
    }
}

(Pixel[,] map, int sourceX, int sourceY, int width, int height) BuildMap(string[] data)
{
    var rocksByPosition =
        data.Select(line =>
            line.Split(" -> ")
                .Select(pair => pair.Split(",").Select(item => int.Parse(item)).ToArray())
                .Select(pair => (x: pair[0], y: pair[1]))
                .ToList())
            .SelectMany(ToPixels)
            .DistinctBy(item => (item.X, item.Y))
            .ToDictionary(key => (x: key.X, y: key.Y), value => value);

    var source = new Pixel(500, 0, '+');
    var minX = Math.Min(source.X, rocksByPosition.Values.Min(item => item.X));
    var maxX = Math.Max(source.X, rocksByPosition.Values.Max(item => item.X));
    var minY = 0;
    var maxY = Math.Max(source.Y, rocksByPosition.Values.Max(item => item.Y));
    var width = maxX - minX + 1;
    var height = maxY - minY + 1;

    var map = new Pixel[width, height];
    for (int j = 0; j < height; j++)
    {
        var y = j + minY;
        for (int i = 0; i < width; i++)
        {
            var x = i + minX;

            if (x == source.X && y == source.Y)
            {
                map[i, j] = source;
                continue;
            }

            map[i, j] = rocksByPosition.GetValueOrDefault((x: x, y: y), new Pixel(x, y, '.'));
        }
    }

    return (map, source.X - minX, source.Y - minY, width, height);
}

void PrintMap(Pixel[,] map)
{
    var width = map.GetLength(0);
    var height = map.GetLength(1);

    for (int j = 0; j < height; j++)
    {
        for (int i = 0; i < width; i++)
        {
            Console.Write(map[i, j].Type);
        }
        Console.WriteLine();
    }
}

static int Part1Simulation(Pixel[,] map, int sourceX, int sourceY, int width, int height)
{
    int sandCount = 0;
    bool isOffMap = false;
    while (!isOffMap)
    {
        //if (sandCount < 20 || sandCount % 100 == 0)
        //{
        //    Console.WriteLine($"============= {sandCount:000} =============");
        //    PrintMap(map);
        //    Console.WriteLine();
        //}
        int i = sourceX;
        int j = sourceY;

        while (j < height)
        {
            var targetY = j + 1;

            if (targetY >= height)
            {
                isOffMap = true;
                break;
            }

            var below = map[i, targetY];

            if (below.IsSolid)
            {
                // roll left
                var targetX = i - 1;
                if (targetX < 0)
                {
                    isOffMap = true;
                    break;
                }

                if (map[targetX, targetY].IsAir)
                {
                    i = targetX;
                    j = targetY;
                    continue;
                }

                // roll right
                targetX = i + 1;

                if (targetX >= width)
                {
                    isOffMap = true;
                    break;
                }

                if (map[targetX, targetY].IsAir)
                {
                    i = targetX;
                    j = targetY;
                    continue;
                }

                sandCount++;
                map[i, j].Type = 'o';
                break;
            }
            else
            {
                j = targetY;
                continue;
            }
        }
    }

    return sandCount;
}

class Map
{
    private readonly Dictionary<(int x, int y), Pixel> pixels;
    private readonly int sourceX;
    private readonly int sourceY;
    private readonly int floorY;

    public Map(IEnumerable<Pixel> rocks, Pixel source)
    {
        this.pixels = new Dictionary<(int x, int y), Pixel>();
        this.sourceX = source.X;
        this.sourceY = source.Y;

        int maxY = 0;
        foreach (var rock in rocks)
        {
            this.pixels[(rock.X, rock.Y)] = rock;
            if (rock.Y > maxY)
            {
                maxY = rock.Y;
            }
        }

        this.pixels[(source.X, source.Y)] = source;

        this.floorY = maxY + 2;
    }

    public int Simulate()
    {
        int sandCount = 0;
        bool isFinished = false;
        while (!isFinished)
        {
            //if (sandCount < 100 || sandCount % 100 == 0)
            //{
            //    Console.WriteLine($"============= {sandCount:000} =============");
            //    Console.WriteLine(this);
            //    Console.WriteLine();
            //}
            int i = sourceX;
            int j = sourceY;

            while (j < this.floorY)
            {
                var targetY = j + 1;

                if (this.IsSolid(i, targetY))
                {
                    // roll left
                    var targetX = i - 1;
                    if (!this.IsSolid(targetX, targetY))
                    {
                        i = targetX;
                        j = targetY;
                        continue;
                    }

                    // roll right
                    targetX = i + 1;
                    if (!this.IsSolid(targetX, targetY))
                    {
                        i = targetX;
                        j = targetY;
                        continue;
                    }

                    sandCount++;
                    this.pixels[(i, j)] = new Pixel(i, j, 'o');

                    if (i == sourceX && j == sourceY)
                    {
                        isFinished = true;
                    }
                    break;
                }
                else
                {
                    j = targetY;
                    continue;
                }
            }
        }

        return sandCount;
    }

    private bool IsSolid(int x, int y) => y == this.floorY || this.pixels.ContainsKey((x, y));

    public override string ToString()
    {
        var result = new StringBuilder();

        var minX = this.pixels.Values.Min(item => item.X);
        var maxX = this.pixels.Values.Max(item => item.X);

        for (int y = 0; y < this.floorY; y++)
        {
            for (int x = minX; x <= maxX; x++)
            {
                if (this.pixels.TryGetValue((x, y), out var value))
                {
                    result.Append(value.Type);
                }
                else
                {
                    result.Append('.');
                }
            }
            result.AppendLine();
        }

        result.AppendLine(new string('#', maxX - minX + 1));

        return result.ToString();
    }
}

class Pixel
{
    public Pixel(int X, int Y, char Type)
    {
        this.X = X;
        this.Y = Y;
        this.Type = Type;
    }

    public bool IsRock => this.Type == '#';
    public bool IsAir => this.Type == '.';
    public bool IsSand => this.Type == 'o';
    public bool IsSandSource => this.Type == '+';
    public bool IsSolid => this.IsRock || this.IsSand;

    public int X { get; }
    public int Y { get; }
    public char Type { get; set; }
}