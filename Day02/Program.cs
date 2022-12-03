using System.Collections;

bool isInTestMode = false;

var input =
    isInTestMode
        ? "test-in.txt"
        : "input.txt";

var data =
    File.ReadAllLines(input)
        .Select(row => row.Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        .ToList();

int ParseOpp(string value) => value switch { "A" => 1, "B" => 2, "C" => 3, _ => throw new ArgumentException() };
int GetShapeBonus(int shape) => shape;


// Part 1
{
    int ParseYou(string value) => value switch { "X" => 1, "Y" => 2, "Z" => 3, _ => throw new ArgumentException() };

    var part1Input =
        data.Select(row => (opp: ParseOpp(row[0]), you: ParseYou(row[1])))
            .ToList();

    //print(data);

    int GetOutcomeBonus((int opp, int you) input)
        => input switch
        {
            // losses
            { opp: 1, you: 3 } or { opp: 2, you: 1 } or { opp: 3, you: 2 } => 0,
            // draw
            { opp: 1, you: 1 } or { opp: 2, you: 2 } or { opp: 3, you: 3 } => 3,
            // victories
            { opp: 3, you: 1 } or { opp: 1, you: 2 } or { opp: 2, you: 3 } => 6,
            _ => throw new ArgumentException()
        };

    int GetRoundScore((int opp, int you) input) => GetShapeBonus(input.you) + GetOutcomeBonus(input);


    var part1 = part1Input.Sum(GetRoundScore);
    Console.WriteLine($"Part 1: {part1}");
}


// Part 2
{
    int ParseOutcome(string value) => value switch { "X" => 1, "Y" => 2, "Z" => 3, _ => throw new ArgumentException() };

    var part2Input =
        data.Select(row => (opp: ParseOpp(row[0]), outcome: ParseOutcome(row[1])))
            .ToList();

    int GetWinningShape(int shape) => 1 + (shape % 3);
    int GetDrawingShape(int shape) => shape;
    int GetLosingShape(int shape) => 1 + ((shape + 1) % 3);

    int GetTargetShape(int shape, int outcome) => outcome switch { 1 => GetLosingShape(shape), 2 => GetDrawingShape(shape), 3 => GetWinningShape(shape), _ => throw new ArgumentException() };
    int GetOutcomeBonus(int outcome) => (outcome - 1) * 3;

    int GetRoundScore((int opp, int outcome) input) => GetShapeBonus(GetTargetShape(input.opp, input.outcome)) + GetOutcomeBonus(input.outcome);

    var part2 = part2Input.Sum(GetRoundScore);
    Console.WriteLine($"Part 2: {part2}");
}

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
