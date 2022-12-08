bool isInTestMode = false;

var input =
    isInTestMode
        ? "input-testing.txt"
        : "input.txt";

var data =
    File.ReadAllLines(input);

var width = data[0].Length;
var depth = data.Length;

var heightMap = new int[width, depth];

for (int y = 0; y < depth; y++)
{
    var line = data[y];
    for (int x = 0; x < width; x++)
    {
        heightMap[x, y] = int.Parse(line.Substring(x, 1));
    }
}

IEnumerable<(int x, int y)> SliceLeftToRight(int y, int startX = 0) => Enumerable.Range(startX, width - startX).Select(x => (x, y));
IEnumerable<(int x, int y)> SliceRightToLeft(int y, int? startX = null) => Enumerable.Range(0, (startX + 1) ?? width).Select(x => (x, y)).Reverse();
IEnumerable<(int x, int y)> SliceTopToBottom(int x, int startY = 0) => Enumerable.Range(startY, depth - startY).Select(y => (x, y));
IEnumerable<(int x, int y)> SliceBottomToTop(int x, int? startY = null) => Enumerable.Range(0, (startY + 1) ?? depth).Select(y => (x, y)).Reverse();
IEnumerable<(int x, int y)> Each() => Enumerable.Range(0, depth).SelectMany(y => Enumerable.Range(0, width).Select(x => (x, y)));

// Part 1
{
    var visibilityMap = new bool[width, depth];

    for (int x = 0; x < width; x++)
    {
        MarkVisibleTrees(heightMap, visibilityMap, SliceTopToBottom(x));
        MarkVisibleTrees(heightMap, visibilityMap, SliceBottomToTop(x));
    }
    for (int y = 0; y < depth; y++)
    {
        MarkVisibleTrees(heightMap, visibilityMap, SliceLeftToRight(y));
        MarkVisibleTrees(heightMap, visibilityMap, SliceRightToLeft(y));
    }

    //DebugPrint(width, depth, heightMap, visibilityMap);

    var part1 = Each().Count(pos => visibilityMap[pos.x, pos.y]);
    Console.WriteLine($"Part 1: {part1}");

    static void MarkVisibleTrees(int[,] heightMap, bool[,] visibilityMap, IEnumerable<(int x, int y)> slice)
    {
        var maxHeight = -1;
        foreach (var (x, y) in slice)
        {
            var height = heightMap[x, y];
            visibilityMap[x, y] |= height > maxHeight;

            maxHeight = Math.Max(height, maxHeight);
            if (maxHeight == 9)
            {
                break;
            }
        }
    }

    void DebugPrint(int width, int depth, int[,] heightMap, bool[,] visibilityMap)
    {
        for (int y = 0; y < depth; y++)
        {
            Console.WriteLine(String.Join(", ", SliceLeftToRight(y).Select(pos => heightMap[pos.x, pos.y])));
        }
        Console.WriteLine();

        for (int y = 0; y < depth; y++)
        {
            Console.WriteLine(String.Join(", ", SliceLeftToRight(y).Select(pos => visibilityMap[pos.x, pos.y] ? 1 : 0)));
        }
        Console.WriteLine();
    }
}

// Part 2
{
    var highScore = 0;

    for (int y = 1; y < depth - 1; y++)
    {
        for (int x = 1; x < width - 1; x++)
        {
            var up = GetSliceScore(heightMap, SliceBottomToTop(x, y));
            var down = GetSliceScore(heightMap, SliceTopToBottom(x, y));
            var left = GetSliceScore(heightMap, SliceRightToLeft(y, x));
            var right = GetSliceScore(heightMap, SliceLeftToRight(y, x));

            var score = up * down * left * right;
            highScore = Math.Max(highScore, score);
        }
    }

    var part2 = highScore;
    Console.WriteLine($"Part 2: {part2}");

    static int GetSliceScore(int[,] heightMap, IEnumerable<(int x, int y)> slice)
    {
        var score = 0;
        var currentHeight = slice.Select(pos => heightMap[pos.x, pos.y]).Take(1).Single();
        foreach (var pos in slice.Skip(1))
        {
            var (x, y) = pos;
            var height = heightMap[x, y];
            score++;
            
            if (height >= currentHeight)
            {
                break;
            }
        }

        return score;
    }
}
