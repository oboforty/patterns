
const w = 8, h = 6;
const pits = 2, halls = 10, bats = 2;


const canvas = document.getElementById('canvas');
const ctx = canvas.getContext('2d');
canvas.width = w*64;
canvas.height = h*64;

const tiles = {};
function loadimg(name) {
    var newImg = new Image;
    newImg.src = `img/${name}.png`;
    tiles[name] = newImg;
}
loadimg('room');
loadimg('blood');
loadimg('smell');
loadimg('nasty');
loadimg('wumpus');
loadimg('bat');
loadimg('pit');
loadimg('hall1');
loadimg('hall2');

const grid = {}, mobs = {};

function* rndcoords(n) {
    if (n >= w*h)
        throw new Exception("not enough cells for n unique values");

    // generates N unique coordinates within a grid
    const was = new Set();

    for (let i = 0; i < n;) {
        let x = Math.floor(Math.random()*w), y = Math.floor(Math.random()*h);

        if (!was.has(x+','+y)) {
            was.add(x+','+y);
            yield [x,y];
            i++;
        }
    }
}

function cid(xy) {
    return xy[0]+','+xy[1];
}

function set_tile(x,y, tile) {
    let coord = cid([x,y]);

    if ((tile == 'smell' && grid[coord] == 'blood') || (tile == 'blood' && grid[coord] == 'smell'))
        grid[coord] = 'nasty';
    else if (!grid[coord])
        grid[coord] = tile;
}

function set_cyclic_coords(dct) {
    // swap cells outside the grid to their cyclic coord values

    for(let coord in dct) {
        let [x,y] = coord.split(',');

        if (x < 0) x = w+parseInt(x);
        else if (x >= w) x -= w;
        if (y < 0) y = h+parseInt(y);
        else if (y >= h) y -= h;

        let coord1 = x+','+y;

        if (coord1 != coord) {
            // replace
            dct[coord1] = dct[coord];
            delete dct[coord];    
        }
    }
}

function generate() {
    const iter = rndcoords(1 + pits + bats + halls);

    // bats
    for (let i = 0; i < bats; i++)
        mobs[cid(iter.next().value)] = 'bat';

    // pits
    for (let i = 0; i < pits; i++) {
        let [x,y] = iter.next().value;
        console.log(x,y);
        grid[cid([x,y])] = 'pit';
        
        set_tile(x+1,y+0, 'smell');
        set_tile(x+0,y+1, 'smell');
        set_tile(x-1,y+0, 'smell');
        set_tile(x+0,y-1, 'smell');
    }

    // wumpus
    let [x,y] = iter.next().value;
    mobs[cid([x,y])] = 'wumpus';

    set_tile(x+1,y+0, 'blood');
    set_tile(x+0,y+1, 'blood');
    set_tile(x+0,y-1, 'blood');
    set_tile(x-1,y+0, 'blood');
    set_tile(x-1,y-1, 'blood');
    set_tile(x+1,y+1, 'blood');
    set_tile(x-1,y+1, 'blood');
    set_tile(x+1,y-1, 'blood');

    set_tile(x+2,y+0, 'blood');
    set_tile(x-2,y+0, 'blood');
    set_tile(x+0,y+2, 'blood');
    set_tile(x+0,y-2, 'blood');

    set_tile(x+0,y+0, 'blood');

    // add halls
    for (let i = 0; i < halls; i++) {
        grid[cid(iter.next().value)] = Math.random() > 0.5 ? 'hall1' : 'hall2';
    }

    // reset coords out of bounds
    set_cyclic_coords(grid);
}

function draw() {
    for (let y = 0; y < h; y++)
        for (let x = 0; x < w; x++) {
            const coord = cid([x,y]);
            let tile = grid[coord] || 'room';
            let mob = mobs[coord] || null;

            ctx.drawImage(tiles[tile], x*64, y*64);

            if (mob)
                ctx.drawImage(tiles[mob], x*64, y*64);
        }
}

setTimeout(() => {
    generate();
    draw();
}, 200);