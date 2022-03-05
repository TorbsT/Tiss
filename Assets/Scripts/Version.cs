using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Version
{
    private static int nextId;
    public Version()
    {
        id = nextId;
        nextId++;
    }
    private int id;
    public static bool operator ==(Version a, Version b) => a.id == b.id;
    public static bool operator !=(Version a, Version b) => a.id != b.id;

    public override bool Equals(object obj) => (Version)obj == this;
    public override int GetHashCode() => id;
    public override string ToString() => "V"+id;
}
