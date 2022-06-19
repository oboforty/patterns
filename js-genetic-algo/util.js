
function sort_two(arr1, arr2, idx)
{
    // sorts two arrays at the same time
    var twoarr = [];
    for (var i in arr1) {
        twoarr.push([arr1[i], arr2[i]]);
    }

    twoarr.sort(function(left, right) {
        return left[idx] < right[idx] ? 1 : -1;
    });

    arr1.length = 0;
    arr2.length = 0;

    for (var j in twoarr) {
        arr1.push(twoarr[j][0]);
        arr2.push(twoarr[j][1]);
    }
}

function getBest(fitness){
    return fitness.indexOf(Math.max(...fitness));
}

function stringify_stat(char, stat_name) {
    if (!stat_name)
      stat_name = "";

    var str0 = stat_name + ' (';
    for (var _attr in char) {
        str0+= ` +${char[_attr]} ${_attr}`;
    }
    str0+=')';

    return str0;
}

var __randomBuffer = new Uint32Array(1);
function getRandom() {
    return Math.random();
    /*
    window.crypto.getRandomValues(__randomBuffer);

    let randomNumber = __randomBuffer[0] / (0xffffffff + 1);
    return randomNumber;
    */
}
function q(url, cb) {
    var base = 'https://api.guildwars2.com/v2/';
    fetch(base+url).then((r)=>r.json()).then(r=>cb(r));
}

function indicesToDistribution(arr) {
    var sumVal = 0, L = arr.length;

    for(let i of range(L))
        sumVal += i+1;

    var dist = {};

    for(let i of range(L))
        dist[i] = (L-i) / sumVal;
    
    return dist;
}
