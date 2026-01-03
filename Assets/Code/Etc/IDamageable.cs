namespace Code
{
    public interface IDamageable
    {
        void TakeDamage(int damage);
        void Heal(int heal);
    }
}