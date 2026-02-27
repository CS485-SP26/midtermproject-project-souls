namespace Farming
{
    public interface IHarvestable //remember to add the interface to plant.cs 
    //or whatever the plant class is named
    {
        //a function in plant.cs calling if a plant is harvestable
        bool CanHarvest();

        //a function in plant.cs that returns the amount added to funds
        int HarvestValue();
    }
}