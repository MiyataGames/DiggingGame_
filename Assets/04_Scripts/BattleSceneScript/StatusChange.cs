using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StatusChange
{
    public STATUS status;
    public int level;
    public int duration;

    public StatusChange(STATUS status, int level, int duration)
    {
        this.status = status;
        this.level = level;
        this.duration = duration;
    }
}
