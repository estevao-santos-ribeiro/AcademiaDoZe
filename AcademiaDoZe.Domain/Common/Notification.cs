// Estevão Santos Ribeiro

namespace AcademiaDoZe.Domain.Common
{
    public sealed class Notification
    {
        public Notification(string propertyName, string message)
        {
            PropertyName = propertyName;
            Message = message;
        }

        public string PropertyName { get; }

        public string Message { get; }

        public static Notification Create(string propertyName, string message)
            => new Notification(propertyName, message);

        public override string ToString()
            => $"{PropertyName}: {Message}";
    }
}
