using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using inventory_v2.Data;
using inventory_v2.Model;
using inventory_v2.Response;

namespace inventory_v2.Services
{
    public class UserLogService : IUserLogService
    {
        private readonly DataContext _context;

        public UserLogService(DataContext context)
        {
            _context = context;
        }

        public async Task<ResponseMessage<List<UserLog>>> GetUserLogList()
        {
            ResponseMessage<List<UserLog>> response = new ResponseMessage<List<UserLog>>();
            response.Data = new List<UserLog>();

            try
            {
                var userLogs = await Task.Run(() => _context.UserLog.ToList());
                if (userLogs != null && userLogs.Count > 0)
                {
                    response.Data = userLogs;
                    response.Count = userLogs.Count;
                }
                else
                {
                    response.Success = false;
                    response.Message = "No user logs found.";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ResponseMessage<UserLog>> AddUserLog(UserLog userLog)
        {
            ResponseMessage<UserLog> response = new ResponseMessage<UserLog>();
            try
            {
                userLog.Id = Guid.NewGuid().ToString();
                userLog.LogCreated = Convert.ToInt32(DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString());
                await _context.UserLog.AddAsync(userLog);
                int result = await _context.SaveChangesAsync();

                if (result <= 0)
                {
                    response.Success = false;
                    response.Message = "Failed to add user log.";
                    return response;
                }

                response.Data = userLog;
                response.Message = "User log added successfully.";
                response.Count = 1;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }
    }
    public interface IUserLogService
    {
        Task<ResponseMessage<List<UserLog>>> GetUserLogList();
        Task<ResponseMessage<UserLog>> AddUserLog(UserLog userLog);
    }
}