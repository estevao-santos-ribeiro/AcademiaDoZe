// Estevão Santos Ribeiro

namespace AcademiaDoZe.Domain.Entities
{
    public abstract class Entity
    {
        public Guid Id { get; protected init; }

        protected Entity()
        {
            Id = Guid.NewGuid();
        }

        protected Entity(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("ID não pode ser Guid.Empty.", nameof(id));

            Id = id;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Entity other)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return Id == other.Id;
        }

        public override int GetHashCode()
            => Id.GetHashCode();

        public static bool operator ==(Entity? left, Entity? right)
        {
            if (left is null && right is null)
                return true;

            if (left is null || right is null)
                return false;

            return left.Equals(right);
        }

        public static bool operator !=(Entity? left, Entity? right)
            => !(left == right);
    }
}
