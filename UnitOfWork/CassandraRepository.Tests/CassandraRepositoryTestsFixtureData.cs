using Cassandra;
using Fakes;

namespace CassandraRepository.Tests
{
    public class CassandraRepositoryTestsFixtureData
    {
        public CassandraRepositoryTestsFixtureData()
        {
            var cluster = Cluster.Builder().AddContactPoint("127.0.0.1").WithDefaultKeyspace(nameof(CassandraRepositoryTests)).Build();
            cluster.ConnectAndCreateDefaultKeyspaceIfNotExists();
            this.UnitOfWork = new CassandraUnitOfWork.CassandraUnitOfWork(cluster, new CassandraEntityMappings());
        }

        public CassandraUnitOfWork.CassandraUnitOfWork UnitOfWork { get; set; }
    }
}
