import {ws_client} from '/js/websocket/wsclient.js';
//import {getColor} from '/engine/colors.js';

import {template} from "/js/worlds/chat.vue.js"

// Game GUI's main frame
export const chat_gui = Vue.component('chat-gui', {
  template: template,
  props: ['roomId'],
  data: function() {
    return {
      show: true,
      config: {},

      prev_iso: null,

      username_func: function(iso) {
        return iso;
      },
      err: null,
      msg_input: "",
      messages: [],
      online: {},
      actions: {},
      emojis: {},
    }
  },
  created: function () {
    // if (ws_client.connected)
    //   this.sub();
  },
  methods: {

    sub: function() {
      ws_client.request("Chat:subscribe", {
        room_id: this.roomId
      }).then(({messages, users})=>{
        this.show = true;

        // reload messages
        this.messages.splice(0, this.messages.length);
        for (let msg of messages) {
          this.add_message(msg);
        }

        //reload users
        this.online = {};
        for (let iso of users) {
          this.add_user(iso, this.username_func(iso));
        }
      });

      ws_client.on("Chat:message", ({room_id, msg})=>{
        this.add_message(msg);
      });
      ws_client.on("Chat:subscribed", ({username, iso})=>{
        this.add_user(iso, username);
      });
      ws_client.on("Chat:unsubscribed", ({username, iso})=>{
        delete this.online[iso];
      });

      if (this.config.toggle_on_enter) {
        window.removeEventListener('keydown', this.onkey)
        window.addEventListener('keydown', this.onkey)
      }
    },

    onkey: function(e) {
      if (e.keyCode === 13) {
        // toggle chat with enter key
        if (!this.show) {
          this.show = true;
        }

        Vue.nextTick(()=>{
          // focus on message input
          if (this.$refs.inputter)
            this.$refs.inputter.focus();
        });
      }
    },

    add_message: function(message) {
      // add emojis
      let emojis = message.msg.match(/\:([a-z0-9\s\_\-]*.)\:/gi);
      if (emojis)
        for (let emoji of emojis) {
          emoji = emoji.substr(1, emoji.length-2);

          if (this.emojis[emoji])
            message.msg = message.msg.replace(':'+emoji+':', `<img title=":${emoji}:" class="chat-emoji" gicon=":${emoji}:" src="/img/chat/${this.emojis[emoji]}" />`);
        }

      // Add message
      this.messages.push(message);

      setTimeout(()=>{
        var container = this.$el.querySelector(".chat-messages");
        container.scrollTop = container.scrollHeight;
      }, 10);
    },

    add_user: function(iso, uname) {
      if (this.online[iso] && !uname)
        return;

      if (uname)
        this.online[iso] = uname;
      else {
        // todo: get username from countries
        this.online[iso] = null;
      }
    },

    on_submit: function() {
      if (this.msg_input == "") {
        if (this.show && this.config.toggle_on_enter)
          this.show = false;

        return;
      }
      // Apply actions
      let actions = this.msg_input.match(/\/([a-z0-9\s\_\-]*.)/gi);

      if (actions) {
        for (let action of actions) {
          action = action.substr(1, action.length-1);

          if (this.actions[action])
            this.actions[action](this.msg_input);
        }
      } else {
        // if there's no action, send message to chat
        ws_client.request("Chat:message", {
          room_id: this.roomId,
          msg: this.msg_input,
        });
      }

      this.msg_input = "";
    },

    style_get_color: function(iso) {
      return 'color: ' + 'crimson';//getColor(iso).rgb();
    }
  },
  watch: {
    messages: function() {
    }
  }
});
