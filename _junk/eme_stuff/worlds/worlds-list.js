import {template} from '/js/worlds/worlds-list.vue.js';
import {request} from '/js/api.js';


export let component = Vue.component('worlds-list', {
  template: template,

  data: function() {
    return {
      worlds: [],
      access_token: null,
      selected: null,
    }
  },

  methods: {
    init: function() {
      request('GET /api/worlds', {}, worlds=>this.worlds = worlds);
    },

    create_world: function() {
      request('POST /api/worlds', {}, world=>this.worlds.push(world));
    },

    delete_world: function() {
      if (!this.selected)
        return;

      request('DELETE /api/worlds/'+selected.wid, {}, ()=>{
        // for (let w of this.worlds)
          // this.worlds.
      });
    },

    set_invlink: function() {
      if (!this.selected)
        return;

      request('PATCH /api/worlds/'+this.selected.wid, {
        invlink: 'auto',
      }, ({wid, invlink})=>{

        this.selected.invlink = invlink;
      });
    },

    set_name: function() {
      if (!this.selected)
        return;

      const name = prompt("New name:");

      if (!name)
        return;

      request('PATCH /api/worlds/'+this.selected.wid, {
        name: name,
      }, ({wid, name})=>{
        this.selected.name = name;
      });
    },

    on_join: function(world) {
      const iso = prompt("ISO:", 'UK');

      if (!iso)
        return;

      request('POST /api/worlds/'+world.wid+'/player', {
        iso: iso,
      }, ({wid, iso})=>{
        this.selected.iso = iso;
      });

    },

    interval: function(t) {
      const dt = time() - t;

      const days = Math.floor(dt / (24*3600));

      if (days <= 0)
        return Math.floor(dt/60) + ' minutes ago';
      else if (days < 30)
        return days + ' days ago';

      var date = new Date(t * 1000);

      return date.toString();
    }
  }
});
