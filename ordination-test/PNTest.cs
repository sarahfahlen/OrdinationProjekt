using Data;
using Microsoft.EntityFrameworkCore;
using Service;

namespace ordination_test;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using shared.Model;

[TestClass]
public class PNTest
{
    private DataService service;
    private Laegemiddel lm = null!;
    [TestInitialize]
    public void SetupBeforeEachTest()
    {
        var optionsBuilder = new DbContextOptionsBuilder<OrdinationContext>();
        optionsBuilder.UseInMemoryDatabase(databaseName: "test-database");
        var context = new OrdinationContext(optionsBuilder.Options);
        service = new DataService(context);
        service.SeedData();
        lm = service.GetLaegemidler().First();

    }
    
    private PN CreatePN(DateTime start, DateTime slut, double antalEnheder)
    {
        return new PN(start, slut, antalEnheder, lm);
    }

    [TestMethod]
    public void GetType_PN()
    {
        // ARRANGE
        var pn = new PN(DateTime.Now, 
            DateTime.Now, 
            2.0, lm);
        
        // ACT
        var type = pn.getType();

        // ASSERT
        Assert.AreEqual("PN", type);
    }

    [TestMethod]
    public void GivDosis_IndenforPeriode()
    {
        // ARRANGE
        var pn = CreatePN(
            new DateTime(2025, 1, 1), 
            new DateTime(2025, 1, 5), 
            2.0);
        var dato = new Dato { dato = new DateTime(2025, 1, 3) };

        // ACT
        bool res = pn.givDosis(dato);

        // ASSERT
        Assert.IsTrue(res);
        Assert.AreEqual(1, pn.getAntalGangeGivet());
    }
    
    [TestMethod]
    public void SamletDosis()
    {
        // ARRANGE
        double antalEnheder = 2.0;
        var pn = CreatePN(new DateTime(2025, 1, 1), new DateTime(2025, 1, 10), antalEnheder);
        
        // ACT
        pn.givDosis(new Dato { dato = new DateTime(2025, 1, 1) });
        pn.givDosis(new Dato { dato = new DateTime(2025, 1, 2) });
        pn.givDosis(new Dato { dato = new DateTime(2025, 1, 3) });

        // ASSERT
        Assert.AreEqual(3, pn.getAntalGangeGivet());
        Assert.AreEqual(3 * antalEnheder, pn.samletDosis(), 0.0001);
    }

    [TestMethod]
    public void DoegnDosis()
    {
        // ARRANGE
        double antalEnheder = 2.0;
        var pn = CreatePN(new DateTime(2025, 1, 1), new DateTime(2025, 1, 10), antalEnheder);

        // ACT
        pn.givDosis(new Dato { dato = new DateTime(2025, 1, 1) });
        pn.givDosis(new Dato { dato = new DateTime(2025, 1, 2) });
        pn.givDosis(new Dato { dato = new DateTime(2025, 1, 3) });

        // ASSERT
        Assert.AreEqual(6.0, pn.samletDosis(), 0.0001);  
        Assert.AreEqual(2.0, pn.doegnDosis(), 0.0001);  
    }
    
}