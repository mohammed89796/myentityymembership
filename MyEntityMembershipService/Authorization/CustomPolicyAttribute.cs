
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class CustomPolicyAttribute : Attribute
{
    public string?[] PolicyName { get; }

    public CustomPolicyAttribute(params string?[] policyName)
    {
        PolicyName = policyName;
    }
}