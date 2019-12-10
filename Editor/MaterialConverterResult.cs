using UnityEngine.UIElements;

namespace JollySamurai.UnrealEngine4.Import
{
    public class MaterialConverterResult
    {
        public string Name { get; }

        public MaterialConverterResult(string name)
        {
            Name = name;
        }
    }

    public interface IMaterialConverterFeedback
    {
        string Message { get; }
    }

    public class MaterialConverterWarning : IMaterialConverterFeedback
    {
        public string Message { get; }

        public MaterialConverterWarning(string message)
        {
            Message = message;
        }
    }

    public class MaterialConverterError : IMaterialConverterFeedback
    {
        public string Message { get; }

        public MaterialConverterError(string message)
        {
            Message = message;
        }
    }
}
