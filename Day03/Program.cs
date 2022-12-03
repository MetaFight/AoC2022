using System.Collections;

bool isInTestMode = false;

var input =
    isInTestMode
        ? "test-in.txt"
        : "input.txt";

var data =
    File.ReadAllLines(input);


static int ToPriority(char value) => value switch
{
    >= 'a' and <= 'z' => 1 + value - 'a',
    >= 'A' and <= 'Z' => 27 + value - 'A',
    _ => throw new ArgumentOutOfRangeException()
};

// Part 1
{
    var priorities =
        data
            .Select(line => line.Substring(0, line.Length / 2).Intersect(line.Substring(line.Length / 2)).Single())
            .Select(ToPriority);

    //print(priorities);

    var part1 = priorities.Sum();
    Console.WriteLine($"Part 1: {part1}");
}


// Part 2
{
    static char GetGroupBadge(string[] group) => group[0].Intersect(group[1].Intersect(group[2])).Single();

    var part2 =
        data.Chunk(3)
            .Select(GetGroupBadge)
            .Select(ToPriority)
            .Sum();
    Console.WriteLine($"Part 2: {part2}");
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
