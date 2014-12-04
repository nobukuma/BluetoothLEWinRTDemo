using System.Collections.Generic;

namespace StrawhatNet.BLEDemo.Services
{
    public interface IDataRepository
    {
        string GetUserEnteredData();
        void SetUserEnteredData(string data);
    }
}
