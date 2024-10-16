using System;

namespace RuneDocMVVM.CommunicationTypes;

public class Entity
{
    public string Name { get; set; }
    public uint CombatLevel { get; set; }
    public uint ServerIndex { get; set; }
    public ulong Address { get; set; }

    public static Entity FromString(string bufferSlice)
    {
        // Name -> combat level -> server index -> address -> ^delimiter
        var spl = bufferSlice.Split("#");
        
        Entity e = new Entity();
        e.Name = spl[0];
        e.CombatLevel = Convert.ToUInt32(spl[1]);
        e.ServerIndex = Convert.ToUInt32(spl[2]);
        e.Address = Convert.ToUInt64(spl[3]);
        
        return e;
    }

    public void Print()
    {
        Console.WriteLine($"{Name} - {CombatLevel} - {ServerIndex} - {Address:X}");
    }
}