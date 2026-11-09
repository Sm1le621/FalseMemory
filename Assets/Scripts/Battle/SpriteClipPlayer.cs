using UnityEngine;

// Показує кадри з .anim кліпів на SpriteRenderer або UI Image.
// Animator додається у грі, тому сцени не потрібно змінювати вручну.
public static class SpriteClipPlayer
{
    // Взяти Animator з об'єкта, а якщо його немає - додати
    public static Animator GetAnimator(GameObject target)
    {
        if (target == null)
        {
            return null;
        }

        Animator animator = target.GetComponent<Animator>();
        if (animator == null)
        {
            animator = target.AddComponent<Animator>();
            animator.applyRootMotion = false;
            animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
        }

        return animator;
    }

    // Показати стан у потрібній точці: progress від 0 до 1
    public static void Sample(
        Animator animator,
        RuntimeAnimatorController controller,
        string stateName,
        float progress)
    {
        if (animator == null || controller == null)
        {
            return;
        }

        Prepare(animator, controller);
        // speed = 0, щоб Animator не зсував кадр після нашого вибору
        animator.speed = 0f;
        animator.Play(stateName, 0, Mathf.Clamp01(progress));
        animator.Update(0f);
    }

    // Запустити циклічний стан (наприклад, ходьбу)
    public static void Loop(
        Animator animator,
        RuntimeAnimatorController controller,
        string stateName)
    {
        if (animator == null || controller == null)
        {
            return;
        }

        bool changed = Prepare(animator, controller);
        animator.speed = 1f;

        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
        // Після вмикання Animator ще не намалював кадр, тому примусово ставимо його
        if (changed || !info.IsName(stateName))
        {
            animator.Play(stateName, 0, 0f);
            animator.Update(0f);
        }
    }

    // Вимкнути Animator, щоб код міг поставити статичний спрайт
    public static void Stop(Animator animator)
    {
        if (animator != null)
        {
            animator.enabled = false;
        }
    }

    // Увімкнути Animator і за потреби замінити контролер.
    // Повертає true, якщо Animator був вимкнений або контролер змінився.
    private static bool Prepare(Animator animator, RuntimeAnimatorController controller)
    {
        bool changed = !animator.enabled;
        animator.enabled = true;
        if (animator.runtimeAnimatorController != controller)
        {
            animator.runtimeAnimatorController = controller;
            changed = true;
        }

        return changed;
    }
}
