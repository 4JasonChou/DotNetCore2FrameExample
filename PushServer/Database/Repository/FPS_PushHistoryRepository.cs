using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;  
using PushServer.Model;
using System.Threading.Tasks;
using PushServer.Database;
using PushServer.Database.DBModels;
using PushServer.Database.Repository.Interface;
using Microsoft.Extensions.Options;

namespace PushServer.Database.Repository {
    public class FPS_PushHistoryRepository : IFPS_PushHistoryRepository {

         private readonly MongoDBContext _context = null;

        public FPS_PushHistoryRepository(IOptions<MongoDBSettings> settings)
        {
            _context = new MongoDBContext(settings);
        }

        public async Task<IEnumerable<FPS_PushHistory>> GetAllFPS_PushHistorys(){
            try
            {
                return await _context.FPS_PushHistorys
                        .Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }
        public async Task<FPS_PushHistory> GetFPS_PushHistory(string id) {
            var filter = Builders<FPS_PushHistory>.Filter.Eq("MessageSN", id);
            try
            {
                return await _context.FPS_PushHistorys
                                .Find(filter)
                                .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }
        public async Task<bool> AddFPS_PushHistory(FPS_PushHistory item){
            try
            {
                await _context.FPS_PushHistorys.InsertOneAsync(item);
                return true;
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }
        public async Task<bool> RemoveFPS_PushHistory(string id){
            try
            {
                DeleteResult actionResult = await _context.FPS_PushHistorys.DeleteOneAsync(
                        Builders<FPS_PushHistory>.Filter.Eq("Id", id));

                return actionResult.IsAcknowledged 
                    && actionResult.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        public async Task<bool> UpdateFPS_PushHistory(string id, FPS_PushHistory body){
            var filter = Builders<FPS_PushHistory>.Filter.Eq(s => s.MessageSN, id);
            var update = Builders<FPS_PushHistory>.Update
                            .Set(s => s , body);
            try
            {
                UpdateResult actionResult
                    = await _context.FPS_PushHistorys.UpdateOneAsync(filter, update);

                return actionResult.IsAcknowledged
                    && actionResult.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

    }
}