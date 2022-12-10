using System.Text;

bool isInTestMode = false;

var input =
    isInTestMode
        ? "input-testing.txt"
        : "input.txt";

var data = File.ReadAllLines(input);

var steps =
    data.Select(line => line.Split())
        .Select<string[], Action<CommLink>>(parts => parts switch
        {
            ["noop"] => comms => comms.NoOp(),
            ["addx", var arg] when int.TryParse(arg, out var intArg) => comms => comms.AddX(intArg),
            _ => throw new NotImplementedException()
        })
        .ToList();

var commLink = new CommLink(20, 60, 100, 140, 180, 220);

foreach (var step in steps)
{
    step(commLink);
}

// Part 1
{
    Console.WriteLine($"Part 1: {commLink.SignalStrength}");
}

// Part 2
{
    Console.WriteLine($"Part 2: {commLink.Crt}");
}

class CommLink
{
    private readonly Queue<int> inspectionCycles;

    private int counter;
    private int x;
    private StringBuilder crt;

    public CommLink(params int[] inspectionCycles)
    {
        this.inspectionCycles = new Queue<int>(inspectionCycles.OrderBy(item => item));

        this.counter = 0;
        this.x = 1;
        this.crt = new StringBuilder();
    }

    public int SignalStrength { get; private set; }

    public string Crt => this.crt.ToString();

    public void AddX(int value)
    {
        this.Spin(2);
        this.x += value;
    }

    public void NoOp() => this.Spin(1);

    private void Spin(int count)
    {
        while (count > 0)
        {
            Part2();

            this.counter++;
            count--;

            Part1();
        }

        void Part1()
        {
            if (this.inspectionCycles.Count > 0 && this.counter == this.inspectionCycles.Peek())
            {
                this.SignalStrength += this.x * this.counter;
                this.inspectionCycles.Dequeue();
            }
        }

        void Part2()
        {
            var position = this.counter % 40;
            if (position == 0)
            {
                this.crt.AppendLine();
            }

            var spriteRelativeToBeam = this.x - position;
            var isSpritePixel = -1 <= spriteRelativeToBeam && spriteRelativeToBeam <= 1;

            this.crt.Append(isSpritePixel ? '#' : '.');
        }
    }
}