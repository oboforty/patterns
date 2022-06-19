
const DT = 1000, TICKS = 35;
let ticks_elapsed = 0;
let timeout = null, interval = null;


export function user_timeout(current_iso, cb) {
  /**
   * Handles GUI update and WS request of user timing out of its turn
   */

  // clear previous timeout (at new turn)
  if (timeout)
    clearTimeout(timeout);
  if (interval)
    clearInterval(interval);
  

  ticks_elapsed = 0;

  // set another timeout for calculating seconds left
  interval = setInterval(()=>{
    let p = (++ticks_elapsed)/TICKS;

    if (p<1.0) {
      cb(p, TICKS-ticks_elapsed);
    } else if (p>=1.0) {
      clearInterval(interval);
    }
  }, DT);

  // start a timeout for new user
  timeout = setTimeout(()=>{
    if (interval)
      clearInterval(interval);

    cb(1.0, 0);
  }, TICKS*DT +200);
}
