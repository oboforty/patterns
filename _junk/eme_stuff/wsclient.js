// websocket client to communicate with game server

const client = new (function(){
  this.groups = {};
  this.model = {};
  this.subscribed = {};
  this.reoccuring = {};
  this.onconnected_func = [];
  this.ondisconnected_func = [];
  this.onerror_func = [];
  this.ws = null;
  this.log_style = "color: purple";
  this.address = null;
  this.c_msid = 1;
  this.trying = false;
  this.request = function(route, params) {
    if (!this.ws || this.ws.readyState != 1) {
      console.error("WebSocket is not connected");
      return {then:()=>{}};
    }

    if (!params && typeof route !== 'string') {
        var params = route;
    } else {
        if (!params) params = {};
        params.route = route;
    }

    if (!params.route)
        console.error("No route defined for request: ", params);
      // todo: itt
    params.msid = this.c_msid++;
    console.info(`%c<${route} (${params.msid})`, client.log_style, params);

    var rwsString = JSON.stringify(params);


    this.ws.send(rwsString);
    return new DeferredResponse(route, params.msid);
    // try {
    // } catch(e){
    //   console.error(e);
    //   client.reconnect();
    // }
  };

  this.onconnect = function(cb) {
    this.onconnected_func.push(cb);
  }
  this.ondisconnect = function(cb) {
    this.ondisconnected_func.push(cb);
  }
  this.onerror = function(cb) {
    this.onerror_func.push(cb);
  }

  this.onmessage = function(event) {
    if (event.data != null)
      var rws = JSON.parse(event.data);
    else
      var rws = event;

    try {
      var gmarr = rws.route.split(':');
      var group = client.groups[gmarr[0]] || this;
      console.info('%c>'+rws.route+` (${rws.msid||'0'})`, "color:purple", rws);

      try {
          var params = Object.assign({}, rws);
          delete params.route;
      } catch(e) {
          var params = rws;
      }

      if (params.params)
          params = params.params;

      if (rws.msid && client.subscribed[rws.msid]) {
        // events handled by request().then(...)
        client.subscribed[rws.msid].apply(group, [params]);
        delete client.subscribed[rws.msid];
      } else if (client.subscribed[rws.route]) {
        // events handled by request().then(...)
        client.subscribed[rws.route].apply(group, [params]);
        delete client.subscribed[rws.route];
      }

      if (client.reoccuring[rws.route]) {
        // events handled by client.on(...)
        client.reoccuring[rws.route].apply(group, [params]);
      }

      var action = group[gmarr[1]];

      if (action) {
          // events handled by groups
          action.apply(group, [params]);
      }
    } catch (e) {
      throw e;
      //console.error(e);

      //client.reconnect();
    }
  };

  this.on = function(route, callback) {
    this.reoccuring[route] = callback;
  };

  this.connect = function(serveraddress, callback) {
    this.address = serveraddress;

    try {
      this.ws = new WebSocket(serveraddress);    
    } catch (e) {
      for (let cb of client.ondisconnected_func) {
        cb();
      }

      return;
    }

    this.ws.onopen = function(event) {
      console.info("%cConnected to websocket", client.log_style);

      for (let cb of client.onconnected_func) {
        cb();
      }

      callback();
     };
    this.ws.onerror = function(event) {
      if (!this.ws || this.ws.readyState == 1) {
        // normal error
        for (let cb of client.onerror_func) {
          cb();
        }
      } else {
        // websocket related error
        console.info("%cError with connecting to websocket", client.log_style);
  
        // reset reconnecting flag
        client.trying = false;

        for (let cb of client.ondisconnected_func) {
          cb();
        }
      }
    };
    this.ws.onclose = function(event) {
      console.info("%cDisconnected from websocket", client.log_style);

      // reset reconnecting flag
      client.trying = false;

      for (let cb of client.ondisconnected_func) {
        cb();
      }
    };
    this.ws.onmessage = this.onmessage;
  };
  this.reconnect = function(cb) {
    if (!client.trying) {
      client.tryReconnect(1, cb);
    }
  };
  this.tryReconnect = function(N, cb, fail_cb, prog_cb) {
    // try reconnecting N times
    if (N == null)
      var N = 1;
    if (N==0) {
      if (fail_cb != null)
        fail_cb();
      return;
    }

    if (client.ws.readyState !== client.ws.OPEN) {

      if (!client.trying) {
        client.trying = true;

        console.info("%cTrying to reconnect..", client.log_style)
        if (prog_cb != null)
          prog_cb();

        client.connect(client.address, function(){
          client.trying = false;
          console.info("%cReconnect successful", client.log_style);

          if (cb != null)
            cb();
        });
      }


      setTimeout(()=>{
        client.tryReconnect(N--);
      }, 4000);
    } else {
      //console.info("Reconnect successful");
    }
  }
})();


var DeferredResponse = function(route, msid) {
  this.then = function(callback) {
    client.subscribed[msid] = callback;
  }
};

export let ws_client = client;
