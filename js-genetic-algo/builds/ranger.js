

class BD_RaidDruid {

    constructor(bf) {
        this.bf = bf;

        this.boonDurMult = 1/15;
        this.defBoonRunes = 25; // rune of the Water (15 for Monk)
        this.defBoonSigils = 10; // sigil of concentration

        this.maxBoonDur = 100;
        this.minBoonDur = 94;
        
        this.forbidden = new Set([
            "Harrier's", "Marshal's",
            
        ]);

        this.reduceFitness = new Set([
            //"Diviner's", "Giver's"
            28, 30
        ]);

        this.reduceFitnessPunishment = 80;

        this.filter_stats = new Set([
            //'Power',
            //'Precision',
            //'Toughness',
            //'Vitality',
            //'CritDamage',
            'Healing',
            //'ConditionDamage',
            'BoonDuration',
            //'ConditionDuration'
        ]);  
    }

    CalcFitness(buildarr) {
        let _char = this.bf.getBuildFromMutant(buildarr);

        // set fittness to 0 if boon duration goes above 100% as it's wasted
        var boonDur = (_char['BoonDuration'] * this.boonDurMult) + this.defBoonRunes + this.defBoonSigils;
        if (boonDur > this.maxBoonDur)
            return 0;
                
        let score = (_char['Healing']||0) * 2 + (_char['Power']||0) * 1;

        // added fitness bonus for Concentration doesn't matter after 95%
        if (boonDur < this.minBoonDur)
            score += (_char['BoonDuration']||0) * 4;

        for (var q of buildarr)
            if (this.reduceFitness.has(q))
                score -= this.reduceFitnessPunishment;

        return score;
    }

}