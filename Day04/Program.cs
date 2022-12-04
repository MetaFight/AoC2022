bool isInTestMode = false;

var input =
    isInTestMode
        ? "test-in.txt"
        : "input.txt";

var data =
    File.ReadAllLines(input)
        .Select(line =>
            line.Split(new[] { '-', ',' }, StringSplitOptions.TrimEntries)
                .Select(item => int.Parse(item))
                .Chunk(2)
                .Select(item => (from: item[0], to: item[1]))
                .OrderBy(item => item.from).ThenByDescending(item => item.to).ToList())
        .ToList();


var part1 = data.Where(pair => pair[0].from <= pair[1].from && pair[1].to <= pair[0].to).Count();
Console.WriteLine($"Part 1: {part1}");

var part2 = data.Where(pair => pair[0].to >= pair[1].from).Count();
Console.WriteLine($"Part 2: {part2}");
