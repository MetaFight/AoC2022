using System.Numerics;
using static Item.IOperation;

bool isInTestMode = false;

var input =
    isInTestMode
        ? "input-testing.txt"
        : "input.txt";

Part1();
Part2();

void Part1(bool showProgress = false)
{
    var monkeys = GetPart1Monkeys(input);

    for (int round = 1; round <= 20; round++)
    {
        foreach (var monkey in monkeys)
        {
            monkey.Update(monkeys);
        }

        if (showProgress)
        {
            Console.WriteLine($"Round {round:00}");
            for (int i = 0; i < monkeys.Count; i++)
            {
                Console.WriteLine($"Monkey {i}: {monkeys[i]}");
            }
            Console.WriteLine();
        }
    }

    var monkeyBusiness =
        monkeys
            .OrderByDescending(item => item.InspectionCount)
            .Take(2)
            .Aggregate(1, (acc, item) => acc * item.InspectionCount);

    var part1 = monkeyBusiness;
    Console.WriteLine($"Part 1: {part1}");
}

void Part2(bool showProgress = true)
{
    var monkeys = GetPart2Monkeys(input);

    for (int round = 1; round <= 10_000; round++)
    {
        foreach (var monkey in monkeys)
        {
            monkey.Update(monkeys);
        }

        var printRound = showProgress && (round == 1 || round == 20 || round % 1000 == 0);
        if (printRound)
        {
            Console.WriteLine($"Round {round:00}");
            for (int i = 0; i < monkeys.Count; i++)
            {
                Console.WriteLine($"Monkey {i}: {monkeys[i].InspectionCount}");
            }
            Console.WriteLine();
        }
    }

    var monkeyBusiness =
        monkeys
            .OrderByDescending(item => item.InspectionCount)
            .Take(2)
            .Aggregate(1L, (acc, item) => acc * item.InspectionCount);

    var part2 = monkeyBusiness;
    Console.WriteLine($"Part 2: {part2}");
}

static List<Monkey1> GetPart1Monkeys(string input)
{
    return File.ReadAllLines(input)
            .Chunk(7)
            .Select(lines =>
                new Monkey1(
                    startingItems: ParseStartingItems(lines[1]),
                    worryFunc: ParseWorryFunction(lines[2]),
                    testDivisor: int.Parse(lines[3].Split(" by ")[1]),
                    trueMonkeyIndex: int.Parse(lines[4].Split(" monkey ")[1]),
                    falseMonkeyIndex: int.Parse(lines[5].Split(" monkey ")[1])
                )
            )
            .ToList();

    static IEnumerable<int> ParseStartingItems(string line)
    => line.Split(":")[1].Split(",", StringSplitOptions.TrimEntries).Select(item => int.Parse(item));

    static Func<BigInteger, BigInteger> ParseWorryFunction(string line)
        => line.Split(" old ")[1].Split() switch
        {
            ["+", var arg] when int.TryParse(arg, out var intArg) => x => x + intArg,
            //["-", var arg] when int.TryParse(arg, out var intArg) => x => x - intArg,
            ["*", var arg] when int.TryParse(arg, out var intArg) => x => x * intArg,
            //["/", var arg] when int.TryParse(arg, out var intArg) => x => x / intArg,
            ["+", "old"] => x => x + x,
            //["-", "old"] => x => x - x,
            ["*", "old"] => x => x * x,
            //["/", "old"] => x => x / x,
            _ => throw new NotImplementedException()
        };
}

static List<Monkey2> GetPart2Monkeys(string input)
{
    return File.ReadAllLines(input)
            .Chunk(7)
            .Select(lines =>
                new Monkey2(
                    startingItems: ParseStartingItems(lines[1]),
                    worryOperation: ParseWorryOperation(lines[2]),
                    testDivisor: int.Parse(lines[3].Split(" by ")[1]),
                    trueMonkeyIndex: int.Parse(lines[4].Split(" monkey ")[1]),
                    falseMonkeyIndex: int.Parse(lines[5].Split(" monkey ")[1])
                )
            )
            .ToList();

    static IEnumerable<Item> ParseStartingItems(string line)
    => line.Split(":")[1].Split(",", StringSplitOptions.TrimEntries).Select(item => new Item(int.Parse(item)));

    static Item.IOperation ParseWorryOperation(string line)
        => line.Split(" old ")[1].Split() switch
        {
            ["+", var arg] when int.TryParse(arg, out var intArg) => new Item.Add(intArg),
            ["*", var arg] when int.TryParse(arg, out var intArg) => new Item.Mul(intArg),
            ["+", "old"] => new Item.Mul(2),
            ["*", "old"] => new Item.Sqr(),
            _ => throw new NotImplementedException()
        };
}

class Monkey1
{
    private readonly Queue<BigInteger> items;

    private readonly Func<BigInteger, BigInteger> worryFunc;
    private readonly int testDivisor;
    private readonly int trueMonkeyIndex;
    private readonly int falseMonkeyIndex;

    public int InspectionCount { get; private set; }

    public Monkey1(
        IEnumerable<int> startingItems,
        Func<BigInteger, BigInteger> worryFunc,
        int testDivisor,
        int trueMonkeyIndex,
        int falseMonkeyIndex)
    {
        this.items = new Queue<BigInteger>(startingItems.Select(item => new BigInteger(item)));

        this.worryFunc = worryFunc;
        this.testDivisor = testDivisor;
        this.trueMonkeyIndex = trueMonkeyIndex;
        this.falseMonkeyIndex = falseMonkeyIndex;
    }

    public void Update(List<Monkey1> monkeys)
    {
        while (this.items.Count > 0)
        {
            var item = this.items.Dequeue();
            item = this.worryFunc(item) / 3;

            this.InspectionCount++;

            if (item % this.testDivisor == 0)
            {
                monkeys[this.trueMonkeyIndex].items.Enqueue(item);
            }
            else
            {
                monkeys[this.falseMonkeyIndex].items.Enqueue(item);
            }
        }
    }

    public override string ToString() => String.Join(", ", this.items);
}

class Monkey2
{
    private readonly Queue<Item> items;

    private readonly Item.IOperation worryOperation;
    private readonly int testDivisor;
    private readonly int trueMonkeyIndex;
    private readonly int falseMonkeyIndex;

    public long InspectionCount { get; private set; }

    public Monkey2(
        IEnumerable<Item> startingItems,
        Item.IOperation worryOperation,
        int testDivisor,
        int trueMonkeyIndex,
        int falseMonkeyIndex)
    {
        this.items = new Queue<Item>(startingItems);

        this.worryOperation = worryOperation;
        this.testDivisor = testDivisor;
        this.trueMonkeyIndex = trueMonkeyIndex;
        this.falseMonkeyIndex = falseMonkeyIndex;
    }

    public void Update(List<Monkey2> monkeys)
    {
        while (this.items.Count > 0)
        {
            var item = this.items.Dequeue();
            item.ChainOperation(this.worryOperation);

            this.InspectionCount++;

            if (item.IsDivisibleBy(this.testDivisor))
            {
                monkeys[this.trueMonkeyIndex].items.Enqueue(item);
            }
            else
            {
                monkeys[this.falseMonkeyIndex].items.Enqueue(item);
            }
        }
    }

    public override string ToString() => String.Join(", ", this.items);
}

class Item
{
    private readonly List<IOperation> operations;

    public Item(int initialWorryLevel)
    {
        this.operations = new List<IOperation>() { new Item.Add(initialWorryLevel) };
    }

    public bool IsDivisibleBy(int divisor)
    {
        long remainder = 0;
        foreach (var op in this.operations)
        {
            if (op is Add addition)
            {
                remainder += addition.Operand;
                remainder %= divisor;
                continue;
            }

            if (op is Mul multiplication)
            {
                remainder *= multiplication.Operand;
                remainder %= divisor;
                continue;
            }

            if (op is Sqr square)
            {
                remainder *= remainder;
                remainder %= divisor;
                continue;
            }
        }

        return remainder == 0;
    }

    public void ChainOperation(IOperation operation)
    {
        var indexOfLast = this.operations.Count - 1;
        var last = this.operations[indexOfLast];

        if (last is Add sumLeft && operation is Add sumRight)
        {
            this.operations.RemoveAt(indexOfLast);
            this.operations.Add(sumLeft.Merge(sumRight));
            return;
        }

        if (last is Mul productLeft && operation is Mul productRight)
        {
            this.operations.RemoveAt(indexOfLast);
            this.operations.Add(productLeft.Merge(productRight));
            return;
        }

        this.operations.Add(operation);
    }

    public interface IOperation
    {
        //long Apply(long old);

        public interface IUnary : IOperation { }
        public interface IBinary : IOperation { public long Operand { get; } }
    }

    public record Add(long Operand) : IBinary
    {
        //public long Apply(long old) => old + this.Operand;

        internal Add Merge(Add y) => new Add(this.Operand + y.Operand);
    }

    public record Mul(long Operand) : IBinary
    {
        //public long Apply(long old) => old * this.Operand;

        internal Mul Merge(Mul productRight) => new Mul(this.Operand * productRight.Operand);
    }

    public record Sqr : IUnary
    {
        //public long Apply(long old) => old * old;
    }
}