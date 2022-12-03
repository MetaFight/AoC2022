using System.Collections;

bool isInTestMode = false;

var input =
    isInTestMode
        ? "test-in.txt"
        : "input.txt";

var data =
    File.ReadAllLines(input);


int ToPriority(char value) => value switch
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

    var groups =
        Enumerable.Range(0, data.Length / 3)
            .Select(i => (start: i * 3, end: (i + 1) * 3))
            .Select(range => data[range.start..range.end]);

    var part2 =
        groups
            .Select(GetGroupBadge)
            .Select(ToPriority)
            .Sum();
    Console.WriteLine($"Part 2: {part2}");
}

void print(IEnumerable source, int indent = 0)
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
