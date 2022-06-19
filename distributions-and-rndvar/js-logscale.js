var min = 1500;
var max = 5000000;

var vals = [1523, 2000, 5000, 12000, 25000, 52330, 100000, 200000, 500000, 800000, 1000000, 2000000, 3000000, 4000000, 4899999];

console.log('bounds:', min, max);

for (var val of vals) {
  var p_lin = (val - min) / (max - min);
  var p_log = Math.log(val - min) / Math.log(max - min);

  console.log('val:', val);
  console.log('lin scale:', Math.round(p_lin*100), '%');
  console.log('log scale:', Math.round(p_log*100), '%');
}
