namespace Weapons
{
    public interface IWeapon
    {
        public float axis { get; }
        
        public void Shoot();
        
        public void SpecialWeaponAbility();
        
        public void Appear();
        
        public void Disappear();
    }
}