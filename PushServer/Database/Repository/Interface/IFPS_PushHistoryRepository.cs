using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;  
using PushServer.Database;
using PushServer.Database.DBModels;
using PushServer.Database.Repository.Interface;
using System.Threading.Tasks;

namespace PushServer.Database.Repository.Interface {
    public interface IFPS_PushHistoryRepository
    {
        // Craete 
        Task<bool> AddFPS_PushHistory(FPS_PushHistory item);
        // Read All
        Task<IEnumerable<FPS_PushHistory>> GetAllFPS_PushHistorys();
        // Read by Id
        Task<FPS_PushHistory> GetFPS_PushHistory(string id);
        // Update by Id
        Task<bool> UpdateFPS_PushHistory(string id, FPS_PushHistory body);
        // Delete by Id
        Task<bool> RemoveFPS_PushHistory(string id);
        
        
    }
}