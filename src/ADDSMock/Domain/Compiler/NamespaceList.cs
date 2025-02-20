using System.Text;
using Microsoft.CodeAnalysis;

namespace ADDSMock.Domain.Compiler
{
    /// <summary>
    /// HashSet of namespaces
    /// </summary>
    public class NamespaceList : HashSet<string>
    {
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            var enumerator = this.GetEnumerator();
            foreach (string ns in this)
            {
                sb.AppendLine($"using {ns};");
            }

            return sb.ToString();
        }
    }

    /// <summary>
    /// HashSet of References
    /// </summary>
    public class ReferenceList : HashSet<PortableExecutableReference>
    {

    }
}
