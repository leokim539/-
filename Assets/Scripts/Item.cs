using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Item
{
    public string Name { get; private set; }

    protected Item(string name)
    {
        Name = name;
    }

    public abstract void Use(Player player);
}
public class HealthPotion : Item
{

    public HealthPotion(int healAmount) : base("ü�� ȸ��")
    {

    }

    public override void Use(Player player)
    {
        
    }
}

public class SpeedBoost : Item
{
    private float speedMultiplier;
    private float duration;

    public SpeedBoost(float speedMultiplier, float duration) : base("���ǵ� ����")
    {
        this.speedMultiplier = speedMultiplier;
        this.duration = duration;
    }

    public override void Use(Player player)
    {

    }
}

public class DefenseBoost : Item
{
    private int defenseAmount;
    private float duration;

    public DefenseBoost(int defenseAmount, float duration) : base("���� ����")
    {
        this.defenseAmount = defenseAmount;
        this.duration = duration;
    }

    public override void Use(Player player)
    {
    }
}