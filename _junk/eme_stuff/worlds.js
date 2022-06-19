import {component} from '/js/worlds/worlds-list.js';

let gui = new Vue({
  el: '#app-gui',
});


gui.$refs['worlds-list'].init();
