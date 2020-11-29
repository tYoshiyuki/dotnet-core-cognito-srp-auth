using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.Extensions.CognitoAuthentication;

namespace DotNetCoreCognitoSrpAuth
{
    /// <summary>
    /// コンソールアプリケーションでCognito認証のAPI Gatewayを呼び出すサンプルです
    /// </summary>
    class Program
    {
        static async Task Main(string[] args)
        {
            // 各種設定情報です
            // Cognito ユーザプール情報
            var poolId = "ap-northeast-1_xxxxx";
            var clientId = "xxxxx";

            // Cognito ユーザ情報
            var userId = "xxxxx";
            var password = "xxxxx";

            // API Gatewayエンドポイント
            var apiUrl = "https://xxxxx.execute-api.ap-northeast-1.amazonaws.com/prod";

            // 認証オブジェクトの初期化
            var provider = new AmazonCognitoIdentityProviderClient(null, RegionEndpoint.APNortheast1);
            var userPool = new CognitoUserPool(poolId, clientId, provider);
            var user = new CognitoUser(userId, clientId, userPool, provider);
            var authRequest = new InitiateSrpAuthRequest
            {
                Password = password
            };

            // トークンの取得
            var authResponse = await user.StartWithSrpAuthAsync(authRequest).ConfigureAwait(false);
            var idToken = authResponse.AuthenticationResult.IdToken;

            // APIの実行
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", idToken);
            var url = new Uri(apiUrl);
            var response = client.GetAsync(url).Result;

            // 取得結果の確認
            Console.WriteLine("-------------------------------------");
            Console.WriteLine(response);
            Console.WriteLine($"実行結果: {response.Content.ReadAsStringAsync().Result}");
            Console.WriteLine("-------------------------------------");
        }
    }
}
