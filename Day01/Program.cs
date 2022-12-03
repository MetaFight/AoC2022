using System.Collections;

bool isInTestMode = false;

var input =
    isInTestMode
        ? "test-in.txt"
        : "input.txt";

var data =
    File.ReadAllText(input)
        .Split("\r\n\r\n")
        .Select(item => item.Split("\r\n", StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)))
        .ToList();

//print(data);


var part1 =
    data.Select(item => item.Sum())
        .Max();
Console.WriteLine($"Part 1: {part1}");


var part2 =
    data.Select(item => item.Sum())
        .OrderByDescending(item => item)
        .Take(3)
        .Sum();
Console.WriteLine($"Part 2: {part2}");






void print(IEnumerable source, int indent = 0)
{
    var padding = new string(' ', indent);
    foreach (var item in source)
    {
        if (item is IEnumerable nested)
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