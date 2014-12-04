using System.Collections.Generic;
using Microsoft.Practices.Prism.Mvvm.Interfaces;

namespace StrawhatNet.BLEDemo.Services
{
    public class DataRepository : IDataRepository
    {
        private const string UserEnteredData = "UserEnteredData";
        ISessionStateService _sessionStateService;

        public DataRepository(ISessionStateService sessionStateService)
        {
            _sessionStateService = sessionStateService;
        }

        public string GetUserEnteredData()
        {
            return _sessionStateService.SessionState.ContainsKey(UserEnteredData)
                ? _sessionStateService.SessionState[UserEnteredData] as string
                : string.Empty;
        }

        public void SetUserEnteredData(string data)
        {
            _sessionStateService.SessionState[UserEnteredData] = data;
        }
    }
}
