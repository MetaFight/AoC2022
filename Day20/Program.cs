bool isInTestMode = true;

var input =
    isInTestMode
        ? "input-testing.txt"
        : "input.txt";

var data =
    File.ReadAllLines(input);

// Part 1
{
    var part1 = 0;
    Console.WriteLine($"Part 1: {part1}");
}

// Part 2
{
    var part2 = 0;
    Console.WriteLine($"Part 2: {part2}");
}
