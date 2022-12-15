using System.Diagnostics;

bool isInTestMode = false;

var input =
    isInTestMode
        ? "input-testing.txt"
        : "input.txt";

var data =
    File.ReadAllLines(input);

var sensors =
    data.Select(line =>
        line.Split(new[] { "Sensor at x=", ", y=", ": closest beacon is at x=" }, StringSplitOptions.RemoveEmptyEntries)
            .Select(item => int.Parse(item))
            .ToArray()
    )
    .Select(row => new Sensor(row[0], row[1], new Beacon(row[2], row[3])));

var stopwatch = new Stopwatch();

// Part 1
{
    stopwatch.Start();

    var knownBeacons = sensors.Select(item => item.NearestBeacon).Distinct().ToList();

    var targetY = isInTestMode ? 10 : 2000000;
    var coverage =
        sensors
            .SelectMany(item => item.GetInfluenceAtY(targetY))
            .Distinct()
            .Except(knownBeacons.Where(item => item.Y == targetY).Select(item => item.X))
            .OrderBy(item => item)
            .ToList();

    var part1 = coverage.Count;

    stopwatch.Stop();

    Console.WriteLine($"Part 1: {part1}");
    Console.WriteLine($"Elapsed: {stopwatch.Elapsed}");
}

stopwatch.Reset();

// Part 2
{
    stopwatch.Start();
    sensors = sensors.ToList();

    var minX = 0;
    var minY = 0;
    var maxX = isInTestMode ? 20 : 4000000;
    var maxY = isInTestMode ? 20 : 4000000;

    (int x, int y) uncovered = default;
    bool done = false;
    for (int y = minY; y <= maxY && !done; y++)
    {
        var inRangeSensors = sensors.Where(item => y >= item.YLowerBound && y <= item.YUpperBound);
        var xRangesCovered = 
            inRangeSensors
                .Select(item => item.GetInfluenceRangeAtY(y, (minX, maxX)))
                .OrderBy(item => item.lowerBound)
                .ThenBy(item => item.upperBound)
                .ToList();

        var coveredRange = xRangesCovered[0];
        foreach (var range in xRangesCovered.Skip(1))
        {
            if (coveredRange.upperBound + 1 < range.lowerBound)
            {
                done = true;
                uncovered = (coveredRange.upperBound + 1, y);
                break;
            }
            coveredRange = (Math.Min(coveredRange.lowerBound, range.lowerBound), Math.Max(coveredRange.upperBound, range.upperBound));
        }

        if (!done && coveredRange.lowerBound != minX)
        {
            done = true;
            uncovered = (minX, y);
            break;
        }

        if (!done && coveredRange.upperBound != maxX)
        {
            done = true;
            uncovered = (maxX, y);
            break;
        }
    }

    stopwatch.Stop();

    var part2 = (uncovered.x * 4000000L + uncovered.y);
    Console.WriteLine($"Part 2: {part2}");
    Console.WriteLine($"Elapsed: {stopwatch.Elapsed}");
}

record Sensor(int X, int Y, Beacon NearestBeacon)
{
    private int BeaconDistance => Math.Abs(this.X - this.NearestBeacon.X) + Math.Abs(this.Y - this.NearestBeacon.Y);

    public IEnumerable<int> GetInfluenceAtY(int y, (int min, int max)? xBounds = null)
    {
        var yDistance = Math.Abs(this.Y - y);
        var horizontalEffect = this.BeaconDistance - yDistance;

        if (horizontalEffect > 0)
        {
            return Enumerable
                .Range(this.X - horizontalEffect, 1 + (2 * horizontalEffect))
                .Where(item => !xBounds.HasValue || (item >= xBounds.Value.min && item <= xBounds.Value.max));
        }
        else
        {
            return Enumerable.Empty<int>();
        }
    }

    public (int lowerBound, int upperBound) GetInfluenceRangeAtY(int y, (int min, int max) xBounds)
    {
        var yDistance = Math.Abs(this.Y - y);
        var horizontalEffect = this.BeaconDistance - yDistance;

        var lowerBound = Math.Max(xBounds.min, this.X - horizontalEffect);
        var upperBound = Math.Min(xBounds.max, lowerBound + (2 * horizontalEffect));

        return (lowerBound, upperBound);
    }

    public bool IsWithinRange(int x, int y) => (Math.Abs(this.X - x) + Math.Abs(this.Y - y)) <= this.BeaconDistance;

    public int YLowerBound => this.Y - this.BeaconDistance;
    public int YUpperBound => this.Y + this.BeaconDistance;
}

record Beacon(int X, int Y);