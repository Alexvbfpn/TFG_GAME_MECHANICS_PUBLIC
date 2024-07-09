namespace Items
{
    public interface IItem
    {
        public virtual void ChangeActiveState(bool state){}
        
        public virtual void ItemInitializations(){}

    }
}