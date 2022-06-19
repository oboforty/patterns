import {load, onload} from '/engine/loader.js';
import {isocolors} from '/engine/colors.js';


export const world = {
  loaded: false,

  wid: null,
  username: null,
  uid: null,
  me: null,

  spectator: false,
  players: {},
  map: null,
};

export const countries = {};

export function set_world(w) {
  world.name = w.name;
  world.loaded = true;
  world.wid = w.wid;

  // add map file name
  if (w.map)
    world.map = w.map;

  // add turns
  if (typeof w.turns !== 'undefined') {
    for (let attr of Object.keys(w)) {
      world[attr] = w[attr];
    }

    world.turns = w.turns || 0;
    world.rounds = w.rounds || 0;
    world.max_rounds = w.max_rounds || 0;
    world.turn_time = w.turn_time || 0;
  }
}

export function set_user(user) {
  if (user && user.wid) {
    world.wid = user.wid;
    world.uid = user.uid;
    world.me = user.iso;
    world.username = user.username;
  }
}

export function load_world(world, cb) {
  if (typeof world !== 'string')
    set_world(world);

  load("world", function() {
    fetch('/worlds/load').then((resp)=>{
      return resp.json();
    }).then((resp)=>{
      this.ctx.world = resp.world;
      this.ctx.countries = resp.countries;
      this.ctx.players = resp.players;

      cb.bind(this)(resp);
      this.loaded();
    });
  });
}

onload("world", (ctx) => {
  if (ctx.world && !world.loaded) {
    set_world(ctx.world);
  }

  if (ctx.debug) {
    window.world = world;
    window.countries = countries;
  }

  if (ctx.countries) {
    world.isos = [];

    for (let country of ctx.countries) {
      if (ctx.players && ctx.players[country.iso]) {
        country.username = ctx.players[country.iso].username;
        country.division = ctx.players[country.iso].division;
      }
      world.isos.push(country.iso);

      if (country.color)
        isocolors[country.iso] = new Color(country.color);

      countries[country.iso] = country;
    }
  
    world.isos.sort((a,b) => {countries[a].order - countries[b].order});
  }
});
