// Globals ===============================================================================================================
console.log('require', Object.keys(require));
console.log('module', Object.keys(module));
console.log('process', Object.keys(process));
console.log(__dirname, __filename);


// OS & files ===============================================================================================================
/*
    fs                  To handle the file system
    os                  Provides information about the operation system
    path                To handle file paths
    readline            To handle readable streams one line at the time
    stream              To handle streaming data
    string_decoder      To decode buffer objects into strings
    zlib                compression
  */
const path = require('path')
const os = require('os')
const fs = require('fs')

const filePath = path.join('/content/', 'subfolder', 'test.txt')
const base = path.basename(filePath)
const absolute = path.resolve(__dirname, 'content', 'subfolder', 'test.txt')
console.log(absolute)

console.log(`The System Uptime is ${os.uptime()} seconds`)
const user = os.userInfo()

const currentOS = {
  name: os.type(),
  release: os.release(),
  totalMem: os.totalmem(),
  freeMem: os.freemem(),
}


// default 64kb
// last buffer - remainder
// highWaterMark - control size
// const stream = createReadStream('./content/big.txt', { highWaterMark: 90000 })
// const stream = createReadStream('../content/big.txt', { encoding: 'utf8' })
const stream = fs.createReadStream('./content/big.txt')

stream.on('data', (result) => {
  console.log(result)
})
stream.on('error', (err) => console.log(err))

// write big file
for (let i = 0; i < 10000; i++) {
  fs.writeFileSync('./content/big.txt', `hello world ${i}\n`, { flag: 'a' })
}

fs.readFile('./content/first.txt', 'utf8', (err, result) => {
    if (err) {
      console.log(err)
      return
    }
});  


// Networking ===============================================================================================================
/*
    dgram               Provides implementation of UDP datagram sockets
    dns                 To do DNS lookups and name resolution functions
    http                To make Node.js act as an HTTP server
    https               To make Node.js act as an HTTPS server.
    net                 To create servers and clients
    querystring         To handle URL query strings
    url                 To parse URL strings
 */

// Process & Threading ===============================================================================================================
/*
    cluster             To split a single Node process into multiple processes
    child_process       To run a child process
    timers              To execute a function after a given number of milliseconds
 */

// Security ===============================================================================================================
/*
    crypto              To handle OpenSSL cryptographic functions
    tls                 To implement TLS and SSL protocols
*/

// Etc ===============================================================================================================
/*
    util                To access utility functions
    assert              Provides a set of assertion tests
    buffer              To handle binary data
    events              To handle events

    domain              Deprecated. To handle unhandled errors
    punycode            Deprecated. A character encoding scheme
    tty                 Provides classes used by a text terminal
    v8                  To access information about V8 (the JavaScript engine)
    vm                  To compile JavaScript code in a virtual machine
*/