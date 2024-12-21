using DemoAPIBot.Messanger;
using System.Collections.Generic;

namespace DemoAPIBot.ServiceDispatchers
{
    public static class DispatchersContainer
    {
        public static SynchronizedCollection<ServiceDispatcher> DispatchersList = new SynchronizedCollection<ServiceDispatcher>();
        public static List<string> possibleDispatcher = new List<string>{ "Telegram", "Discord", "Teams" };
        public static void addDispatcher (ServiceDispatcher service)
        {
            DispatchersList.Add(service);
        }

        public static void deleteDispatcher(ServiceDispatcher service)
        {
            DispatchersList.Remove(service);
        }

        public static bool containDispatcher (ServiceDispatcher service)
        {
            foreach(ServiceDispatcher dispatcher in DispatchersList)
            {
                if ((dispatcher.Name == service.Name) && (service.Ip == null))
                {
                    return true;
                }
                if ((dispatcher.Name == service.Name) && (dispatcher.Ip != service.Ip || dispatcher.Port != service.Port))
                {
                    deleteDispatcher(dispatcher);
                    return false;
                }
                else if (dispatcher.Name == service.Name)
                    return true;
            }
            return false;
        }

    }
}
