using System.Text;

const bool isInTestMode = false;

var input =
    isInTestMode
        ? "input-testing.txt"
        : "input.txt";

var data = File.ReadAllText(input);

const long debuggingIterationCount = 400 + (1400 * 3) + 5;
const bool isInCompareMode = false;

long part1;
// Part 1
{
    var gas = new Gas(data);

    var minY = -0L;
    var map = new Map();

    const long iterationCount = isInCompareMode ? debuggingIterationCount : 2022;
    for (int rockCount = 0; rockCount < iterationCount; rockCount++)
    {
        var rock = Rock.GetNext(minY);
        //DrawMap(map, rock);

        bool descended;
        do
        {
            // gas
            if (gas.IsLeft)
            {
                rock.TryMoveLeft(map);
            }
            else
            {
                rock.TryMoveRight(map);
            }

            // descent
            descended = rock.TryMoveDown(map);

            gas.Update();
        } while (descended);

        rock.AddSelfToMap(map, false, out var rockMinY, out _);
        minY = Math.Min(minY, rockMinY);
        
        PrintFinalSteps(iterationCount, rockCount, rock);
    };

    part1 = -minY;
    Console.WriteLine($"Part 1: {part1}");
    Console.WriteLine();
    Console.WriteLine();
}

Rock.Reset();

// Part 2
{
    var gas = new Gas(data);

    var minY = 0L;
    var drawDepthLimit = 0L;
    var map = new Map();

    const int gasPeriod = isInTestMode ? 40 : 10091;
    const int shapePeriod = 5;
    const int gasShapePeriod = shapePeriod * gasPeriod;

    const int stabilisationIterationCount = 2;
    const int gasShapeMapPeriodInGasShapeCycles = isInTestMode ? 7 : 339;

    long lastGasShapeLoopHeight = 0;
    long lastGasShapeMapLoopHeight = 0;

    long lastGasShapeIncrement = 0;

    long lastGasShapeMapIncrement = 0;
    int gasShapeMapLoopNumber = 0;


    long heightSkipped = 0;

    const long iterationCount = isInCompareMode ? debuggingIterationCount : 1_000_000_000_000;

    for (var rockCount = 0L; rockCount < iterationCount; rockCount++)
    {
        bool isGasShapeLoopPoint = rockCount % gasShapePeriod == 0;
        if (isGasShapeLoopPoint)
        {
            long gashShapeLoopNumber = rockCount / gasShapePeriod;
            bool isStabilised = gashShapeLoopNumber >= stabilisationIterationCount;

            var gasShapeIncrement = lastGasShapeLoopHeight - minY;
            lastGasShapeLoopHeight = minY;
            lastGasShapeIncrement = gasShapeIncrement;

            //Console.WriteLine($"Gas-Shape Loop: {gashShapeLoopNumber:0000} -- Increment: {gasShapeIncrement}");

            bool isGasShapeMapLoopPoint = isStabilised && (gashShapeLoopNumber - stabilisationIterationCount) % gasShapeMapPeriodInGasShapeCycles == 0;
            if (isGasShapeMapLoopPoint)
            {
                var gasShapeMapIncrement = lastGasShapeMapLoopHeight - minY;
                var isIncrementMatch = gasShapeMapIncrement == lastGasShapeMapIncrement;

                lastGasShapeMapIncrement = gasShapeMapIncrement;
                lastGasShapeMapLoopHeight = minY;

                Console.WriteLine($"= Gas-Shape-Map Loop: {gasShapeMapLoopNumber:0000} -- Rock Count : {rockCount} -- Increment {gasShapeMapIncrement}");
                gasShapeMapLoopNumber++;

                if (isIncrementMatch || gasShapeMapLoopNumber == 2)
                {
                    var fastForwardAmount = gasShapePeriod * gasShapeMapPeriodInGasShapeCycles;

                    var remainingRocks = iterationCount - rockCount;
                    var skipCount = remainingRocks / fastForwardAmount;

                    var rocksSkipped = skipCount * fastForwardAmount;
                    heightSkipped = skipCount * gasShapeMapIncrement;

                    Console.WriteLine();
                    Console.WriteLine("!!!!!!!!!!!!!!!");
                    Console.WriteLine($"Skipping {skipCount} gas-shape-map cycles, {rocksSkipped} rocks, {heightSkipped} units of height.");
                    Console.WriteLine("!!!!!!!!!!!!!!!");
                    Console.WriteLine();
                    rockCount += rocksSkipped;
                    rockCount--;
                    //rockCount -= 2;
                    continue;
                }
            }
        }


        var rock = Rock.GetNext(minY);

        bool descended;
        do
        {
            // gas
            if (gas.IsLeft)
            {
                rock.TryMoveLeft(map);
            }
            else
            {
                rock.TryMoveRight(map);
            }

            // descent
            descended = rock.TryMoveDown(map);

            gas.Update();
        } while (descended);

        rock.AddSelfToMap(map, map.Count > 25_000_000, out var rockMinY, out var mapMaxY);

        minY = Math.Min(minY, rockMinY);
        drawDepthLimit = mapMaxY ?? drawDepthLimit;

        PrintFinalSteps(iterationCount, rockCount, rock);
    };

    Console.WriteLine();
    Console.WriteLine();

    var part2 = heightSkipped - minY;
    Console.WriteLine($"Part 2: {part2}");

    if (isInCompareMode)
    {
        Console.WriteLine();
        Console.WriteLine($"Is Match: {part1 == part2}");
    }
}

void DrawMap(HashSet<(int x, long y)> map, Rock? rock = null, long drawDepthLimit = 0L)
{
    var mapMinY = map.Count > 0 ? map.Min(item => item.y) : 0;
    var rockMinY = rock?.MapMinY ?? 0;

    var minY = Math.Min(mapMinY, rockMinY);

    StringBuilder output = new StringBuilder();
    output.AppendLine();

    for (long y = minY; y <= drawDepthLimit; y++)
    {
        output.Append("|");
        for (int x = 0; x < 7; x++)
        {
            output.Append(
                map.Contains((x, y))
                    ? "#"
                    : (rock?.GlobalPoints.Contains((x, y)) ?? false)
                        ? "@"
                        : ".");
        }
        output.AppendLine("|");
    }

    output.Append("+-------+");

    Console.WriteLine(output.ToString());
}

static void PrintFinalSteps(long iterationCount, long rockCount, Rock rock)
{
    var remaining = iterationCount - rockCount;
    if (remaining <= 5 && isInCompareMode)
    {
        Console.WriteLine($"T-minus {remaining} -- rock type: {rock.Type.ToString()}");
    }
}

class Rock
{
    private static readonly RockType[] Types = Enum.GetValues<RockType>();
    private static int NextTypeIndex;

    public static void Reset() => NextTypeIndex = 0;

    public static Rock GetNext(long minY)
    {
        var rockType = Types[NextTypeIndex];

        NextTypeIndex++;
        NextTypeIndex %= Types.Length;

        var bottomOffset = rockType switch
        {
            RockType.Horizontal => 0,
            RockType.Plus => 2,
            RockType.J => 2,
            RockType.Vertical => 3,
            RockType.Square => 1,
            _ => throw new ArgumentException()
        };

        return new Rock(2, minY - 3 - bottomOffset, rockType);
    }

    private readonly HashSet<(int x, int y)> localPoints;

    private Rock(int x, long y, RockType type)
    {
        this.X = x;
        this.Y = y;
        this.Type = type;
        this.localPoints = type switch
        {
            RockType.Horizontal => new() { (0, 0), (1, 0), (2, 0), (3, 0) },
            RockType.Plus => new() { (1, 0), (0, 1), (1, 1), (2, 1), (1, 2) },
            RockType.J => new() { (2, 0), (2, 1), (0, 2), (1, 2), (2, 2) },
            RockType.Vertical => new() { (0, 0), (0, 1), (0, 2), (0, 3) },
            RockType.Square => new() { (0, 0), (1, 0), (0, 1), (1, 1) },
            _ => throw new ArgumentException()
        };
    }

    public bool TryMoveLeft(Map map, int minX = 0)
    {
        var mapPoints = this.localPoints.Select(item => (x: this.X + item.x - 1, y: this.Y + item.y));

        var collidesWithBounds = (this.X - 1) < minX;
        if (collidesWithBounds) return false;

        var collidesWithMap = mapPoints.Any(point => map.Contains(point.x, point.y));
        if (collidesWithMap) return false;

        this.X--;
        return true;
    }

    public bool TryMoveRight(Map map, int maxX = 6)
    {
        var mapPoints = this.localPoints.Select(item => (x: this.X + item.x + 1, y: this.Y + item.y));

        var collidesWithBounds = (this.X + this.Width) > maxX;
        if (collidesWithBounds) return false;

        var collidesWithMap = mapPoints.Any(point => map.Contains(point.x, point.y));
        if (collidesWithMap) return false;

        this.X++;
        return true;
    }

    public bool TryMoveDown(Map map, int maxY = 0)
    {
        var mapPoints = this.localPoints.Select(item => (x: this.X + item.x, y: this.Y + item.y + 1));

        var collidesWithBounds = mapPoints.Any(point => point.y > maxY);
        if (collidesWithBounds) return false;

        var collidesWithMap = mapPoints.Any(point => map.Contains(point.x, point.y));
        if (collidesWithMap) return false;

        this.Y++;
        return true;
    }

    public void AddSelfToMap(Map map, bool attemptPruning, out long rockMinY, out long? mapMaxY)
    {
        rockMinY = 0;
        mapMaxY = null;

        // Update map
        foreach (var mapPoint in this.GlobalPoints)
        {
            if (mapPoint.y < rockMinY)
            {
                rockMinY = mapPoint.y;
            }
            map.Add(mapPoint.x, mapPoint.y);
        };

        // Look for blockages and prune map
        if (attemptPruning)
        {
            foreach (var mapPoint in this.GlobalPoints.OrderBy(item => item.x))
            {
                // naive search for horizontal or diagonal lines across field
                // and naive pruning
                var createsBlockage =
                    ConnectToLeftWall(mapPoint.x, mapPoint.y) &&
                    ConnectToRightWall(mapPoint.x, mapPoint.y);

                if (createsBlockage)
                {
                    // naive calculation of prune point
                    var prunePoint = mapPoint.y + 8;
                    var removeCount = map.Prune(prunePoint);

                    //Console.WriteLine($"Pruned {removeCount} points");

                    mapMaxY = prunePoint;
                    break;
                }
            }
        }

        rockMinY--;

        bool ConnectToLeftWall(int x, long y)
        {
            if (x == 0) return true;
            return (
                (map.Contains(x - 1, y) && ConnectToLeftWall(x - 1, y)) ||
                (map.Contains(x - 1, y - 1) && ConnectToLeftWall(x - 1, y - 1)) ||
                (map.Contains(x - 1, y + 1) && ConnectToLeftWall(x - 1, y + 1))
                );
        }

        bool ConnectToRightWall(int x, long y)
        {
            if (x == 6) return true;
            return (
                (map.Contains(x + 1, y) && ConnectToRightWall(x + 1, y)) ||
                (map.Contains(x + 1, y - 1) && ConnectToRightWall(x + 1, y - 1)) ||
                (map.Contains(x + 1, y + 1) && ConnectToRightWall(x + 1, y + 1))
                );
        }
    }

    public int X { get; private set; }
    public long Y { get; private set; }
    public RockType Type { get; }

    public int Width => this.Type switch
    {
        RockType.Horizontal => 4,
        RockType.Plus => 3,
        RockType.J => 3,
        RockType.Vertical => 1,
        RockType.Square => 2,
        _ => throw new ArgumentException()
    };
    public long MapMinY => this.localPoints.Min(item => item.y) + this.Y;
    public IEnumerable<(int x, long y)> GlobalPoints => this.localPoints.Select(item => (x: item.x + this.X, y: item.y + this.Y));
}

enum RockType
{
    Horizontal,
    Plus,
    J,
    Vertical,
    Square
}

class Map
{
    private readonly HashSet<long>[] columns;

    public Map()
    {
        this.columns = Enumerable.Range(0, 7).Select(item => new HashSet<long>()).ToArray();
    }

    public bool Contains(int x, long y)
    {
        if (x < 0 || x > 6) return false;
        return this.columns[x].Contains(y);
    }

    public long Prune(long prunePoint) => this.columns.Sum(set => set.RemoveWhere(item => item >= prunePoint));

    public void Add(int x, long y) => this.columns[x].Add(y);

    public long Count =>
        this.columns[0].Count +
        this.columns[1].Count +
        this.columns[2].Count +
        this.columns[3].Count +
        this.columns[4].Count +
        this.columns[5].Count +
        this.columns[6].Count;
}

class Gas
{
    private readonly string buffer;
    private int position;

    public Gas(string buffer)
    {
        this.position = 0;
        this.buffer = buffer;
    }

    public void Update()
    {
        this.position++;
        this.position %= this.buffer.Length;
    }

    public bool IsLeft => this.buffer[this.position] == '<';
    public bool IsRight => this.buffer[this.position] == '>';
}
