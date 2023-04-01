using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBatteryListener
{
    void NewCharge(int oldCharge, int newCharge);
}
