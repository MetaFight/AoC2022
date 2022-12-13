bool isInTestMode = false;

var input =
    isInTestMode
        ? "input-testing.txt"
        : "input.txt";

var data =
    File.ReadAllLines(input)
        .Chunk(3)
        .Select(triplet => triplet.Take(2).ToList());
        

// Part 1
{
    int part1 = 0;
    int pairIndex = 1;

    foreach (var pair in data)
    {
        bool isCorrect = Comparer(pair[0], pair[1]) == -1;

        if (isCorrect)
        {
            part1 += pairIndex;
        }

        pairIndex++;
    }

    Console.WriteLine($"Part 1: {part1}");
}
// Part 2
{
    var lines = data.SelectMany(pair => pair).ToList();
    const string divider1 = "[[2]]";
    const string divider2 = "[[6]]";

    lines.Add(divider1);
    lines.Add(divider2);
    lines.Sort(Comparer);

    var index1 = lines.IndexOf(divider1) + 1;
    var index2 = lines.IndexOf(divider2) + 1;


    var part2 = index1 * index2;
    Console.WriteLine($"Part 2: {part2}");
}

static int Comparer(string rawLeft, string rawRight)
{
    var left = new PacketReader(rawLeft);
    var right = new PacketReader(rawRight);

    var isCorrect = false;

    left.Next();
    right.Next();
    while (!left.HasOverrun && !right.HasOverrun)
    {
        if (left.IsListStart && right.IsListStart)
        {
            left.Next();
            right.Next();
            continue;
        }

        if (left.IsListEnd && right.IsListEnd)
        {
            left.Next();
            right.Next();
            continue;
        }

        if (left.IsNumber && right.IsNumber)
        {
            if (left.Value < right.Value)
            {
                // correct
                isCorrect = true;
                break;
            }
            if (left.Value > right.Value)
            {
                // incorrect
                break;
            }

            // inconclusive
            left.Next();
            right.Next();
            continue;
        }

        if (left.IsNumber && right.IsListStart)
        {
            left.InsertListEnd();
            right.Next();
            continue;
        }

        if (left.IsListStart && right.IsNumber)
        {
            right.InsertListEnd();
            left.Next();
            continue;
        }

        if (left.IsListEnd)
        {
            // correct
            isCorrect = true;
            break;
        }

        if (right.IsListEnd)
        {
            // incorrect
            break;
        }
    }

    return isCorrect ? -1 : 1;
}


class PacketReader
{
    private List<string> input;
    private int position;

    public PacketReader(string input)
    {
        this.input = input.Replace("[", "[,").Replace("]", ",]").Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
        this.position = -1;
    }

    public void Next()
    {
        if (this.position < this.input.Count -1)
        {
            this.position++;
        }
        else
        {
            throw new InvalidOperationException();
        }
    }

    internal void InsertListEnd() => this.input.Insert(this.position + 1, "]");

    public string Current => this.input[this.position];

    public bool IsNumber => int.TryParse(this.Current.ToString(), out _);
    public int Value => int.Parse(this.Current.ToString());
    public bool IsListStart => this.Current == "[";
    public bool IsListEnd => this.Current == "]";
    public bool HasOverrun => this.position >= this.input.Count;
}