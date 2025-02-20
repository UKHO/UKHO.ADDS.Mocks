#pragma warning disable CA1822

namespace ReferenceTest
{
    public class Test
    {
        public string HelloWorld(string name = null) => $"Hello, {name ?? "anonymous"}. Time is: {DateTime.Now:hh:mm:ss t}";

        public int Add(int num1, int num2) => num1 + num2;
    }
}
