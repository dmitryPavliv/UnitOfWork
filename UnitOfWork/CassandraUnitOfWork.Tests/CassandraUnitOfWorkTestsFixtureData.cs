using Cassandra;
using Fakes;

namespace CassandraUnitOfWork.Tests
{
    public class CassandraUnitOfWorkTestsFixtureData
    {
        public CassandraUnitOfWorkTestsFixtureData()
        {
            var cluster = Cluster.Builder().AddContactPoint("127.0.0.1").WithDefaultKeyspace(nameof(CassandraUnitOfWorkTests)).Build();
            cluster.ConnectAndCreateDefaultKeyspaceIfNotExists();
            this.UnitOfWork = new CassandraUnitOfWork(cluster, new CassandraEntityMappings());
        }

        public CassandraUnitOfWork UnitOfWork { get; set; }
    }
}
