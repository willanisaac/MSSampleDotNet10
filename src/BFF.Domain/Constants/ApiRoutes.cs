namespace BFF.Domain.Constants;

public static class ApiRoutes
{
    public const string BaseRoute = "api";
    
    public static class Users
    {
        public const string Base = $"{BaseRoute}/users";
        public const string GetById = "{id}";
        public const string Create = "";
        public const string Update = "{id}";
        public const string Delete = "{id}";
        public const string GetAll = "";
    }
    
    public static class Orders
    {
        public const string Base = $"{BaseRoute}/orders";
        public const string GetById = "{id}";
        public const string Create = "";
        public const string Update = "{id}";
        public const string Cancel = "{id}/cancel";
        public const string GetByUser = "user/{userId}";
    }
    
    public static class Payments
    {
        public const string Base = $"{BaseRoute}/payments";
        public const string GetById = "{id}";
        public const string Process = "";
        public const string Refund = "{id}/refund";
        public const string GetByOrder = "order/{orderId}";
    }
}
