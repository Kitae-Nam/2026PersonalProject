using UnityEngine;

public class EnumManager : MonoBehaviour
{
}

public enum HandType
{
    None,
    Holding,
    Equipment,
}
public enum ItemType
{
    None,
    Material,
    Equipment,
    Rail,
}
public enum MaterialType
{
    None,
    Wood,
    Rock,
}

public enum EquipmentType
{
    None,
    Pickaxe,
    Axe,
    Bucket,
}

public enum BiomeType
{
    Grass, 
    Stone
}