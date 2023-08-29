using System.Text.Json;
using System.Text.Json.Serialization;

namespace VirtuTestTask2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var file1 = File.ReadAllText("object1.json");
            var basePolicy1 = JsonSerializer.Deserialize<BasePolicy>(file1);

            var file2 = File.ReadAllText("object2.json");
            var basePolicy2 = JsonSerializer.Deserialize<BasePolicy>(file2);

            var file3 = File.ReadAllText("object3.json");
            var basePolicy3 = JsonSerializer.Deserialize<BasePolicy>(file3);
        }
    }
}