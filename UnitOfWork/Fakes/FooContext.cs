// pluskal

using System.Data.Common;
using System.Data.Entity;

namespace Fakes
{
    public class FooContext : DbContext
    {
        public FooContext(DbConnection connection)
            : base(connection, true)
        {
        }

        public IDbSet<Foo> Foos { get; set; }
    }
}