// Estevão Santos Ribeiro

namespace AcademiaDoZe.Domain.Common
{
    public sealed class Result<T>
    {
        private Result(bool isSuccess, T? value, IReadOnlyList<Notification>? notifications)
        {
            IsSuccess = isSuccess;
            Value = value;
            Notifications = notifications ?? new List<Notification>();
        }

        public bool IsSuccess { get; }

        public T? Value { get; }

        public IReadOnlyList<Notification> Notifications { get; }

        public static Result<T> Success(T value)
            => new Result<T>(true, value, null);

        public static Result<T> Failure(Notification notification)
            => new Result<T>(false, default, new[] { notification });

        public static Result<T> Failure(IEnumerable<Notification> notifications)
            => new Result<T>(false, default, notifications.ToList());

        public void Match(Action<T> onSuccess, Action<IReadOnlyList<Notification>> onFailure)
        {
            if (IsSuccess && Value != null)
            {
                onSuccess(Value);
            }
            else
            {
                onFailure(Notifications);
            }
        }

        public Result<TOut> Map<TOut>(Func<T, TOut> mapper)
        {
            if (IsSuccess && Value != null)
            {
                return Result<TOut>.Success(mapper(Value));
            }

            return Result<TOut>.Failure(Notifications);
        }
    }
}
