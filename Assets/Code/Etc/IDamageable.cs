namespace Code.Etc
{
    public interface IDamageable
    {
        void TakeDamage(int damage);
        void Heal(int heal);
    }
}