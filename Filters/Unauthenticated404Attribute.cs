using Microsoft.AspNetCore.Mvc;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class Unauthenticated404Attribute : TypeFilterAttribute
{
    public Unauthenticated404Attribute()
        : base(typeof(Unauthenticated404Filter))
    {
    }
}