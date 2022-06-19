using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum MinionState
{
    Idle,
    Moving,
    Mining,
}

public class MinionTask
{
    MinionState TaskType;

    Block Target;
}
