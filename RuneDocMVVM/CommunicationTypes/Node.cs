using System;

namespace RuneDocMVVM.CommunicationTypes;

public class Node
{
    public string Name { get; set; }
    public uint ServerIndex { get; set; }
    public ulong Address { get; set; }

    public static Node FromString(string bufferSlice)
    {
        // Name -> server index -> address
        var spl = bufferSlice.Split("#");
        
        Node n = new Node();
        n.Name = spl[0];
        n.ServerIndex = Convert.ToUInt32(spl[1]);
        n.Address = Convert.ToUInt64(spl[2]);
        
        return n;
    }
}