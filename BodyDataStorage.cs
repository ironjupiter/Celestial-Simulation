using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BodyDataStorage : IEquatable<BodyDataStorage>
{
    public int id { get; set; }
    public float mass { get; set; }
    public float denstity { get; set; }
    public Vector3 velocity { get; set; }
    public Vector3 momentuem { get; set; }
    public Vector3 position { get; set; }
    


    public override string ToString()
    {
        return "ID: " + id;
    }
    public override bool Equals(object obj)
    {
        if (obj == null) return false;
        BodyDataStorage objAsPart = obj as BodyDataStorage;
        if (objAsPart == null) return false;
        else return Equals(objAsPart);
    }
    public override int GetHashCode()
    {
        return id;
    }
    public bool Equals(BodyDataStorage other)
    {
        if (other == null) return false;
        return (this.id.Equals(other.id));
    }
}
