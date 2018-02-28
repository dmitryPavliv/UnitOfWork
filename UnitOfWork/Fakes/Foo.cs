// pluskal

using System;
using BaseDataModel;

namespace Fakes
{
    public class Foo : IDataModel
    {
        public String Name { get; set; }
        public Guid Id { get; set; }

        public override Boolean Equals(Object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return this.Equals((Foo) obj);
        }

        public override Int32 GetHashCode()
        {
            unchecked
            {
                return (this.Id.GetHashCode() * 397) ^ (this.Name?.GetHashCode() ?? 0);
            }
        }

        private Boolean Equals(Foo foo)
        {
            return this.Id.Equals(foo.Id) && String.Equals(this.Name, foo.Name);
        }
    }
}