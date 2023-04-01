using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILocomotionModifier
{
    float Amount { get; }
    string Tag { get; }
}
