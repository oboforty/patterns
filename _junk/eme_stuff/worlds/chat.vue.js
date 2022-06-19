export const template = `
<div v-if="show">
  
  <div class="d-flex">
    <div class="p-2 flex-fill">
      
      <div class="chat-messages-wrapper">

        <!-- list of messages -->
        <div class="scroll chat-messages">
          <div v-for="message in messages" class="chat-message">
            <strong v-if="config.msg_show_country && message.iso && config.country_style == 'iso'" :style="style_get_color(message.iso)">[{{message.iso}}]</strong> 
            <div v-else-if="config.msg_show_country && message.iso && config.country_style == 'flag'" :class="'flag d-inline-block flag-xs flag-'+message.iso"></div>
            <div v-else-if="config.msg_show_country && message.iso && config.country_style == 'herald'">NO_HERALD</div>

            <strong v-if="config.msg_show_username && message.username" :style="style_get_color(message.iso)">{{message.username}}:</strong> 

            <span v-html="message.msg"></span>
          </div>
        </div>
        
      </div>

    </div>
    <div class="p-2">

      <div v-if="err" class="text-danger bg-light">{{ err }}</div>

      <!-- online users -->
      <div v-for="(username,iso) in online" class="chat-user">
        <strong v-if="config.prof_show_country && iso && config.country_style == 'iso'" :style="style_get_color(iso)">[{{iso}}]</strong> 
        <div v-else-if="config.prof_show_country && iso && config.country_style == 'flag'" :class="'flag d-inline-block flag-xs flag-'+iso"></div>
        <div v-else-if="config.prof_show_country && iso && config.country_style == 'herald'">NO_HERALD</div>

        <strong v-if="config.prof_show_username && username" :style="style_get_color(iso)">{{username}}</strong> 
      </div>
      
    </div>
  </div>

  <!-- Bottom inputs -->
  <div class="">
    <div class="input-group">
      <input ref="inputter" class="form-control chat-input" v-model="msg_input" v-on:keydown.enter="on_submit" v-on:keydown.stop=""  />

      <div class="input-group-append">
        <button v-on:click="on_submit" class="btn btn-outline-warning" type="button">Send</button>
      </div>
    </div>
  </div>

</div>
`;