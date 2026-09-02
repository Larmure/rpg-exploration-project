using UnityEngine;

public class NPC : Entity, IInteractable
{
    [Header("Identity")]
    [SerializeField] protected string npcName = "PNJ";
    [SerializeField] protected bool invincible = true;
    [Header("Dialogs")]
    [SerializeField]
    protected string[] dialogueLines = new string[]
    {
        "Hi !"
    };



    public virtual void Interact()
    {
        DialogueManager.Instance.StartDialogue(
            npcName,
            dialogueLines,
            null,
            null,
            OnDialogueEnd
        );
    }

    protected virtual void OnDialogueEnd() { }

    public override void TakeDamage(int amount)
    {
        if (isDead || invincible) return;

        int mitigated = Mathf.Max(0, amount - armorPoints);
        currentHealth -= mitigated;

        animator?.SetTrigger("Hit");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public override void Die()
    {
        if (isDead) return;
        isDead = true;

        animator?.SetTrigger("Die");
        // Par défaut, on désactive le PNJ. À adapter selon ton besoin
        // (loot, destruction différée, désactivation du collider, etc.)
        enabled = false;
    }
}