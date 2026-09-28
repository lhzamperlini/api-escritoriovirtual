namespace EscritorioVirtual.API.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
public class RequireWorkspaceAttribute : Attribute
{
}
