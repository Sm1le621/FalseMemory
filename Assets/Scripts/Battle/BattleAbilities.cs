// Тут логіка предметів (зілля) і заклинань у бою.
// Ефекти беруться з даних (ScriptableObject), тому нове заклинання
// з уже існуючим ефектом можна додати без змін у BattleManager.
public static class BattleAbilities
{
    // Чи можна використати предмет item: user використовує його на target
    public static bool CanUseItem(BattleUnit user, BattleUnit target, BattleConsumable item, ItemUseLocation location = ItemUseLocation.Battle)
    {
        // 1) Всі об'єкти мають існувати
        if (user == null || target == null || item == null)
        {
            return false;
        }

        // 2) Обидва мають бути живі
        if (!user.IsAlive || !target.IsAlive)
        {
            return false;
        }

        // 3) Це має бути саме витратний предмет
        if (item.category != ItemCategory.Consumable)
        {
            return false;
        }

        // 4) Місце використання: тільки "світ" або "бій"
        if (location != ItemUseLocation.Exploration && location != ItemUseLocation.Battle)
        {
            return false;
        }

        // 5) Предмет має дозволяти використання в цьому місці (це прапорці, тому &)
        if ((item.useLocations & location) != location)
        {
            return false;
        }

        // 6) Значення не можуть бути від'ємними
        if (item.healing < 0 || item.manaRestoration < 0)
        {
            return false;
        }

        // 7) Предмет має щось робити: або лікувати, або відновлювати ману
        if (item.healing <= 0 && item.manaRestoration <= 0)
        {
            return false;
        }

        // 8) Але НЕ обидва одразу (такий "гібрид" вважаємо помилкою)
        if (item.healing > 0 && item.manaRestoration > 0)
        {
            return false;
        }

        // 9) Перевіряємо ціль: "на себе" - тільки на себе, інакше - тільки на іншого
        if (item.target == BattleTarget.Self)
        {
            if (user != target)
            {
                return false;
            }
        }
        else
        {
            if (user == target)
            {
                return false;
            }
        }

        // 10) Предмет має бути в інвентарі
        if (user.Inventory == null)
        {
            return false;
        }
        if (user.Inventory.Count(item) <= 0)
        {
            return false;
        }

        return true;
    }

    // Спробувати використати предмет. amount - скільки HP/MP відновилось
    public static bool TryUseItem(BattleUnit user, BattleUnit target, BattleConsumable item, out int amount, ItemUseLocation location = ItemUseLocation.Battle)
    {
        amount = 0;

        if (!CanUseItem(user, target, item, location))
        {
            return false;
        }

        // Забираємо 1 штуку з інвентаря
        if (!user.Inventory.TryConsume(item))
        {
            return false;
        }

        if (item.manaRestoration > 0)
        {
            amount = target.RestoreMP(item.manaRestoration);
        }
        else
        {
            amount = target.Health.Heal(item.healing);
        }
        return true;
    }

    // Чи може caster скастувати spell на target
    public static bool CanCast(BattleUnit caster, BattleUnit target, BattleSpell spell)
    {
        if (caster == null || target == null || spell == null)
        {
            return false;
        }

        if (!caster.IsAlive || !target.IsAlive)
        {
            return false;
        }

        // Вартість не може бути від'ємною, і мани має вистачати
        if (spell.mpCost < 0)
        {
            return false;
        }
        if (caster.CurrentMP < spell.mpCost)
        {
            return false;
        }

        // Перевірка цілі (як і в предметах)
        if (spell.target == BattleTarget.Self)
        {
            if (caster != target)
            {
                return false;
            }
        }
        else
        {
            if (caster == target)
            {
                return false;
            }
        }

        // Поки що є тільки один ефект - магічна шкода
        if (spell.effect != BattleEffect.MagicDamage)
        {
            return false;
        }

        return true;
    }

    // Спробувати скастувати заклинання. amount - скільки шкоди нанесено
    public static bool TryCast(BattleUnit caster, BattleUnit target, BattleSpell spell, BattleRules rules, out int amount)
    {
        amount = 0;

        if (rules == null)
        {
            return false;
        }
        if (!CanCast(caster, target, spell))
        {
            return false;
        }
        // Знімаємо ману
        if (!caster.TrySpendMP(spell.mpCost))
        {
            return false;
        }

        amount = BattleCalculator.MagicalDamage(caster.EffectiveMagicAttack, target.EffectiveMagicDefense, spell.power, rules);
        target.Health.TakeDamage(amount);
        return true;
    }
}
