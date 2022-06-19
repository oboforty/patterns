

class BD_Berserker {

    constructor(bf) {
        this.bf = bf;
        
        this.forbidden = new Set(["Harrier's", "Marshal's"]);

        this.filter_stats = new Set([
            'Power',
            'Precision',
            'CritDamage',
        ]);  
    }

    CalcFitness(buildarr) {
        let _char = this.bf.getBuildFromMutant(buildarr);
        
        return (_char['Power']||0) * 4 + (_char['Precision']||0) * 1 + (_char['CritDamage']||0) * 1.2;
    }

}