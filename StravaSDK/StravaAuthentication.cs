//using DataObjects.Activities;
//using Newtonsoft.Json;
//using RestSharp;
//using System.Data;
//using System.Diagnostics;
//using System.Globalization;

//namespace StravaSDK
//{
//    public class StravaAuthentication
//    {
//        static string clientId = "71479";
//        static string clientSecret = "2d40919c62e6186b989cd893e5af5970b19bf4b0";
//        static string accessToken;
//        private static RestClient client;

//        public static async Task StravaAuthenticationAsync(List<BaseActivity> activities)
//        {
//            // Strava API Credentials
//            string redirectUri = "http://localhost";

//            // OAuth Authentication Flow
//            Console.WriteLine("Open the following URL in a browser:");
//            string authUrl = $"https://www.strava.com/oauth/authorize?client_id={clientId}&response_type=code&redirect_uri={redirectUri}&scope=activity:write";
//            Console.WriteLine(authUrl);

//            Console.WriteLine("\nEnter the Authorization Code:");
//            string authCode = Console.ReadLine();

//            // Exchange authorization code for access token
//            var httpClient = new HttpClient();
//            var tokenRequest = new HttpRequestMessage(HttpMethod.Post, "https://www.strava.com/oauth/token")
//            {
//                Content = new FormUrlEncodedContent(new[]
//                {
//                new KeyValuePair<string, string>("client_id", clientId),
//                new KeyValuePair<string, string>("client_secret", clientSecret),
//                new KeyValuePair<string, string>("code", authCode),
//                new KeyValuePair<string, string>("grant_type", "authorization_code")
//            })
//            };

//            //var apiInstance = new ActivitiesApi();
//            //var before = 56;  // Integer | An epoch timestamp to use for filtering activities that have taken place before a certain time. (optional) 
//            //var after = 56;  // Integer | An epoch timestamp to use for filtering activities that have taken place after a certain time. (optional) 
//            //var page = 56;  // Integer | Page number. Defaults to 1. (optional) 
//            //var perPage = 56;  // Integer | Number of items per page. Defaults to 30. (optional)  (default to 30)

//            //try
//            //{
//            //    // List Athlete Activities
//            //    array[SummaryActivity] result = apiInstance.getLoggedInAthleteActivities(before, after, page, perPage);
//            //    Debug.WriteLine(result);
//            //}
//            //catch (Exception e)
//            //{
//            //    Debug.Print("Exception when calling ActivitiesApi.getLoggedInAthleteActivities: " + e.Message);
//            //}


//            var tokenResponse = await httpClient.SendAsync(tokenRequest);
//            var tokenContent = await tokenResponse.Content.ReadAsStringAsync();
//            dynamic tokenData = JsonConvert.DeserializeObject(tokenContent);
//            accessToken = tokenData?.access_token;

//            Console.WriteLine("Access token obtained!");

//            //// Upload an activity file to Strava
//            //string filePath = "path_to_your_activity_file.gpx"; // Replace with the actual GPX/TCX/FIT file path


//            foreach (var activity in activities)
//            {
//                await CreateActivityAsync(activity);
//            }
//            //request.AddFile("file", filePath);
//            //request.AddParameter("data_type", "gpx"); // File type
//            //request.AddParameter("activity_type", "ride"); // Activity type

//            //var response = await client.ExecutePostAsync(request);

//            //if (response.IsSuccessful)
//            //{
//            //    Console.WriteLine("Activity uploaded successfully!");
//            //    Console.WriteLine(response.Content);
//            //}
//            //else
//            //{
//            //    Console.WriteLine("Failed to upload activity.");
//            //    Console.WriteLine(response.Content);
//            //}
//        }

//        private static async Task CreateActivityAsync(BaseActivity activity)
//        {
//            client = new RestClient("https://www.strava.com/api/v3/activities");
//            var request = new RestRequest();
//            request.AddHeader("Authorization", $"Bearer {accessToken}");

//            switch (activity.ActivityType)
//            {
//                case ActivityType.Weight:
//                    FillWeightInformation(request, (Weights)activity);
//                    break;
//            }


//            var response = await client.ExecutePostAsync(request);

//            if (response.IsSuccessful)
//            {
//                Console.WriteLine("Activity uploaded successfully!");
//                Console.WriteLine(response.Content);
//            }
//            else
//            {
//                Console.WriteLine("Failed to upload activity.");
//                Console.WriteLine(response.Content);
//            }

//        }

//        private static void FillWeightInformation(RestRequest request, Weights weights)
//        {

//            request.AddParameter(StravaParamName.name.ToString(), weights.Name);
//            request.AddParameter(StravaParamName.sport_type.ToString(), SportType.WeightTraining.ToString());
//            request.AddParameter(StravaParamName.type.ToString(), StravaActivityType.WeightTraining.ToString());
//            request.AddParameter(StravaParamName.start_date_local.ToString(), weights.StartDate.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture));
//            request.AddParameter(StravaParamName.elapsed_time.ToString(), $"{weights.Duration.TotalSeconds}");
//            request.AddParameter(StravaParamName.description.ToString(), weights.Description);




//        }
//    }
//}
