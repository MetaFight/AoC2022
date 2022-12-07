bool isInTestMode = true;

var input =
    isInTestMode
        ? "input-testing.txt"
        : "input.txt";

var data =
    File.ReadAllLines(input);

var directories = new List<Directory>();

// Part 1
{
    var pwd = new Stack<Directory>();
    for(int i = 0; i < data.Length; i++)
    {
        var line = data[i];
        var parts = line.Split();
        
        if (parts[0] == "$" && parts[1] == "cd")
        {
            var target = parts[2];

            if (target == "..")
            {
                pwd.Pop();
            }
            else
            {
                var targetDir = new Directory(target);
                directories.Add(targetDir);
                if (pwd.Count > 0)
                {
                    pwd.Peek().Subdirectories.Add(targetDir);
                }
                pwd.Push(targetDir);
            }
            continue;
        }

        if (parts[0] == "$" && parts[1] == "ls")
        {
            while (i + 1 < data.Length)
            {
                if (data[i + 1].StartsWith("$"))
                {
                    break;
                }

                line = data[i + 1];
                parts = line.Split();

                if (int.TryParse(parts[0], out var fileSize))
                {
                    pwd.Peek().Files.Add((fileSize, parts[1]));
                }

                i++;
            }
            continue;
        }
    }

    var part1 = directories.Where(item => item.TotalSize <= 100_000).Sum(item => item.TotalSize);
    Console.WriteLine($"Part 1: {part1}");
}

// Part 2
{
    const int totalStorage = 70_000_000;
    const int requiredStorage = 30_000_000;

    var usedStorage = directories[0].TotalSize;
    var remainingStorage = totalStorage - usedStorage;
    var gap = requiredStorage - remainingStorage;

    var part2 = directories.OrderBy(item => item.TotalSize).First(item => item.TotalSize >= gap).TotalSize;
    Console.WriteLine($"Part 2: {part2}");
}

public class Directory
{
    public Directory(string name)
    {
        this.Name = name;
        this.Files = new List<(int size, string name)>();
        this.Subdirectories = new List<Directory>();
    }
    public string Name { get; set; }
    public List<(int size, string name)> Files { get; set; }
    public List<Directory> Subdirectories { get; set; }

    public long TotalSize => this.Files.Sum(file => file.size) + this.Subdirectories.Sum(dir => dir.TotalSize);
}