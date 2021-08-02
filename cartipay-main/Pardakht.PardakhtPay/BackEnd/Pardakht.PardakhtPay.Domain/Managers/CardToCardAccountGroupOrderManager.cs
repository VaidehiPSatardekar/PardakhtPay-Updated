using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Pardakht.PardakhtPay.Domain.Interfaces;

namespace Pardakht.PardakhtPay.Domain.Managers
{
    public class CardToCardAccountGroupOrderManager: ICardToCardAccountGroupOrderManager
    {
        Dictionary<int, int> _Orders = new Dictionary<int, int>();
        static object _LockObject = new object();
        ILogger _Logger = null;

        public CardToCardAccountGroupOrderManager(ILogger<CardToCardAccountGroupOrderManager> logger)
        {
            _Logger = logger;
        }

        public int GetOrder(int groupId)
        {
            lock (_LockObject)
            {
                int value = 0;

                if (_Orders.ContainsKey(groupId))
                {
                    value = _Orders[groupId];
                }

                value++;
                _Orders[groupId] = value;

                return value;
            }
        }
    }
}
