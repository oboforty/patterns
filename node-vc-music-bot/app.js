const Discord = require("discord.js");
const { prefix, token } = require("./config.json");
const ytdl = require("ytdl-core");
const ytsr = require('ytsr');

const client = new Discord.Client({intents: ["GUILDS", "GUILD_MESSAGES"]});

const queue = new Map();

client.once("ready", () => {
  console.log("Ready!");
});

client.once("reconnecting", () => {
  console.log("Reconnecting!");
});

client.once("disconnect", () => {
  console.log("Disconnect!");
});

client.on("message", async message => {
  if (message.author.bot) return;
  if (!message.content.startsWith(prefix)) return;

  const serverQueue = queue.get(message.guild.id);

  if (message.content.startsWith(`${prefix}play`) || message.content.startsWith(`${prefix}p `)) {
    execute(message, serverQueue);
    return;
  } else if (message.content.startsWith(`${prefix}skip`) || message.content.startsWith(`${prefix}s `)) {
    skip(message, serverQueue);
    return;
  } else if (message.content.startsWith(`${prefix}stop`)) {
    oborage(message, serverQueue);
    //stop(message, serverQueue);
    return;
  } else if (message.content.startsWith(`${prefix}queue`)) {
    gogoqueue(message, serverQueue);
    return;
  } else if (message.content.startsWith(`${prefix}remove`)) {
    remove(message, serverQueue);
    return;
  } else {
    message.channel.send("You need to enter a valid command!");
  }
});

async function execute(message, serverQueue) {
  const args = message.content.split(" ");

  if (args.length <= 1) {
    return;
  }

  const voiceChannel = message.member.voice.channel;
  if (!voiceChannel)
    return message.channel.send(
      "You need to be in a voice channel to play music!"
    );
  const permissions = voiceChannel.permissionsFor(message.client.user);
  if (!permissions.has("CONNECT") || !permissions.has("SPEAK")) {
    return message.channel.send(
      "I need the permissions to join and speak in your voice channel!"
    );
  }

  let err = null;
  let searchString = args[1];
  if (!searchString.startsWith('http')) {
    // search entire term
    args.shift();
    searchString = args.join(" ")  
    const searchResults = await ytsr(searchString, {
      pages: 1
    });

    const vidSearch = searchResults.items.find(f=>f.type=='video');
    searchString = vidSearch.url;
    //return message.channel.send("ajgdkfgfsfhfgs");
  }

  const songInfo = await ytdl.getInfo(searchString).catch((error) => {
    err = true;
    //done();
  });
  
  if (err)
    return message.channel.send("YT link not found! Are you sure it's a youtube link?");

  const song = {
        title: songInfo.videoDetails.title,
        url: songInfo.videoDetails.video_url,
   };

  if (!serverQueue) {
    const queueContruct = {
      textChannel: message.channel,
      voiceChannel: voiceChannel,
      connection: null,
      songs: [],
      volume: 5,
      playing: true
    };

    queue.set(message.guild.id, queueContruct);

    queueContruct.songs.push(song);

    try {
      var connection = await voiceChannel.join();
      queueContruct.connection = connection;
      play(message.guild, queueContruct.songs[0]);
    } catch (err) {
      console.log(err);
      queue.delete(message.guild.id);
      return message.channel.send(err);
    }
  } else {
    serverQueue.songs.push(song);
    return message.channel.send(`${song.title} has been added to the queue!`);
  }
}

function skip(message, serverQueue) {
  if (!message.member.voice.channel)
    return message.channel.send(
      "You have to be in a voice channel to stop the music!"
    );
  if (!serverQueue)
    return message.channel.send("There is no song that I could skip!");
  serverQueue.connection.dispatcher.end();
}

function stop(message, serverQueue) {
  if (!message.member.voice.channel)
    return message.channel.send(
      "You have to be in a voice channel to stop the music!"
    );
    
  if (!serverQueue)
    return message.channel.send("There is no song that I could stop!");
    
  serverQueue.songs = [];
  serverQueue.connection.dispatcher.end();
}

function gogoqueue(message, serverQueue) {
  if (!serverQueue)
    return message.channel.send("Nothing is playing my dude!");
  else {
    let asdf = "\n", i = 1;
    for(let song of serverQueue.songs) {
      asdf += "["+i+"] "+song.title + "\n";
      i++;
    }
    return message.channel.send(asdf);
  }
}

function remove(message, serverQueue) {
  const songIdx = parseInt(message.content.split(" ")[1]) -1;
  const remSong = serverQueue.songs.splice(songIdx, 1)[0];
  console.log(remSong);

  return message.channel.send(`I yeeted the fuck out of **${remSong.title}**`);

}

function play(guild, song) {
  const serverQueue = queue.get(guild.id);
  if (!song) {
    serverQueue.voiceChannel.leave();
    queue.delete(guild.id);
    return;
  }

  const dispatcher = serverQueue.connection
    .play(ytdl(song.url))
    .on("finish", () => {
      serverQueue.songs.shift();
      play(guild, serverQueue.songs[0]);
    })
    .on("error", error => console.error(error));
  dispatcher.setVolumeLogarithmic(serverQueue.volume / 5);
  serverQueue.textChannel.send(`Start playing: **${song.title}**`);
}
async function oborage(message, serverQueue) {
  if (!message.member.voice.channel)
    return message.channel.send(
      "You have to be in a voice channel to stop the music!"
    );
  
  const _disp = serverQueue.connection.dispatcher;
  const dispatcher = serverQueue.connection
    .play('obo/a_kurva_anyad.mp3')
    .on("finish", () => {
      serverQueue.songs = [];
      _disp.end();
    })
    .on("error", error => {
      console.error(error);
      _disp.end();
    });
  // dispatcher.setVolumeLogarithmic(serverQueue.volume / 5);
  // serverQueue.textChannel.send('okay then');
}


console.log("invite bot with https://discord.com/api/oauth2/authorize?client_id=893947200286126090&permissions=36714496&scope=bot")
client.login(token);
