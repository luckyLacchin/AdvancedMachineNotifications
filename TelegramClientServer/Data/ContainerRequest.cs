namespace TelegramClientServer.Data
{
    public static class ContainerRequest
    {
        public static List<string> subscribeList = new List<string>();
        public static List<string> unsubscribeList = new List<string>();
        public static List<string> getMessagesMid = new List<string>();
        public static void addSubRequest(string chatId)
        {
            subscribeList.Add(chatId);
        }

        public static void deleteSubRequest(string chatId)
        {
            subscribeList.Remove(chatId);
        }

        public static bool containsSubRequest (string chatId)
        {
            return subscribeList.Contains(chatId);
        }

        public static void addUnsubRequest(string chatId)
        {
            unsubscribeList.Add(chatId);
        }

        public static void deleteUnsubRequest(string chatId)
        {
            unsubscribeList.Remove(chatId);
        }

        public static bool containsUnsubRequest(string chatId)
        {
            return unsubscribeList.Contains(chatId);
        }
    }
}
