namespace Quasar.Core
{
    public interface IQuasarConfiguration
    {
        ProductEnvironment[] Environments { get; set; }
        Product[] Products { get; set; }
        ServerType[] Servers { get; set; }
    }
}