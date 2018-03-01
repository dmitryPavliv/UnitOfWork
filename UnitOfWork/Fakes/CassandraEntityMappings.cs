using Cassandra.Mapping;

namespace Fakes
{
    public class CassandraEntityMappings : Mappings
    {
        public CassandraEntityMappings()
        {
            // Define mappings in the constructor of your class
            // that inherits from Mappings
            this.For<Foo>()
                .TableName("Foos")
                .PartitionKey(u => u.Id)
                .Column(u => u.Id, cm => cm.WithName("id"));
        }
    }
}