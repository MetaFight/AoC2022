using System.Collections;

bool isInTestMode = false;

var input =
    isInTestMode
        ? "input-testing2.txt"
        : "input.txt";

var data =
    File.ReadAllLines(input);

var instructions =
    data.Select(line => line.Split())
        .SelectMany(line => Enumerable.Repeat(ConvertDirection(line[0]), int.Parse(line[1])))
        .ToList();

Point ConvertDirection(string direction)
    => direction switch
    {
        "U" => new Point(0, +1),
        "D" => new Point(0, -1),
        "L" => new Point(-1, 0),
        "R" => new Point(+1, 0),
        _ => throw new NotImplementedException()
    };

// Part 1
{
    var origin = new Point(0, 0);
    var head = origin;
    var tail = origin;

    var tailPositions = new List<Point>();
    tailPositions.Add(tail);

    foreach (var item in instructions)
    {
        head += item;
        tail = tail.Chase(head);

        tailPositions.Add(tail);

        //printState(item, head, new List<Point>(), tail);
    }

    var distinctLocations = tailPositions.Distinct().ToList();
    var part1 = distinctLocations.Count();
    Console.WriteLine($"Part 1: {part1}");
}

// Part 2
{
    var origin = new Point(0, 0);
    var head = origin;
    var body = Enumerable.Repeat(origin, 8).ToList();
    var tail = origin;

    var tailPositions = new List<Point>();
    tailPositions.Add(tail);

    foreach (var item in instructions)
    {
        head += item;
        var previous = head;

        var newBody = new List<Point>();
        foreach (var segment in body)
        {
            var newSegment = segment.Chase(previous);
            newBody.Add(newSegment);
            previous = newSegment;
        }
        body = newBody;

        tail = tail.Chase(previous);

        //printState(item, head, body, tail, xMin: -11, width: 25, yMin: -5, height: 21);

        tailPositions.Add(tail);
    }

    var distinctLocations = tailPositions.Distinct().ToList();
    var part2 = distinctLocations.Count();
    Console.WriteLine($"Part 2: {part2}");
}

static void printState(Point instruction, Point head, List<Point> body, Point tail, int xMin = 0, int width = 6, int yMin = 0, int height = 5)
{
    Console.WriteLine($"Instruction: {instruction}");
    for (int j = height - 1; j >= 0; j--)
    {
        for (int i = 0; i < width; i++)
        {
            char output = '.';

            var x = xMin + i;
            var y = yMin + j;
            var point = new Point(x, y);

            if (x == 0 && y == 0)
            {
                output = 's';
            }

            if (tail == point)
            {
                output = 'T';
            }

            int bodyIndex = 0;
            foreach (var segment in body)
            {
                if (segment == point)
                {
                    output = (char)('0' + bodyIndex + 1);
                    break;
                }
                bodyIndex++;
            }

            if (head == point)
            {
                output = 'H';
            }
            Console.Write(output);
        }
        Console.WriteLine();
    }
    Console.WriteLine();
}

static void print(IEnumerable source, int indent = 0)
{
    var padding = new string(' ', indent);
    foreach (var item in source)
    {
        if (item is not string && item is IEnumerable nested)
        {
            Console.WriteLine(padding + ">");
            print(nested, indent + 2);
            Console.WriteLine(padding + "<");
        }
        else
        {
            Console.WriteLine(
                padding +
                item.ToString().PadLeft(indent));
        }
    }
}

record Point(int X, int Y)
{
    public Point Chase(Point leader)
    {
        int xDiff = leader.X - this.X;
        int yDiff = leader.Y - this.Y;

        int dominantPullDirection = Math.Abs(xDiff) - Math.Abs(yDiff);

        // x
        if (dominantPullDirection > 0)
        {
            return leader - new Point(Math.Sign(xDiff), 0);
        }

        // y
        if (dominantPullDirection < 0)
        {
            return leader - new Point(0, Math.Sign(yDiff));
        }

        // diagonal
        if (dominantPullDirection == 0)
        {
            // within 1 diagonally means no motion
            if (Math.Abs(xDiff) <= 1)
            {
                return this;
            }

            return leader - new Point(Math.Sign(xDiff), Math.Sign(yDiff));
        }

        return this;
    }

    public static Point operator +(Point a, Point b) => new Point(a.X + b.X, a.Y + b.Y);
    public static Point operator -(Point a, Point b) => new Point(a.X - b.X, a.Y - b.Y);
}