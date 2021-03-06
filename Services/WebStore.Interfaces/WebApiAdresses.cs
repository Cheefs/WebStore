
namespace WebStore.Interfaces;

public static class WebApiAdresses
{
    public static class V1
    {
        private const string VERSION = "1";
        public const string Employees = $"api/v{VERSION}/employees";
        public const string Orders = $"api/v{VERSION}/orders";
        public const string Products = $"api/v{VERSION}/products";
        public const string Values = $"api/v{VERSION}/values";

        public const string Console = $"api/v{VERSION}/console";

        public static class Identity
        {
            public const string Users = $"api/v{VERSION}/identity/users";
            public const string Roles = $"api/v{VERSION}/identity/roles";
        }
    }
}
