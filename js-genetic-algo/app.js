
// ================================================ \\
// GW2 config
const rar = 'exotic';
const build_name = 'RaidDruid';
// ================================================ //


var interval = null;
const GW2 = new GW2Builds(rar);
const _thebuild = GW2.build(build_name);

function startGenetic(stats) {
    // Run genetic algorithm
    //var mutants = null;
    let iter = 0;
    var op = 0;
    var op_char = null;
    
    const elitism_kept = 3;
    const fos_kept = 12;
    const mut_rate = 0.02;
    const pop_size = 400;
    const speed = 20;

    //console.log(stats);
    //GW2.feed_stats(stats);

    function getInitPopulation() {
        return [...new Array(pop_size)]
        .map( d => Array(GW2.slot_names.length).fill('a').map( d => getRandomStatPrefix() ) )
    }

    function getRandomStatPrefix() {
        return Math.floor(getRandom()*stats.length);
    }

    function mutate(child) {
        return child.map(d => getRandom() < mut_rate ? getRandomStatPrefix() : d);
    }

    function crossover(mother, father) {


        let midpoint = Math.floor(getRandom()*stats.length) // Pick a midpoint

        // @TODO: try other form of crossover?
        // Half from one, half from the other
        return mother.slice(0,midpoint).concat(  father.slice(midpoint,stats.length) );
    }

    function newGeneration(population) {
        let fitness = population.map(e => _thebuild.CalcFitness(e) )
        let avgFitness = fitness.reduce((a, b) => a + b ) / fitness.length

        sort_two(population, fitness, 1);

        // 0) get metrics for best specimen in population
        let sigmaMaleGrindset = population[0];
        let bestFitness = fitness[0];
        if (bestFitness > op) {
            op = bestFitness;
            op_char = sigmaMaleGrindset;
        }
        logMetrics(population, iter++, avgFitness, sigmaMaleGrindset, op, op_char)
        
        let mutants = [];

        // 1) selection (select distribution by rank)
        // guarantee constant pop size: (remaining pop space are filled using regular GA selection)
        let remainingSpaces = population.length - fos_kept// - elitism_kept;
        // survival chance depends on fitness rank
        let dist = indicesToDistribution(fitness);
        for (let _ of range(remainingSpaces)) {
            let i = random.distribution(dist);
            mutants.push(population[i]);
            //delete dist[rndPop];
        }
        
        mutants = mutants.map((a) => {
            // 1) pick two (surviving) parents
            let i = Math.floor(getRandom()*mutants.length);
            let j = Math.floor(getRandom()*mutants.length);

            // 2) crossover (create a child by combining DNA)
            let child = crossover(mutants[i], mutants[j])

            // 3) mutation (mutate the child based on a given probability)
            let mutant = mutate(child)

            // 4) add the new child to the population
            return mutant
        })

        // 1b) belitism - always leave the N best specimen
        mutants.concat(population.slice(0, elitism_kept));
        
        // 1c) keep random - leave some original speciman of any fitness as-is
        for (let _ of range(fos_kept)) {
            let i = Math.floor(getRandom()*population.length);
            mutants.push(population[i]);
        }

        return mutants;
    }

    function logMetrics(population, iter, avgFitness, best, op, op_char) {
        var li = population.map(d => "<li>" + d + "</li>").join("")
        document.getElementById("genetic").innerHTML = "<ul style='columns: 4'>"+li+"</ul>"
        document.getElementById("counter").innerHTML = iter + " generations"
        document.getElementById("avgFitness").innerHTML = Math.round(avgFitness*100)/100
        document.getElementById("maxPop").innerHTML = population.length

        
        for(var i in GW2.slot_names) {
            var _slot = GW2.slot_names[i];

            // display best in population
            var _statID = best[i];
            var stat = stats[_statID];
            var givenAttributes = stat.attr[_slot];
            document.getElementById("best_"+_slot).innerHTML = stringify_stat(givenAttributes, stat.name);

            if (op != null) {
                // display best ever (op) char stats
                var _statID = op_char[i];
                var stat = stats[_statID];
                var givenAttributes = stat.attr[_slot];
                document.getElementById("op_"+_slot).innerHTML = stringify_stat(givenAttributes, stat.name);
            }
        }

        var best_char = GW2.getBuildFromMutant(best);
        var op_char2 = GW2.getBuildFromMutant(op_char);
        document.getElementById("best_total").innerHTML = stringify_stat(best_char);
        document.getElementById("op_total").innerHTML = stringify_stat(op_char2);
        document.getElementById("best_fitness").innerHTML = _thebuild.CalcFitness(best);
        document.getElementById("op_fitness").innerHTML = _thebuild.CalcFitness(op_char);
    }


    // OK, passing
    // console.log(GW2.getBuildFromMutant([0,0,0,0,0,0,0,0,0,0,0,0,0]));

    // START genetic algorithm:
    var mutants = getInitPopulation();

    var aaa = 0;
    interval = setInterval(()=>{
        mutants = newGeneration(mutants);

        // aaa++;
        // if (aaa>2)
        //     clearInterval(interval);
    }, speed);
}

function stopgen() {
    clearInterval(interval);
}

GW2.load_stats((stats)=>{
    startGenetic(stats);
}, _thebuild.filter_stats, _thebuild.forbidden);

