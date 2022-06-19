using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StartUpScript
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void OnStartup()
    {
        // This step is done before the Start() of other MonoBehavior that rely on the server.
        string client_id = "geopoly";
        string client_secret = "UGs6tFYGnE9wUTdHlUWXR2195vbIP55wx5mVo225c9Xg4TFZ";

        string doors_url = "https://doorsauth.online";
        string geopoly_url = "http://localhost:5000";

        // temporal mock:
        //geopoly_url = doors_url;

        // Initialize singleton authentication & game services:
        (new DoorsAuthService(doors_url, client_id, client_secret)).SetSubServerHandler(new DoorsAuthSubService<DoorsUser>(geopoly_url));
    }
}
