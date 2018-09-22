namespace LinkedInClient.Services
{
    public static class WebApiClientFactory
    {
        public static IWebApiClientService CreateWebApiClientService()
        {
            return new WebApiClientService();
        }
    }
}