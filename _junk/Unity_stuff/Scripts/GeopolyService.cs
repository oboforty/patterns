using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using UnityEngine;

public class GeoUser : DoorsUser
{
    public string wid;
    public string iso;

    public GeoUser(DoorsUser user, string wid = null, string iso = null)
    {
        // inherit:
        accessToken = user.accessToken;
        username = user.username;
        uid = user.uid;

        // game object:
        this.wid = wid;
        this.iso = iso;
    }
}

