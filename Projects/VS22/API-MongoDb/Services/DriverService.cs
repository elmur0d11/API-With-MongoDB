using API_MongoDb.Configurations;
using API_MongoDb.Models;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic.FileIO;
using MongoDB.Driver;

namespace API_MongoDb.Services
{
    public class DriverService
    {
        private readonly IMongoCollection<Driver> _driverCollection;

        public DriverService(IOptions<DatabaseSettings> databaseSettings)
        {
            var mongoClient = new MongoClient(databaseSettings.Value.ConnectionString);
            var mongoDb = mongoClient.GetDatabase(databaseSettings.Value.DatabaseName);
            _driverCollection = mongoDb.GetCollection<Driver>(databaseSettings.Value.CollectionName);
        }

        public async Task<List<Driver>> GetAsync()
            => await _driverCollection.Find(_ =>  true).ToListAsync();

        public async Task<Driver> GetAsync(string id)
            => await _driverCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Driver driver)
            => await _driverCollection.InsertOneAsync(driver);

        public async Task UpdateAsync(Driver driver)
            => await _driverCollection.ReplaceOneAsync(x => x.Id == driver.Id, driver);

        public async Task RemoveAsync(string id)
            => _driverCollection.DeleteOneAsync(x => x.Id == id);
    }
}
