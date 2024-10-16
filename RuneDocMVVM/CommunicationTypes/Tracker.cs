using System;

namespace RuneDocMVVM.CommunicationTypes;

public class Tracker
{
    public uint TotalExpGain { get; set; }
    public double HourlyExpGain { get; set; }
    public uint CurrentExp { get; set; }
    public uint CurrentLevel { get; set; }
    public string Skill { get; set; }
    
    public static Tracker FromString(string bufferSlice)
    {
        var spl = bufferSlice.Split("#");
        
        // skillId:level:current:gained:hourly
        Tracker t = new Tracker();
        t.Skill = SkillIdentification.ConvertSkillToFormattedString(Convert.ToInt32(spl[0]));
        t.CurrentLevel = Convert.ToUInt32(spl[1]);
        t.CurrentExp = Convert.ToUInt32(spl[2]);
        t.TotalExpGain = Convert.ToUInt32(spl[3]);
        t.HourlyExpGain = Convert.ToDouble(spl[4]);
        
        return t;
    }

    public void Print()
    {
        Console.WriteLine($"{Skill} - {CurrentLevel} - {CurrentExp} - {TotalExpGain} - {HourlyExpGain}");
    }
}