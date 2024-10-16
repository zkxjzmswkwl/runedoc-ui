using System;
using System.Globalization;

namespace RuneDocMVVM.CommunicationTypes;

public class SkillIdentification
{
    public static string ConvertSkillToFormattedString(int skillValue)
    {
        if (Enum.IsDefined(typeof(Skill), skillValue))
        {
            Skill skill = (Skill)skillValue;
            string skillName = skill.ToString().ToLower();
            return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(skillName);
        }
        else
        {
            return "Unknown Skill";
        }
    } 
}

// Unused for now.
public enum Skill
{
    ATTACK = 0,
    DEFENCE = 1,
    STRENGTH = 2,
    HITPOINTS = 3,
    RANGED = 4,
    PRAYER = 5,
    MAGIC = 6,
    COOKING = 7,
    WOODCUTTING = 8,
    FLETCHING = 9,
    FISHING = 10,
    FIREMAKING = 11,
    CRAFTING = 12,
    SMITHING = 13,
    MINING = 14,
    HERBLORE = 15,
    AGILITY = 16,
    THIEVING = 17,
    SLAYER = 18,
    FARMING = 19,
    RUNECRAFTING = 20,
    HUNTER = 21,
    CONSTRUCTION = 22,
    SUMMONING = 23,
    DUNGEONEERING = 24,
    DIVINATION = 25,
    INVENTION = 26,
    ARCHAEOLOGY = 27,
    NECROMANCY = 28,
}