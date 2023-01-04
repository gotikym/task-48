using System;
using System.Collections.Generic;

internal class Program
{
    static void Main(string[] args)
    {
        Fight game = new Fight();
        game.Play();
    }
}

class Fight
{
    public void Play()
    {
        byte playerCount = 1;
        string detachmentCountry1 = ReadName(playerCount);
        playerCount++;
        string detachmentCountry2 = ReadName(playerCount);
        List<Warrior> firstDetachment = CreateDetachment(detachmentCountry1);
        List<Warrior> secondDetachment = CreateDetachment(detachmentCountry2);
        Console.WriteLine("Да начнется гладиаторский бой!");

        StartBattle(firstDetachment, secondDetachment, detachmentCountry1, detachmentCountry2);
    }

    private void StartBattle(List<Warrior> firstDetachment, List<Warrior> secondDetachment,string detachmentCountry1, string detachmentCountry2)
    {
        while (firstDetachment.Count > 0 && secondDetachment.Count > 0)
        {
            int indexWarrior = 0;
            Warrior firstWarrior = firstDetachment[indexWarrior];
            Warrior secondWarrior = secondDetachment[indexWarrior];
            Console.WriteLine("\nСледующий бой");

            while (firstWarrior.Health > 0 && secondWarrior.Health > 0)
            {
                firstWarrior.Attack(secondWarrior);
                secondWarrior.Attack(firstWarrior);
                ShowStats(firstWarrior);
                ShowStats(secondWarrior);
                Console.ReadKey();

                if (firstWarrior.Health < 0 || firstWarrior.Health == 0)
                {
                    Console.Write("\n" + firstWarrior + " из отряда страны " + detachmentCountry1 + " был убит");
                    firstDetachment.Remove(firstWarrior);
                    Console.Write(", в отряде осталось " + firstDetachment.Count + " бойцов");
                }
                else if (secondWarrior.Health < 0 || secondWarrior.Health == 0)
                {
                    Console.Write("\n" + secondWarrior + " из отряда страны " + detachmentCountry2 + " был убит");
                    secondDetachment.Remove(secondWarrior);
                    Console.Write(", в отряде осталось " + secondDetachment.Count + " бойцов");
                }
            }

            if (firstDetachment.Count == 0 )
            {
                Console.WriteLine("\nПобедил отряд страны " + detachmentCountry2);
            }
            else if (secondDetachment.Count == 0)
            {
                Console.WriteLine("\nПобедил отряд страны " + detachmentCountry1); 
            }
            else if(firstDetachment.Count == 0 && secondDetachment.Count == 0)
            {
                Console.WriteLine("\nНичья, оба отряда мертвы");
            }
        }
    }

    private List<Warrior> CreateDetachment(string nameDetachment)
    {
        List<Warrior> detachment = new List<Warrior>();
        int warriorsCount = 5;

        Console.WriteLine("\nвыберите " + warriorsCount + " бойцов для отряда страны: " + nameDetachment);

        while (warriorsCount > 0)
        {
            detachment.Add(ChooseWarrior());
            warriorsCount--;
        }
        
        return detachment;
    }

    private Warrior ChooseWarrior()
    {
        List<Warrior> warriors = CreateWarriors();
        
        ShowWarriors(warriors);
        int warriorIndex = GetNumber();
        int defoltIndex = 0;

        if (warriorIndex >= warriors.Count || warriorIndex < 0)
        {
            Console.WriteLine("Вы странный, ввели то, чего не было в выборе, ваш отряд будет состоять из танков");
            return warriors[defoltIndex];
        }

        return warriors[warriorIndex];
    }

    private List<Warrior> CreateWarriors()
    {
        List<Warrior> warriors = new List<Warrior>();
        warriors.Add(new Tank());
        warriors.Add(new Priest());
        warriors.Add(new Rogue());
        warriors.Add(new Shaman());
        warriors.Add(new Hunter());

        return warriors;
    }

    private string ReadName(byte playerCount)
    {
        Console.WriteLine("Для старта битвы введите название страны, которую будет представлять ваш отряд, игрок номер " + playerCount);
        return Console.ReadLine();
    }

    private void ShowWarriors(List<Warrior> warriors)
    {
        byte idWarrior = 0;

        foreach (Warrior warrior in warriors)
        {
            Console.Write(idWarrior++ + "  ");
            warrior.ShowInfo();
        }
    }

    private void ShowStats(Warrior warrior)
    {
        Console.WriteLine(warrior.Name + " HP: " + warrior.Health);
    }

    private int GetNumber()
    {
        bool isParse = false;
        int numberForReturn = 0;

        while (isParse == false)
        {
            string userNumber = Console.ReadLine();
            isParse = int.TryParse(userNumber, out numberForReturn);

            if (isParse == false)
            {
                Console.WriteLine("Вы не корректно ввели число.");
            }
        }

        return numberForReturn;
    }
}

abstract class Warrior
{
    static private Random _random = new Random();
    public string Name { get; protected set; }
    public int Health { get; protected set; }
    public int Armor { get; protected set; }
    public int Damage { get; protected set; }
    public int AttackSpeed { get; protected set; }

    public Warrior()
    {
        Name = "Warrior";
        Health = 100;
        Armor = 15;
        Damage = 25;
        AttackSpeed = 1;
    }

    public virtual void TakeDamage(int damage)
    {
        if (Armor < damage)
        {
            Health -= damage - Armor;
        }
    }

    public virtual void Attack(Warrior warrior)
    {
        warrior.TakeDamage(Damage);
    }

    public virtual void ShowInfo()
    {
        Console.WriteLine("Меня не видно, я абстрактный...");
    }

    public virtual bool GetChance(int chance)
    {
        int minChance = 0;
        int maxChance = 101;

        return chance >= _random.Next(minChance, maxChance);
    }
}

class Tank : Warrior
{
    public Tank() : base()
    {
        Damage = 35;
        Name = "Tank";
        Armor = 30;
    }

    public override void TakeDamage(int damage)
    {
        if (Armor > damage)
        {
            Health += damage - Armor;
        }
        else
        {
            Health -= damage - Armor;
        }
    }

    public override void ShowInfo()
    {
        Console.WriteLine("Танк, медленный и неповоротливый, но с мощной дубиной и если ваш удар слаб, ТАНК лишь поднимет свои хп =)");
    }
}

class Priest : Warrior
{
    public int Mana { get; private set; }
    public int Heal { get; private set; }

    public Priest() : base()
    {
        Name = "Priest";
        Armor = 10;
        Mana = 100;
        Heal = 30;
    }

    public override void TakeDamage(int damage)
    {
        int manaCostHeal = 20;

        if (Armor < damage)
        {
            if (Mana > 0)
            {
                Mana -= manaCostHeal;
                Health -= damage - Armor;
                Health += Heal;
            }
            else
            {
                Health -= damage - Armor;
            }
        }
    }

    public override void ShowInfo()
    {
        Console.WriteLine("Жрец, отхиливается каждый раз на " + Heal + "% за счет маны.");
    }
}

class Rogue : Warrior
{
    public int LethalHitChance { get; private set; }
    public int IvasionChance { get; private set; }

    public Rogue() : base()
    {
        Name = "Rogue";
        LethalHitChance = 5;
        IvasionChance = 15;
    }

    public override void TakeDamage(int damage)
    {
        if (Armor < damage)
        {
            if (GetChance(IvasionChance))
            {
                Health -= damage - Armor;
            }
        }
    }

    public override void Attack(Warrior warrior)
    {
        int damage = Damage * AttackSpeed;
        int lethalHit = 99999;

        if (GetChance(LethalHitChance))
        {
            warrior.TakeDamage(lethalHit);
        }
        else
        {
            warrior.TakeDamage(damage);
        }
    }

    public override void ShowInfo()
    {
        Console.WriteLine("Разбойник, имеет шанс в " + LethalHitChance + "% отравить быстродействующим смертельным ядом, так же шанс уклониться от атаки в " + IvasionChance + "%");
    }
}

class Shaman : Warrior
{
    public int Mana { get; private set; }
    public int AmountHealth { get; private set; }

    public Shaman() : base()
    {
        Name = "Shaman";
        AttackSpeed = 2;
        Mana = 100;
        AmountHealth = 50;
    }

    public override void TakeDamage(int damage)
    {
        if (Armor < damage)
        {
            if (Health + Armor <= damage && Mana == 100)
            {
                Mana = 0;
                Health += AmountHealth;
                Health -= damage - Armor;
            }
            else
            {
                Health -= damage - Armor;
            }
        }
    }

    public override void Attack(Warrior warrior)
    {
        int damage = Damage * AttackSpeed;
        warrior.TakeDamage(damage);
    }

    public override void ShowInfo()
    {
        Console.WriteLine("Шаман, имеет изначально повышенную скорость атаки х" + AttackSpeed + ", при смертельном ударе восстанавливает себе " + AmountHealth + "% жизней за счет всей маны");
    }
}

class Hunter : Warrior
{
    public int AttackSpeedBow { get; private set; }
    public int DamageBow { get; private set; }
    public int Distance { get; private set; }

    public Hunter() : base()
    {
        Name = "Hunter";
        AttackSpeedBow = 2;
        DamageBow = 35;
        Distance = 2;
    }

    public override void TakeDamage(int damage)
    {
        if (Distance > 0)
        {
            Distance--;
        }
        else
        {
            if (Armor < damage)
            {
                Health -= damage - Armor;
            }
        }
    }

    public override void Attack(Warrior warrior)
    {
        int damage = Damage;

        if (Distance > 0)
        {
            damage = AttackSpeedBow * DamageBow;
        }

        warrior.TakeDamage(damage);
    }

    public override void ShowInfo()
    {
        int attackCount = Distance * AttackSpeedBow;
        Console.WriteLine("Охотник, пока враг дойдет, охотник успеет выстрелить " + attackCount + "раза, а после будет сражаться в ближнем бою");
    }
}
