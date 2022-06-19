export const template = `
  <div class="d-flex">
    <div>
      <button @click="create_world" class="btn btn-block btn-success">Create</button>
      <button @click="set_invlink" class="btn btn-block btn-warning">Invite link</button>
      <button @click="set_name" class="btn btn-block btn-warning">Set name</button>
      <button @click="delete_world" class="btn btn-block btn-danger">Delete</button>
    </div>
    <div class="flex-fill p-3">

      <ul class="list-group">

        <li v-for="world in worlds" @click="selected = world" :class="{'list-group-item list-group-item-action flex-column align-items-start pointer': true, 'active': selected && selected.wid == world.wid}">
          <div class="d-flex w-100 justify-content-between">
            <h5 class="mb-1">{{ world.name }}</h5>
            <small>{{ (new Date(world.created_at*1000.0)).toUTCString() }}</small>
          </div>
          <p class="mb-1">
            <i>#{{ world.invlink }}</i>

            <button class="btn btn-primary" @click="on_join(world)">Join</button>
          </p>
          <small>Last update: {{ interval(world.last_update) }}</small>
        </li>

      </ul>

      <p v-if="worlds.length == 0" class="text-danger">No worlds found.</p>

    </div>
  </div>
`;