namespace GiveandTake_API.Constants
{
    public class ApiEndPointConstant
    {
        public const string RootEndPoint = "/api";
        public const string ApiVersion = "/v1";
        public const string ApiEndpoint = RootEndPoint + ApiVersion;

        public class Category
        {
            public const string CategoriesEndPoint = ApiEndpoint + "/categories";
            public const string CategoryEndPoint = CategoriesEndPoint + "/{id}";
        }
    }
}
