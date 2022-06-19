import {chat_gui} from '/js/worlds/chat.js';
import {ws_client} from '/js/websocket/wsclient.js';


let gui = new Vue({
  el: '#app-gui',
});




export function init(ws_address, user, cb) {
  const ws = ws_client;
  ws.groups = {
  };

  ws.connect(ws_address, ()=>{
    console.log("Connected");

    const r = ws.request("Users:auth", {
      uid: user.uid,
      access_token: user.access_token,
    }).then((resp)=>{
      console.log("Authenticated", resp.uid);

      // ws.request("Users:me", {}).then((usr)=>{
      //   console.log("works!", usr);
      // });

      gui.$refs['global-chat'].sub();

      window.ws = ws;
      window.chat = gui.$refs['global-chat'];

      if(cb)
        cb();
    });
  });

  ws.onerror((event)=>{
    console.error(event);
  });

  ws.ondisconnect((event)=>{
    if (gui.opened != 'disconnect')
      console.error("Disconnected");

    // try reconnecting
    // ws.tryReconnect(1, ()=>{
    //   this.request_auth(user, ()=>{
    //     gui.$refs['global-chat'].err = null;
    //   });
    // }, ()=>{
    //   // couldn't reconnect
    //   gui.$refs['global-chat'].err = "Failed to reconnect.";
    // }, ()=>{
    //   // reconnect attempt
    //   gui.$refs['global-chat'].err = "Attempting to reconnect...";
    // });
  });

}