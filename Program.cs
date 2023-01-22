using System;
using System.Collections.Generic;

internal class Program
{
    static void Main(string[] args)
    {
        Fight game = new Fight();
        game.StartBattle();
    }
}

class Fight
{
    private Detachment _firstDetachment = new Detachment();
    private Detachment _secondDetachment = new Detachment();

    public void StartBattle()
    {
        while (_firstDetachment.Warriors.Count > 0 && _secondDetachment.Warriors.Count > 0)
        {
            int indexWarrior = 0;
            Warrior firstWarrior = _firstDetachment.Warriors[indexWarrior];
            Warrior secondWarrior = _secondDetachment.Warriors[indexWarrior];
            Console.WriteLine("\nСледующий бой");

            while (firstWarrior.Health > 0 && secondWarrior.Health > 0)
            {
                firstWarrior.Attack(secondWarrior);
                secondWarrior.Attack(firstWarrior);
                ShowStats(firstWarrior);
                ShowStats(secondWarrior);
                Console.ReadKey();
            }

            if (firstWarrior.Health <= 0 && secondWarrior.Health <= 0)
            {
                Console.Write("\nбойцы убили друг друга");
                _firstDetachment.RemoveWarrior(firstWarrior);
                _secondDetachment.RemoveWarrior(secondWarrior);
                Console.Write(", в отряде страны " + _firstDetachment.Name + " осталось " + _firstDetachment.Warriors.Count + " бойцов");
                Console.Write(", в отряде страны " + _secondDetachment.Name + " осталось " + _secondDetachment.Warriors.Count + " бойцов");
            }
            else if (firstWarrior.Health <= 0)
            {
                Console.Write("\n" + firstWarrior + " из отряда страны " + _firstDetachment.Name + " был убит");
                _firstDetachment.RemoveWarrior(firstWarrior);
                Console.Write(", в отряде осталось " + _firstDetachment.Warriors.Count + " бойцов");
            }
            else if (secondWarrior.Health <= 0)
            {
                Console.Write("\n" + secondWarrior + " из отряда страны " + _secondDetachment.Name + " был убит");
                _secondDetachment.RemoveWarrior(secondWarrior);
                Console.Write(", в отряде осталось " + _secondDetachment.Warriors.Count + " бойцов");
            }

            if (_firstDetachment.Warriors.Count == 0 && _secondDetachment.Warriors.Count == 0)
            {
                Console.WriteLine("\nНичья, оба отряда мертвы");
            }
            else if (_firstDetachment.Warriors.Count == 0)
            {
                Console.WriteLine("\nПобедил отряд страны " + _secondDetachment.Name);
            }
            else if (_secondDetachment.Warriors.Count == 0)
            {
                Console.WriteLine("\nПобедил отряд страны " + _firstDetachment.Name);
            }
        }
    }

    private void ShowStats(Warrior warrior)
    {
        Console.WriteLine(warrior.Name + " HP: " + warrior.Health);
    }
}

class Detachment
{
    private List<Warrior> _warriors = new List<Warrior>();

    public Detachment()
    {
        _numberPlayer++;
        Name = ReadName(_numberPlayer);
        _warriors = Create(Name);
    }

    private static byte _numberPlayer = 0;
    public IReadOnlyList<Warrior> Warriors => _warriors;
    public string Name { get; private set; }

    public void RemoveWarrior(Warrior warrior)
    {
        _warriors.Remove(warrior);
    }

    private string ReadName(byte numberPlayer)
    {
        Console.WriteLine("Для старта битвы введите название страны, которую будет представлять ваш отряд, игрок номер " + numberPlayer);
        return Console.ReadLine();
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

    private List<Warrior> Create(string nameDetachment)
    {
        List<Warrior> detachment = new List<Warrior>();
        int warriorsCount = 5;

        Console.WriteLine("\nвыберите " + warriorsCount + " бойцов для отряда страны: " + nameDetachment);

        for (int i = 0; i < warriorsCount; i++)
        {
            detachment.Add(ChooseWarrior());
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

    private void ShowWarriors(List<Warrior> warriors)
    {
        byte idWarrior = 0;

        foreach (Warrior warrior in warriors)
        {
            Console.Write(idWarrior++ + "  ");
            warrior.ShowInfo();
        }
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

    public Warrior()
    {
        Name = "Warrior";
        Health = 100;
        Armor = 15;
        Damage = 25;
        AttackSpeed = 1;
    }

    public string Name { get; protected set; }
    public int Health { get; protected set; }
    public int Armor { get; protected set; }
    public int Damage { get; protected set; }
    public int AttackSpeed { get; protected set; }

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
            Health += Armor - damage;
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
    public Priest() : base()
    {
        Name = "Priest";
        Armor = 10;
        Mana = 100;
        Heal = 30;
    }

    public int Mana { get; private set; }
    public int Heal { get; private set; }

    public override void TakeDamage(int damage)
    {
        int manaCostHeal = 20;

        if (Armor < damage)
        {
            if (Mana >= manaCostHeal)
            {
                Mana -= manaCostHeal;
                Health += Heal;
            }

            Health -= damage - Armor;
        }
    }

    public override void ShowInfo()
    {
        Console.WriteLine("Жрец, отхиливается каждый раз на " + Heal + "% за счет маны.");
    }
}

class Rogue : Warrior
{
    public Rogue() : base()
    {
        Name = "Rogue";
        LethalHitChance = 5;
        IvasionChance = 15;
    }

    public int LethalHitChance { get; private set; }
    public int IvasionChance { get; private set; }

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
    public Shaman() : base()
    {
        Name = "Shaman";
        AttackSpeed = 2;
        Mana = 100;
        AmountHealth = 50;
    }

    public int Mana { get; private set; }
    public int AmountHealth { get; private set; }

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
