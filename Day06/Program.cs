bool isInTestMode = false;

var input =
    isInTestMode
        ? "input-testing.txt"
        : "input.txt";

var data = File.ReadAllText(input);
const int PacketStartMarkerSize = 4;

// Part 1
{
    var part1 = IndexOfMarker(data, PacketStartMarkerSize);
    Console.WriteLine($"Part 1: {part1}");
}

// Part 2
{
    const int MessageStartMarkerSize = 14;

    var markerIndex = IndexOfMarker(data, PacketStartMarkerSize);
    var part2 = markerIndex + IndexOfMarker(data.Substring(markerIndex), MessageStartMarkerSize);
    Console.WriteLine($"Part 2: {part2}");
}

static int IndexOfMarker(string data, int windowSize)
{
    int windowStart;
    for (windowStart = 0; windowStart < data.Length - windowSize; windowStart++)
    {
        var isMarker =
            data.Substring(windowStart, windowSize)
                .Distinct()
                .Count() == windowSize;
        if (isMarker)
        {
            return windowStart + windowSize;
        }
    }

    return -1;
}
