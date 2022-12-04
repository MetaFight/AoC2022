using System.Collections;

bool isInTestMode = false;

var input =
    isInTestMode
        ? "test-in.txt"
        : "input.txt";

var data =
    File.ReadAllLines(input)
        .Select(line => line.Split(new[] { '-', ',' }, StringSplitOptions.TrimEntries).Select(item => int.Parse(item)).Chunk(2))
        .Select(pair => pair.Select(item => (from: item[0], to: item[1])))
        .ToList();
        


// Part 1
{
    var part1 =
        data.Select(pair => pair.OrderBy(item => item.from).ThenByDescending(item => item.to).ToList())
            .Where(pair => pair[0].from <= pair[1].from && pair[1].to <= pair[0].to)
            .Count();
    Console.WriteLine($"Part 1: {part1}");
}


// Part 2
{
    var part2 =
        data.Select(pair => pair.OrderBy(item => item.from).ThenByDescending(item => item.to).ToList())
            .Where(pair => pair[0].to >= pair[1].from)
            .Count();
    Console.WriteLine($"Part 1: {part2}");
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
