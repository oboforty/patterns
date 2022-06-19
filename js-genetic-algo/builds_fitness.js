
class GW2Builds {

    constructor(rar) {
        this.rar = rar;
        
        this.slot_names = [
            'wep', 'chest', 'legs', 'head','shoulders','gloves','boots','amulet','ring1','ring2','earring1','earring2','back'
        ];

        try {
            this.builds = {
                'RaidDruid': new BD_RaidDruid(this),
                'Berserker': new BD_Berserker(this)
            };
        } catch(e) {
            console.log("Builds not found");
        }
        this.build_name = null;

                
        this.skip_stats = new Set([
            "Rabid and Apothecary's"
        ]);
        this.total_bonuses = {
            'ascended': {
                3: 3303,
                4: 3612,
                9: 5751
            },
            'exotic': {
                3: 3087,
                4: 3366,	
                9: 5436
            }
        };
        this.stat_dist = {
            'exotic': {
                3: {
                    'wep': [240,170],
                    'chest': [134,96],
                    'legs': [90,64],
                    'head': [60,43],
                    'shoulders': [45,32],
                    'gloves': [45,32],
                    'boots': [45,32],
                    'amulet': [120,85],
                    'ring1': [90,64],
                    'ring2': [90,64],
                    'earring1': [75,53],
                    'earring2': [75,53],
                    'back': [30,21],
                },
                4: {
                    'wep': [204,112],
                    'chest': [115,63],
                    'legs': [77,42],
                    'head': [51,28],
                    'shoulders': [38,21],
                    'gloves': [38,21],
                    'boots': [38,21],
                    'amulet': [102,56],
                    'ring1': [77,42],
                    'ring2': [77,42],
                    'earring1': [64,35],
                    'earring2': [64,35],
                    'back': [26,14],
                }
            },
            'ascended': {
                3: {
                    'wep': [250,180],
                    'chest': [141,101],
                    'legs': [94,67],
                    'head': [63,45],
                    'shoulders': [47,34],
                    'gloves': [47,34],
                    'boots': [47,34],
                    'amulet': [157,108],
                    'ring1': [126,85],
                    'ring2': [126,85],
                    'earring1': [110,74],
                    'earring2': [110,74],
                    'back': [63,40],
                },
                4: {
                    'wep': [216,118],
                    'chest': [121,67],
                    'legs': [81,44],
                    'head': [54,30],
                    'shoulders': [40,22],
                    'gloves': [40,22],
                    'boots': [40,22],
                    'amulet': [133,71],
                    'ring1': [106,56],
                    'ring2': [106,56],
                    'earring1': [92,49],
                    'earring2': [92,49],
                    'back': [52,27],
                }
            }
        
        };
        
    }

    build(name) {
        this.build_name = name;
        return this.builds[name];
    }

    getBuildFromMutant(source) {
        // calculate attributes
        var _allstats = {};
        
        for(var i in source) {
            // iterate on all inv slots
            var _slot = this.slot_names[i];
            var attrs = this.stats[source[i]].attr[_slot];
            
            // iterate on all given attributes (3 or 4) of that item
            for (var attr_name in attrs) {
                _allstats[attr_name] = (_allstats[attr_name]||0)+attrs[attr_name];
            }
        }

        return _allstats;
    }

    config_to_stats(d) {
        var arr = [];

        for (var slot in d) {
            var stat_name = d[slot];

            var i = this.stats.findIndex(s=>s.name == stat_name);
            //var stat = this.stats[i];

            arr.push(i);
        }

       return arr;
    }

    load_stats(cb, filter_stats, forbidden) {
        q('itemstats', (stats)=>{
            q('itemstats?ids='+stats.join(','), (stats)=>{

                // filter out relevant stats
                stats = stats.filter((el)=>{
                    // skip noob items & celestial
                    var L = el.attributes.length;
                    if (L != 3 && L != 4)
                        return false;
                    else if (this.skip_stats.has(el.name))
                        return false;
        
                    var match_attr_count = el.attributes.reduce((acc, {attribute, value, multiplier})=>value > 0 && multiplier > 0 && filter_stats.has(attribute) ? acc+1 : acc, 0);
                    return match_attr_count > 0;
                });
                // filter out duplicates and unavailable prefixes
                stats = stats.reduce(function (p, c) {
                    if (!forbidden.has(c.name) && !p.some(function (el) { return el.name === c.name; }))
                        p.push(c);
                    return p;
                }, []);
                this.stats = stats;

                // find in-game values for each stat
                stats = stats.map((stat)=>{
                    var obj = {
                        name: stat.name,
                        id: stat.id,
                        L: stat.attributes.length,
                        attr: {},
                    };
                    //const tb = total_bonuses[rar][obj.L];
                    const sdist = this.stat_dist[this.rar][obj.L];
            
                    for (let slot in sdist) {
                        obj.attr[slot] = {};
        
                        for(let {attribute, multiplier} of stat.attributes) {
                            const isPrimaryStat = (obj.L==4 && multiplier == 0.3) || (obj.L==3 && multiplier == 0.35);

                            //console.log(attribute, Math.round(tb*multiplier), isPrimaryStat);
                            // iterate over all inventory slots and map the given bonus
                            var stat_per_slot = sdist[slot][isPrimaryStat?0:1];
                            obj.attr[slot][attribute]= stat_per_slot;
                        }
                    }
        
                    return obj;
                });
                this.stats = stats;

                cb(stats);
            });
        });
        
    }
}