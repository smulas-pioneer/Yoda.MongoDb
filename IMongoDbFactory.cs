using MongoDB.Driver;

namespace Yoda.MongoDb
{
    public interface IMongoDbFactory<out TDatabase> where TDatabase : MongoDatabase
    {
        //
        // Summary:
        //     /// Creates a new instance of a derived database. ///
        //
        // Parameters:
        //   options:
        //     Information about the environment an application is running in.
        //
        // Returns:
        //     An instance of TDatabase.
        TDatabase Create(MongoDbFactoryOptions options);
    }
}