using System;

namespace SourceGeneratorConsumer
{
    public class MyClass
    {
	    public void DoSomething()
	    {
            var validatable = new ClassWithValidation.ClassWithValidation();
            validatable.ValidateAll();
	    }
    }
}

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class RequiresValidationAttribute : Attribute
{
	public RequiresValidationAttribute() { }
}

[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class MaxStringLengthAttribute : Attribute
{
    public int MaxLength { get; }

    public MaxStringLengthAttribute(int maxLength)
    {
        MaxLength = maxLength;
    }
}

namespace ClassWithValidation
{
	[RequiresValidation]
	public partial class ClassWithValidation
	{
        [MaxStringLength(50)]
        public string Name { get; set; }
    }
}