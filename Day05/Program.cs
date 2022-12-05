using System.Collections;

bool isInTestMode = false;

var input =
    isInTestMode
        ? "input-testing.txt"
        : "input.txt";

var data =
    File.ReadAllText(input)
        .Split("\r\n\r\n");

// Part 1
{
    var stacks = GetInitialStacks(data);

    var instructionData = data[1].Split("\r\n");
    var instructions =
        instructionData
            .Select(line => StepSlice(line.Split(), 1, 1).Select(item => int.Parse(item)).ToArray())
            .SelectMany(line => Enumerable.Repeat((from: line[1], to: line[2]), line[0]))
            .ToList();

    foreach (var instruction in instructions)
    {
        stacks[instruction.to - 1].Push(stacks[instruction.from - 1].Pop());
    }

    var part1 = new string(stacks.Select(item => item.First()).ToArray());
    Console.WriteLine($"Part 1: {part1}");
}

// Part 2
{
    var stacks = GetInitialStacks(data);

    var instructionData = data[1].Split("\r\n");
    var instructions =
        instructionData
            .Select(line => StepSlice(line.Split(), 1, 1).Select(item => int.Parse(item)).ToArray())
            .Select(line => (from: line[1], to: line[2], count: line[0]))
            .ToList();

    var transfer = new Stack<char>();
    foreach (var instruction in instructions)
    {
        for (int i = 0; i < instruction.count; i++)
        {
            transfer.Push(stacks[instruction.from - 1].Pop());
        }
        while (transfer.Count > 0)
        {
            stacks[instruction.to - 1].Push(transfer.Pop());
        }
    }

    var part2 = new string(stacks.Select(item => item.First()).ToArray());
    Console.WriteLine($"Part 2: {part2}");
}


static void print(IEnumerable source, int indent = 0)
{
    var padding = new string(' ', indent);

    if (source is string)
    {
        Console.WriteLine(padding + source.ToString().PadLeft(indent));
        return;
    }

    var isFinalLevel = source.Cast<object>().First() is not IEnumerable;
    if (isFinalLevel)
    {
        Console.WriteLine($"{padding}[ {String.Join(", ", source.Cast<object>().ToArray())} ]");
        return;
    }

    foreach (var item in source)
    {
        var next = item is IEnumerable enumerable ? enumerable : item.ToString();

        Console.WriteLine(padding + ">");
        print(next, indent + 2);
        Console.WriteLine(padding + "<");
    }
}

static IEnumerable<T> StepSlice<T>(IEnumerable<T> value, int index, int step)
{
    var sequence = value.Skip(index);
    while (sequence.Any())
    {
        yield return sequence.First();
        sequence = sequence.Skip(step + 1);
    }
}

static List<Stack<char>> GetInitialStacks(string[] data)
{
    var stackData = data[0].Split("\r\n");
    return stackData
        .Take(stackData.Length - 1)
        .SelectMany(line => StepSlice(line, 1, 3).Select((item, index) => (index: index, value: item)))
        .Where(item => item.value != ' ')
        .GroupBy(item => item.index)
        .OrderBy(group => group.Key)
        .Select(group => new Stack<char>(group.Select(item => item.value).Reverse()))
        .ToList();
}